using CUWebinars.Business.Models;
using CUWebinars.Web.Core.Orchestrators;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Infrastructure.Attributes;
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
        private readonly ILogger _logger;
        private readonly ICartControllerOrchestrator _cartControllerOrchestrator;
        private bool _disposed;

        public CartController(ILogger logger,
            ICartControllerOrchestrator cartControllerOrchestrator)
        {
            _logger = logger;
            _cartControllerOrchestrator = cartControllerOrchestrator;
        }

        [HttpPost]
        public ActionResult ApplyDiscountCode(string code)
        {
            // HARD CODED RESPONSE
            return Json(new { result = "100"});
        }


        public PartialViewResult GetAdditionalLocationByOrderId(int webinarId, int? webUserId = null)
        {
            //var order = _orderManagementService.GetOrdersByUserId(webUserId);

             var addAdditionalLocationViewModel = new AdditionalLocationOfferViewModel
            {
                // AdditionalLocations = order.OrderRows.First().AdditionalLocation.ToList()
                AdditionalLocations = new List<AdditionalLocation>(),
                OrderExists = false,
                Emails = new string []{""}

            };

            return PartialView("~/Views/Webinar/Partials/_AdditionalLocationsModal.cshtml", addAdditionalLocationViewModel);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ConfirmOrder( string referred, int? id = null)
        {
            if (ModelState.IsValid)
            {
                var model = _cartControllerOrchestrator.BuildCheckOutViewModel(id);
                var orderID = model.Order.OrderRows.SingleOrDefault().idOrderRow;

                _cartControllerOrchestrator.FireOrderSubmittedNotification(order: model.Order);
                _cartControllerOrchestrator.SetOrderStatusToSubmitted(order: model.Order);
                

                return Json(new
                {
                    success = "success",
                    orderRowID = orderID,
                    msg = string.Format("Your Order ID is {0}.{1}Please check your email for connection information for the webinar.", orderID, Environment.NewLine)
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


        public ActionResult CheckoutOptions(int? idWebinar, int? idOrderRow, int? idOrder)
        {
            var model = _cartControllerOrchestrator.BuildCheckoutOptionsViewModel(null, idWebinar, idOrderRow, idOrder);
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

        public PartialViewResult UpdateOrderWithUserIdForm()
        {
            return PartialView("~/Views/cart/Partials/_UpdateOrderWithUserId.cshtml");
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Signup2(CheckoutOptionsViewModel formModel, string stageOfCheckout)
        {
            if (ModelState.IsValid)
            {
                var order = _cartControllerOrchestrator.CreateOrder(
                    formModel, 
                    stageOfCheckout
                    );


                //  TODO: this all changes and user can create order whilst not authenticated
                if (!Request.IsAuthenticated)
                {
                    _logger.Error("ERROR: CartController | Signup - currentUser is null" + order.idOrder);
                    //TODO: assign appropriate ModelError and error logging/handling
                }

                try
                {
                    //I believe 'stageOfCheckout' (as well as CheckoutInProcess) are deprecated by 
                    // implementation of SM, yes?
                    if (stageOfCheckout == "preReg")
                    {
                        return PartialView("Partials/_DisplayRowPrice", order.OrderRows.FirstOrDefault());
                    }

                    return Json(new
                    {
                        success = "success",
                        orderId = order.idOrder,
                        orderRowId = order.OrderRows.Single().idOrderRow,
                        webinarId = formModel.idWebinar
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

        [HttpPost]
        public ActionResult RemoveAdditionalLocationsFromOrder(int? idOrderRow = null)
        {
            if (idOrderRow.HasValue)
            {
                _cartControllerOrchestrator.RemoveAdditionalLocationsFromOrder(idOrderRow.Value);
            }
            return View();
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

        [HttpPost, ValidateJsonAntiForgeryToken]
        public ActionResult UpdateOrderWithUserId(int? orderId, int? userId)
        {
            if (orderId.HasValue && userId.HasValue)
            {
                _cartControllerOrchestrator.UpdateOrderWithUserId(orderId.Value, userId.Value);

                return Json(new {Result = WebUiConstants.Success});
            }

            return View();
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