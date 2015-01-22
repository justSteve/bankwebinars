using System.Collections.Generic;
using CUWebinars.Business.Core;
using CUWebinars.Business.Models;
using CUWebinars.Business.Notification;
using CUWebinars.Web.ViewModel;
using System;

namespace CUWebinars.Web.Core.Orchestrators
{
    public interface ICartControllerOrchestrator : IDisposable
    {
        Discount ApplyDiscountCode(string code, OrderRow row);
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
        OrderHasAdditionalLocationsViewModel BuildOrderHasAdditionalLocationsViewModel(OrderRow orderRow, int? idOrderRow);
        void CancelOrder(int idOrder);
        Tuple<string, string> CheckIfAddLocShouldHide(int optionId);
        Order CreateOrder(CheckoutOptionsViewModel formModel);
        OrderRow GetOrderRowLoaded(int idOrderRow);
        IEnumerable<WebUser> GetWebUsersByLastName(string lastName);
        void FireOrderSubmittedNotification(Order order, bool? userCreatedInCart = null);
        INotificationMessage GenerateMessagePreview(Order order);
        RegType GetRegTypeById(int idRegType);
        OrderRow LoadOrderRow(int id, OrderStatus status);
        void RemoveAdditionalLocationsFromOrder(int value);
        PricesAndDiscounts UpdateOrderPricing(Order order);
        void UpdateOrderWithUserId(int orderId, int userId);
    }
}