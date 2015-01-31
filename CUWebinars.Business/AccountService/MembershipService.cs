
using BrockAllen.MembershipReboot;
using CUWebinars.Business.Constants;
using CUWebinars.Business.Models;
using CUWebinars.Business.Repository;
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
        private readonly ILogger _logger;
        private bool _disposed;

        public MembershipService(IInstitutionRepository institutionRepository,
            IRefDataRepository refDataRepository,
            AuthenticationService samAuthenticationService,
            UserAccountService userAccountService,
            IWebUserRepository webUserRepository,
            ILogger logger)
        {
            _institutionRepository = institutionRepository;
            _refDataRepository = refDataRepository;
            _samAuthenticationService = samAuthenticationService;
            _userAccountService = userAccountService;
            _webUserRepository = webUserRepository;
            _logger = logger;
        }

        public WebUser GetDetailsOfUser(string email)
        {
            if (email == null) throw new ArgumentNullException("email");
            var webUser = _refDataRepository.GetWebUserByEmail(email);
            return webUser;
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

        public WebUser GetWebUserById(int userId)
        {
            return _webUserRepository.FindById(userId);
        }

        public IEnumerable<WebUser> GetWebUsersByLastName(string lastName)
        {
            return _webUserRepository.GetWebUsersByLastName(lastName);
        }

        public IEnumerable<Institution> GetInstitutionsByName(string name)
        {
            return _institutionRepository.GetInstitutionsByName(name);
        }

        public UserAccount GetUserAccountByEmail(string tenant, string email)
        {
            if (tenant == null) throw new ArgumentNullException("tenant");
            if (email == null) throw new ArgumentNullException("email");
            // While attempting to naviate to the definition of GetByEmail 
            return _userAccountService.GetByEmail(tenant, email);
        }

        public Institution GetInstitutionByDomain(string domain)
        {
            if (domain == null) throw new ArgumentNullException("domain");
            var institution = _institutionRepository.GetAll().FirstOrDefault(i => i.domainName == domain);
            return institution;
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

            var webUser = new WebUser
            {
                idUser = idUserImported.HasValue ? idUserImported.Value : _refDataRepository.GetMaxWebUserId() + 1,
                // Check what default should be for non-nullable field -- A: default is 'New'.
                AcctStatus = accountStatus ?? DomainConstants.New,
                DateCreated = DateTime.Now,
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
            _userAccountService.AddClaim(
                userAccount.ID,
                claimType,
                claimValue
                );
        }

        public void AddAccountTypeNotVerifiedClaim(UserAccount userAccount, string accountType)
        {
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

        public bool ChangePasswordFromResetKey(string key, string newPassword)
        {
            try
            {
                return _userAccountService.ChangePasswordFromResetKey(key, newPassword);
            }
            catch (Exception exception)
            {
                _logger.ErrorException("ChangePasswordFromResetKey", exception);
                throw;
            }
        }

        public void UpdateNameTitle(string firstName, string lastName, string email, string title)
        {
            // TODO: [sjh] deal with during construct of User/Institution Editor [dar] Seems to be an unfinished method. Comes down from EditNameTitle in AccountController
            try
            {
                var webUser = GetDetailsOfUser(email);

                var auditChanges = new StringBuilder();

                auditChanges.Append("Record edited on " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + Environment.NewLine);

                _webUserRepository.Update(webUser);
            }
            catch (Exception exception)
            {
                _logger.ErrorException("UpdateNameTitle", exception);

            }
        }

        public void UpdateUserDetails(string tenant, string firstName, string lastName, string email, string institutionName, Address billingAddress, Address shippingAddress, string title)
        {

            //if (_userAccountService.AuthenticateWithEmail(tenant, email, password))
            //{
            // removed authenticate requirement to permit Admin editing
            // of accounts
            try
            {
                var webUser = GetDetailsOfUser(email);

                var auditChanges = new StringBuilder();

                auditChanges.Append("Record edited on " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + Environment.NewLine);


                var billingAddressFromDb = webUser.Addresses.Single(a => a.AddressType == DomainConstants.BillingAddress);


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
                    billingAddress.Name =  string.Format("{0} {1}", firstName, lastName);
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

                //  ** Important ** update this address object before grabbing the next one from the context.
                //  Doing so is important as the entity.state of the object needs to be either detached or modified, but NOT unchanged. 
                _webUserRepository.UpdateAddresses(billingAddressFromDb);


                var shippingAddressFromDb = webUser.Addresses.Single(a => a.AddressType == DomainConstants.ShippingAddress);

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
            
                _webUserRepository.UpdateAddresses(shippingAddressFromDb);

                webUser.Addresses.Clear();
                webUser.Addresses.Add(billingAddressFromDb);
                webUser.Addresses.Add(shippingAddressFromDb);

                // This string has blown out a couple of times. This following code ensures it does not exceed the max size of the database column.
                var comments = (auditChanges + Environment.NewLine + "--------" +
                                Environment.NewLine + webUser.generalComments);
                webUser.generalComments = comments.Length < 1000 ? comments : comments.Substring(0, 1000);


            
                _webUserRepository.Update(webUser);
            }
            catch (Exception exception)
            {
                _logger.ErrorException("UpdateUserDetails", exception);
            }
        }

        public UserAccount VerifyEmailFromKey(string key, string password)
        {
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
            //todo: [sjh] REMOVING THIS BRAKES Handle(RegisterNewAccountCommand command)  - can we refactor? [dar] I think this method is redundant. Refer to GetCityStateFromZip inAppHelper
            return USTimeZone.Central;
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

        public void UpdateDiscountDetails(Discount discount)
        {
            throw new NotImplementedException();
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
