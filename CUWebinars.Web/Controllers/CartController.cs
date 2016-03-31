using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Web.Mvc;
using CUWebinars.Business.AccountService;
using CUWebinars.Business.Constants;
using CUWebinars.Business.Core;
using CUWebinars.Business.Core.Helpers;
using CUWebinars.Business.Models;
using CUWebinars.Web.Core.Orchestrators;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Infrastructure.Attributes;
using CUWebinars.Web.Infrastructure.Extensions;
using CUWebinars.Web.Mapping.Mappers;
using CUWebinars.Web.Models;
using CUWebinars.Web.Models.DataTablesModels;
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
        private readonly IMembershipService _membershipService;
        private bool _disposed;

        public CartController(ILogger logger,
            ICartControllerOrchestrator cartControllerOrchestrator,
            IAppHelper appHelper,
            IUniversalMapper universalMapper,
            IStateService stateService,
            IMembershipService membershipService)
        {
            _logger = logger;
            _cartControllerOrchestrator = cartControllerOrchestrator;
            _appHelper = appHelper;
            _universalMapper = universalMapper;
            _stateService = stateService;
            _membershipService = membershipService;
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

        [System.Web.Mvc.AllowAnonymous]
        [System.Web.Mvc.HttpGet]
        public string InsertOnDemandClaim(int orderID)
        {
            var result = _cartControllerOrchestrator.InsertOnDemandClaim(orderID);

            return result;
        }


        [HttpPost]
        [ValidateJsonAntiForgeryToken(Order = 0)]
        [HandleAjaxException(Order = 1)]
        [AllowAnonymous]
        public ActionResult ApplyDiscountCode(string code, int orderRowId)
        {
            try
            {
                var row = _cartControllerOrchestrator.GetOrderRowLoaded(orderRowId);

                var myDiscount = _cartControllerOrchestrator.ApplyDiscountCode(code, row);

                if (!ReferenceEquals(myDiscount, null))
                {
                    _cartControllerOrchestrator.UpdateOrderPricing(row.Order);

                    var pricesAndDiscounts = _cartControllerOrchestrator.UpdateOrderPricing(row.Order);

                    return
                        Json(
                            new
                            {
                                regTypeShort = row.RegistrationType.OptionLabelShort,
                                BasePrice = pricesAndDiscounts.UnitPrice,
                                Discount = pricesAndDiscounts.TotalDiscount,
                                OptionsPrice = pricesAndDiscounts.TotalCostOfOptions,
                                Tax = pricesAndDiscounts.TaxAmount,
                                Total = pricesAndDiscounts.TotalOrderPrice,
                                FlatOff = pricesAndDiscounts.Discount.FlatOff,
                                PercentOff = pricesAndDiscounts.Discount.PercentOff
                            });
                }
                else
                {
                    return Json(new { Result = 0, Code = code, Order = orderRowId });
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
                    _cartControllerOrchestrator.AddClaimForPostEventMaterials(model.WebUser.email, model.Order.OrderRows.FirstOrDefault());


                    if (User.Identity.IsAuthenticated)
                    {
                        _cartControllerOrchestrator.FireOrderSubmittedNotification(model.Order, userCreatedInCart: false);
                    }
                    else
                    {
                        _cartControllerOrchestrator.FireOrderSubmittedNotification(model.Order, userCreatedInCart: true);
                    }

                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty,
                        "ConfirmOrder|ConfirmOrder failed.");
                    _logger.ErrorException("ConfirmOrder|ConfirmOrder failed ", exception);
                    ErrorSignal.FromCurrentContext().Raise(exception);
                }
                try
                {
                    _cartControllerOrchestrator.UpdateOrderPricing(model.Order);
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, "ConfirmOrder|UpdateOrderPricing failed.");
                    _logger.ErrorException("ConfirmOrder|UpdateOrderPricing failed ", exception);
                    ErrorSignal.FromCurrentContext().Raise(exception);
                }

                try
                {
                    //_cartControllerOrchestrator.CreatePostEventClaim(model.Order);
                    _cartControllerOrchestrator.AddClaimForPostEventMaterials(model.Order.BillingEmail, model.Order.OrderRows.FirstOrDefault());

                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, "ConfirmOrder|CreatePostEventClaim failed");
                    _logger.ErrorException("ConfirmOrder|CreatePostEventClaim failed ", exception);
                    ErrorSignal.FromCurrentContext().Raise(exception);
                }

                return Json(new
                {
                    Result = WebUiConstants.Success,
                    OrderRowID = model.Order.idOrder,
                    Msg = string.Format("Your registration is confirmed. Complete details will be emailed to {0}.", model.Order.BillingEmail)
                }, JsonRequestBehavior.AllowGet);

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

                    JProperty adminMsg = new JProperty(JsonPropertyKeys.AffiliateCheckout, JsonConvert.SerializeObject(model.Order.Affiliate, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        }));

                    var editUserModel = _stateService.GetValue<EditUserModel>("editUserModel");
                    if (editUserModel != null)
                    {
                        adminMsg = new JProperty(JsonPropertyKeys.AffiliateCheckout, JsonConvert.SerializeObject(editUserModel, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        }));

                        model.Order.AdminComments = JsonHelpers.MergeJsonWithStoredField(model.Order.AdminComments, adminMsg);

                        model.Order.FirstName = editUserModel.FirstName;
                        model.Order.LastName = editUserModel.LastName;
                        model.Order.Institution = editUserModel.Institution;
                        model.Order.BillingAddress = editUserModel.BillingAddress.StreetAddress;
                        model.Order.BillingAddress2 = editUserModel.BillingAddress.StreetAddress2;
                        model.Order.BillingCity = editUserModel.BillingAddress.City;
                        model.Order.BillingState = editUserModel.BillingAddress.State;
                        model.Order.BillingZip = editUserModel.BillingAddress.Zip;
                        model.Order.ShippingAddress = editUserModel.ShippingAddress.StreetAddress;
                        model.Order.ShippingAddress2 = editUserModel.ShippingAddress.StreetAddress2;
                        model.Order.ShippingCity = editUserModel.ShippingAddress.City;
                        model.Order.ShippingState = editUserModel.ShippingAddress.State;
                        model.Order.ShippingZip = editUserModel.ShippingAddress.Zip;
                        _stateService.SetValue<EditUserModel>("editUserModel", null);
                    }

                    model.Order.OrderStatus = OrderStatus.Submitted;
                    _cartControllerOrchestrator.AddClaimForPostEventMaterials(model.WebUser.email, model.Order.OrderRows.FirstOrDefault());

                    model.Order.Origin = DomainConstants.CartByAffiliate;


                    if (Request.IsAuthenticated && !adminCreatedWebUser.HasValue)
                    {
                        _cartControllerOrchestrator.FireOrderSubmittedNotification(model.Order, userCreatedInCart: false);
                    }
                    else
                    {
                        _cartControllerOrchestrator.FireOrderSubmittedNotification(model.Order, userCreatedInCart: true);
                    }

                    _cartControllerOrchestrator.UpdateOrderPricing(model.Order);
                    var orderId = model.Order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).idOrderRow;

                    return Json(new
                    {
                        Result = WebUiConstants.Success,
                        OrderRowID = orderId,
                        Msg = string.Format("<p>Your Order ID is {0}.</p>", orderId)
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
            ModelState.AddModelError(string.Empty, "No Order ID was posted to the Server. In case of persistant error contact us at support@ttstrain.com. For immediate assistance, use our Help & Feedback button in your lower right screen.");
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
                    var order =  _cartControllerOrchestrator.LoadOrder(ID.Value);
                    ViewBag.Order = order;
                    //ViewBag.TaxAmount = model.DisplayRowPriceViewModel.PricesAndDiscounts.TaxAmount;
                    if ( order.OrderRows.Single(r => r.RowStatus == OrderRowStatus.Active).Discount != null)
                    {
                        ViewBag.DiscountCaption = _cartControllerOrchestrator.GetDiscountCaption(
                            order.OrderRows.Single(r => r.RowStatus == OrderRowStatus.Active).Discount,
                            order.OrderRows.Single(r => r.RowStatus == OrderRowStatus.Active),
                            null,
                            1);
                    }

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

            if (ID != null && ID > 0)
            {
                ViewBag.Order = _cartControllerOrchestrator.LoadOrder(ID.Value);
                ViewBag.TaxAmount = model.DisplayRowPriceViewModel.PricesAndDiscounts.TaxAmount;
            }
            else
            {
                _logger.Fatal("CheckoutConfirm was passed a null or zero value: ", new Exception("null or zero ID passed to CheckoutConfirm partial"));
            }
            return PartialView("Partials/CheckoutConfirmForAffiliate", model);
        }
        public ActionResult CheckoutDisplayRowPrice(int ID)
        {
            var model = _cartControllerOrchestrator.BuildDisplayRowPriceViewModel(null, ID);
            return PartialView("Partials/_DisplayRowPrice", model);
        }

        public PartialViewResult CheckoutContactDetails(int? fromAffCheckout)
        {
            if (fromAffCheckout == null)
            {
                return PartialView("~/Views/cart/Partials/CheckoutContact.cshtml",
                    _cartControllerOrchestrator.BuildRegisterViewModel());
            }
            else
            {
                return PartialView("~/Views/cart/Partials/CheckoutContactForAffiliate.cshtml",
                    _cartControllerOrchestrator.BuildRegisterViewModel());
            }
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
                _logger.Info("Signup2 Enters: " + _appHelper.GetSessionStartInfo());

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
                        _logger.ErrorException(string.Format("Signup2 | {0}", _appHelper.GetUserAuditInfo()),
                            exception);
                        return Json(new
                        {
                            Result = WebUiConstants.Fail,
                            Msg = exception.Message
                        }, JsonRequestBehavior.AllowGet);
                    }

                    ModelState.AddModelError(string.Empty,
                        exception.Message.Contains(ErrorMessageConstants.ExistingNonCancelledOrderMessage)
                            ? ErrorMessageConstants.ExistingNonCancelledOrderMessage
                            : "Error condition ." + exception.Message);

                    _logger.FatalException("Signup2 order died. ", exception);

                    ErrorSignal.FromCurrentContext().Raise(exception);
                }
            }

            return this.ModelStateJson(ModelState);
        }

        [HttpPost]
        public ActionResult SignupAffiliate(CheckoutOptionsViewModel formModel)
        {

            _logger.Info("AffiliateSignup: " + JsonConvert.SerializeObject(formModel, Formatting.None, new JsonSerializerSettings { MaxDepth = 1, ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
            if (ModelState.IsValid)
            {
                var orderAlreadyExists = _cartControllerOrchestrator.GetOrderByUserIdAndWebinar(
                    formModel.idUser, formModel.idWebinar);

                if (orderAlreadyExists != null && orderAlreadyExists.Count > 0)
                {
                    _logger.Warn("SignupAffiliate pulls existing order: " + formModel.idUser + " " + formModel.idWebinar);
                    foreach (var order in orderAlreadyExists)
                    {
                        if (order.OrderStatus == OrderStatus.Submitted ||
                            order.OrderStatus == OrderStatus.Paid)
                        {
                            return Json(new
                            {
                                success = "AlreadySubmitted",
                                orderId = order.idOrder,
                                orderRowId =
                                    order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active)
                                        .idOrderRow,
                                webinarId = formModel.idWebinar
                            }, JsonRequestBehavior.AllowGet);
                        }


                        if (order.OrderStatus == OrderStatus.Canceled)
                        {
                            return Json(new
                            {
                                success = "WasCanceled",
                                orderId = order.idOrder,
                                orderRowId =
                                    order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active)
                                        .idOrderRow,
                                webinarId = formModel.idWebinar
                            }, JsonRequestBehavior.AllowGet);
                        }
                        if (order.OrderStatus == OrderStatus.InProcess ||
                            order.OrderStatus == OrderStatus.AwaitingVerification)
                        {
                            return Json(new
                            {
                                success = "InProcess",
                                orderId = order.idOrder,
                                orderRowId =
                                    order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active)
                                        .idOrderRow,
                                webinarId = formModel.idWebinar
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }


            try
            {
                var order = _cartControllerOrchestrator.CreateOrderByAffiliate(
                    formModel, _stateService.GetValue<Affiliate>("CurrentAffiliate")

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
                return Json(new
                {
                    success = "fail",
                    orderId = 0,
                    orderRowId = 0,
                    webinarId = formModel.idWebinar
                }, JsonRequestBehavior.AllowGet);

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
                    ModelState.AddModelError(string.Empty, "There has been an error. For customer service contact us by using the Online Chat button below or emailing Support@ttsTrain.com.");
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

                    var newRegType = regType.OptionLabel;
                    var oldRegType = _cartControllerOrchestrator.GetOrderRowLoaded(idOrderRow.Value).RegistrationType.OptionLabel;

                    var model = _cartControllerOrchestrator.BuildCheckOutViewModel(idOrderRow);
                    model.Order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).RegistrationType = regType;
                    
                    

                    var pricesAndDiscounts = _cartControllerOrchestrator.UpdateOrderPricing(model.Order);

                    var discountCaption = "";
                    if (pricesAndDiscounts.Discount == null)
                    {
                        pricesAndDiscounts.Discount = new Discount();
                    }
                    else
                    {
                     discountCaption = _cartControllerOrchestrator.GetDiscountCaption(
                            model.Order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).Discount,
                            model.Order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active), null, 1);
                    }



                    _logger.Info("EditRegTypeTo: " + newRegType + " From: " + oldRegType + " on orderId: " + model.Order.idOrder);
                    var UpdateSuccessCaption = "Order updated to: " + newRegType ;
                    
                    return
                        Json(
                            new
                            {
                                DiscountCaption = discountCaption,
                                UpdateSuccessCaption = UpdateSuccessCaption,
                                regTypeShort = regType.OptionLabelShort,
                                BasePrice = pricesAndDiscounts.UnitPrice,
                                Discount = pricesAndDiscounts.TotalDiscount,
                                OptionsPrice = pricesAndDiscounts.TotalCostOfOptions,
                                Tax = pricesAndDiscounts.TaxAmount,
                                Total = pricesAndDiscounts.TotalOrderPrice,
                                FlatOff = pricesAndDiscounts.Discount.FlatOff,
                                PercentOff = pricesAndDiscounts.Discount.PercentOff
                            });
                }
                catch (Exception exception)
                {
                    _logger.ErrorException(
                        String.Format("UpdateOrderDetails failed on {0} with {1} Session={2}", idRegType, exception.Message, _appHelper.GetUserAuditInfo()), exception);
                    ErrorSignal.FromCurrentContext().Raise(exception);

                    return Json(new { Result = WebUiConstants.Fail });

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

        //handle search engine cra
        public ActionResult ExpressCheckout4IE1()
        {
            return Content("Nothing for Search Engines here!");
        }


        public ActionResult ExpressCheckout4IE1CU()
        {
            return Content("Nothing for Search Engines here!");
        }

        [HttpPost]
        public ActionResult ExpressPostback2(ExpressCheckoutModel form)
        {
            try
            {
                if (form.q11_orderid != null && form.q11_orderid > 0)
                {
                    var order = _cartControllerOrchestrator.GetOrderById(form.q11_orderid);
                    
                    order.Origin = DomainConstants.OriginExpress;

                    order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active).RegistrationType =
                        _cartControllerOrchestrator.FindRegType4ExpressPostback2(form.q10_registrationType, form.q18_q_webinarid18);

                    _cartControllerOrchestrator.UpdateOrderPricing(order);

                    _logger.Info("ExpressPostback from: "+" - " + form.q11_orderid + _appHelper.GetUserAuditInfo() );

                    return RedirectToAction("Details", "Webinar", new { id = form.q18_q_webinarid18, idOrder = form.q11_orderid, source = "ExpressPostback2" });

                }

            }
            catch (Exception ex)
            {
                _logger.FatalException("ExpressCheckoutInternalPostback tossed: " + ex.Message, ex);
            }
            return null;

        }

        public ActionResult ExpressCheckout(int? idOrder, int? idWebinar, int? idAffiliate, string email)
        {
            try
            {
                if (idOrder != null && idOrder > 0)
                {
                    var order = _cartControllerOrchestrator.GetOrderById(idOrder);
                    WebUser user = _cartControllerOrchestrator.GetWebUserByEmail(email);

                    if (ReferenceEquals(user, null))
                    {

                        user = _membershipService.CreateBareUserFromEmail(email);
                        //throw new ArgumentNullException("user");

                    }

                    if (order != null)
                    {
                        order.AdminComments = "";
                        var newJson = new JProperty(string.Concat(JsonPropertyKeys.OrderCreatedByExpressCheckoutKey), JsonConvert.SerializeObject(_appHelper.GetSessionStartInfo()));

                        JProperty createdByImpersonatedUserMsg = new JProperty(JsonPropertyKeys.OrderCreatedByExpressCheckoutKey, newJson.Value);

                        order.AdminComments = JsonHelpers.MergeJsonWithStoredField(order.AdminComments, createdByImpersonatedUserMsg);


                        ExpressCheckoutModel model = _cartControllerOrchestrator.ExpressCheckout(order, user);
                        return View(model);
                    }
                    else
                    {
                        _logger.Fatal("ExpressChecked Null Order: " + idOrder.Value);
                    }


                }
                else
                {
                    var regTypeID =
                        _cartControllerOrchestrator.GetRegTypeByLabel("OnDemand Recording Only", idWebinar: idWebinar).idRegType;

                    int selectedUser = 0;
                    if (User.Identity.IsAuthenticated)
                    {
                        var identity = ClaimsPrincipal.Current;

                        if (identity != null)
                        {
                            if (identity.Identity.IsAuthenticated)
                            {
                                var userAccount = _membershipService.GetUserAccountByUserId(ClaimsExtensions.GetUserID(identity));

                                selectedUser = _membershipService.GetUserByEmail(userAccount.Email).idUser;       
                            }
                        }
                    }

                    var order = _cartControllerOrchestrator.CreateOrder(new CheckoutOptionsViewModel
                    {
                        idWebinar = idWebinar.Value,
                        RegistrationTypeId =
                            regTypeID,
                        SelectedWebUser = selectedUser
                    });

                    ViewBag.FromEarlyID = "true";
                    _logger.Info("ExpressCheckout PREIE10 builds form for: " + order.idOrder);
                    if (order.WebUser != null)
                    {
                        ExpressCheckoutModel model = _cartControllerOrchestrator.ExpressCheckout(order, order.WebUser);
                        return View(model);
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.FatalException("ExpressCheckout tossed: " + ex.Message, ex);
            }
            return null;

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

        [HttpPost]
        public ActionResult ThankYou(FormCollection form)
        {
            string formFields = Request.Form.ToString();
            _logger.Info("ExpressCheckoutThankYou postback: " + formFields);

            return View();
            //string formFields = Request.Form.ToString();
            //_logger.Info("ExpressCheckoutThankYou postback: " + JsonConvert.SerializeObject(formFields));
            //_logger.Info("ExpressCheckoutThankYou postback: " + formFields);

            //try
            //{
            //    return RedirectToAction("Signup2", new CheckoutOptionsViewModel
            //     {
            //         idOrder = Convert.ToInt32(form.orderid),
            //         idWebinar = Convert.ToInt32(form.q_webinarid18)
            //     });

            //}
            //catch (Exception ex)
            //{
            //    _logger.Fatal("", ex);
            //}

            //try
            //{
            //    form = _cartControllerOrchestrator.BuildExpressPostback(form);

            //    return RedirectToAction("Signup2", new CheckoutOptionsViewModel
            //     {
            //         idOrder = Convert.ToInt32(form.orderid),
            //         idWebinar = Convert.ToInt32(form.q_webinarid18)
            //     });

            //}
            //catch (Exception ex)
            //{
            //    _logger.Fatal("BuildExpressPostback toss: ", ex.Message);
            //}
            //return View(form);
        }

        [HttpPost]
        public ActionResult PostBackMoneris(MonerisResponse form)
        {
            string formFields = Request.Form.ToString();
            _logger.Info("PostBackMoneris: " + formFields);
            var order = _cartControllerOrchestrator.LoadOrder(Convert.ToInt32(form.order_no.Split('-')[1]));

            if (order == null) throw new ArgumentNullException("order");

            JProperty monerisResponse = new JProperty(JsonPropertyKeys.MonerisResponse, JsonConvert.SerializeObject(form));

            order.AdminComments = JsonHelpers.MergeJsonWithStoredField(order.AdminComments, monerisResponse);

            _cartControllerOrchestrator.UpdateOrderPricing(order);

            if (form.message.StartsWith("APPROVED"))
            {
                try
                {
                    _logger.Info("Confirming Moneris submission with Id BW-{0}", JsonConvert.SerializeObject(order, Formatting.None, new JsonSerializerSettings { MaxDepth = 1, ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));

                    order.OrderStatus = OrderStatus.Paid;

                    //_cartControllerOrchestrator.CreatePostEventClaim(order);
                    _cartControllerOrchestrator.AddClaimForPostEventMaterials(order.BillingEmail, order.OrderRows.FirstOrDefault());

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
                        "Connection Error #552. Please contact the administrator to complete transaction.");
                    _logger.ErrorException("MonerisConfirmOrder returned APPROVED but controller failed ", exception);
                    ErrorSignal.FromCurrentContext().Raise(exception);
                }

                _logger.Error("ConfirmOrder Action | Id parameter was null");
                _logger.Error(string.Format("ConfirmOrder Action | {0}", _appHelper.GetSessionStartInfo()));
            }
            else
            {
                _logger.Info("Moneris declined with msg {0}: order {1}", form.message, JsonConvert.SerializeObject(order, Formatting.None, new JsonSerializerSettings { MaxDepth = 1, ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
                return Json(new
                {
                    Result = WebUiConstants.Fail,
                    OrderRowID = order.idOrder,
                    Msg = string.Format("Your credit card transaction did not complete successfully. Our processor replied with this message: {0}", form.message)
                }, JsonRequestBehavior.AllowGet);

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
            try
            {
                _cartControllerOrchestrator.UpdateAdditionalLocationsForOrderRow(additionalLocations, newOrderRowId.Value);
                return Json(new { Result = WebUiConstants.Success });
            }
            catch (Exception ex)
            {
                _logger.Fatal("UpdateAdditionalLocations", ex);
                return Json(new { Result = WebUiConstants.Fail });
            }
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