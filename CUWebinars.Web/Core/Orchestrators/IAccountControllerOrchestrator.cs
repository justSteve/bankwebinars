
using System.Security.Claims;
using CUWebinars.Business.Models;
using CUWebinars.Web.Infrastructure;
using CUWebinars.Web.Models;
using CUWebinars.Web.ViewModel;
using System;
using System.Collections.Generic;
using CUWebinars.Web.Models.DataTablesModels;

namespace CUWebinars.Web.Core.Orchestrators
{
    public interface IAccountControllerOrchestrator : IDisposable
    {
        void AddFullNameClaim(RegisterViewModel model);
        bool AddPasswordForCartCreatedUser(CreateUserConfirmedViewModel model);
        //AddQuizEditModel BuildAddQuizEditModel();
        EditBillingAddressModel BuildBillingAddressModel();
        DiscountModel BuildDiscountModel();

        ManageModel BuildManageModel(ManageMessageId? message);
        EditShippingAddressModel BuildShippingAddressModel();
        void BuildCityStateTimeZoneData(Dictionary<string, string> cityStateTimeZoneData, string zipAddress);
        bool ChangePasswordFromResetKey(string key, string password);
        CreateUserConfirmedViewModel ConfirmUser(string email, string surname);
        string CreateUserAccountFromCart(string email);
        WebUser CreateWebUserFromCart(RegisterViewModel model);
        void EditContactInfo(EditContactInfoModel editContactInfoModel);
        CreateUserConfirmedViewModel GetCreateUserConfirmedViewModel(string email, int idOrder, bool viaBillMePostRequest = false);
        Institution GetInstitutionFromEmail(string email);
        IEnumerable<Institution> GetInstitutionsByName(string name);
        Order GetOrderById(int idOrder);
        WebUser GetWebUserByEmail(string email);
        WebUser GetWebUserById(int id);
        int? GetWebUserIdByEmail(string email);
        WebUser GetWebUserFromIPrincipal();
        string GetZipAddress(int zip);
        LoginModel BuildLoginModel(string returnUrl);
        bool LogUserIn(SignInModel signInModel);
        void LogUserOut(ClaimsPrincipal user = null);
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
        void EditUser(EditUserViewModel model);
        void AddShippingAddressVerifiedClaim(int userId);
        // ReSharper disable once InconsistentNaming
        MyWebinarsDTO BuildMyWebinarsDTO(DiscountModel discountModel, ClaimsIdentity claimsIdentityOfAuthenticatedUser);
        int CreateUserForAdmin(EditUserModel editUserModel);
        Address BuildPlaceHolderAddressBilling(string email);
        Address BuildPlaceHolderAddressShipping(string email);
        void EditInstitution(EditInstitutionInfoModel model);
        void UpdateDiscountDetails(Discount thisSubscription);
        void EditEmail(string oldEmail, string email, string tenant);

        CompliancePerspectivesModel BuildCompPersectivesModel();
        DiscountModel BuildDiscountModel(Discount discount, int idUser);
    }
}