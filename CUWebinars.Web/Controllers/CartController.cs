using CUWebinars.Business.Models;
using CUWebinars.Web.Core.Orchestrators;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Infrastructure.Attributes;
using CUWebinars.Web.Infrastructure.Extensions;
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
        private readonly IAppHelper _appHelper;
        private bool _disposed;

        public CartController(ILogger logger,
            ICartControllerOrchestrator cartControllerOrchestrator,
            IAppHelper appHelper)
        {
            _logger = logger;
            _cartControllerOrchestrator = cartControllerOrchestrator;
            _appHelper = appHelper;
        }

        [HttpPost]
        public ActionResult ApplyDiscountCode(string code)
        {
            //_cartControllerOrchestrator.ApplyDiscountCode
            
            return Json(new { Result = "100"});
        }


        public PartialViewResult GetAdditionalLocationByOrderId(int webinarId, int? webUserId = null)
        {
            //var order = _orderManagementService.GetOrdersByUserId(webUserId);

             var addAdditionalLocationViewModel = new AdditionalLocationOfferViewModel
            {
                // AdditionalLocations = order.OrderRows.First().AdditionalLocation.ToList()
                AdditionalLocations = new List<AdditionalLocation>(),
                OrderExists = false,
                Emails = new string [0]
            };

            return PartialView("~/Views/Webinar/Partials/_AdditionalLocationsModal.cshtml", addAdditionalLocationViewModel);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ConfirmOrder( string referred, int? id = null)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var model = _cartControllerOrchestrator.BuildCheckOutViewModel(id);
                    var orderID = model.Order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).idOrderRow;
                     model.Order.OrderStatus = OrderStatus.Submitted;

                    if (User.Identity.IsAuthenticated)
                    {
                        _cartControllerOrchestrator.FireOrderSubmittedNotification(model.Order, userCreatedInCart: false);
                    }
                    else
                    {
                        _cartControllerOrchestrator.FireOrderSubmittedNotification(model.Order, userCreatedInCart: true);
                    }

                    _cartControllerOrchestrator.UpdateOrderPricing(model.Order);
                

                    return Json(new
                    {
                        Result = WebUiConstants.Success,
                        OrderRowID = orderID,
                        Msg = string.Format("<p>Your Order ID is {0}. Please check your email for connection information for the webinar.</p>", orderID)
                    }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, "There was a problem at the server. Please contact the administrator.");
                    _logger.ErrorException("ConfirmOrder|ConfirmOrder failed ", exception);
                    Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                }
            }

            return this.ModelStateJson(ModelState);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CancelOrder(int? id = null)
        {
            if (id.HasValue)
            {
                _cartControllerOrchestrator.CancelOrder(id.Value);

                return Json(new
                {
                    success = "success"

                }, JsonRequestBehavior.AllowGet);
                //return RedirectToAction("Step2");
            }
            ModelState.AddModelError(string.Empty, "No Order ID was posted to the Server. In case of persistant error contact us at support@ttstrain.com, or, for immediate assistance, 800-831-0678 ext. 707."); // 
            return this.ModelStateJson(ModelState);
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
        public ActionResult Signup2(CheckoutOptionsViewModel formModel)
        {
            if (ModelState.IsValid)
            {
                //var telemetry = new TelemetryClient();
                //telemetry.TrackEvent("Signup2Start");
                try
                {
                    _logger.Info("Signup2 Enters: " + _appHelper.GetUserAuditInfo());
                    var order = _cartControllerOrchestrator.CreateOrder(
                        formModel
                        );

                    // todo: __sjh confirms that in-process handling appears to be implmented -- if in progress, will have to show populated partial view.
                    //

                    _logger.Info("Signup2 order initialized: " + _appHelper.GetUserAuditInfo());


                    return Json(new
                    {
                        success = "success",
                        orderId = order.idOrder,
                        orderRowId = order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).idOrderRow,
                        webinarId = formModel.idWebinar
                    }, JsonRequestBehavior.AllowGet);

                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty,
                        "There has been an error at the server which has been logged.");
                    _logger.FatalException("Signup2 order excepted: ", exception);

                    Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                }
            }
            return this.ModelStateJson(ModelState);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult CheckIfAddLocShouldHide(int optionID)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // CheckIfAddLocShouldHide checks whether a particular RegType can have AdditionalLocations or not. 
                    // It also checks whether a particular RegType has materials that are required to be posted (snail mail). 
                    // The 1st string in the tuple is either 'yes' or 'no' and is an instruction to the View whether to show the
                    // AdditionalLocationContainer on the 1st tab. The 2nd string in the tuple is an instruction to the
                    // View whether or not to display the modal popup upon entering the 3rd tab which displays an edit
                    // form for the user's shipping details. This is because that RegType will have associated materials
                    // that are posted via traditional mail e.g. handouts.
                    Tuple<string,string> showPlusShip = _cartControllerOrchestrator.CheckIfAddLocShouldHide(optionID);
                    return Json(new { shouldShow = showPlusShip.Item1, shippingDetailsRqrd = showPlusShip.Item2 }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception exception)
                {
                    _logger.ErrorException(string.Format("CheckIfAddLocShouldHide in cart. | Session{0}", _appHelper.GetUserAuditInfo()), exception);
                    ModelState.AddModelError(string.Empty, "There has been an error. Please call 800-831-0678 ext 706 for immediate assistance.");
                }
            }
            return this.ModelStateJson(ModelState);
        }

        [HttpPost]
        public ActionResult RemoveAdditionalLocationsFromOrder(int? idOrderRow = null)
        {
            if (idOrderRow.HasValue)
            {
                try
                {
                    _cartControllerOrchestrator.RemoveAdditionalLocationsFromOrder(idOrderRow.Value);
                }
                catch (Exception exception)
                {
                    _logger.ErrorException("RemoveAdditionalLocationsFromOrder|Session=" + _appHelper.GetUserAuditInfo(), exception);
                    Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                }

                return Json(new { Result = WebUiConstants.Success });
            }
            return Json(new {});
        }

        [HttpPost]
        public ActionResult UpdateOrderDetails(int? idOrderRow = null, int? idRegType = null)
        {
            if (idOrderRow.HasValue && idRegType.HasValue)
            {
                try
                {
                    var regType = _cartControllerOrchestrator.GetRegTypeById(idRegType.Value);
                    var model = _cartControllerOrchestrator.BuildCheckOutViewModel(idOrderRow);
                    model.Order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).RegistrationType = regType;

                    var pricesAndDiscounts = _cartControllerOrchestrator.UpdateOrderPricing(model.Order);

                    return
                        Json(
                            new
                            {
                                BasePrice = pricesAndDiscounts.UnitPrice,
                                Discount = pricesAndDiscounts.TotalDiscount,
                                OptionsPrice = pricesAndDiscounts.TotalOptions,
                                Total = pricesAndDiscounts.TotalOrderPrice
                            });
                }
                catch (Exception exception)
                {
                    _logger.ErrorException(
                        "RemoveAdditionalLocationsFromOrder|Session=" + _appHelper.GetUserAuditInfo(), exception);
                    Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                }

                return Json(new {Result = WebUiConstants.Success});
            }
            return Json(new {});
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