
using System;
using CUWebinars.Business.Models;
using CUWebinars.Web.Models;
using CUWebinars.Web.ViewModel;

namespace CUWebinars.Web.Core.Orchestrators
{
    public interface IAccountControllerOrchestrator : IDisposable
    {
        CreateUserConfirmedViewModel ConfirmUser(string email, string surname);
        EditBillingAddressModel BuildBillingAddressModel();
        EditShippingAddressModel BuildShippingAddressModel();
        bool ChangePasswordFromResetKey(string key, string password);
        WebUser GetWebUserByEmail(string email);
        WebUser GetWebUserFromIPrincipal();
        LoginModel LogUserIn(string returnUrl);
        void LogUserOut();
        void ResetPassword(string tenant, string email);
        bool SignUserIn(SignInModel model, out string userMustVerify);
        bool UserConfirmed(CreateUserConfirmedViewModel model);
    }
}