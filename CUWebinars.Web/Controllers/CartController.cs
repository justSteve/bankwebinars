using CUWebinars.Business.Models;
using CUWebinars.Web.Core.Orchestrators;
using CUWebinars.Web.Infrastructure.Extensions;
using CUWebinars.Web.Services;
using CUWebinars.Web.ViewModel;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CUWebinars.Web.Controllers
{
    public class CartController : Controller
    {
        private TTSWebinarsContext db = new TTSWebinarsContext();
        readonly IStateService _stateService;

        private readonly ILogger _logger;
        private readonly ICartControllerOrchestrator _cartControllerOrchestrator;
        private bool _disposed;

        public CartController(ILogger logger,
            ICartControllerOrchestrator cartControllerOrchestrator)
        {
            _logger = logger;
            _cartControllerOrchestrator = cartControllerOrchestrator;
        }

        public PartialViewResult GetAdditionalLocationByOrderId(int webUserId, int webinarId)
        {
            //  TODO: Implement

            //var order = _orderManagementService.GetOrdersByUserId(webUserId);

            var addAdditionalLocationViewModel = new AdditionalLocationAddViewModel
            {
                // AdditionalLocations = order.OrderRows.First().AdditionalLocation.ToList()
                AdditionalLocation = new List<AdditionalLocation>()
            };

            return PartialView("~/Views/Webinar/Partials/_AdditionalLocationsModal.cshtml", addAdditionalLocationViewModel);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ConfirmOrder(int? id = null)
        {
            if (ModelState.IsValid)
            {
                var model = _cartControllerOrchestrator.BuildCheckOutViewModel(id);

                return Json(new
                {
                    success = "success",
                    orderRowID = model.Order.OrderRows.SingleOrDefault().idOrderRow,
                    msg = "OrderFacade.Instance.BuildConnectionInfo(order.Rows.SingleOrDefault())"
                }, JsonRequestBehavior.AllowGet);
            }

            return this.ModelStateJson(ModelState);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CancelOrder(int id)
        {
            //var order = _checkoutWorkflow.CurrentOrder;

            //_logger.Info("User Cancels order: " + order.ID);

            //OrderFacade.Instance.DeleteOrderRow(order, id);

            //_checkoutWorkflow.RemoveOrder(order.ID);
            //_checkoutWorkflow.;


            Session.Remove("LastOrderId");
            Session.Remove("CurrentOrderId");
            Session.Remove("CurrentOrderIds");
            Session.Remove("IsOrderPaid");

            return Json(new
            {
                success = "success"

            }, JsonRequestBehavior.AllowGet);
            //return RedirectToAction("Step2");
        }


        public ActionResult CheckoutOptions(int? idWebinar, int? idOrderRow)
        {
            var model = _cartControllerOrchestrator.BuildCheckoutOptionsViewModel(null, idWebinar, idOrderRow);
            return PartialView("Partials/CheckoutOptions", model);
        }

        public ActionResult CheckoutContact(int ID)
        {
            var model = _cartControllerOrchestrator.BuildCheckoutConfirmViewModel(ID);
            return PartialView("Partials/CheckoutContact", model);
        }
        public ActionResult CheckoutConfirm(int ID)
        {
            var model = _cartControllerOrchestrator.BuildCheckoutConfirmViewModel(ID);
            return PartialView("Partials/CheckoutConfirm", model);
        }
        public ActionResult CheckoutDisplayRowPrice(int ID)
        {
            var model = _cartControllerOrchestrator.BuildDisplayRowPriceViewModel(null, ID);
            return PartialView("Partials/_DisplayRowPrice", model); 
        }

        public PartialViewResult CheckoutContactDetails()
        {
            return PartialView("~/Views/cart/Partials/CheckoutContact.cshtml", _cartControllerOrchestrator.BuildRegisterViewModel());
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Signup2(WebinarDetailsViewModel formModel, string stageOfCheckout)
        {
            if (ModelState.IsValid)
            {
                var order = _cartControllerOrchestrator.CreateOrder(
                    formModel, 
                    stageOfCheckout,
                    Request.Form["RegistrationType"]
                    );


                if (Request.IsAuthenticated)
                {
                    formModel.UserIsLoggedIn = true;
                }
                else
                {
                    formModel.UserIsLoggedIn = false;
                    _logger.Error("ERROR: CartController | Signup - currentUser is null" + order.idOrder);
                    //TODO: assign appropriate ModelError and error logging/handling
                }

                //if (formModel.Webinar.idWebinar == 883 && formModel.Webinar.Status == WebinarStatus.Scheduled)
                //{
                //    try
                //    {
                //        _orderManagementService.CreateCPSubscription(orderRow);
                //    }
                //    catch (Exception)
                //    {
                //        throw;
                //    }
                //}

                try
                {
                    if (stageOfCheckout == "preReg")
                    {
                        return PartialView("Partials/_DisplayRowPrice", formModel.Order.OrderRows.Single());
                    }

                    formModel.Order = order;

                    return Json(new
                    {
                        success = "success",
                        orderRowId = formModel.Order.OrderRows.Single().idOrderRow
                    }, JsonRequestBehavior.AllowGet
                        );
                }
                catch(Exception exception)
                {
                    ModelState.AddModelError(string.Empty, exception.Message);
                }
            }
            return this.ModelStateJson(ModelState);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult CheckIfAddLocShouldHide(int optionID)
        {
            if (ModelState.IsValid)
            {
                var shouldShow = _cartControllerOrchestrator.CheckIfAddLocShouldHide(optionID);

                return Json(new {shouldShow}, JsonRequestBehavior.AllowGet);
            }
            return this.ModelStateJson(ModelState);
        }

        //[Authorize(Roles = AppRoles.Admin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SetOrderStatus(int orderRowID, OrderStatus status)
        {
            if (ModelState.IsValid)
            {
                var orderRow = _cartControllerOrchestrator.LoadOrderRow(orderRowID, status);
                return Json(orderRow.Order.OrderStatus.ToString());
            }
            return this.ModelStateJson(ModelState);
        }

        public virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                var logger = _logger as IDisposable;
                if (logger != null)
                    logger.Dispose();

                _cartControllerOrchestrator.Dispose();

                _disposed = true;
            }
        }
    }
}