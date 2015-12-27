
using System.Configuration;
using BrockAllen.MembershipReboot;
using CUWebinars.Business.Constants;
using CUWebinars.Business.Core;
using CUWebinars.Business.Core.Helpers;
using CUWebinars.Business.Models;
using CUWebinars.Business.Repository;
using FluentValidation;
using FluentValidation.Results;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CUWebinars.Business.AccountService
{
    public class MembershipService : IMembershipService
    {
        private readonly IInstitutionRepository _institutionRepository;
        private readonly IRefDataRepository _refDataRepository;
        private readonly AuthenticationService _samAuthenticationService;
        private readonly UserAccountService _userAccountService;

        private readonly IWebUserRepository _webUserRepository;
        private readonly FluentValidation.IValidator<Tuple<string, string>> _postEventMaterialsAccessClaimValidator;
        private readonly IDomainHelper _domainHelper;
        private readonly ILogger _logger;
        private bool _disposed;

        public MembershipService(IInstitutionRepository institutionRepository,
            IRefDataRepository refDataRepository,
            AuthenticationService samAuthenticationService,
            UserAccountService userAccountService,
            IWebUserRepository webUserRepository,
            FluentValidation.IValidator<Tuple<string, string>> postEventMaterialsAccessClaimValidator,
            IDomainHelper domainHelper,
            ILogger logger)
        {
            _institutionRepository = institutionRepository;
            _refDataRepository = refDataRepository;
            _samAuthenticationService = samAuthenticationService;
            _userAccountService = userAccountService;
            _webUserRepository = webUserRepository;
            _postEventMaterialsAccessClaimValidator = postEventMaterialsAccessClaimValidator;
            _domainHelper = domainHelper;
            _logger = logger;
        }

        public WebUser GetDetailsOfUser(string email)
        {
            if (email == null) throw new ArgumentNullException("email");
            var webUser = _refDataRepository.GetWebUserByEmail(email);
            return webUser;
        }

        public UserAccount GetUserAccountByWebUserId(string tenant, int userId)
        {
            var webUser = GetWebUserById(userId);
            return GetUserAccountByEmail(tenant, webUser.email);
        }

        /// <summary>
        /// serves to check if email exists before attempting to create account.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public WebUser GetUserByEmail(string email)
        {
            if (email == null) throw new ArgumentNullException("email");
            var webUser = _webUserRepository.GetWebUserByEmail(email);
            return webUser;
        }

        public WebUser GetUserByEmailLoadedWithOrdersData(string email)
        {
            if (email == null) throw new ArgumentNullException("email");
            var webUser = _webUserRepository.GetWebUserByEmailLoadedWithOrdersData(email);
            return webUser;
        }

        WebUser IMembershipService.GetUserFromLegacy(string email)
        {
            var dataOperations = new DataOperations(ConfigurationManager.ConnectionStrings["MembershipReboot"].ConnectionString);
            return dataOperations.GetWebUserFromLegacy(email);

        }

        public WebUser GetWebUserById(int userId)
        {
            return _webUserRepository.FindByIdLoaded(userId);
        }

        public WebUser GetWebUserByIdFromLegacy(int userId)
        {
            throw new NotImplementedException();
        }

        public int? GetWebUserIdByEmail(string email)
        {
            return _webUserRepository.GetWebUserIdByEmail(email);
        }

        public IEnumerable<WebUser> GetWebUsersByLastNameForAffiliate(string lastName, int idAffiliate)
        {
            return _webUserRepository.GetWebUsersByLastNameForAffiliate(lastName, idAffiliate);
        }

        public IEnumerable<Institution> GetInstitutionsByName(string name)
        {
            return _institutionRepository.GetInstitutionsByName(name);
        }

        public DateTime? GetPostEventAccessExpireyDate(UserAccount userAccount, int idOrder)
        {
            if (userAccount == null || !userAccount.HasClaim(ClaimTypes.PostEventMaterials) || !userAccount.HasClaim(ClaimTypes.PostEventMaterialsExtended)) return null;


            var claimsForOrder = userAccount.Claims
                .Where(c => c.Value.ToLower().Contains(idOrder.ToString()))
                .FirstOrDefault(c => c.Type == ClaimTypes.PostEventMaterials || c.Type == ClaimTypes.PostEventMaterialsExtended);

            // extract the date
            if (claimsForOrder == null) return null;

            var expiryAsString = JObject.Parse(claimsForOrder.Value).GetValue(JsonPropertyKeys.ExpiryDate).ToString();

            DateTime expiryDate;
            if (DateTime.TryParse(expiryAsString, out expiryDate))
            {
                return expiryDate;
            }

            return null;
        }

        public UserAccount GetUserAccountByEmail(string tenant, string email)
        {
            if (tenant == null) throw new ArgumentNullException("tenant");
            if (email == null) throw new ArgumentNullException("email");
            // While attempting to naviate to the definition of GetByEmail 
            return _userAccountService.GetByEmail(tenant, email);
        }
        public UserAccount GetUserAccountByVerificationKey(string key)
        {
            if (key == null) throw new ArgumentNullException("key");
            return _userAccountService.GetByVerificationKey(key);
        }

        public Institution GetInstitutionByDomain(string domain)
        {
            if (domain == null) throw new ArgumentNullException("domain");
            return _institutionRepository.GetByDomain(domain);

        }

        public bool HasPassword(string tenant, string emailAddress)
        {
            if (tenant == null) throw new ArgumentNullException("tenant");
            if (emailAddress == null) throw new ArgumentNullException("emailAddress");

            var userAccount = _userAccountService.GetByEmail(tenant, emailAddress);

            if (ReferenceEquals(null, userAccount))
            {
                return !string.IsNullOrEmpty(userAccount.HashedPassword);
            }

            return false;
        }

        //public string CheckDisplayPostEventMaterials(string tenant, string email, out string messageIfFalse)
        //{
        //    messageIfFalse = string.Empty;
        //    var userAccount = GetUserAccountByEmail(tenant, email);

        //    if (userAccount != null)
        //    {
        //        var claimValue = GetDisplayPostEventMaterialsClaimValue(userAccount);

        //        if (!string.IsNullOrWhiteSpace(claimValue))
        //        {
        //            var expiryAsString = claimValue.Substring(claimValue.IndexOf(":", StringComparison.Ordinal) + 1);

        //            DateTime expiryDate;

        //            if (DateTime.TryParse(expiryAsString, out expiryDate))
        //            {
        //                return claimValue;
        //            }
        //        }
        //        else
        //        {
        //            messageIfFalse = string.Format("User with email {0} is not authorised to access materials", email);
        //        }
        //    }
        //    else
        //    {
        //        messageIfFalse = string.Format("No UserAccount exists with the email {0}", email);
        //    }

        //    return "false";
        //}


        public void CleanUser(string tenant, string email, string newPassword)
        {
            var dataOperations = new DataOperations(ConfigurationManager.ConnectionStrings["MembershipReboot"].ConnectionString);
            var userAccount = _userAccountService.GetByEmail(tenant, email);

            if (ReferenceEquals(null, userAccount))
            {
                throw new NullReferenceException(DomainConstants.UserNotFound);
            }

            dataOperations.SetFieldsConsistantWithVerifiedUser(userAccount);
            _userAccountService.SetRequiresPasswordReset(userAccount.ID, false);
            _userAccountService.SetPassword(userAccount.ID, newPassword);
        }

        public UserAccount CreateUser(
            string tenant,
            string firstName,
            string lastName,
            string userName,
            string password,
            string email
            )
        {
            if (firstName == null) throw new ArgumentNullException("firstName");
            if (lastName == null) throw new ArgumentNullException("lastName");

            // let MembershipReboot throw exception if other params are null
            var account = _userAccountService.CreateAccount(tenant, userName, password, email);
            _userAccountService.AddClaim(account.ID, ClaimTypes.FullName, string.Format("{0} {1}", firstName, lastName));
            _userAccountService.AddClaim(account.ID, System.Security.Claims.ClaimTypes.Role, "WebUser");

            return account;
        }

        public UserAccount CreateUserFromCart(
            string tenant,
            string password,
            string email
            )
        {
            _logger.Info("Creating User from cart: " + email);
            var account = _userAccountService.CreateAccount(tenant, string.Empty, password, email);
            _userAccountService.AddClaim(account.ID, System.Security.Claims.ClaimTypes.Role, "WebUser");

            return account;
        }

        public WebUser CreateWebUser(
            string tenant,
            string firstName,
            string lastName,
            string userName,
            string email,
            USTimeZone timeZone,
            UserType userType,
            int institutionId,
            IList<Address> addresses,
            string title,
            int? idUserImported,
            string accountStatus = null)
        {
            if (firstName == null) throw new ArgumentNullException("firstName");
            if (lastName == null) throw new ArgumentNullException("lastName");
            if (email == null) throw new ArgumentNullException("email");
            //if (addresses == null) throw new ArgumentNullException("addresses");

            if (idUserImported == 0) idUserImported = null;

            DateTime timeUtc = DateTime.UtcNow;
            timeUtc = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));

            var webUser = new WebUser
            {
                idUser = idUserImported.HasValue ? idUserImported.Value : _refDataRepository.GetMaxWebUserId() + 1,
                // Check what default should be for non-nullable field -- A: default is 'New'.
                AcctStatus = accountStatus ?? DomainConstants.New,
                DateCreated = timeUtc,
                email = email,
                FirstName = firstName,
                idUserInstitution = institutionId,
                LastName = lastName,
                Addresses = addresses,
                Title = title,
                timeZone = timeZone,
                UserType = UserType.Customer,
            };

            _webUserRepository.Add(webUser);

            return webUser;
        }

        public bool LogInUser(string tenant, string emailAddress, string password, bool persistent)
        {
            UserAccount userAccount = null;

            if (_userAccountService.AuthenticateWithEmail(tenant, emailAddress, password, out userAccount))
            {
                _samAuthenticationService.SignIn(userAccount, persistent);
                return true;
            }
            return false;
        }

        public bool LogInUser(string tenant, string emailAddress, string password, bool persistent, out string userMustVerify)
        {
            UserAccount userAccount;
            userMustVerify = string.Empty;

            if (_userAccountService.AuthenticateWithEmail(tenant, emailAddress, password, out userAccount))
            {
                if ((userAccount.HasClaim(ClaimTypes.HasNotVerified, ClaimValues.OrderImportRegistration) ||
                    userAccount.HasClaim(ClaimTypes.HasNotVerified, ClaimValues.CartRegistration)))
                {
                    userMustVerify = "User Must Verify";
                    return false;
                }

                if (!userAccount.HasClaim(ClaimTypes.FullName))
                {
                    var user = GetUserByEmail(emailAddress);
                    AddClaim(userAccount, ClaimTypes.FullName, string.Concat(user.FirstName.Trim(), ' ', user.LastName.Trim()));
                }
                _samAuthenticationService.SignIn(userAccount, persistent);
                return true;
            }
            return false;
        }

        public bool LogInAdminUserAsOtherUser(string tenant, string emailAddress, string password, UserAccount account)
        {
            UserAccount userAccount = null;

            if (_userAccountService.AuthenticateWithEmail(tenant, emailAddress, password, out userAccount))
            {
                _samAuthenticationService.SignIn(account, false); // issue auth cookie to impersonated user account
                return true;
            }
            return false;
        }

        public bool LogOutUser()
        {
            _samAuthenticationService.SignOut();
            return true;
        }


        public Institution ProcessInstitutionForUser(string institutionName,
            string email,
            string city,
            string state,
            string regIdentifier,
            string institutionType,
            string zip)
        {
            List<Institution> institutionsList = null;

            try
            {
                var institutions = _institutionRepository.GetByNameAndZipCode(institutionName, zip);
                institutionsList = institutions.ToList();
            }
            catch (Exception exception)
            {
                _logger.ErrorException(string.Format("ProcessInstitutionForUser exception | email:{0}", email), exception);
            }

            if (institutionsList != null && institutionsList.Count == 1)
            {
                return institutionsList.First();
            }

            var newInstitution = new Institution
            {
                InstitutionName = institutionName,
                City = city,
                State = state,
                Zip = zip,
                RegIdentifier = regIdentifier,
                InstitutionType = institutionType,
                domainName = new string(email.SkipWhile(ltr => ltr != '@').Skip(1).ToArray())
            };

            _institutionRepository.Add(newInstitution);

            return newInstitution;
        }

        public Institution ProcessInstitutionForUserFromLegacy(string institutionName,
            string email,
            string city,
            string state,
            string regIdentifier,
            string institutionType,
            string zip)
        {
            List<Institution> institutionsList = null;

            try
            {
                var institutions = _institutionRepository.GetByNameAndZipCode(institutionName, zip);
                institutionsList = institutions.ToList();
            }
            catch (Exception exception)
            {
                _logger.ErrorException(string.Format("ProcessInstitutionForUserFromLegacy exception | email:{0}", email), exception);
            }

            if (institutionsList != null && institutionsList.Count == 1)
            {
                return institutionsList.First();
            }

            var newInstitution = new Institution
            {
                InstitutionName = institutionName,
                City = city,
                State = state,
                Zip = zip,
                RegIdentifier = regIdentifier,
                InstitutionType = institutionType,
                domainName = new string(email.SkipWhile(ltr => ltr != '@').Skip(1).ToArray())
            };

            _institutionRepository.Add(newInstitution);

            return newInstitution;

        }

        public void RemoveClaim(string tenant, string email, string claim, string claimValue = null)
        {
            var userAccount = GetUserAccountByEmail(tenant, email);

            if (string.IsNullOrWhiteSpace(claimValue))
                _userAccountService.RemoveClaim(userAccount.ID, claim);
            else
                _userAccountService.RemoveClaim(userAccount.ID, claim, claimValue);
        }

        public void ResetPassword(string tenant, string email)
        {
            try
            {
                _userAccountService.ResetPassword(tenant, email);
            }
            catch (Exception exception)
            {
                _logger.ErrorException("ResetPassword", exception);
                throw;
            }
        }

        public void SignIn(UserAccount userAccount, bool persistant)
        {
            try
            {
                _samAuthenticationService.SignIn(userAccount, persistant);
            }
            catch (Exception exception)
            {
                _logger.ErrorException("SignIn", exception);
                throw;
            }
        }

        public void AddClaim(UserAccount userAccount, string claimType, string claimValue)
        {
            _logger.Info("AddClaim - UserAccount: {0}, ClaimType: {1}, Value: {2}", userAccount.Email, claimType, claimValue);

            _userAccountService.AddClaim(
                userAccount.ID,
                claimType,
                claimValue
                );
        }

        public ValidationResult ValidatePostEventMaterialsAccessClaimValue(string claimValue, string claimType)
        {
            return _postEventMaterialsAccessClaimValidator.Validate(new Tuple<string, string>(claimType, claimValue));
        }

        public WebUser CreateExpressCheckoutUser(string tenant, string email, string firstName, string lastName, string phone, string institution, string title)
        {
            _logger.Info("CreateExpressCheckoutUser: {0}", email);
            var webUser = new WebUser
            {
                idUser = _refDataRepository.GetMaxWebUserId() + 1,
                AcctStatus = DomainConstants.New,
                UserType = UserType.Customer,
                DateCreated = DomainConstants.BuildUtcNowAsCts,
                FirstName = firstName,
                LastName = lastName,
                idUserInstitution = 8,
                email = email,
                timeZone = USTimeZone.Central,
                generalComments = "Origin: CreatedAtExpressCheckout"
            };

            _webUserRepository.Add(webUser);

            CreateUserFromCart(tenant, RandomHelpers.GetUniqueCode(8), email);

            return webUser;
        }

        public Address BuildPlaceHolderAddressBilling(string email)
        {
            var starterUser = _webUserRepository.GetWebUserByEmailDomain(email);
            if (starterUser != null && starterUser.Addresses.SingleOrDefault(a => a.AddressType == "Billing") != null)
            {
                Address addBilling = new Address { AddressType = "Billing" };

                var bill = starterUser.Addresses.SingleOrDefault(a => a.AddressType == "Billing");

                addBilling.City = bill.City;
                addBilling.Country = bill.Country;
                addBilling.Name = bill.Name;
                addBilling.Phone = bill.Phone;
                addBilling.State = bill.State;
                addBilling.StreetAddress = bill.StreetAddress;
                addBilling.StreetAddress2 = bill.StreetAddress2;
                addBilling.Zip = bill.Zip;

                return addBilling;
            }
            else
            {
                starterUser = _webUserRepository.GetWebUserLegacyByEmail(email);
                //starterUser = _webUserRepository.GetWebUserByEmail("placeholder@ttstrain.com");

                Address addressBilling = new Address { AddressType = "Billing" };

                var billing = starterUser.Addresses.SingleOrDefault(a => a.AddressType == "Billing");

                addressBilling.City = billing.City;
                addressBilling.Country = billing.Country;
                addressBilling.Name = billing.Name;
                addressBilling.Phone = billing.Phone;
                addressBilling.State = billing.State;
                addressBilling.StreetAddress = billing.StreetAddress;
                addressBilling.StreetAddress2 = billing.StreetAddress2;
                addressBilling.Zip = billing.Zip;

                return addressBilling;
            }
        }

        public Address BuildPlaceHolderAddressShipping(string email)
        {
            var starterUser = _webUserRepository.GetWebUserByEmailDomain(email);
            if (starterUser != null && starterUser.Addresses.SingleOrDefault(a => a.AddressType == "Shipping") != null)
            {
                Address addShipping = new Address { AddressType = "Shipping" };

                var bill = starterUser.Addresses.SingleOrDefault(a => a.AddressType == "Shipping");

                addShipping.City = bill.City;
                addShipping.Country = bill.Country;
                addShipping.Name = bill.Name;
                addShipping.Phone = bill.Phone;
                addShipping.State = bill.State;
                addShipping.StreetAddress = bill.StreetAddress;
                addShipping.StreetAddress2 = bill.StreetAddress2;
                addShipping.Zip = bill.Zip;

                return addShipping;
            }
            else
            {
                starterUser = _webUserRepository.GetWebUserByEmail("placeholder@ttstrain.com");

                Address addressShipping = new Address { AddressType = "Shipping" };

                var ship = starterUser.Addresses.SingleOrDefault(a => a.AddressType == "Shipping");

                addressShipping.City = ship.City;
                addressShipping.Country = ship.Country;
                addressShipping.Name = ship.Name;
                addressShipping.Phone = ship.Phone;
                addressShipping.State = ship.State;
                addressShipping.StreetAddress = ship.StreetAddress;
                addressShipping.StreetAddress2 = ship.StreetAddress2;
                addressShipping.Zip = ship.Zip;

                return addressShipping;
            }
        }

        public Institution GetInstitutionById(int idInstitution)
        {
            return _institutionRepository.GetById(idInstitution);
        }

        public void UpdateInstitutionDetails(Institution saveInst)
        {
            _institutionRepository.Update(saveInst);
        }

        public void AddAccountTypeNotVerifiedClaim(UserAccount userAccount, string accountType)
        {
            _logger.Info("AddAccountTypeNotVerifiedClaim: {0}", userAccount.Email);
            if (userAccount == null)
                throw new ArgumentNullException("userAccount");
            if (string.IsNullOrWhiteSpace(accountType))
                throw new ArgumentException("String parameter cannot be white space or null.", "accountType");

            try
            {
                _userAccountService.AddClaim(userAccount.ID, ClaimTypes.HasNotVerified, accountType);
            }
            catch (Exception exception)
            {
                _logger.ErrorException("AddAccountTypeNotVerifiedClaim", exception);
                throw;
            }
        }

        public bool ChangePasswordFromResetKey(string tenant, string key, string newPassword)
        {
            _logger.Info("ChangePasswordFromResetKey: {0}", key);
            try
            {
                //Should we clear UserNotVerified claims here?
                //var dataOperations = new DataOperations(ConfigurationManager.ConnectionStrings["MembershipReboot"].ConnectionString);

                var userAccount = _userAccountService.GetByVerificationKey(key);

                //dataOperations.SetFieldsConsistantWithVerifiedUser(userAccount);
                RemoveClaim(tenant, userAccount.Email, ClaimTypes.HasNotVerified);

                return _userAccountService.ChangePasswordFromResetKey(key, newPassword);
            }
            catch (Exception exception)
            {
                _logger.ErrorException("ChangePasswordFromResetKey", exception);
                throw;
            }
        }

        private UserAccount GetUserAccountByVerificationKey(string tenant, string key)
        {
            if (tenant == null) throw new ArgumentNullException("tenant");
            if (key == null) throw new ArgumentNullException("key");

            var dataOperations = new DataOperations(ConfigurationManager.ConnectionStrings["MembershipReboot"].ConnectionString);

            var email = dataOperations.GetUserEmailByVerificationKey(tenant, key);

            return _userAccountService.GetByEmail(tenant, email);
        }

        public void UpdateNameTitle(string firstName, string lastName, string email, string title)
        {
            try
            {
                var webUser = GetDetailsOfUser(email);

                var auditChanges = new StringBuilder();

                auditChanges.Append("Record edited on " + DomainConstants.BuildUtcNowAsCts + Environment.NewLine);

                _webUserRepository.Update(webUser);
            }
            catch (Exception exception)
            {
                _logger.ErrorException("UpdateNameTitle", exception);

            }
        }

        public void UpdateUserDetails(string tenant, string firstName, string lastName, string email, string institutionName, Address billingAddress, Address shippingAddress, string title, int? sageAccountId)
        {
            _logger.Info("UpdateUserDetails: {0}", email);
            //if (_userAccountService.AuthenticateWithEmail(tenant, email, password))
            //{
            // removed authenticate requirement to permit Admin editing
            // of accounts
            try
            {
                var webUser = GetDetailsOfUser(email);

                var auditChanges = new StringBuilder();

                auditChanges.Append("Record edited on " + DomainConstants.BuildUtcNowAsCts.ToShortDateString() + " " + DomainConstants.BuildUtcNowAsCts.ToShortTimeString() + Environment.NewLine);


                var billingAddressFromDb = webUser.Addresses.SingleOrDefault(a => a.AddressType == DomainConstants.BillingAddress);

                if (billingAddressFromDb == null)
                {

                    billingAddressFromDb = billingAddress;

                    webUser.Addresses.Add(billingAddressFromDb);
                }

                if (!(firstName.Equals(webUser.FirstName, StringComparison.OrdinalIgnoreCase)))
                {
                    auditChanges.Append("firstName from: " + webUser.FirstName + " to: " + firstName + Environment.NewLine);
                }

                if (!(lastName.Equals(webUser.LastName, StringComparison.OrdinalIgnoreCase)))
                {
                    auditChanges.Append("lastName from: " + webUser.LastName + " to: " + lastName + Environment.NewLine);
                }

                if (!(title.Equals(webUser.Title, StringComparison.OrdinalIgnoreCase)))
                {
                    auditChanges.Append("title from: " + webUser.Title + " to: " + title + Environment.NewLine);
                }

                if (!(firstName.Equals(webUser.FirstName, StringComparison.OrdinalIgnoreCase))
                    || !(lastName.Equals(webUser.LastName, StringComparison.OrdinalIgnoreCase))
                    )
                {
                    billingAddress.Name = string.Format("{0} {1}", firstName, lastName);
                    _userAccountService.RemoveClaim(_userAccountService.GetByEmail(tenant, webUser.email).ID, ClaimTypes.FullName);
                    _userAccountService.AddClaim(_userAccountService.GetByEmail(tenant, webUser.email).ID, ClaimTypes.FullName, string.Format("{0} {1}", firstName, lastName));

                }

                webUser.email = email;
                webUser.FirstName = firstName;
                webUser.LastName = lastName;
                webUser.Title = title;
                //if (!(webUser.Institution.InstitutionName.Equals(institutionName, StringComparison.OrdinalIgnoreCase)))
                //    //&&
                //    //billingAddress.State.Equals(billingAddressFromDb.State, StringComparison.OrdinalIgnoreCase) &&
                //    //billingAddress.Zip.Equals(billingAddressFromDb.Zip, StringComparison.OrdinalIgnoreCase)) ||
                //    //!webUser.Institution.InstitutionName.Equals(institutionName, StringComparison.OrdinalIgnoreCase))
                //{
                //    webUser.Institution = ProcessInstitutionForUser(institutionName, email, billingAddress.City, billingAddress.State, "N", "New", billingAddress.Zip);
                //}


                _webUserRepository.Update(webUser);
                if (!(billingAddress.City.Equals(billingAddressFromDb.City, StringComparison.OrdinalIgnoreCase)))
                {
                    auditChanges.Append("City from: " + billingAddressFromDb.City + " to: " + billingAddress.City + Environment.NewLine);
                }
                if (!(billingAddress.StreetAddress.Equals(billingAddressFromDb.StreetAddress, StringComparison.OrdinalIgnoreCase)))
                {
                    auditChanges.Append("StreetAddress from: " + billingAddressFromDb.StreetAddress + " to: " + billingAddress.StreetAddress + Environment.NewLine);
                }
                if (billingAddress.StreetAddress2 != null && !(billingAddress.StreetAddress2.Equals(billingAddressFromDb.StreetAddress2, StringComparison.OrdinalIgnoreCase)))
                {
                    auditChanges.Append("StreetAddress2 from: " + billingAddressFromDb.StreetAddress2 + " to: " + billingAddress.StreetAddress2 + Environment.NewLine);
                }
                if (!(billingAddress.State.Equals(billingAddressFromDb.State, StringComparison.OrdinalIgnoreCase)))
                {
                    auditChanges.Append("State from: " + billingAddressFromDb.State + " to: " + billingAddress.State + Environment.NewLine);
                }
                if (!(billingAddress.Phone.Equals(billingAddressFromDb.Phone, StringComparison.OrdinalIgnoreCase)))
                {
                    auditChanges.Append("Phone from: " + billingAddressFromDb.Phone + " to: " + billingAddress.Phone + Environment.NewLine);
                }
                if (!(billingAddress.Zip.Equals(billingAddressFromDb.Zip, StringComparison.OrdinalIgnoreCase)))
                {
                    auditChanges.Append("Zip from: " + billingAddressFromDb.Zip + " to: " + billingAddress.Zip + Environment.NewLine);
                }
                if (!(billingAddress.Country.Equals(billingAddressFromDb.Country, StringComparison.OrdinalIgnoreCase)))
                {
                    auditChanges.Append("Country from: " + billingAddressFromDb.Country + " to: " + billingAddress.Country + Environment.NewLine);
                }


                billingAddressFromDb.Name = billingAddress.Name;
                billingAddressFromDb.City = billingAddress.City;
                billingAddressFromDb.StreetAddress = billingAddress.StreetAddress;
                billingAddressFromDb.StreetAddress2 = billingAddress.StreetAddress2;
                billingAddressFromDb.State = billingAddress.State;
                billingAddressFromDb.Phone = billingAddress.Phone;
                billingAddressFromDb.Zip = billingAddress.Zip;
                billingAddressFromDb.Country = billingAddress.Country;
                _webUserRepository.Update(webUser);
                //  ** Important ** update this address object before grabbing the next one from the context.
                //  Doing so is important as the entity.state of the object needs to be either detached or modified, but NOT unchanged. 
                _webUserRepository.UpdateAddresses(billingAddressFromDb);


                var shippingAddressFromDb = webUser.Addresses.SingleOrDefault(a => a.AddressType == DomainConstants.ShippingAddress);

                if (shippingAddressFromDb == null)
                {
                    shippingAddressFromDb = shippingAddress;
                    webUser.Addresses.Add(shippingAddressFromDb);
                }

                if (!(shippingAddress.Name.Equals(shippingAddressFromDb.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    auditChanges.Append("Shipping Name from: " + shippingAddressFromDb.Name + " to: " + shippingAddress.Name + Environment.NewLine);
                }

                if (!(shippingAddress.City.Equals(shippingAddressFromDb.City, StringComparison.OrdinalIgnoreCase)))
                {
                    auditChanges.Append("Shipping City from: " + shippingAddressFromDb.City + " to: " + shippingAddress.City + Environment.NewLine);
                }
                if (!(shippingAddress.StreetAddress.Equals(shippingAddressFromDb.StreetAddress, StringComparison.OrdinalIgnoreCase)))
                {
                    auditChanges.Append("Shipping StreetAddress from: " + shippingAddressFromDb.StreetAddress + " to: " + shippingAddress.StreetAddress + Environment.NewLine);
                }
                if (shippingAddress.StreetAddress2 != null && !(shippingAddress.StreetAddress2.Equals(shippingAddressFromDb.StreetAddress2, StringComparison.OrdinalIgnoreCase)))
                {
                    auditChanges.Append("Shipping StreetAddress2 from: " + shippingAddressFromDb.StreetAddress2 + " to: " + shippingAddress.StreetAddress2 + Environment.NewLine);
                }
                if (!(shippingAddress.State.Equals(shippingAddressFromDb.State, StringComparison.OrdinalIgnoreCase)))
                {
                    auditChanges.Append("Shipping State from: " + shippingAddressFromDb.State + " to: " + shippingAddress.State + Environment.NewLine);
                }
                if (!(shippingAddress.Phone.Equals(shippingAddressFromDb.Phone, StringComparison.OrdinalIgnoreCase)))
                {
                    auditChanges.Append("Shipping Phone from: " + shippingAddressFromDb.Phone + " to: " + shippingAddress.Phone + Environment.NewLine);
                }
                if (!(shippingAddress.Zip.Equals(shippingAddressFromDb.Zip, StringComparison.OrdinalIgnoreCase)))
                {
                    auditChanges.Append("Shipping Zip from: " + shippingAddressFromDb.Zip + " to: " + shippingAddress.Zip + Environment.NewLine);
                }
                if (!(shippingAddress.Country.Equals(shippingAddressFromDb.Country, StringComparison.OrdinalIgnoreCase)))
                {
                    auditChanges.Append("Shipping Country from: " + shippingAddressFromDb.Country + " to: " + shippingAddress.Country + Environment.NewLine);
                }


                shippingAddressFromDb.Name = shippingAddress.Name;
                shippingAddressFromDb.City = shippingAddress.City;
                shippingAddressFromDb.StreetAddress = shippingAddress.StreetAddress;
                shippingAddressFromDb.StreetAddress2 = shippingAddress.StreetAddress2;
                shippingAddressFromDb.State = shippingAddress.State;
                shippingAddressFromDb.Phone = shippingAddress.Phone;
                shippingAddressFromDb.Zip = shippingAddress.Zip;
                shippingAddressFromDb.Country = shippingAddress.Country;
                _webUserRepository.Update(webUser);
                _webUserRepository.UpdateAddresses(shippingAddressFromDb);

                webUser.Addresses.Clear();
                webUser.Addresses.Add(billingAddressFromDb);
                webUser.Addresses.Add(shippingAddressFromDb);

                // This string has blown out a couple of times. This following code ensures it does not exceed the max size of the database column.
                // I'll update field to VarChar(max) - just not worth skimping. Let's ensure we apply same to all *Comments fields
                // TODO: Convert comments using the newly adopted 'everything in JSON' pattern.
                // serialize webUser and auditChanges to json, right?
                var comments = (auditChanges + Environment.NewLine + "--------" +
                                Environment.NewLine + webUser.generalComments);
                webUser.generalComments = comments.Length < 1000 ? comments : comments.Substring(0, 1000);
                webUser.SageAccountId = sageAccountId.Value;

                _webUserRepository.Update(webUser);
            }
            catch (Exception exception)
            {
                _logger.ErrorException("UpdateUserDetails", exception);
            }
        }
        public void UpdateUserDetails(WebUser webUser)
        {
            _webUserRepository.Update(webUser);
        }

        public string GetDisplayPostEventMaterialsClaimValue(string tenant, string email, int idOrder, string onDemandCode)
        {
            return GetDisplayPostEventMaterialsClaimValue(GetUserAccountByEmail(tenant, email), idOrder, onDemandCode);
        }


        public string GetDisplayPostEventMaterialsClaimValue(UserAccount userAccount, int idOrder, string onDemandCode)
        {
            if (userAccount == null) throw new ArgumentNullException("userAccount");

            var claim = userAccount.Claims
                .Where( c=> c.Type == ClaimTypes.PostEventMaterials || c.Type == ClaimTypes.PostEventMaterialsExtended)
                .FirstOrDefault(c => c.Value.ToLower().Contains(idOrder.ToString()) && c.Value.Contains(onDemandCode));

            if (ReferenceEquals(null, claim))
            {
                _logger.Warn("OnDemandCode not found. " + onDemandCode + " for " + userAccount.Email);
            }

            return !ReferenceEquals(null, claim) ? claim.Value : null;
        }

        public UserAccount VerifyEmailFromKey(string key, string password)
        {
            _logger.Info("VerifyEmailFromKey: {0}", key);
            UserAccount userAccount = null;

            try
            {
                _userAccountService.VerifyEmailFromKey(key, password, out userAccount);
            }
            catch (Exception exception)
            {
                _logger.ErrorException("VerifyEmailFromKey", exception);

            }
            return userAccount;
        }

        public USTimeZone GetTimeZoneByZip()
        {

            //todo: [sjh] This is a problem needing resolution. Ensure the RUIC (the dropdown on user creation) relfects this value. 
            // REMOVING THIS BRAKES Handle(RegisterNewAccountCommand command)  - can we refactor? [dar] I think this method is redundant. Refer to GetCityStateFromZip inAppHelper
            return USTimeZone.Central;
        }

        public void UpdatePostEventMaterialsClaim(string tenant, string email, DateTime newDate, Order order)
        {
            var userAccount = _userAccountService.GetByEmail(tenant, email);
            UpdatePostEventMaterialsClaim(userAccount, newDate, order);
        }

        public void UpdatePostEventMaterialsClaim(UserAccount userAccount, DateTime newDate, Order order)
        {
            _logger.Info("UpdatePostEventMaterialsClaim Email: {0}, DateToExpire: {1}, idOrder: {2}", userAccount.Email, newDate, order);
            var allPostEventMaterialsClaimsForUser = userAccount.Claims
                .Where(c => c.Type == ClaimTypes.PostEventMaterials
                || c.Type == ClaimTypes.PostEventMaterialsExtended);

            var claimForOrder = allPostEventMaterialsClaimsForUser
                .FirstOrDefault(c => c.Value.Contains(order.ToString()));

            if (!ReferenceEquals(null, claimForOrder))
            {
                JObject jsonParsedClaim = JObject.Parse(claimForOrder.Value);
                var dateJProperty = jsonParsedClaim.Property(JsonPropertyKeys.ExpiryDate);
                dateJProperty.Value = newDate.ToString(DomainConstants.ClaimDateFormatText);

                _userAccountService.RemoveClaim(userAccount.ID, ClaimTypes.PostEventMaterials,
                    claimForOrder.Value);

                _userAccountService.AddClaim(userAccount.ID, ClaimTypes.PostEventMaterialsExtended,
                    jsonParsedClaim.ToString(Formatting.None));
            }
            else
            {
                var onDemandCode = order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active)
                    .OnDemandCode;

                var orderIdProperty = new JProperty(JsonPropertyKeys.OrderId, order.idOrder);
                var expiryDateProperty = new JProperty(JsonPropertyKeys.ExpiryDate, newDate.ToString(DomainConstants.ClaimDateFormatText));
                var onDemandCodeProperty = new JProperty(JsonPropertyKeys.OnDemandCode, onDemandCode);

                var claimValue = new JObject(
                    orderIdProperty,
                    expiryDateProperty,
                    onDemandCodeProperty
                    );

                _userAccountService.AddClaim(userAccount.ID, ClaimTypes.PostEventMaterialsExtended, claimValue.ToString(Formatting.None));
            }
        }

        public void UpdateShippingAddressDetails(Address shippingAddress)
        {
            try
            {
                var webUser = _webUserRepository.FindByIdLoaded(shippingAddress.idUser);

                var shippingAddressOnFile = webUser.Addresses.SingleOrDefault(a => a.AddressType == DomainConstants.ShippingAddress);

                if (ReferenceEquals(null, shippingAddressOnFile))
                {
                    webUser.Addresses.Add(shippingAddress);
                }
                else
                {
                    shippingAddressOnFile.City = shippingAddress.City;
                    shippingAddressOnFile.Country = shippingAddress.Country;
                    shippingAddressOnFile.Name = shippingAddress.Name;
                    shippingAddressOnFile.Phone = shippingAddress.Phone;
                    shippingAddressOnFile.State = shippingAddress.State;
                    shippingAddressOnFile.StreetAddress = shippingAddress.StreetAddress;
                    shippingAddressOnFile.StreetAddress2 = shippingAddress.StreetAddress2;
                    shippingAddressOnFile.Zip = shippingAddress.Zip;
                }

                _webUserRepository.DbContext.SaveChanges();
            }
            catch (Exception exception)
            {
                _logger.ErrorException("UpdateShippingAddressDetails", exception);
            }
        }

        public bool UserHasClaim(UserAccount userAccount, string claim, string value = null)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return userAccount.HasClaim(claim);
            }
            return userAccount.HasClaim(claim, value);
        }

        public WebUser CreateBareUserFromEmail(string email)
        {
            _logger.Info("CreateBareUserFromEmail: {0}", email);
            var webUser = new WebUser
            {
                idUser = _refDataRepository.GetMaxWebUserId() + 1,
                AcctStatus = DomainConstants.New,
                UserType = UserType.Customer,
                DateCreated = DomainConstants.BuildUtcNowAsCts,
                FirstName = "Impromptu",
                LastName = "User",
                idUserInstitution = 8,
                email = email,
                timeZone = USTimeZone.Central,
                generalComments = "Origin: CreateBareUserFromEmail"
            };
            webUser.Addresses = new List<Address>();
            Address billing = BuildPlaceHolderAddressBilling(email);
            Address shipping = BuildPlaceHolderAddressShipping(email);

            webUser.Addresses.Add(billing);
            webUser.Addresses.Add(shipping);
            _webUserRepository.Add(webUser);

            return webUser;
        }

        public string FindDisplayPostEventMaterialsClaimValue(Order order)
        {
            if (order == null) throw new ArgumentNullException("order");
            var userAccount = _userAccountService.GetByEmail(order.BillingEmail);

            var claim = userAccount.Claims.FirstOrDefault(c => c.Type == ClaimTypes.PostEventMaterials
                && c.Value.Contains(order.idOrder.ToString())
                );

            return !ReferenceEquals(null, claim) ? claim.Value : null;
        }


        public IEnumerable<Address> GetAddressesForUser(int id)
        {
            try
            {
                return _refDataRepository.GetAddressesForUser(id);
            }
            catch (Exception exception)
            {
                _logger.ErrorException("GetAddressesForUser", exception);
            }
            return null;
        }

        public Institution GetInstitutionForUser(int id)
        {
            try
            {
                return _refDataRepository.GetInstitutionForUser(id);
            }
            catch (Exception exception)
            {
                _logger.ErrorException("GetInstitutionForUser", exception);
            }
            return null;
        }

        public UserAccount GetUserAccountByUserId(Guid userId)
        {
            try
            {
                return _userAccountService.GetByID(userId);
            }
            catch (Exception exception)
            {
                _logger.ErrorException("GetUserAccountByUserId", exception);
            }
            return null;
        }

        public bool VerifyUserByEmail(string tenant, string email)
        {
            if (tenant == null) throw new ArgumentNullException("tenant");
            if (email == null) throw new ArgumentNullException("email");
            try
            {
                var account = _userAccountService.GetByEmail(tenant, email);

                if (account.HasClaim(ClaimTypes.HasNotVerified))
                {
                    _userAccountService.RemoveClaim(account.ID, ClaimTypes.HasNotVerified);
                    return true;
                }

                return false;
            }
            catch (Exception exception)
            {
                _logger.ErrorException("VerifyUserByEmail", exception);
            }

            return false;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                var logger = _logger as IDisposable;
                if (logger != null)
                    logger.Dispose();

                _institutionRepository.Dispose();
                _webUserRepository.Dispose();

            }
            _disposed = true;
        }
    }
}
