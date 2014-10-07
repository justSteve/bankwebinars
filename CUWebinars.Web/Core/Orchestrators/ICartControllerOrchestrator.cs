using CUWebinars.Business.Models;
using CUWebinars.Web.ViewModel;
using System;

namespace CUWebinars.Web.Core.Orchestrators
{
    public interface ICartControllerOrchestrator : IDisposable
    {
        RegisterViewModel BuildRegisterViewModel();
        WebinarDetailsViewModel BuildCheckOutViewModel(int? idOrderRow);
        string CheckIfAddLocShouldHide(int optionId);
        Order CreateOrder(WebinarDetailsViewModel formModel, string stageOfCheckout, string registrationType);
        OrderRow LoadOrderRow(int id, OrderStatus status);
    }
}