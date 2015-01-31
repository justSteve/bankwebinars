
using System;
using System.Collections.Generic;
using BrockAllen.MembershipReboot;
using CUWebinars.Business.Models;
using CUWebinars.Web.Models;
using CUWebinars.Web.ViewModel;

namespace CUWebinars.Web.Core.Orchestrators
{
    public interface IAccountControllerOrchestrator : IDisposable
    {
        void AddPasswordForCartCreatedUser(CreateUserConfirmedViewModel model);
        EditBillingAddressModel BuildBillingAddressModel();
        EditShippingAddressModel BuildShippingAddressModel();
        void BuildCityStateTimeZoneData(Dictionary<string, string> cityStateTimeZoneData, string zipAddress);
        bool ChangePasswordFromResetKey(string key, string password);
        CreateUserConfirmedViewModel ConfirmUser(string email, string surname);
        string CreateUserAccountFromCart(RegisterViewModel model);
        WebUser CreateWebUserFromCart(RegisterViewModel model);
        void EditContactInfo(EditContactInfoModel editContactInfoModel);
        CreateUserConfirmedViewModel GetCreateUserConfirmedViewModel(string email, bool viaBillMePostRequest = false);
        Institution GetInstitutionFromEmail(string email);
        IEnumerable<Institution> GetInstitutionsByName(string name);
        WebUser GetWebUserByEmail(string email);
        WebUser GetWebUserById(int id);
        WebUser GetWebUserFromIPrincipal();
        string GetZipAddress(int zip);
        LoginModel BuildLoginModel(string returnUrl);
        bool LogUserIn(SignInModel signInModel);
        void LogUserOut();
        int? ParseZip(string zip);
        CreateUserConfirmedViewModel PrepareViewForCartUserAddingPassword(string email, bool viaBillMePostRequest = false);
        void RegisterAndLogInUser(RegisterViewModel registerViewModel);
        void ResetPassword(string tenant, string email);
        bool SignUserIn(SignInModel model, out string userMustVerify);
        void UpdateBillingEmailOfOrder(int idOrder, string email);
        bool UserConfirmed(CreateUserConfirmedViewModel model);
        void UpdateNameTitle(string firstName, string lastName, string email, string title);
        void UpdateUserDetails(ManageModel model);
        void UpdateShippingAddressDetails(AddressModel shippingAddressModel, int idUser);
        void UpdateDiscountDetails(DiscountModel discount, int userId);
    }
}