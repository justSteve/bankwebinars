using CUWebinars.Business.Models;
using CUWebinars.Web.ViewModel;
using System;

namespace CUWebinars.Web.Core.Orchestrators
{
    public interface ICartControllerOrchestrator : IDisposable
    {
        DisplayRowPriceViewModel BuildDisplayRowPriceViewModel(OrderRow orderRow, int? idOrderRow);
        RegisterViewModel BuildRegisterViewModel();
        CheckoutConfirmViewModel BuildCheckoutConfirmViewModel(int? idOrderRow);
        CheckoutOptionsViewModel BuildCheckoutOptionsViewModel(
            WebinarDetailsViewModel webinarDetailsViewModel,
            int? idWebinar,
            int? idOrderRow,
            int? idOrder);
        WebinarDetailsViewModel BuildCheckOutViewModel(int? idOrderRow);
        OrderHasAdditionalLocationsViewModel BuildOrderHasAdditionalLocationsViewModel(OrderRow orderRow, int? idOrderRow);
        string CheckIfAddLocShouldHide(int optionId);
        Order CreateOrder(CheckoutOptionsViewModel formModel, string stageOfCheckout);
        OrderRow LoadOrderRow(int id, OrderStatus status);
    }
}