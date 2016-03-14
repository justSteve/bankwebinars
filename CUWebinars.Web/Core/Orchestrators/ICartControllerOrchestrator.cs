using System.Collections.Generic;
using CUWebinars.Business.Core;
using CUWebinars.Business.Models;
using CUWebinars.Business.Notification;
using CUWebinars.Web.ViewModel;
using System;
using CUWebinars.Web.Models;

namespace CUWebinars.Web.Core.Orchestrators
{
    public interface ICartControllerOrchestrator : IDisposable
    {
        void AdjustUserDetails(AdjustUserDetailsEditModel adjustUserDetailsEditModel);
        Discount ApplyDiscountCode(string code, OrderRow row);
        AdditionalLocationOfferViewModel BuildAdditionalLocationOfferViewModel(int idUser, int idWebinar);
        DisplayRowPriceViewModel BuildDisplayRowPriceViewModel(OrderRow orderRow, int? idOrderRow, decimal? optionsCost = null);
        RegisterViewModel BuildRegisterViewModel();
        CheckoutConfirmViewModel BuildCheckoutConfirmViewModel(int? idOrderRow);
        CheckoutOptionsViewModel BuildCheckoutOptionsViewModel(
            WebinarDetailsViewModel webinarDetailsViewModel,
            int? idWebinar,
            int? idOrderRow,
            int? idOrder);
        WebinarDetailsViewModel BuildCheckOutViewModel(int? idOrderRow);
        DisplayOptionsInDropDownViewModel BuildDisplayOptionsInDropDownViewModel(OrderRow orderRow,
            int? idOrderRow);
        AdditionalLocationsViewModel BuildAdditionalLocationsViewModel(OrderRow orderRow, int? idOrderRow);
        void CancelOrder(int idOrder);
        Tuple<string, string> CheckIfAddLocShouldHide(int optionId);
        Order CreateOrder(CheckoutOptionsViewModel formModel);
        OrderRow GetOrderRowLoaded(int idOrderRow);
        IEnumerable<WebUser> GetWebUsersByLastNameForAffiliate(string lastName, int idAffiliate);
        void FireOrderSubmittedNotification(Order order, bool? userCreatedInCart = null);
        INotificationMessage GenerateMessagePreview(Order order);
        RegType GetRegTypeById(int idRegType);
        OrderRow LoadOrderRow(int id, OrderStatus status);
        void RemoveAdditionalLocationsFromOrder(int value);
        PricesAndDiscounts UpdateOrderPricing(Order order);
        void UpdateOrderWithUserId(int orderId, int userId);
        string GetDiscountAmountAsPercentageOrDollarAmount(Discount myDiscount);
        void UpdateAdditionalLocationsForOrderRow(IEnumerable<AdditionalLocation> additionalLocations, int newOrderRowId);
        Order LoadOrder(int id);
        //void SetOrderPaidByCC(int qOrder, string s, string formFields);
        //int ProcessModelForConfirmation(WebinarDetailsViewModel model, bool? adminCreatedWebUser);
        
        //string CreatePostEventClaim(Order order);
        Webinar LoadWebinar(int idWebinar);
        bool UserHasPriorOrders(WebUser webUser);
        //void CheckOnDemandClaims(int? idWebinar);
        Order GetOrderById(int? idOrder);
        Discount GetDiscountById(int value);
        bool AssignWebUserToOrder(Order order);
        WebUser GetWebUserByEmail(string email);
        ExpressCheckoutModel ExpressCheckout(Order order, WebUser user);
        RegType GetRegTypeByLabel(string livePlusFive, int? idWebinar);
        void AddClaimForPostEventMaterials(string email, OrderRow row);
        IList<Order> GetOrderByUserIdAndWebinar(int selectedWebUser, int idWebinar);
        string InsertOnDemandClaim(int orderId);
        Order CreateOrderByAffiliate(CheckoutOptionsViewModel formModel, Affiliate affiliate);
        ExpressCheckoutPostBackModel BuildExpressPostback(ExpressCheckoutPostBackModel form);
        
    }
}