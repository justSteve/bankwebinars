
using System;
using System.Collections.Generic;
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
        void BuildCityStateTimeZoneData(Dictionary<string, string> cityStateTimeZoneData, string zipAddress);
        bool ChangePasswordFromResetKey(string key, string password);
        Institution GetInstitutionFromEmail(string email);
        WebUser GetWebUserByEmail(string email);
        WebUser GetWebUserFromIPrincipal();
        string GetZipAddress(int zip);
        LoginModel LogUserIn(string returnUrl);
        void LogUserOut();
        int? ParseZip(string zip);
        void ResetPassword(string tenant, string email);
        bool SignUserIn(SignInModel model, out string userMustVerify);
        bool UserConfirmed(CreateUserConfirmedViewModel model);
    }
}