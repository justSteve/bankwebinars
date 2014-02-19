using System.Collections.Generic;
using CUWebinars.Business.Models;
using BrockAllen.MembershipReboot;
using System;

namespace CUWebinars.Business.AccountService
{
    public interface IMembershipService
    {
        bool ChangePasswordFromResetKey(string key, string newPassword);
        WebUser CreateUser(
            string tenant,
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
            string accountStatus = null);

        IEnumerable<Address> GetAddressesForUser(int id);

        UserAccount GetByVerificationKey(string id);
        WebUser GetDetailsOfUser(string email);
        UserAccount GetUserAccountByUserId(Guid userId);
        WebUser GetUserByEmail(string email);
        Institution GetInstitutionByDomain(string domain);
        bool HasPassword(string tenant, string emailAddress);
        bool LogInUser(string tenant, string emailAddress, string password, bool persistent);
        bool LogOutUser();
        Institution ProcessInstitutionForUser(string institutionName,
            string city,
            string state,
            string regIdentifier,
            string institutionType,
            string zip);
        void ResetPassword(string tenant, string email);

        void SignIn(UserAccount userAccount, bool persistant);
        void UpdateUserDetails(string tenant, 
            string firstName,
            string lastName,
            string password,
            string email,
            string institutionName,
            Address billingAddress,
            Address shippingAddress,
            string title
            );

        UserAccount VerifyEmailFromKey(string key, string password);
    }
}
