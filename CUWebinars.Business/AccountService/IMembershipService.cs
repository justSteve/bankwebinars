using System.Collections.Generic;
using CUWebinars.Business.Models;
using BrockAllen.MembershipReboot;
using System;
using FluentValidation.Results;

namespace CUWebinars.Business.AccountService
{
    public interface IMembershipService : IDisposable
    {
        void AddClaim(UserAccount userAccount, string claimType, string claimValue);
        void AddAccountTypeNotVerifiedClaim(UserAccount userAccount, string accountType);
        bool ChangePasswordFromResetKey(string tenant, string key, string newPassword);
        //string CheckDisplayPostEventMaterials(string tenant, string email, out string messageIfFalse);
        void CleanUser(string tenant, string email, string newPassword);
        UserAccount CreateUser(
            string tenant,
            string firstName,
            string lastName,
            string userName,
            string password,
            string email
            );

        UserAccount CreateUserFromCart(string tenant, string password, string email);

        WebUser CreateWebUser(
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
            string accountStatus = null);
        IEnumerable<Address> GetAddressesForUser(int id);
        WebUser GetDetailsOfUser(string email);
        Institution GetInstitutionByDomain(string domain);
        IEnumerable<Institution> GetInstitutionsByName(string name);
        DateTime? GetPostEventAccessExpireyDate(UserAccount userAccount, int idOrder);
        USTimeZone GetTimeZoneByZip();
        UserAccount GetUserAccountByEmail(string tenant, string email);
        UserAccount GetUserAccountByUserId(Guid userId);
        UserAccount GetUserAccountByWebUserId(string tenant, int userId);
        WebUser GetUserByEmail(string email);
        WebUser GetUserByEmailLoadedWithOrdersData(string email);
        WebUser GetWebUserById(int userId);
        int? GetWebUserIdByEmail(string email);
        IEnumerable<WebUser> GetWebUsersByLastNameForAffiliate(string lastName, int idAffiliate);
        bool HasPassword(string tenant, string emailAddress);
        bool LogInUser(string tenant, string emailAddress, string password, bool persistent);
        bool LogInUser(string tenant, string emailAddress, string password, bool persistent, out string userMustVerify);
        bool LogInAdminUserAsOtherUser(string tenant, string emailAddress, string password, UserAccount account);
        bool LogOutUser();
        Institution ProcessInstitutionForUser(string institutionName,
            string email,
            string city,
            string state,
            string regIdentifier,
            string institutionType,
            string zip);

        void RemoveClaim(string tenant, string email, string claim, string claimValue = null);
        void ResetPassword(string tenant, string email);
        void SignIn(UserAccount userAccount, bool persistant);
        void UpdateNameTitle(
            string firstName,
            string lastName,
            //string password,
            string email,
            string title);
        void UpdateUserDetails(string tenant, string firstName, string lastName, string email, string institutionName, Address billingAddress, Address shippingAddress, string title);
        void UpdateUserDetails(WebUser webUser);
        string GetDisplayPostEventMaterialsClaimValue(string tenant, string email, int idOrder, string onDemandCode);
        //string GetDisplayPostEventMaterialsClaimValue(UserAccount userAccount);
        UserAccount VerifyEmailFromKey(string key, string password);
        bool VerifyUserByEmail(string tenant, string email);
        void UpdateDisplayPostEventMaterialsClaim(string tenant, string email, DateTime newDate, Order order);
        void UpdateDisplayPostEventMaterialsClaim(UserAccount userAccount, DateTime newDate, Order order);
        void UpdateShippingAddressDetails(Address shippingAddress);
        void UpdateDiscountDetails(Discount discount);
        bool UserHasClaim(UserAccount userAccount, string claim, string value = null);
        WebUser CreateBareUserFromEmail(string email);
        string FindDisplayPostEventMaterialsClaimValue(Order order);
        UserAccount GetUserAccountByVerificationKey(string key);
        ValidationResult ValidatePostEventMaterialsAccessClaimValue(string claimValue, string claimType);
        WebUser CreateExpressCheckoutUser(string tenant, string email, string firstName, string lastName, string phone, string institution, string title);
        Address BuildPlaceHolderAddressBilling(string email);
        Address BuildPlaceHolderAddressShipping(string email);
    }
}
