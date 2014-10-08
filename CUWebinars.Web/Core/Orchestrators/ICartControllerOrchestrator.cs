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
        CheckoutOptionsViewModel BuildCheckoutOptionsViewModel(WebinarDetailsViewModel webinarDetailsViewModel,
            int? idWebinar, int? idOrder);
        WebinarDetailsViewModel BuildCheckOutViewModel(int? idOrderRow);
        OrderHasAdditionalLocationsViewModel BuildOrderHasAdditionalLocationsViewModel(OrderRow orderRow, int? idOrderRow);
        string CheckIfAddLocShouldHide(int optionId);
        Order CreateOrder(WebinarDetailsViewModel formModel, string stageOfCheckout, string registrationType);
        OrderRow LoadOrderRow(int id, OrderStatus status);
    }
}