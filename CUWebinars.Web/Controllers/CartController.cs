using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using CUWebinars.Business.Constants;
using CUWebinars.Business.Core.Helpers;
using CUWebinars.Business.Models;
using CUWebinars.Web.Core.Orchestrators;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Infrastructure.Attributes;
using CUWebinars.Web.Infrastructure.Extensions;
using CUWebinars.Web.Mapping.Mappers;
using CUWebinars.Web.Models;
using CUWebinars.Web.Services;
using CUWebinars.Web.ViewModel;
using Elmah;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ninject.Extensions.Logging;

namespace CUWebinars.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ILogger _logger;
        private readonly ICartControllerOrchestrator _cartControllerOrchestrator;
        private readonly IAppHelper _appHelper;
        private readonly IUniversalMapper _universalMapper;
        private readonly IStateService _stateService;
        private bool _disposed;

        public CartController(ILogger logger,
            ICartControllerOrchestrator cartControllerOrchestrator,
            IAppHelper appHelper,
            IUniversalMapper universalMapper,
            IStateService stateService)
        {
            _logger = logger;
            _cartControllerOrchestrator = cartControllerOrchestrator;
            _appHelper = appHelper;
            _universalMapper = universalMapper;
            _stateService = stateService;
        }


        [HttpPost]
        public ActionResult AddOrder(int? idWebinar)
        {
            try
            {
                _cartControllerOrchestrator.CheckOnDemandClaims(idWebinar);
                return Json(new { Result = WebUiConstants.Success });
            }
            catch (Exception exception)
            {
                ModelState.AddModelError(string.Empty, "The operation failed.");
                _logger.ErrorException("CheckOnDemandClaims failed ", exception);
                return this.ModelStateJson(ModelState);
            }
        }

        [ValidateJsonAntiForgeryToken(Order = 0)]
        [HandleAjaxException(Order = 1)]
        public ActionResult CheckOnDemandClaims(int? idWebinar)
        {
            try
            {
                _cartControllerOrchestrator.CheckOnDemandClaims(idWebinar);
                return Json(new { Result = WebUiConstants.Success });
            }
            catch (Exception exception)
            {
                ModelState.AddModelError(string.Empty, "The operation failed.");
                _logger.ErrorException("CheckOnDemandClaims failed ", exception);
                return this.ModelStateJson(ModelState);
            }
        }

        [HttpPost]
        [ValidateJsonAntiForgeryToken(Order = 0)]
        [HandleAjaxException(Order = 1)]
        public ActionResult AdjustUserDetails(AdjustUserDetailsEditModel adjustUserDetailsEditModel)
        {
            try
            {
                _cartControllerOrchestrator.AdjustUserDetails(adjustUserDetailsEditModel);
                return Json(new { Result = WebUiConstants.Success });
            }
            catch (Exception exception)
            {
                ModelState.AddModelError(string.Empty, "The operation failed.");
                _logger.ErrorException("AdjustUserDetails|AdjustUserDetails failed ", exception);
                return this.ModelStateJson(ModelState);
            }
        }

        [HttpPost]
        //[ValidateJsonAntiForgeryToken(Order = 0)]
        [HandleAjaxException(Order = 1)]
        public ActionResult ApplyDiscountCode(string code, int orderRowId)
        {
            try
            {
                var row = _cartControllerOrchestrator.GetOrderRowLoaded(orderRowId);

                var myDiscount = _cartControllerOrchestrator.ApplyDiscountCode(code, row);


                if (!ReferenceEquals(myDiscount, null))
                {
                    if (myDiscount.CreditsRemain == 0)
                    {
                        return Json(new { Result = -1 });
                    }
                    _cartControllerOrchestrator.UpdateOrderPricing(row.Order);

                    var amountToDiscount =
                        _cartControllerOrchestrator.GetDiscountAmountAsPercentageOrDollarAmount(myDiscount);

                    return Json(new { Result = amountToDiscount });
                }
                else
                {

                    return Json(new { Result = 0 });
                }

            }
            catch (Exception exception)
            {
                ModelState.AddModelError(string.Empty, "Code Not Found.");
                _logger.ErrorException("ApplyDiscountCode|ApplyDiscountCode failed ", exception);
                return this.ModelStateJson(ModelState);
            }
        }


        public PartialViewResult GetAdditionalLocationByOrderId(int webinarId, int? webUserId = null)
        {
            if (webUserId.HasValue)
            {
                return PartialView(
                    "~/Views/Webinar/Partials/_AdditionalLocationsModal.cshtml",
                    _cartControllerOrchestrator.BuildAdditionalLocationOfferViewModel(webUserId.Value, webinarId)
                    );
            }
            return null;
        }


        [HttpPost]
        [ValidateAntiForgeryToken(Order = 0)]
        [HandleAjaxException(Order = 1)]
        public ActionResult ConfirmOrder(string referred, int? id = null)
        {
            if (id.HasValue)
            {
                _logger.Info("Confirming Order for OrderRow with Id {0}", id.Value);
                var model = _cartControllerOrchestrator.BuildCheckOutViewModel(id);
                try
                {

                    model.Order.OrderStatus = OrderStatus.Submitted;
                    if (ReferenceEquals(model.Order.Affiliate, null))
                    {
                        var a = 0;
                    }

                    if (User.Identity.IsAuthenticated)
                    {
                        _cartControllerOrchestrator.FireOrderSubmittedNotification(model.Order, userCreatedInCart: false);
                    }
                    else
                    {
                        _cartControllerOrchestrator.FireOrderSubmittedNotification(model.Order, userCreatedInCart: true);
                    }

                    _cartControllerOrchestrator.UpdateOrderPricing(model.Order);


                    _cartControllerOrchestrator.CreatePostEventClaim(model.Order);

                    return Json(new
                    {
                        Result = WebUiConstants.Success,
                        OrderRowID = model.Order.idOrder,
                        Msg = string.Format("Your registration is confirmed. Complete details will be emailed to {0}.", model.Order.BillingEmail)
                    }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, "There was a problem at the server. Please contact the administrator.");
                    _logger.ErrorException("ConfirmOrder|ConfirmOrder failed ", exception);
                    ErrorSignal.FromCurrentContext().Raise(exception);
                }
            }

            _logger.Error("ConfirmOrder Action | Id parameter was null");
            _logger.Error(string.Format("ConfirmOrder Action | {0}", _appHelper.GetUserAuditInfo()));

            return this.ModelStateJson(ModelState);
        }


        [HttpPost]
        public ActionResult ConfirmOrderForAffiliate(string referred, int? id = null, bool? adminCreatedWebUser = null)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var model = _cartControllerOrchestrator.BuildCheckOutViewModel(id);
                    int orderId = _cartControllerOrchestrator.ProcessModelForConfirmation(model, adminCreatedWebUser);

                    return Json(new
                    {
                        Result = WebUiConstants.Success,
                        OrderRowID = orderId,
                        Msg = string.Format("<p>Your Order ID is {0}. Please check your email for connection information for the webinar.</p>", orderId)
                    }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, "There was a problem at the server. Please contact the administrator.");
                    _logger.ErrorException("ConfirmOrder|ConfirmOrder failed ", exception);
                    ErrorSignal.FromCurrentContext().Raise(exception);
                }
            }

            return this.ModelStateJson(ModelState);
        }

        [HttpPost]
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

        public ActionResult CheckoutConfirm(int? ID = null)
        {
            var model = _cartControllerOrchestrator.BuildCheckoutConfirmViewModel(ID);
            if (model == null) throw new ArgumentNullException("model");
            try
            {
                if (ID != null && ID > 0)
                {
                    ViewBag.Order = _cartControllerOrchestrator.LoadOrder(ID.Value);
                    ViewBag.TaxAmount = model.DisplayRowPriceViewModel.PricesAndDiscounts.TaxAmount;
                }
                else
                {
                    _logger.Fatal("CheckoutConfirm was passed a null or zero value: ", new Exception("null or zero ID passed to CheckoutConfirm partial"));
                }
            }
            catch (Exception ex)
            {
                _logger.Fatal("CheckoutConfirm heard: " + ex);
                throw;
            }
            return PartialView("Partials/CheckoutConfirm", model);
        }
        public ActionResult CheckoutConfirmForAffiliate(int? ID = null)
        {
            var model = _cartControllerOrchestrator.BuildCheckoutConfirmViewModel(ID);
            return PartialView("Partials/CheckoutConfirmForAffiliate", model);
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


        [HttpPost]
        public ActionResult Signup2(CheckoutOptionsViewModel formModel)
        {
            if (ModelState.IsValid)
            {
                _logger.Info("Signup2 Enters: " + _appHelper.GetUserAuditInfo());

                //var telemetry = new TelemetryClient();
                //telemetry.TrackEvent("Signup2Start");
                try
                {
                    var order = _cartControllerOrchestrator.CreateOrder(
                        formModel
                        );

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
                    if (exception.Message.Equals(
                        ErrorMessageConstants.ExistingNonCancelledOrderMessage,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        _logger.ErrorException(string.Format("Signup2 | {0}", _appHelper.GetUserAuditInfo()), exception);
                        return Json(new
                        {
                            Result = WebUiConstants.Fail,
                            Msg = exception.Message
                        }, JsonRequestBehavior.AllowGet);
                    }

                    ModelState.AddModelError(string.Empty,
                        exception.Message.Contains(ErrorMessageConstants.ExistingNonCancelledOrderMessage)
                            ? ErrorMessageConstants.ExistingNonCancelledOrderMessage
                            : "There has been an error at the server which has been logged.");

                    _logger.FatalException("Signup2 order died. ", exception);

                    ErrorSignal.FromCurrentContext().Raise(exception);
                }

            }
            return this.ModelStateJson(ModelState);
        }

        [HttpPost]
        public ActionResult SignupAffiliate(CheckoutOptionsViewModel formModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var order = _cartControllerOrchestrator.CreateOrder(
                        formModel
                        );

                    
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

                    ErrorSignal.FromCurrentContext().Raise(exception);
                }
            }
            return this.ModelStateJson(ModelState);
        }

        public ActionResult SearchWebUsers(string lastName)
        {
            if (!string.IsNullOrWhiteSpace(lastName))
            {
                var currentAffiliate = _stateService.GetValue<Affiliate>(WebUiConstants.CurrentAffiliate);

                var webUsers = _cartControllerOrchestrator.GetWebUsersByLastNameForAffiliate(lastName,
                    currentAffiliate.idUserAff)
                    .Select(w => new
                    {
                        id = w.idUser,
                        lastname = w.LastName,
                        firstname = w.FirstName
                    });

                return Json(new { people = webUsers }, JsonRequestBehavior.AllowGet);
            }
            return View();
        }


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
                    Tuple<string, string> showPlusShip = _cartControllerOrchestrator.CheckIfAddLocShouldHide(optionID);
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

        [HttpGet]
        public ActionResult PreviewEmail(int? id)
        {
            if (id.HasValue)
            {
                var model = _cartControllerOrchestrator.BuildCheckOutViewModel(id);

                var message = _cartControllerOrchestrator.GenerateMessagePreview(model.Order);

                return File(message.Body.GenerateStreamFromString(), "text/html");
            }

            return View();
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
                    ErrorSignal.FromCurrentContext().Raise(exception);
                }

                return Json(new { Result = WebUiConstants.Success });
            }
            return Json(new { });
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
                                OptionsPrice = pricesAndDiscounts.TotalCostOfOptions,
                                Total = pricesAndDiscounts.TotalOrderPrice
                            });
                }
                catch (Exception exception)
                {
                    _logger.ErrorException(
                        String.Format("UpdateOrderDetails failed on {0} with {1} Session={2}", idRegType, exception.Message, _appHelper.GetUserAuditInfo()), exception);
                    ErrorSignal.FromCurrentContext().Raise(exception);
                }

                return Json(new { Result = WebUiConstants.Success });
            }
            return Json(new { });
        }

        //[Authorize(Roles = AppRoles.Admin)]
        [HttpPost]
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

                return Json(new { Result = WebUiConstants.Success });
            }

            return View();
        }



        public ActionResult ExpressCheckout4IE1CU(int idWebinar, int? idAffiliate)
        {

            var webinar = _cartControllerOrchestrator.LoadWebinar(idWebinar);
            var _idAffiliate = 19;
            if (!ReferenceEquals(idAffiliate, null))
            {
                _idAffiliate = idAffiliate.Value;
            }

            var model = new ExpressCheckoutModel
            {
                q18_q_webinarid18 = idWebinar,
                q15_affiliateid15 = _idAffiliate,
                q12_webinarTitle = webinar.Title
            };

            return View(model);

        }

        public ActionResult ExpressCheckout4IE1(int idWebinar, int? idAffiliate)
        {

            var webinar = _cartControllerOrchestrator.LoadWebinar(idWebinar);
            var _idAffiliate = 19;
            if (!ReferenceEquals(idAffiliate, null))
            {
                _idAffiliate = idAffiliate.Value;
            }

            var model = new ExpressCheckoutModel
            {
                q18_q_webinarid18 = idWebinar,
                q15_affiliateid15 = _idAffiliate,
                q12_webinarTitle = webinar.Title
            };

            return View(model);

        }

        public ActionResult ComplianceSchoolCheckout()
        {

            //var webinar = _cartControllerOrchestrator.LoadWebinar(idWebinar);
            //var _idAffiliate = 19;
            //if (!ReferenceEquals(idAffiliate, null))
            //{
            //    _idAffiliate = idAffiliate.Value;
            //}

            //var model = new ExpressCheckoutModel
            //{
            //    q18_q_webinarid18 = idWebinar,
            //    q15_affiliateid15 = _idAffiliate,
            //    q12_webinarTitle = webinar.Title
            //};

            return View();

        }
        public ActionResult ThankYou(FormCollection form)
        {
            string formFields = Request.Form.ToString();
            _logger.Info("ThankYou postback: " + formFields);

            return View();
        }

        [HttpPost]
        public ActionResult PostBackMonerisBW(MonerisResponse form)
        {
            string formFields = Request.Form.ToString();
            _logger.Info("PostBackMonerisBW: " + formFields);
            var order = _cartControllerOrchestrator.LoadOrder(Convert.ToInt32(form.order_no.Split('-')[1]));

            if (order == null) throw new ArgumentNullException("order");

            JProperty monerisResponse = new JProperty(JsonPropertyKeys.MonerisResponse, JsonConvert.SerializeObject(form));

            order.AdminComments = JsonHelpers.MergeJsonWithStoredField(order.AdminComments, monerisResponse);

            _cartControllerOrchestrator.UpdateOrderPricing(order);

            if (form.message.StartsWith("APPROVED"))
            {
                try
                {
                    _logger.Info("Confirming Moneris submission with Id BW-{0}", order.idOrder);

                    order.OrderStatus = OrderStatus.Paid;

                    _cartControllerOrchestrator.CreatePostEventClaim(order);
                    var userHasPriorOrders = _cartControllerOrchestrator.UserHasPriorOrders(order.WebUser);


                    if (User.Identity.IsAuthenticated)
                    {
                        _cartControllerOrchestrator.FireOrderSubmittedNotification(order, userCreatedInCart: false);
                    }
                    else
                    {
                        _cartControllerOrchestrator.FireOrderSubmittedNotification(order, userCreatedInCart: true);
                    }

                    return RedirectToAction("OrderComplete", "Account", new { id = order.idOrder, message = form.message });
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty,
                        "There was a problem at the server. Please contact the administrator.");
                    _logger.ErrorException("MonerisConfirmOrder returned APPROVED but controller failed ", exception);
                    ErrorSignal.FromCurrentContext().Raise(exception);
                }

                _logger.Error("ConfirmOrder Action | Id parameter was null");
                _logger.Error(string.Format("ConfirmOrder Action | {0}", _appHelper.GetUserAuditInfo()));
            }
            else
            {
                _logger.Info("Moneris declined with msg {0}", form.message);
                return Json(new
                {
                    Result = WebUiConstants.Fail,
                    OrderRowID = order.idOrder,
                    Msg = string.Format("Your credit card transaction did not complete successfully. Our processor replied with this message: {0}", form.message)
                }, JsonRequestBehavior.AllowGet);
                //return RedirectToAction("OrderDidNotComplete", "Account", new { id = order.idOrder, message = form.message });
            }
            return this.ModelStateJson(ModelState);

        }

        [HttpPost]
        public ActionResult PostBackMoneris(MonerisResponse form)
        {
            string formFields = Request.Form.ToString();
            _logger.Info("PostBackMoneris: " + formFields);

            var order = _cartControllerOrchestrator.LoadOrder(Convert.ToInt32(form.order_no.Split('-')[1]));

            if (order == null) throw new ArgumentNullException("order");

            JProperty monerisResponse = new JProperty(JsonPropertyKeys.MonerisResponse,
                JsonConvert.SerializeObject(form));

            order.AdminComments = JsonHelpers.MergeJsonWithStoredField(order.AdminComments, monerisResponse);

            _cartControllerOrchestrator.UpdateOrderPricing(order);

            if (form.message.StartsWith("APPROVED"))
            {
                try
                {
                    _logger.Info("Confirming Moneris submission with Id CU-{0}", order.idOrder);


                    order.OrderStatus = OrderStatus.Paid;


                    _cartControllerOrchestrator.CreatePostEventClaim(order);

                    if (User.Identity.IsAuthenticated)
                    {
                        _cartControllerOrchestrator.FireOrderSubmittedNotification(order, userCreatedInCart: false);
                    }
                    else
                    {
                        _cartControllerOrchestrator.FireOrderSubmittedNotification(order, userCreatedInCart: true);
                    }

                    return RedirectToAction("OrderComplete", "Account",
                        new { id = order.idOrder, message = form.message });
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty,
                        "There was a problem at the server. Please contact the administrator.");
                    _logger.ErrorException("ConfirmOrder|ConfirmOrder failed ", exception);
                    ErrorSignal.FromCurrentContext().Raise(exception);
                }

                _logger.Error("ConfirmOrder Action | Id parameter was null");
                _logger.Error(string.Format("ConfirmOrder Action | {0}", _appHelper.GetUserAuditInfo()));
            }
            else
            {
                _logger.Info("Moneris declined with msg {0}", form.message);
                return Json(new
                {
                    Result = WebUiConstants.Fail,
                    OrderRowID = order.idOrder,
                    Msg =
                        string.Format(
                            "Your credit card transaction did not complete successfully. Our processor replied with this message: {0}",
                            form.message)
                }, JsonRequestBehavior.AllowGet);
                //return RedirectToAction("OrderDidNotComplete", "Account", new { id = order.idOrder, message = form.message });
            }
            return this.ModelStateJson(ModelState);
        }



        public void PostBackWPS(FormCollection form)
        {
            //wps = webinar package subscription
            string formFields = Request.Form.ToString();
            _logger.Info("PostBackWPS: " + formFields);

        }


        [HttpPost]
        public ActionResult UpdateAdditionalLocations(IEnumerable<AdditionalLocation> additionalLocations, int? newOrderRowId)
        {
            _cartControllerOrchestrator.UpdateAdditionalLocationsForOrderRow(additionalLocations, newOrderRowId.Value);

            return Json(new { Result = WebUiConstants.Success });
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                var logger = _logger as IDisposable;
                if (logger != null)
                    logger.Dispose();

                _cartControllerOrchestrator.Dispose();

                _disposed = true;

                base.Dispose();
            }
        }
    }

}