
using BrockAllen.MembershipReboot;
using CUWebinars.Business.Constants;
using CUWebinars.Business.Models;
using CUWebinars.Business.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CUWebinars.Business.AccountService
{
    public class MembershipService : IMembershipService
    {
        private readonly IInstitutionRepository _institutionRepository;
        private readonly IRefDataRepository _refDataRepository;
        private readonly AuthenticationService _samAuthenticationService;
        private readonly UserAccountService _userAccountService;
        private readonly IWebUserRepository _webUserRepository;


        public MembershipService(IInstitutionRepository institutionRepository,
            IRefDataRepository refDataRepository,
            AuthenticationService samAuthenticationService,
            UserAccountService userAccountService,
            IWebUserRepository webUserRepository)
        {
            _institutionRepository = institutionRepository;
            _refDataRepository = refDataRepository;
            _samAuthenticationService = samAuthenticationService;
            _userAccountService = userAccountService;
            _webUserRepository = webUserRepository;
        }

        public UserAccount GetByVerificationKey(string id)
        {
            return _userAccountService.GetByVerificationKey(id);
        }

        public WebUser GetDetailsOfUser(string email)
        {
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
            var webUser = _webUserRepository.GetWebUserByEmail(email);
            return webUser;
        }

        public Institution GetInstitutionByDomain(string domain)
        {
            var institution = _institutionRepository.GetAll().FirstOrDefault(i => i.domainName == domain);
            return institution;
        }

        public bool HasPassword(string tenant, string emailAddress)
        {
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
            //TODO: Needs another check for email in use condition
            //   or a bulletproof method of ensuring that for every Membership UserAccount created
            //   a WebUser account as also been created.
            var account = _userAccountService.CreateAccount(tenant, userName, password, email);
            _userAccountService.AddClaim(account.ID, ClaimTypes.FullName, string.Format("{0} {1}", firstName, lastName));
            _userAccountService.AddClaim(account.ID, System.Security.Claims.ClaimTypes.Role, "WebUser");
            _userAccountService.AddClaim(account.ID, ClaimTypes.HasNotVerified, "true");

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
            if (idUserImported == 0) idUserImported = null;

            var webUser = new WebUser
            {
                idUser = idUserImported.HasValue ? idUserImported.Value : _refDataRepository.GetMaxWebUserId() + 1,
                // Check what default should be for nun-nullable field
                AcctStatus = accountStatus ?? "A",
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
            List<Institution> institution = null;

            try
            {
                var i = _institutionRepository.GetByNameAndZipCode(institutionName, zip);
                institution = i.ToList();
            }
            catch (Exception exception)
            {
                string bla = exception.Message;
            }

            if (institution.Count == 1)
            {
                return institution.First();
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
            //TODO: are calls to this method logged my Membership Reboot?
            try
            {
                _userAccountService.ResetPassword(tenant, email);
            }
            catch (Exception exception)
            {
                var a = 2;
                throw;
            }
        }

        public void SignIn(UserAccount userAccount, bool persistant)
        {
            _samAuthenticationService.SignIn(userAccount, persistant);
        }

        public bool ChangePasswordFromResetKey(string key, string newPassword)
        {
            var userAccount = _userAccountService.GetByVerificationKey(key);
            //_userAccountService.RemoveClaim(userAccount.ID, ClaimTypes.HasNotVerified);
            return _userAccountService.ChangePasswordFromResetKey(key, newPassword);
        }

        public void UpdateUserDetails(
            string tenant,
            string firstName,
            string lastName,
            string password,
            string email,
            string institutionName,
            Address billingAddress,
            Address shippingAddress,
            string title
            )
        {
            if (_userAccountService.AuthenticateWithEmail(tenant, email, password))
            {
                var webUser = GetDetailsOfUser(email);

                var billingAddressFromDb = webUser.Addresses.Where(a => a.AddressType == DomainConstants.BillingAddress).Single();

                if (!(billingAddress.City.Equals(billingAddressFromDb.City, StringComparison.OrdinalIgnoreCase) &&
                    billingAddress.State.Equals(billingAddressFromDb.State, StringComparison.OrdinalIgnoreCase) &&
                    billingAddress.Zip.Equals(billingAddressFromDb.Zip, StringComparison.OrdinalIgnoreCase)) ||
                    !webUser.Institution.InstitutionName.Equals(institutionName, StringComparison.OrdinalIgnoreCase))
                {

                    webUser.Institution = ProcessInstitutionForUser(institutionName, email, billingAddress.City, billingAddress.State, "N", "New", billingAddress.Zip);
                }

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

                var shippingAddressFromDb = webUser.Addresses.Where(a => a.AddressType == DomainConstants.ShippingAddress).Single();
                shippingAddressFromDb.City = shippingAddress.City;
                shippingAddressFromDb.StreetAddress = shippingAddress.StreetAddress;
                shippingAddressFromDb.StreetAddress2 = shippingAddress.StreetAddress2;
                shippingAddressFromDb.State = shippingAddress.State;
                shippingAddressFromDb.Phone = shippingAddress.Phone;
                shippingAddressFromDb.Zip = shippingAddress.Zip;
                shippingAddressFromDb.Country = shippingAddress.Country;

                webUser.Addresses.Clear();
                webUser.Addresses.Add(billingAddressFromDb);
                webUser.Addresses.Add(shippingAddressFromDb);

                webUser.email = email;
                webUser.FirstName = firstName;
                webUser.LastName = lastName;
                webUser.Title = title;

                _webUserRepository.UpdateAddresses(shippingAddressFromDb);
                _webUserRepository.Update(webUser);
            }
        }

        public UserAccount VerifyEmailFromKey(string key, string password)
        {
            UserAccount userAccount;
            _userAccountService.VerifyEmailFromKey(key, password, out userAccount);
            _userAccountService.RemoveClaim(userAccount.ID, ClaimTypes.HasNotVerified);

            return userAccount;
        }

        public USTimeZone GetTimeZoneByZip()
        {
            return USTimeZone.Central;
        }

        public IEnumerable<Address> GetAddressesForUser(int id)
        {
            return _refDataRepository.GetAddressesForUser(id);
        }

        public UserAccount GetUserAccountByUserId(Guid userId)
        {
            return _userAccountService.GetByID(userId);
        }
    }
}
