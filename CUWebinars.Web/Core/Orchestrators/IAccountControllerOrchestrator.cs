
using System;
using CUWebinars.Web.Models;
using CUWebinars.Web.ViewModel;

namespace CUWebinars.Web.Core.Orchestrators
{
    public interface IAccountControllerOrchestrator : IDisposable
    {
        CreateUserConfirmedViewModel ConfirmUser(string email, string surname);
        LoginModel LogUserIn(string returnUrl);
        bool SignUserIn(SignInModel model, out string userMustVerify);
        bool UserConfirmed(CreateUserConfirmedViewModel model);
    }
}