using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.Ef;
using CUWebinars.Business.Repository;
using CUWebinars.Business.Models;
using CUWebinars.Business.Constants;

namespace CUWebinars.Business.AccountService
{
    public class MembershipService : IMembershipService
    {
        private readonly IInstitutionRepository institutionRepository;
        private readonly IRefDataRepository refDataRepository;
        private readonly AuthenticationService samAuthenticationService;
        private readonly UserAccountService userAccountService;
        private readonly IWebUserRepository webUserRepository;


        public MembershipService(IInstitutionRepository institutionRepository,
            IRefDataRepository refDataRepository,
            AuthenticationService samAuthenticationService,
            UserAccountService userAccountService,
            IWebUserRepository webUserRepository)
        {

            this.institutionRepository = institutionRepository;
            this.refDataRepository = refDataRepository;
            this.samAuthenticationService = samAuthenticationService;
            this.userAccountService = userAccountService;
            this.webUserRepository = webUserRepository;
        }

        public WebUser GetDetailsOfUser(string email)
        {
            var webUser = refDataRepository.GetWebUserByEmail(email);
            return webUser;
        }

        //public WebUser GetUserByUserName(string userName)
        //{
        //    var webUser = refDataRepository.GetWebUserByUserName(userName);
        //    return webUser;
        //}

        /// <summary>
        /// serves to check if email exists before attempting to create account.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public WebUser GetUserByEmail(string email)
        {
            var webUser = refDataRepository.GetWebUserByEmail(email);
            return webUser;
        }

        public bool HasPassword(string emailAddress)
        {
            var userAccount = userAccountService.GetByEmail(emailAddress);

            if (ReferenceEquals(null, userAccount))
            {
                return !string.IsNullOrEmpty(userAccount.HashedPassword);
            }
            return false;
        }

        public WebUser CreateUser(
            string firstName,
            string lastName,
            string userName,
            string password,
            string email,
            USTimeZone timeZone,
            UserType userType,
            int institutionId,
            IList<Address> addresses,
            string title,
            int? idUserImported,
            string accountStatus = null
            )
        {
            userName = userName.Replace(" ", "").Replace(".", "");
            var account = userAccountService.CreateAccount(userName, password, email);
            userAccountService.AddClaim(account.ID, CUWebinars.Business.Constants.ClaimTypes.FullName, string.Format("{0} {1}", firstName, lastName));

            //var webUserId = m
            //.GetAll().Where(c => c.InstitutionName == institutionName && c.Zip == zip).ToList();

            WebUser webUser = new WebUser
            {
                idUser = idUserImported.HasValue ? idUserImported.Value : refDataRepository.GetMaxWebUserId() + 1,
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

            webUserRepository.Add(webUser);
            return webUser;
        }

        public bool LogInUser(string emailAddress, string password)
        {
            if (userAccountService.AuthenticateWithEmail(emailAddress, password))
            {
                var userAccount = userAccountService.GetByEmail(emailAddress);
                samAuthenticationService.SignIn(userAccount);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool LogOutUser()
        {
            samAuthenticationService.SignOut();
            return true;
        }


        public Institution ProcessInstitutionForUser(string institutionName,
            string city,
            string state,
            string regIdentifier,
            string institutionType,
            string zip)
        {
            var institution =
                institutionRepository.GetAll().Where(c => c.InstitutionName == institutionName && c.Zip == zip).ToList();

            if (institution.Count == 1)
            {
                return institution.First();
            }

            var newInstitution = new Institution();

            newInstitution.InstitutionName = institutionName;
            newInstitution.City = city;
            newInstitution.State = state;
            newInstitution.Zip = zip;
            newInstitution.RegIdentifier = regIdentifier;
            newInstitution.InstitutionType = institutionType;

            institutionRepository.Add(newInstitution);

            return newInstitution;
        }

        public void ResetPassword(string email)
        {
            userAccountService.ResetPassword(email);
        }

        public bool ChangePasswordFromResetKey(string key, string newPassword)
        {
            return userAccountService.ChangePasswordFromResetKey(key, newPassword);
        }



        public void UpdateUserDetails(
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
            if (userAccountService.AuthenticateWithEmail(email, password))
            {                
                var webUser = GetDetailsOfUser(email);

                var billingAddressFromDb = webUser.Addresses.Where(a => a.AddressType == DomainConstants.BillingAddress).Single();

                if(!(billingAddress.City.Equals(billingAddressFromDb.City, StringComparison.OrdinalIgnoreCase) && 
                    billingAddress.State.Equals(billingAddressFromDb.State, StringComparison.OrdinalIgnoreCase) && 
                    billingAddress.Zip.Equals(billingAddressFromDb.Zip, StringComparison.OrdinalIgnoreCase)) ||
                    !webUser.Institution.InstitutionName.Equals(institutionName, StringComparison.OrdinalIgnoreCase))
                {

                    webUser.Institution = ProcessInstitutionForUser(institutionName, billingAddress.City, billingAddress.State, "N", "New", billingAddress.Zip);
                }

                billingAddressFromDb.City = billingAddress.City;
                billingAddressFromDb.StreetAddress = billingAddress.StreetAddress;
                billingAddressFromDb.StreetAddress2 = billingAddress.StreetAddress2;
                billingAddressFromDb.State = billingAddress.State;
                billingAddressFromDb.Phone = billingAddress.Phone ;
                billingAddressFromDb.Zip = billingAddress.Zip;
                billingAddressFromDb.Country = billingAddress.Country;

                //  ** Important ** update this address object before grabbing the next one from the context.
                //  Doing so is important as the entity.state of the object needs to be either detached or modified, but NOT unchanged. 
                webUserRepository.UpdateAddresses(billingAddressFromDb);

                var shippingAddressFromDb = webUser.Addresses.Where(a => a.AddressType == DomainConstants.ShippingAddress).Single();
                shippingAddressFromDb.City = shippingAddress.City;
                shippingAddressFromDb.StreetAddress = shippingAddress.StreetAddress;
                shippingAddressFromDb.StreetAddress2 = shippingAddress.StreetAddress2;
                shippingAddressFromDb.State = shippingAddress.State;
                shippingAddressFromDb.Phone = shippingAddress.Phone ;
                shippingAddressFromDb.Zip = shippingAddress.Zip;
                shippingAddressFromDb.Country = shippingAddress.Country;

                webUser.Addresses.Clear();
                webUser.Addresses.Add(billingAddressFromDb);
                webUser.Addresses.Add(shippingAddressFromDb);
                
                webUser.email = email;
                webUser.FirstName = firstName;
                webUser.LastName = lastName;
                webUser.Title = title;

                webUserRepository.UpdateAddresses(shippingAddressFromDb);
                webUserRepository.Update(webUser);
            }
        }

        public IEnumerable<Address> GetAddressesForUser(int id)
        {
            return refDataRepository.GetAddressesForUser(id);
        }


        public UserAccount GetUserAccountByUserId(Guid userId)
        {
            return userAccountService.GetByID(userId);            
        }
    }
}
