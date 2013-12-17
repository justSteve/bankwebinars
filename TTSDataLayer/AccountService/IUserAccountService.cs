using System;
using BrockAllen.MembershipReboot;

namespace CUWebinars.Business.AccountService
{
    public interface IUserAccountService
    {
        bool Authenticate(string tenant, string username, string password);
        bool Authenticate(string tenant, string username, string password, out UserAccount account);
        bool Authenticate(string username, string password);
        bool Authenticate(string username, string password, out UserAccount account);
        bool AuthenticateWithCertificate(Guid accountID, System.Security.Cryptography.X509Certificates.X509Certificate2 certificate);
        bool AuthenticateWithCertificate(Guid accountID, System.Security.Cryptography.X509Certificates.X509Certificate2 certificate, out UserAccount account);
        bool AuthenticateWithCertificate(System.Security.Cryptography.X509Certificates.X509Certificate2 certificate);
        bool AuthenticateWithCertificate(System.Security.Cryptography.X509Certificates.X509Certificate2 certificate, out UserAccount account);
        bool AuthenticateWithCode(Guid accountID, string code);
        bool AuthenticateWithCode(Guid accountID, string code, out UserAccount account);
        bool AuthenticateWithEmail(string email, string password);
        bool AuthenticateWithEmail(string email, string password, out UserAccount account);
        bool AuthenticateWithEmail(string tenant, string email, string password);
        bool AuthenticateWithEmail(string tenant, string email, string password, out UserAccount account);
        bool AuthenticateWithUsernameOrEmail(string tenant, string userNameOrEmail, string password, out UserAccount account);
        bool AuthenticateWithUsernameOrEmail(string userNameOrEmail, string password, out UserAccount account);
        bool CancelNewAccount(string key);
        bool ChangeEmailFromKey(Guid accountID, string password, string key, string newEmail);
        void ChangeEmailRequest(Guid accountID, string newEmail);
        bool ChangeMobilePhoneFromCode(Guid accountID, string code);
        void ChangeMobilePhoneRequest(Guid accountID, string newMobilePhoneNumber);
        void ChangePassword(Guid accountID, string oldPassword, string newPassword);
        bool ChangePasswordFromResetKey(string key, string newPassword);
        void ChangeUsername(Guid accountID, string newUsername);
        MembershipRebootConfiguration Configuration { get; set; }
        void ConfigureTwoFactorAuthentication(Guid accountID, TwoFactorAuthMode mode);
        UserAccount CreateAccount(string tenant, string username, string password, string email);
        UserAccount CreateAccount(string username, string password, string email);
        void DeleteAccount(Guid accountID);
        bool EmailExists(string email);
        bool EmailExists(string tenant, string email);
        System.Linq.IQueryable<UserAccount> GetAll();
        System.Linq.IQueryable<UserAccount> GetAll(string tenant);
        UserAccount GetByCertificate(string tenant, string thumbprint);
        UserAccount GetByCertificate(string thumbprint);
        UserAccount GetByEmail(string email);
        UserAccount GetByEmail(string tenant, string email);
        UserAccount GetByID(Guid id);
        UserAccount GetByLinkedAccount(string provider, string id);
        UserAccount GetByLinkedAccount(string tenant, string provider, string id);
        UserAccount GetByUsername(string tenant, string username);
        UserAccount GetByUsername(string username);
        UserAccount GetByVerificationKey(string key);
        bool IsPasswordExpired(UserAccount account);
        bool IsPasswordExpired(Guid accountID);
        void RemoveMobilePhone(Guid accountID);
        void ResetPassword(string email);
        void ResetPassword(string tenant, string email);
        void SendTwoFactorAuthenticationCode(Guid accountID);
        void SendUsernameReminder(string email);
        void SendUsernameReminder(string tenant, string email);
        void SetPassword(Guid accountID, string newPassword);
        void Update(UserAccount account);
        bool UsernameExists(string tenant, string username);
        bool UsernameExists(string username);
        bool VerifyAccount(string key);
    }
}
