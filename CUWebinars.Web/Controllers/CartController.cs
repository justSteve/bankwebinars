using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.ModelBinding;
using System.Web.Mvc;
using System.Web.Routing;
using CUWebinars.Business.AccountService;
using CUWebinars.Business.Constants;
using CUWebinars.Business.Core;
using CUWebinars.Business.Core.Helpers;
using CUWebinars.Business.Models;
using CUWebinars.Business.Notification.ViewModel;
using CUWebinars.Web.Core;
using CUWebinars.Web.Core.Orchestrators;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Infrastructure.Attributes;
using CUWebinars.Web.Infrastructure.Extensions;
using CUWebinars.Web.Mapping.Mappers;
using CUWebinars.Web.Models;
using CUWebinars.Web.Models.DataTablesModels;
using CUWebinars.Web.Models.Importers;
using CUWebinars.Web.Services;
using CUWebinars.Web.ViewModel;
using Elmah;
using Microsoft.ApplicationInsights;
using NameParser;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ninject.Extensions.Logging;
using RestSharp;
using ClaimTypes = CUWebinars.Business.Constants.ClaimTypes;
using HttpCookie = System.Web.HttpCookie;

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
        private readonly GlobalConfig _globalConfig = GlobalConfig.GlobalConfigSingleton;

        private bool _disposed;
        private TelemetryClient telemetry = new TelemetryClient();
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
        public ActionResult RemoveDiscountCode(string code, int orderRowId)
        {
            try
            {
                var row = _cartControllerOrchestrator.GetOrderRowLoaded(orderRowId);
                var myDiscount = _cartControllerOrchestrator.GetDiscountById(row.Discount.idDiscount);
                var result = _cartControllerOrchestrator.RemoveDiscountCode(code, row);

                if (result == "Succeeded")
                {
                    //_cartControllerOrchestrator.UpdateOrderPricing(row.Order);
                    var newJson = new JProperty(
                        "DiscountIsRemoved",
                        new JObject(new JProperty("By", _appHelper.GetUserAuditInfo())));

                    row.Order.AdminComments = JsonHelpers.ReplaceJsonWithStoredField(row.Order.AdminComments, newJson,
                        "DiscountIsRemoved");
                    var discountCaption = "";

                    var updateSuccessCaption = "Removed Discount";

                    var pricesAndDiscounts = _cartControllerOrchestrator.UpdateOrderPricingReadOnly(row.Order);
                    if (pricesAndDiscounts.Discount == null)
                    {
                        pricesAndDiscounts.Discount = new Discount();
                    }
                    else
                    {
                        discountCaption = _cartControllerOrchestrator.GetDiscountCaption(
                               row.Discount,
                               row, null, 1);
                    }
                    return
                        Json(
                            new
                            {
                                msg = "Discount Removed",
                                UpdateSuccessCaption = updateSuccessCaption,
                                DiscountCaption = discountCaption,
                                regTypeShort = row.RegistrationType.OptionLabelShort,
                                BasePrice = pricesAndDiscounts.UnitPrice,
                                Discount = pricesAndDiscounts.TotalDiscount,
                                OptionsPrice = pricesAndDiscounts.TotalCostOfOptions,
                                Tax = pricesAndDiscounts.TaxAmount,
                                TotalPaid = row.Order.TotalPaid,
                                OutstandingBalance = (pricesAndDiscounts.UnitPrice + pricesAndDiscounts.TotalCostOfOptions + pricesAndDiscounts.TaxAmount - pricesAndDiscounts.TotalDiscount) - row.Order.TotalPaid,
                                Total = pricesAndDiscounts.TotalOrderPrice,
                                FlatOff =
                                (pricesAndDiscounts.Discount != null) ? pricesAndDiscounts.Discount.FlatOff : 0,
                                PercentOff =
                                (pricesAndDiscounts.Discount != null) ? pricesAndDiscounts.Discount.PercentOff : 0,
                                CreditsRemain =
                                _cartControllerOrchestrator.CalculateCreditsRemaining(myDiscount).ToString()
                            });
                }
                else
                {
                    return Json(new { Result = 0, Code = "Failed to Remove: " + code, Order = orderRowId });
                }

            }
            catch (Exception exception)
            {
                //ModelState.AddModelError(string.Empty, "Code Not Found.");
                _logger.ErrorException("RemoveDiscountCode| failed " + code + " orderRowId: " + orderRowId, exception);
                return Json(new { Result = 0, Code = "Failed to Remove: " + code, Order = orderRowId });
            }
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

                    var msg = myDiscount.Notes;
                    //_cartControllerOrchestrator.UpdateOrderPricing(row.Order);
                    var newJson = new JProperty(
                        "DiscountIsApplied",
                        new JObject(new JProperty("DiscountID", myDiscount.idDiscount)
                            , new JProperty("By", _appHelper.GetUserAuditInfo())));


                    row.Order.AdminComments = JsonHelpers.ReplaceJsonWithStoredField(row.Order.AdminComments, newJson,
                        "DiscountIsApplied");
                    //row.Order.AffiliateComments = JsonHelpers.AddObjectToJsonArray(row.Order.AffiliateComments, newJson);
                    var discountCaption = "";


                    if (myDiscount.DiscountType == DiscountType.Subscription &&
                        myDiscount.DateValidFrom != myDiscount.DateValidTo
                        && row.RegistrationType.ShowShippedNotifications.ToLower() == "yes")
                    {
                        ViewBag.DiscountSurcharge = "A $50 surcharge is added for shipping & handling";
                        msg += " A $50 surcharge is added for shipping & handling.";
                    }
                    var updateSuccessCaption = "Applied Discount";

                    var pricesAndDiscounts = _cartControllerOrchestrator.UpdateOrderPricingReadOnly(row.Order);
                    if (pricesAndDiscounts.Discount == null)
                    {
                        pricesAndDiscounts.Discount = new Discount();
                    }
                    else
                    {
                        discountCaption = _cartControllerOrchestrator.GetDiscountCaption(
                               row.Discount,
                               row, null, 1);
                    }
                    return
                        Json(
                            new
                            {
                                msg = msg,
                                UpdateSuccessCaption = updateSuccessCaption,
                                DiscountCaption = discountCaption,
                                regTypeShort = row.RegistrationType.OptionLabelShort,
                                BasePrice = pricesAndDiscounts.UnitPrice,
                                Discount = pricesAndDiscounts.TotalDiscount,
                                OptionsPrice = pricesAndDiscounts.TotalCostOfOptions,
                                Tax = pricesAndDiscounts.TaxAmount,
                                TotalPaid = row.Order.TotalPaid,
                                OutstandingBalance = (pricesAndDiscounts.UnitPrice + pricesAndDiscounts.TotalCostOfOptions + pricesAndDiscounts.TaxAmount - pricesAndDiscounts.TotalDiscount) - row.Order.TotalPaid,
                                Total = pricesAndDiscounts.TotalOrderPrice,
                                FlatOff =
                                (pricesAndDiscounts.Discount != null) ? pricesAndDiscounts.Discount.FlatOff : 0,
                                PercentOff =
                                (pricesAndDiscounts.Discount != null) ? pricesAndDiscounts.Discount.PercentOff : 0,
                                CreditsRemain =
                                _cartControllerOrchestrator.CalculateCreditsRemaining(myDiscount).ToString()
                            });
                }
                else
                {
                    return Json(new { Result = 0, Code = code, Order = orderRowId });
                }

            }
            catch (Exception exception)
            {
                //ModelState.AddModelError(string.Empty, "Code Not Found.");
                _logger.ErrorException("ApplyDiscountCode|ApplyDiscountCode failed " + code + " orderRowId: " + orderRowId, exception);
                return Json(new { Result = 0, Code = code, Order = orderRowId });
            }
        }


        public PartialViewResult GetAdditionalLocationByOrderId(int webinarId, int? webUserId = 0)
        {
            // webUserId is hard-coded to 0 because it should never be used. --- prove me wrong
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
                return ConfirmOrder(id);
            }

            _logger.Error("ConfirmOrder Action | Id parameter was null");
            _logger.Error(string.Format("ConfirmOrder Action | {0}", _appHelper.GetUserAuditInfo()));

            return this.ModelStateJson(ModelState);
        }

        private ActionResult ConfirmOrder(int? id)
        {
            _logger.Info("Confirming Order for OrderRow with Id {0}", id.Value);
            var model = _cartControllerOrchestrator.BuildCheckOutViewModel(id);
            var row = model.Order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active);


            if (_cartControllerOrchestrator.UserHasMultipleEvents(id))
            {
                return Json(new
                {
                    Result = "UserHasMulti"
                }, JsonRequestBehavior.AllowGet);
            }
            var createdSeriesOrders = "";
            try
            {
                if (model.Order.OrderStatus == OrderStatus.InProcess
                    || model.Order.OrderStatus == OrderStatus.Canceled
                    || model.Order.OrderStatus == OrderStatus.AwaitingVerification
                )
                    model.Order.OrderDate = DateTime.Now;
                model.Order.OrderStatus = OrderStatus.Submitted;
                _cartControllerOrchestrator.AddClaimForPostEventMaterials(model.WebUser.email, row);
            }
            catch (Exception exception)
            {
                ModelState.AddModelError(string.Empty,
                    "ConfirmOrder|ConfirmOrder failed.");
                _logger.ErrorException("ConfirmOrder|ConfirmOrder failed " + model.Order.idOrder, exception);
                ErrorSignal.FromCurrentContext().Raise(exception);
            }
            try
            {
                if (model.Order.Total == 0)
                {
                    model.Order.OrderStatus = OrderStatus.Paid;
                    model.Order.TotalPaid = model.Order.Total;
                }

                _cartControllerOrchestrator.SaveOrder(model.Order);

                if (model.Webinar.idWebinar == 2520)
                {
                    //is a WSP order
                    _cartControllerOrchestrator.CreateWspCode(model.Order);
                }

                if (model.Webinar.SeriesInfo.Contains("Children"))
                    if (model.Order.OrderRows != null)
                        createdSeriesOrders =
                            _cartControllerOrchestrator.CreateSeriesOrders(
                                model.Order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active));


                if (_globalConfig.Tenant == "DirectorSeries")
                {
                    Order order = model.Order;
                    Discount desSub = new Discount
                    {
                        DiscountType = DiscountType.ComplianceSeries,
                        DateValidTo = order.OrderDate.AddYears(1),
                        DateValidFrom = order.OrderDate,
                        Cost = row.RowPrice,
                        DateBilled = DateTime.UtcNow,
                        DateVerified = DateTime.Now,
                        DiscountCode = "DES" + order.idOrder,
                        FlatOff = 0,
                        Notes = "V3 entry",
                        PercentOff = 0,
                        RenewalTerm = 1,
                        Status = "Active",
                        TotalCount = 1,
                        idAffiliate = order.idAffiliate,
                        idDiscount = order.idOrder
                    };
                    row.Discount = desSub;
                }
                if (_globalConfig.Tenant == "CCS")
                {
                    Order order = model.Order;
                    Discount ccsSub = new Discount
                    {
                        DiscountType = DiscountType.DirectorSeries,
                        DateValidTo = order.OrderDate.AddYears(1),
                        DateValidFrom = order.OrderDate,
                        Cost = row.RowPrice,
                        DateBilled = DateTime.UtcNow,
                        DateVerified = DateTime.Now,
                        DiscountCode = "CCS" + order.idOrder,
                        FlatOff = 0,
                        Notes = "V3 entry",
                        PercentOff = 0,
                        RenewalTerm = 1,
                        Status = "Active",
                        TotalCount = 1,
                        idAffiliate = order.idAffiliate,
                        idDiscount = order.idOrder
                    };
                    row.Discount = ccsSub;
                }
                _cartControllerOrchestrator.SaveOrder(model.Order);
                _SendOrderConfirmation2(model.Order.idOrder);
            }
            catch (Exception exception)
            {
                ModelState.AddModelError(string.Empty, "ConfirmOrder|UpdateOrderPricing failed.");
                _logger.ErrorException("ConfirmOrder|UpdateOrderPricing failed:  " + model.Order.idOrder, exception);
                ErrorSignal.FromCurrentContext().Raise(exception);
            }

            if (model.Order.Origin.ToLower().StartsWith("express"))
            {

                return RedirectToAction("OrderComplete", "Account", new { id = model.Order.idOrder });
            }
            else
            {
                return Json(new
                {
                    Result = WebUiConstants.Success,
                    OrderRowID = model.Order.idOrder,
                    Msg =
                    string.Format(
                        "Thank you! Your registration is confirmed as: " + model.Order.idOrder +
                        ". Complete details will be emailed to {0}. {1}",
                        model.Order.BillingEmail, createdSeriesOrders)
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult SendOrderConfirmation2(int orderId)
        {
            _SendOrderConfirmation2(orderId);
            return null;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult SendOrderConfirmation2Batch(string _orderIds)
        {
            IList<string> orderIds = _orderIds.Split(',');
            foreach (var orderId in orderIds)
            {
                _SendOrderConfirmation2(Convert.ToInt32(orderId));

            }
            return null;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult RegChange()
        {
            var sb = new StringBuilder();

            if (User.Identity.IsAuthenticated)
            {
                sb.Append("User:" + User.Identity.Name);
            }

            sb.Append("SessionInfo" + _appHelper.GetUserAuditInfo());

            _logger.Info(sb.ToString());
            return Redirect("https://www.RegChange.com");
        }

        private void _SendOrderConfirmation2(int idOrder)
        {
            _logger.Info("_SendOrderConfirmation2 begins: " + idOrder);
            var order = _cartControllerOrchestrator.GetOrderById(idOrder);
            if (order == null)
                throw new NullReferenceException();
            var row = order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active);
            var sendToAddresses = order.BillingEmail;
            var hasCc = _cartControllerOrchestrator.OrderHasCc(order);
            if (hasCc != null)
            {
                sendToAddresses = sendToAddresses + "; " + hasCc;
            }
            var affiliateAddresses = order.Affiliate.NotiOrders.Replace(',', ';');
#if DEBUG
            sendToAddresses = ConfigurationManager.AppSettings["TestEmailAddress"];
            affiliateAddresses = ConfigurationManager.AppSettings["TestEmailAddress"];
#endif

            if (User.Identity.IsAuthenticated)
            {
                //M4Gen
                _logger.Info("ConfirmOrder UserIdent: " + User.Identity.Name);
                var orderConfirmString =
                    _cartControllerOrchestrator.BuildOrderSubmitted2Notification(order);
                orderConfirmString = _appHelper.CleanHtmlCodesAndLogo(orderConfirmString,
                    _globalConfig.TenantLogo, _cartControllerOrchestrator.GetAddLocPrice(row.Webinar), null);


                _cartControllerOrchestrator.FireMandrillNotificationEvent(
                    sendToAddresses
                    , "Confirmation of Registration for " + row.Webinar.Title, orderConfirmString);
                _cartControllerOrchestrator.FireMandrillNotificationEvent(
                    affiliateAddresses
                    , "Order Placed For " + order.BillingEmail + " - " + row.Webinar.Title, orderConfirmString);

                _cartControllerOrchestrator.FireMandrillNotificationEvent(
                    "info@ttstrain.com"
                    , "Catch All Orders " + order.BillingEmail + " - " + row.Webinar.Title, orderConfirmString);

            }
            else
            {
                if (order.Origin == "Express")
                {
                    _logger.Info("ConfirmOrder Express: " + order.idOrder);

                    //M4Gen
                    var orderConfirmString =
                        _cartControllerOrchestrator.BuildOrderSubmitted2Notification(order);

                    orderConfirmString = _appHelper.CleanHtmlCodesAndLogo(orderConfirmString,
                        _globalConfig.TenantLogo, _cartControllerOrchestrator.GetAddLocPrice(row.Webinar), null);
                    _cartControllerOrchestrator.FireMandrillNotificationEvent(
                       sendToAddresses //ConfigurationManager.AppSettings["TestEmailAddress"]
                        , "Confirmation of Registration for " + row.Webinar.Title, orderConfirmString);

                    _cartControllerOrchestrator.FireMandrillNotificationEvent(
                        affiliateAddresses
                        , "Order Placed For " + order.BillingEmail + " - " + row.Webinar.Title, orderConfirmString);


                    _cartControllerOrchestrator.FireMandrillNotificationEvent(
                        "info@ttstrain.com"
                        , "Catch All Orders " + order.BillingEmail + " - " + row.Webinar.Title, orderConfirmString);
                }
                else
                {
                    // order is being placed by a New User
                    var subject = "Confirmation of Registration for " + row.Webinar.Title;

                    if (_globalConfig.Tenant == "Director Series")
                        subject = "Confirmation of Director Series Subscription";

                    _logger.Info("ConfirmOrder of New User: " + order.idOrder);

                    //M4Gen
                    var orderConfirmString = _cartControllerOrchestrator.BuildOrderSubmitted2Notification(order);

                    orderConfirmString = _appHelper.CleanHtmlCodesAndLogo(orderConfirmString,
                        _globalConfig.TenantLogo, _cartControllerOrchestrator.GetAddLocPrice(row.Webinar), null);
                    _cartControllerOrchestrator.FireMandrillNotificationEvent(
                        sendToAddresses
                        , subject, orderConfirmString);

                    _cartControllerOrchestrator.FireMandrillNotificationEvent(
                        affiliateAddresses
                        , "Order Placed For " + order.BillingEmail + " - " + row.Webinar.Title, orderConfirmString);

                    _cartControllerOrchestrator.FireMandrillNotificationEvent(
                        "info@ttstrain.com"
                        , "Catch All Orders " + order.BillingEmail + " - " + row.Webinar.Title, orderConfirmString);

                }

            }

        }

        [HttpPost]
        public ActionResult ConfirmOrderForAffiliate(string sendHardcopy, int? id = null, bool? adminCreatedWebUser = null)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (id == null)
                    {
                        id = Convert.ToInt32(Request.UrlReferrer.ToString().Split('=')[1].Split('&')[0]);
                        id =
                            _cartControllerOrchestrator.GetOrderById(id)
                                .OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active)
                                .idOrderRow;
                    }

                    var model = _cartControllerOrchestrator.BuildCheckOutViewModel(id);

                    JProperty adminMsg = new JProperty(JsonPropertyKeys.AffiliateCheckout,
                        JsonConvert.SerializeObject(model.Order.Affiliate, Formatting.None,
                            new JsonSerializerSettings()
                            {
                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                            }));

                    var editUserModel = _stateService.GetValue<EditUserModel>("editUserModel");
                    if (editUserModel != null)
                    {
                        adminMsg = new JProperty(JsonPropertyKeys.AffiliateCheckout,
                            JsonConvert.SerializeObject(editUserModel, Formatting.None,
                                new JsonSerializerSettings()
                                {
                                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                                }));

                        model.Order.AdminComments = JsonHelpers.MergeJsonWithStoredField(model.Order.AdminComments,
                            adminMsg);

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

                    _cartControllerOrchestrator.SaveOrder(model.Order);
                    if (model.Webinar.idWebinar == 2520)
                    {
                        //is a WSP order
                        _cartControllerOrchestrator.CreateWspCode(model.Order);
                    }
                    model.Order.OrderStatus = OrderStatus.Submitted;
                    _cartControllerOrchestrator.AddClaimForPostEventMaterials(model.WebUser.email,
                        model.Order.OrderRows.FirstOrDefault());

                    model.Order.Origin = DomainConstants.CartByAffiliate;
                    _cartControllerOrchestrator.SaveOrder(model.Order);
                    _SendOrderConfirmation2(model.Order.idOrder);

                    var row = model.Order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active);

                    Debug.Assert(row != null, "row != null");
                    var createdSeriesOrders = "";
                    if (row.Webinar.SeriesInfo.Contains("Children"))
                        createdSeriesOrders = _cartControllerOrchestrator.CreateSeriesOrders(row);


                    if (row.Webinar.Title.Contains("Compliance Perspectives")
                    //test if CP code needs to be created by ensuring it's not a trial
                        && row.RowPrice > 200)
                        _cartControllerOrchestrator.CreateCompliancePerspectivesSubscription(row);


                    var orderId = model.Order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).idOrderRow;
                    _cartControllerOrchestrator.UpdateOrderPricing(model.Order);

                    return Json(new
                    {
                        Result = WebUiConstants.Success,
                        OrderRowID = orderId,
                        Msg = string.Format("<p>Your Order ID is {0}.{1}</p>", orderId, createdSeriesOrders)
                    }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty,
                        "There was a problem at the server. Please contact the administrator.");
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
                try
                {
                    _cartControllerOrchestrator.CancelOrder(id.Value);

                    _logger.Info("CancelOrder idOrder: " + id);

                    return Json(new
                    {
                        success = "success"

                    }, JsonRequestBehavior.AllowGet);

                }
                catch (Exception ex)
                {
                    _logger.FatalException("CancelOrder " + id, ex);
                }
            }
            ModelState.AddModelError(string.Empty,
                "No Order ID was posted to the Server. In case of persistent error contact us at " +
                _globalConfig.TenantEmail +
                ". For immediate assistance, use our Help & Feedback button in your lower right screen.");
            return this.ModelStateJson(ModelState);
        }


        public ActionResult CheckoutOptions(int? idWebinar, int? idOrderRow, int? idOrder)
        {
            var model = _cartControllerOrchestrator.BuildCheckoutOptionsViewModel(null, idWebinar, idOrderRow, idOrder);
            return PartialView("Partials/CheckoutOptions", model);
        }


        [HttpPost]
        public ActionResult CheckoutConfirmSetCookie(CheckoutOptionsViewModel formModel)
        {
            var result = "";
            var setAffCookie = "";
            bool foundStartOrderCookie = false;
            var startedCookie = "";

            try
            {
                //int loop1, loop2;
                HttpCookieCollection MyCookieColl;
                //HttpCookie MyCookie;

                MyCookieColl = Request.Cookies;

                var cookieName = "StartOrder";
                var removeStartOrderCookies = MyCookieColl.AllKeys
                    .Select((name, i) => new { name, i })
                    .Where(x => x.name == cookieName)
                    .Select(x => ProcessStartOrder(MyCookieColl[x.i]));

                cookieName = WebUiConstants.AffiliateSessionSource;
                var checkAffSessionSourceCookie = MyCookieColl.AllKeys
                    .Select((name, i) => new { name, i })
                    .Where(x => x.name == cookieName)
                    .Select(x => ProcessAffSessionSource(MyCookieColl[x.i]));

                cookieName = WebUiConstants.MailChimpSource;
                var checkMailChimpSourceCookie = MyCookieColl.AllKeys
                    .Select((name, i) => new { name, i })
                    .Where(x => x.name == cookieName)
                    .Select(x => ProcessMailChimpSource(MyCookieColl[x.i]));



                var model = new CheckoutConfirmCookieModel
                {
                    InitialAffiliate = setAffCookie
                };
                return Json(new { model }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                _logger.FatalException("SetCookie", e);

                return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        private string ProcessMailChimpSource(HttpCookie httpCookie)
        {

            var mcCookie = httpCookie.Value.Split('|')[1];

            if (mcCookie != null && mcCookie !=
                _stateService.GetValue<Affiliate>(WebUiConstants.CurrentAffiliate).idUserAff.ToString())
            {
                _logger.Warn("Cookie/Session mismatch: " + mcCookie + " | " + _stateService
                                 .GetValue<Affiliate>(WebUiConstants.CurrentAffiliate).idUserAff.ToString());
                if (_stateService.GetValue<Affiliate>(WebUiConstants.CurrentAffiliate).idUserAff == 19)
                    _stateService.SetValue(WebUiConstants.CurrentAffiliate,
                        _cartControllerOrchestrator.GetAffiliateById(Convert.ToInt32(mcCookie)));
            }
            return mcCookie;
        }

        private string ProcessAffSessionSource(HttpCookie httpCookie)
        {
            var affByCookie = httpCookie.Value.Split('|')[1];

            if (affByCookie != null && affByCookie !=
                _stateService.GetValue<Affiliate>(WebUiConstants.CurrentAffiliate).idUserAff.ToString())
            {
                _logger.Warn("Cookie/Session mismatch: " + affByCookie + " | " + _stateService
                                 .GetValue<Affiliate>(WebUiConstants.CurrentAffiliate).idUserAff.ToString());
                if (_stateService.GetValue<Affiliate>(WebUiConstants.CurrentAffiliate).idUserAff == 19)
                    _stateService.SetValue(WebUiConstants.CurrentAffiliate,
                        _cartControllerOrchestrator.GetAffiliateById(Convert.ToInt32(affByCookie)));
            }
            return affByCookie;
        }

        private string ProcessStartOrder(HttpCookie httpCookie)
        {
            //currently just removing cookie
            httpCookie.Expires = DateTime.Now.AddDays(-1d);
            Response.Cookies.Add(httpCookie);
            return "";
        }

        public ActionResult CheckoutConfirm(int? ID = null)
        {
            var model = _cartControllerOrchestrator.BuildCheckoutConfirmViewModel(ID);
            if (model == null) throw new ArgumentNullException("model");
            try
            {
                var sessionAff = _stateService.GetValue<Affiliate>(WebUiConstants.CurrentAffiliate);
                if (ID != null && ID > 0)
                {
                    var order = _cartControllerOrchestrator.LoadOrder(ID.Value);
                    if (sessionAff != null && (sessionAff.idUserAff != order.Affiliate.idUserAff ||
                                               sessionAff.idUserAff != order.idAffiliate
                                               )
                                            && order.idAffiliate == 19)
                    {
                        _logger.Fatal("CheckoutConfirm has MISMATCHED AFFILIATE IDS: " + order.idOrder + " SessionAff: " +
                                      sessionAff.idUserAff);
                        order.Affiliate = sessionAff;
                        order.idAffiliate = sessionAff.idUserAff;
                        //_cartControllerOrchestrator.SaveOrder(order);
                    }
                    var row = order.OrderRows.Single(r => r.RowStatus == OrderRowStatus.Active);
                    ViewBag.TaxAmount = model.DisplayRowPriceViewModel.PricesAndDiscounts.TaxAmount;
                    if (row.Discount != null)
                    {
                        ViewBag.DiscountCaption = _cartControllerOrchestrator.GetDiscountCaption(
                            row.Discount, row, null, 1);
                    }

                    ViewBag.Order = order;
                    model.DisplayRowPriceViewModel.Origin = order.Origin;
                }
                else
                {
                    _logger.FatalException("CheckoutConfirm was passed a null or zero value: ",
                        new Exception("null or zero ID passed to CheckoutConfirm partial"));
                }
            }
            catch (Exception ex)
            {
                _logger.FatalException("CheckoutConfirm heard: ", ex);
                throw;
            }
            return PartialView("Partials/CheckoutConfirm", model);
        }


        public ActionResult CheckoutConfirmForAffiliate(int? ID = null)
        {
            {
                var model = _cartControllerOrchestrator.BuildCheckoutConfirmViewModel(ID);
                if (model == null) throw new ArgumentNullException("model");
                try
                {
                    var user = Request.RequestContext.HttpContext.User as ClaimsPrincipal;
                    if (user == null) throw new ArgumentNullException("user null at CreateOrderByAffiliate");

                    var currentAffByClaim = user.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Affiliate);
                    var currentAffiliate = _stateService.GetValue<Affiliate>("CurrentAffiliate");
                    if (currentAffByClaim != null && currentAffByClaim.Value != currentAffiliate.ttsDomain)
                        throw new Exception("CurrentAffiliate Mismatched!!");

                    var order = _cartControllerOrchestrator.LoadOrder(ID.Value);

                    if (order.OrderStatus == OrderStatus.Error)
                    {
                        order.OrderStatus = OrderStatus.InProcess;
                    }

                    ViewBag.Order = order;
                    ViewBag.TaxAmount = model.DisplayRowPriceViewModel.PricesAndDiscounts.TaxAmount;
                    if (order.OrderRows.Single(r => r.RowStatus == OrderRowStatus.Active).Discount != null)
                    {
                        ViewBag.DiscountCaption = _cartControllerOrchestrator.GetDiscountCaption(
                            order.OrderRows.Single(r => r.RowStatus == OrderRowStatus.Active).Discount,
                            order.OrderRows.Single(r => r.RowStatus == OrderRowStatus.Active),
                            null,
                            1);
                    }
                }
                catch (Exception ex)
                {
                    _logger.FatalException("CheckoutConfirm heard: ", ex);
                    throw;
                }
                return PartialView("Partials/CheckoutConfirmForAffiliate", model);
            }
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


        [HttpGet]
        public JsonResult PayTraceModalIni(int idOrder, string multi)
        {
            if (CheckIfMultiOrdersExist(idOrder) && !Request.QueryString.ToString().Contains("checked"))
            {
                return Json(new
                {
                    success = "success",
                    redirectUrl = Url.Action("Checkout", "Cart", new RouteValueDictionary("checked=true")),
                    isRedirect = true
                }, JsonRequestBehavior.AllowGet);
            }
            _logger.Info("Paytrace: " + idOrder + "  Starts to validate, multi: " + multi);
            dynamic ptLogger = new JObject();
            ptLogger.Starts = new JArray(idOrder, multi);

            //ensures that the price passed to PayTrace reflects order price with discount applied.
            var order = _cartControllerOrchestrator.GetOrderById(idOrder);
            //if the discount is a wsp null it out.
            if (order.OrderRows.Single(r => r.RowStatus == OrderRowStatus.Active).Discount != null && order.OrderRows.Single(r => r.RowStatus == OrderRowStatus.Active).Discount.DiscountType == DiscountType.Subscription
                && order.Total == 0)
                order.OrderRows.Single(r => r.RowStatus == OrderRowStatus.Active).Discount = null;
            var newPrice = _cartControllerOrchestrator.UpdateOrderPricing(order);

            ptLogger.Price = (decimal)order.Total;
            ptLogger.Time = DateTime.Now;


            string wTitle = order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active).Webinar.Title;

            string regType =
                order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active).RegistrationType.OptionLabel;
            decimal totalAmt = newPrice.TotalOrderPrice;

            string[] orderList = null;
            var ProdDesc = "";
            var ProdDescText = "";

            var loggerStruc = new PaytraceRequestToQueueModel();
            if (multi != null)
            {

                orderList = multi.TrimEnd(',').Split(',');

            }
            if (orderList != null && orderList.Length > 0)
            {

                loggerStruc.SingleOrMulti = "Multi";
                loggerStruc.idOrder = order.idOrder;
                loggerStruc.idUser = order.idUser;
                loggerStruc.idAffiliate = order.idAffiliate;
                loggerStruc.OrderList = orderList;
                loggerStruc.TimeStamp = TtsConfig.UtcNowAsCts.ToString(DomainConstants.DateTimeLongFormat);

                totalAmt = 0;
                ProdDesc += "<tr><td colspan=3 align=left><font size=2><b>Registration Details - " +
                            _globalConfig.Tenant + ".</b><br></font></td></tr>";
                ProdDesc += "<tr><td colspan=3 height=1 bgcolor=000000></td></tr>";
                ProdDesc += "<tr bgcolor=CCCCCC>";
                ProdDesc += "    <td align='Center'><b>Title</b></td>";
                ProdDesc += "    <td align='center'><b>Type</b></td>";
                ProdDesc += "    <td align='center'><b>Price</b></td>";
                ProdDesc += "</tr>";
                //foreach (var _idOrder in orderList)
                //{

                //}
                foreach (var _idOrder in orderList)
                {

                    var _order = _cartControllerOrchestrator.GetOrderById(Convert.ToInt32(_idOrder));
                    var _newPrice = _cartControllerOrchestrator.UpdateOrderPricing(_order);
                    _logger.Info("Paytrace: " + order.idOrder + "  multi-order  -- adds: " + _idOrder);
                    ptLogger.Msg += _idOrder + " -- adds: " + _order.Total;


                    //_cartControllerOrchestrator.SaveOrder(order);
                    _logger.Info("Paytrace Multi-order add: " + _idOrder);
                    totalAmt = totalAmt + _newPrice.TotalOrderPrice;

                    wTitle = _order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active).Webinar.Title;
                    wTitle = wTitle.Substring(0, Math.Min(wTitle.Length, 40)) + " <font size=2>(" +
                             _globalConfig.TenantPrefix +
                             _order.idOrder + ")</font>";
                    regType =
                        _order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active)
                            .RegistrationType.OptionLabel;
                    var _totalAmt = _newPrice.TotalOrderPrice.ToString("C").Replace(".00", "");

                    ProdDesc += "<tr bgcolor=CCCCCC>";
                    ProdDesc += "    <td align='left'>" + wTitle + "</td>";
                    ProdDesc += "    <td align='left'>" + regType + "</td>";
                    ProdDesc += "    <td align='left'>" + _totalAmt + "</td>";
                    ProdDesc += "</tr>";
                    if (ProdDescText.Length > 150)
                    {
                        ProdDescText += wTitle.Substring(0, Math.Min(wTitle.Length, 20)) + " (" +
                                        _globalConfig.TenantPrefix + _order.idOrder + ")" + Environment.NewLine;
                    }
                    else
                    {
                        ProdDescText += wTitle.Substring(0, Math.Min(wTitle.Length, 40)) + " (" +
                                        _globalConfig.TenantPrefix + _order.idOrder + ")" + Environment.NewLine;
                    }
                    if (_globalConfig.Tenant == "DirectorSeries")
                        ProdDescText = "Directors Education Series Subscription";
                    if (_globalConfig.Tenant == "CCS")
                        ProdDescText = "Core Compliance Suite Subscription";
                    _logger.Info("Paytrace: " + idOrder + "MultiPreValidate ProdDesc ends. ");
                }
            }
            else
            {
                //Single order is prevalidated
                loggerStruc.SingleOrMulti = "Single";
                loggerStruc.idOrder = order.idOrder;
                loggerStruc.idUser = order.idUser;
                loggerStruc.idAffiliate = order.idAffiliate;
                loggerStruc.OrderList = orderList;
                loggerStruc.TimeStamp = TtsConfig.UtcNowAsCts.ToString(DomainConstants.DateTimeLongFormat);

                _logger.Info("Paytrace: " + idOrder + " Single starts");

                ProdDesc += "<tr><td colspan=3 align=left><font size=2><b>Registration Details - " +
                            _globalConfig.Tenant + "</b></font></td></tr>";
                ProdDesc += "<tr><td colspan=3 height=1 bgcolor=000000></td></tr>";
                ProdDesc += "<tr bgcolor=CCCCCC>";
                ProdDesc += "    <td align='Center'><b>Title</b></td>";
                ProdDesc += "    <td align='center'><b>Type</b></td>";
                ProdDesc += "    <td align='center'><b>Price</b></td>";
                ProdDesc += "</tr>";

                ProdDesc += "<tr bgcolor=CCCCCC>";
                ProdDesc += "    <td align='left'>" + wTitle + "</td>";
                ProdDesc += "    <td align='left'>" + regType + "</td>";
                ProdDesc += "    <td align='left'>" + totalAmt + "</td>";
                ProdDesc += "</tr>";
                ProdDescText += wTitle.Substring(0, Math.Min(wTitle.Length, 40))
                    + " (" + _globalConfig.TenantPrefix + order.idOrder + ") " + Environment.NewLine;

                if (_globalConfig.Tenant == "DirectorSeries")
                    ProdDescText = "Directors Education Series Subscription";
                if (_globalConfig.Tenant == "CCS")
                    ProdDescText = "Core Compliance Suite Subscription";
            }

            //format parameters for request 
            // to get an approval amount set: AMOUNT~1.00
            // to get a declined amount set: AMOUNT~1.12


            string parameters = "UN~shuener|PSWD~" + _globalConfig.PayTracePassword + "|TERMS~Y|TRANXTYPE~Sale|";

            if (_globalConfig.EmailSendingMode != "live")
            {
                //if id of the webinar is odd send a magic tx amount to trigger a decline.
                if (order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active).idWebinar % 2 != 0)
                {
                    totalAmt = .21m;
                }
                else
                {
                    totalAmt = .08m;
                }
                parameters = "UN~demo123|PSWD~demo123|TERMS~Y|TRANXTYPE~Sale|";

                parameters += "ORDERID~" + idOrder + "|AMOUNT~" + totalAmt + "|";
                //parameters += "ApproveURL~" + _globalConfig.NGrokServer + "/cart/PayTraceApproved/|";
                //parameters += "DeclineURL~" + _globalConfig.NGrokServer + "/cart/PayTraceDeclined/|";
                //parameters += "ReturnURL~" + _globalConfig.NGrokServer + "/cart/PayTracePostBack/|";
                parameters += "ApproveURL~https://bwdev.azurewebsites.net/cart/PayTraceApproved/|";
                parameters += "DeclineURL~https://bwdev.azurewebsites.net/cart/PayTraceDeclined/|";
                parameters += "ReturnURL~https://bwdev.azurewebsites.net/cart/PayTracePostBack/|";
            }
            else
            {
                parameters += "ORDERID~" + idOrder + "|AMOUNT~" + totalAmt + "|";
                parameters += "ApproveURL~" + _globalConfig.TenantURL + "/cart/PayTraceApproved/|";
                parameters += "DeclineURL~" + _globalConfig.TenantURL + "/cart/PayTraceDeclined/|";
                parameters += "ReturnURL~" + _globalConfig.TenantURL + "/cart/PayTracePostBack/|";
            }



            string parameter_list = "PARMLIST=" + parameters;

            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] bytes = encoding.GetBytes(parameter_list);
            _logger.Info("PayTrace: " + order.idOrder + "  validation request: " + parameter_list);
            var responseString = "";
            var strResponse = "";
            try
            {

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://beta.paytrace.com/api/validate.pay ");
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://paytrace.com/api/validate.pay");
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = bytes.Length;

                // send validation request
                Stream str = request.GetRequestStream();
                str.Write(bytes, 0, bytes.Length);
                str.Flush();
                str.Close();

                // get response and parse
                WebResponse response = request.GetResponse();
                Stream rsp_stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(rsp_stream);

                // read the response string
                strResponse = reader.ReadToEnd();
                responseString = strResponse;
            }
            catch (Exception ex)
            {
                _logger.FatalException("PayTrace: " + idOrder + " dies at pre-auth.", ex);
                return Json(new
                {
                    success = "failed",
                    responseString,
                }, JsonRequestBehavior.AllowGet);
            }
            string authKey;

            // if we have errors if so output to ui
            if (!strResponse.Contains("ERROR"))
            {
                string[] _parameters = strResponse.Split('|');
                authKey = _parameters[1].Split('~')[1];

                _logger.Info("Paytrace: " + idOrder + " passed validation. AuthKey:" + authKey);
            }
            else
            {
                responseString = strResponse;
                authKey = "failed";
                _logger.Fatal("PayTrace: " + order.idOrder + " tx failed validation! " + order.idOrder + " " + strResponse);

                return Json(new
                {
                    success = "failed",
                    responseString,
                }, JsonRequestBehavior.AllowGet);
            }

            string paramList =
                string.Format(
                    "DISPLAYTRUSTLOGO~Y|DISABLETERMS~Y|ENABLEREDIRECT~N|RETURNPARIS~Y|authKey~{0}|disablelogin~y|disableoptional~N|showbname~y|hideinvoice~n|hidepassword~y|orderid~{1}|bname~{2}",
                    authKey, idOrder, order.FirstName + ' ' + order.LastName);
            paramList += "|ProductDetails~" + ProdDesc.Replace(System.Environment.NewLine, "");

            if (_globalConfig.EmailSendingMode != "live")
            {
                paramList += "|test~y";
            }

            paramList += "|baddress~" + order.BillingAddress;
            paramList += "|bcity~" + order.BillingCity;
            paramList += "|bstate~" + order.BillingState;
            paramList += "|bzip~" + order.BillingZip;
            paramList += "|bcountry~US";
            paramList += "|email~" + order.BillingEmail;
            //Paytrace does not permit phone 'extensions' and will not permit the tx to continue
            // and also will not let user correct the 'problem'. let's try just not sending that field.
            //paramList += "|phone~" + order.BillingPhone;
            paramList += "|CUSTOMDBA~" + _globalConfig.TenantDomain;
            paramList += "|DESCRIPTION~" + ProdDescText;
            paramList += "|IMAGEURL~" + _globalConfig.TenantLogo;
            paramList += "|CANCELURL~" + _globalConfig.TenantURL + "/cart/PayTraceCanceled";

            _stateService.SetValue(WebUiConstants.PayTraceSubmit, paramList);

            _logger.Info("PayTrace: " + order.idOrder + " submits: {1}, Session: {0}",
                _appHelper.GetUserAuditInfo(), paramList
            );

            try
            {

                loggerStruc.ValidateRequest = parameter_list;
                loggerStruc.ValidateResponse = strResponse;
                loggerStruc.TxRequest = paramList;
                //to be filled at postback.
                loggerStruc.TxResponse = "PendingPaytraceResponse";

                loggerStruc.AuditInfo = _appHelper.GetUserAuditInfo();
                if (loggerStruc.SingleOrMulti == "Single")
                {
                    order.AdminComments = JsonHelpers.AddObjectToJsonArray(
                        order.AdminComments,
                        JsonPropertyKeys.Single_OrderCheckout,
                        loggerStruc
                    );
                    _cartControllerOrchestrator.SaveOrder(order);
                }
                else
                {
                    foreach (var _idOrder in orderList)
                    {
                        var _order = _cartControllerOrchestrator.GetOrderById(Convert.ToInt32(_idOrder));

                        _order.AdminComments = JsonHelpers.AddObjectToJsonArray(
                            _order.AdminComments,
                            JsonPropertyKeys.Multi_OrderCheckout,
                            loggerStruc
                        );
                        _cartControllerOrchestrator.SaveOrder(_order);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.FatalException("Paytrace: " + order.idOrder + "  Multi_OrderCheckout failed to save and threw: " + order.AdminComments, ex);
            }

            _cartControllerOrchestrator.SaveOrder(order);


            return Json(new
            {
                success = "success",
                responseString,
                paramList

            }, JsonRequestBehavior.AllowGet);
        }

        private bool CheckIfMultiOrdersExist(int idOrder)
        {
            // do not allow anonymous (it errors due to null/empty list of orders)


            List<Order> orders = _cartControllerOrchestrator.GetOrdersByUser(_cartControllerOrchestrator.GetOrderById(idOrder).BillingEmail)
                .Where(o => o.OrderStatus == OrderStatus.InProcess).ToList();
            IList<int> iDsToRemove = new List<int>();
            var x = 0;
            foreach (var order in orders)
            {
                x++;

                var hasAnyOthers = _cartControllerOrchestrator.GetOrdersByUser(order.BillingEmail)
                    .Where(o => o.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active).idWebinar == order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active).idWebinar).OrderBy(o => o.idOrder);

                if (hasAnyOthers.Count() > x)
                {
                    iDsToRemove.Add(order.idOrder);
                }
            }
            foreach (var id in iDsToRemove)
            {
                var itemToRemove = orders.SingleOrDefault(o => o.idOrder == id);
                itemToRemove.OrderStatus = OrderStatus.Canceled;
                _cartControllerOrchestrator.SaveOrder(itemToRemove);
                if (itemToRemove != null)
                    orders.Remove(itemToRemove);
                _logger.Warn("Checkout found and canceled duped order: " + id);
                //_cartControllerOrchestrator.CancelOrder(id);
            }
            if (orders.Count > 1)
            {
                return true;
            }
            return false;
        }
        [AllowAnonymous]
        public void Incoming_ratewatch(string incoming)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                _logger.Info("Incoming_rateWatch Starts");

                var msgHtml = JsonConvert.DeserializeObject<MandrillIncomingMsg.mandrill_events>(incoming.ToString());
                //_logger.Info("Incoming_rateWatch " + msgHtml.msg);
                var parsedOrder = _appHelper.ParseRateWatch(msgHtml.msg.html);
                //var parsedOrder = ParseMandrillMsg.ParseRateWatch("---------- Forwarded message----------\nFrom: Stephen Hueners < steve@ttstrain.com >\nDate: Fri, Sep 8, 2017 at 9:12 AM\nSubject: FW: Webinar Registration\nTo: Steve Hueners<steve@juststeve.com>\n\n\n\n\n\n\n* From:*RateWatch[mailto: webmaster@rate - watch.com]\n* Sent:*Thursday, September 7, 2017 1:45 PM\n* To:*Info < info@ttstrain.com >; importer @ttsRegistrations.com\n* Subject:*Webinar Registration\n\n\n\n[image: RateWatch: Providing Financial Data For Over 20 Years |\n800.348.1831 | www.rate - watch.com] < http://www.rate-watch.com>\n\n\n\n\n\n*This form was submitted on:*   *Thursday, September 7, 2017, 1:45pm CDT*\n\n\n\n\n\n*REGISTRANT INFORMATION*\n\n\n\n\n\n*Name:*\n\nLindsay Weisensel\n\n\n\n\n\n*Email*\n\nlindsay.weisensel@rate-watch.com\n\n\n\n\n\n*Company Name:*\n\nRateWatch\n\n\n\n\n\n*Title*\n\nTesting\n\n\n\n\n\n*Address:*\n\n123 Main Street\nFort Atkinson, WI 53538\n\n\n\n\n\n*Phone Number:*\n\n9205681401 <(920)%20568-1401>\n\n\n\n\n\n*Account Number:*\n\nWI-99-90\n\n\n\n\n\n\n\n*WEBINAR INFORMATION*\n\n\n\n\n\n* Partner:*\n\nCUWebinars.com\n\n\n\n\n\n* Event Name: *\n\n* CU FOCUSED * -Advanced Underwriting for Consumer Loans\n\n\n\n\n\n * Date:*\n\nThursday, September 14, 2017\n\n\n\n\n\n * Time:*\n\n10:00am - 11:30am CDT\n\n\n\n\n\n * Event Type:*\n\n$195.00(Live Session with 7 Day OnDemand Weblink)\n\n\n\n\n\n * Total Price:*\n\n *$195.00 *\n\n\n\n\n\n\n\nRateWatch < http://www.rate-watch.com> | 201 N. Main Street, Suite 4 | Fort\nAtkinson, WI 53538 | Tel: 800.348.1831 <(800)%20348-1831>\n© 2017 RateWatch\n\n");
                if (parsedOrder.LoggerNotes != null && parsedOrder.LoggerNotes.StartsWith("ERR"))
                {

                    _cartControllerOrchestrator.FireMandrillNotificationEvent(
                        "Steve@ttstrain.com",
                        "ERROR! Parser Error from RateWatch: " + parsedOrder.Email,
                        JToken.FromObject(parsedOrder).ToString()
                        + "\n\nincoming:" + msgHtml.msg.html
                    );

                    _logger.Error("Incoming\n" + JToken.FromObject(parsedOrder).ToString());
                }


                MigrateOrderModel migrateOrder = _appHelper.ConvertToMigrator(parsedOrder);
                migrateOrder.idAffiliate = 16132;
                migrateOrder.Origin = "ImporterForRateWatch";
                migrateOrder.OrderDate = DateTime.Now;

                var parseTime = parsedOrder.EventTime.Split('-')[0].Trim(' ') + "-5:00";
                var webinar = _cartControllerOrchestrator.LoadWebinarForImporter(parsedOrder.EventTitle,
                    parsedOrder.EventDate, parseTime);


                if (webinar != null)
                {
                    migrateOrder.idWebinar = webinar.idWebinar;

                    try
                    {
                        migrateOrder.idRegType =
                            _cartControllerOrchestrator.GetRegTypeById(
                                _cartControllerOrchestrator.GetRegTypeByRateWatch(
                                    parsedOrder.RegTypeAsString, webinar.idWebinar)).idRegType;
                    }
                    catch (Exception ex)
                    {
                        _cartControllerOrchestrator.FireMandrillNotificationEvent(
                            "Steve@ttstrain.com",
                            "ERROR! Importer for RateWatch tossed getting RegType: " + parsedOrder.Email,
                            JsonConvert.SerializeObject(migrateOrder, Formatting.Indented)
                            + "\n Model: \n" + msgHtml.msg.text
                        );
                        throw;
                    }
                }
                else
                {
                    _cartControllerOrchestrator.FireMandrillNotificationEvent(
                        "steve@ttstrain.com"
                        , "Webinar Not Found" + parsedOrder.EventTitle
                        , JsonConvert.SerializeObject(parsedOrder));
                }

                if (migrateOrder.idRegType < 1)
                {
                    _cartControllerOrchestrator.FireMandrillNotificationEvent(
                        "Steve@ttstrain.com",
                        "ERROR! Importer for RateWatch could not determine RegType: " + parsedOrder.Email,
                        parsedOrder.LoggerNotes
                        + "\n Model: \n" + JsonConvert.SerializeObject(migrateOrder, Formatting.Indented)
                    );
                }
                var orderTotal = _cartControllerOrchestrator.GetRegTypeById(migrateOrder.idRegType).Price;

                _logger.Info("Incoming_RateWatchFromMandrillParsed: "
                             + JsonConvert.SerializeObject(migrateOrder, Formatting.Indented));

                if (
                    _cartControllerOrchestrator.CheckIfEmailAlreadyRegisteredForWebinar(
                        Convert.ToInt32(parsedOrder.idWebinar), parsedOrder.Email) > 0)
                {
                    _logger.Info("Incoming_rateWatch found dupe: " + parsedOrder.idWebinar + " : " + parsedOrder.Email);
                }
                else
                {
                    var PostForm = "";

                    PostForm = "idAffiliate=" +
                               HttpUtility.UrlEncode(migrateOrder.idAffiliate.ToString()) + "&BillingAddress.AddressType=Billing";
                    PostForm += "&BillingAddress.Name=" +
                                HttpUtility.UrlEncode(migrateOrder.FirstName + " " + HttpUtility.UrlEncode(migrateOrder.LastName));
                    PostForm += "&BillingAddress.Phone=" + HttpUtility.UrlEncode(migrateOrder.BillingAddress.Phone);
                    PostForm += "&BillingAddress.StreetAddress=" + HttpUtility.UrlEncode(migrateOrder.BillingAddress.StreetAddress);
                    PostForm += "&BillingAddress.StreetAddress2=" + HttpUtility.UrlEncode(migrateOrder.BillingAddress.StreetAddress2);
                    PostForm += "&BillingAddress.City=" + HttpUtility.UrlEncode(migrateOrder.BillingAddress.City);
                    PostForm += "&BillingAddress.Zip=" + HttpUtility.UrlEncode(migrateOrder.BillingAddress.Zip);
                    PostForm += "&BillingAddress.State=" + HttpUtility.UrlEncode(migrateOrder.BillingAddress.State);
                    PostForm += "&BillingAddress.Country=" + HttpUtility.UrlEncode(migrateOrder.BillingAddress.Country);
                    PostForm += "&ShippingAddress.AddressType=Shipping";
                    PostForm += "&ShippingAddress.Name=" +
                                HttpUtility.UrlEncode(migrateOrder.FirstName + " " + HttpUtility.UrlEncode(migrateOrder.LastName));
                    PostForm += "&ShippingAddress.Phone=" + HttpUtility.UrlEncode(migrateOrder.ShippingAddress.Phone);
                    PostForm += "&ShippingAddress.StreetAddress=" + HttpUtility.UrlEncode(migrateOrder.ShippingAddress.StreetAddress);
                    PostForm += "&ShippingAddress.StreetAddress2=" + HttpUtility.UrlEncode(migrateOrder.ShippingAddress.StreetAddress2);
                    PostForm += "&ShippingAddress.City=" + HttpUtility.UrlEncode(migrateOrder.ShippingAddress.City);
                    PostForm += "&ShippingAddress.Zip=" + HttpUtility.UrlEncode(migrateOrder.ShippingAddress.Zip);
                    PostForm += "&ShippingAddress.State=" + HttpUtility.UrlEncode(migrateOrder.ShippingAddress.State);
                    PostForm += "&ShippingAddress.Country=" + HttpUtility.UrlEncode(migrateOrder.ShippingAddress.Country);
                    PostForm += "&Email=" + HttpUtility.UrlEncode(migrateOrder.Email);
                    PostForm += "&Title=" + HttpUtility.UrlEncode(migrateOrder.Title);
                    PostForm += "&Institution=" + HttpUtility.UrlEncode(migrateOrder.Institution);
                    PostForm += "&FirstName=" + HttpUtility.UrlEncode(migrateOrder.FirstName);
                    PostForm += "&LastName=" + HttpUtility.UrlEncode(migrateOrder.LastName);
                    PostForm += "&idRegType=" + HttpUtility.UrlEncode(migrateOrder.idRegType.ToString());
                    PostForm += "&idWebinar=" + HttpUtility.UrlEncode(migrateOrder.idWebinar.ToString());
                    PostForm += "&AdditionalLocationsString=" + HttpUtility.UrlEncode(migrateOrder.AdditionalLocationsString);
                    PostForm += "&idOrderLegacy=" + HttpUtility.UrlEncode("0");
                    PostForm += "&OrderDate=" + HttpUtility.UrlEncode(migrateOrder.OrderDate.ToString());
                    PostForm += "&ShippingDate=";
                    PostForm += "&DiscountCode=" + HttpUtility.UrlEncode(migrateOrder.DiscountCode);
                    PostForm += "&Status=" + (int)migrateOrder.Status;
                    PostForm += "&Total=" + orderTotal;

                    _cartControllerOrchestrator.FireMandrillNotificationEvent(
                        "Steve@ttstrain.com",
                        "ERROR! RateWatch migrated: " + parsedOrder.Email,
                        JsonConvert.SerializeObject(migrateOrder, Formatting.Indented)
                        + "\n Model: \n" + parsedOrder.LoggerNotes
                    );

                    WebRequest req = WebRequest.Create("https://www.bankwebinars.com/order/MigrateOrder");
#if DEBUG
                    {
                        req = WebRequest.Create("http://localhost:3538/order/MigrateOrder");
                    }
#endif
                    byte[] send = Encoding.Default.GetBytes(PostForm);
                    req.Method = "POST";
                    req.ContentType = "application/x-www-form-urlencoded";
                    req.ContentLength = send.Length;

                    Stream sout = req.GetRequestStream();
                    sout.Write(send, 0, send.Length);
                    sout.Flush();
                    sout.Close();

                    WebResponse res = req.GetResponse();
                    StreamReader sr = new StreamReader(res.GetResponseStream());
                    string returnvalue1 = sr.ReadToEnd();
                    var importResult = returnvalue1;
                }
            }
            catch (Exception ex)
            {
                _logger.Warn("Incoming_rateWatch: " + incoming + " exception: " + ex);

            }
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult ImportConfSem()
        {
            return View();
        }

        [AllowAnonymous]
        [AcceptVerbs(HttpVerbs.Post), ValidateInput(false)]
        public JsonResult ImportConfSem(string incoming, string mode)
        {
            if (mode == "Commit")
            {
                try
                {
                    var tenant = _globalConfig.Tenant;
                    MigrateOrderModel migrateOrder = JsonConvert.DeserializeObject<MigrateOrderModel>(incoming);
                    migrateOrder.Tenant = tenant;
                    return CommitImport(migrateOrder);
                }
                catch (Exception ex)
                {
                    _logger.FatalException("Incoming_confSemCommit: " + incoming + " exception: ", ex);
                    return Json(new { Result = WebUiConstants.Fail, UpdateCaption = "Error: " + ex.Message });
                }
            }
            else
            {
                try
                {

                    var parsedOrder = _appHelper.ParseConfSem(incoming, DateTime.Now.ToString());
                    parsedOrder.idAffiliate = 22805;
                    parsedOrder.Origin = "ImporterByConfSem";

                    try
                    {
                        if (parsedOrder.LoggerNotes != null && parsedOrder.LoggerNotes.StartsWith("ERR"))
                        {

                            _cartControllerOrchestrator.FireMandrillNotificationEvent(
                                "Steve@ttstrain.com",
                                "ERROR! Parser Error from ConfSem: " + parsedOrder.Email,
                                JToken.FromObject(parsedOrder).ToString()
                                + "\n\nincoming:" + incoming
                            );

                            _logger.Error(JToken.FromObject(parsedOrder).ToString());
                        }
                        else
                        {
                            parsedOrder.LoggerNotes = incoming;
                        }

                        MigrateOrderModel migrateOrder = _appHelper.ConvertToMigrator(parsedOrder);

                        var regTypeString = "Live Plus Five";
                        if (parsedOrder.RegTypeAsString.Contains("CD"))
                            regTypeString = "Premier Package";

                        var webinar = _cartControllerOrchestrator.LoadWebinarForImporter(parsedOrder.EventTitle.Trim(),
                            parsedOrder.EventDate.Trim(), parsedOrder.EventTime.Trim());

                        if (webinar == null)
                        {
                            _logger.Info("Incoming - failed to find Webinar");
                            _cartControllerOrchestrator.FireMandrillNotificationEvent(
                                "Steve@ttstrain.com",
                                "ERROR! Importer for confSem could not determine webinar: " + parsedOrder.Email,
                                JsonConvert.SerializeObject(migrateOrder, Formatting.Indented, new JsonSerializerSettings { MaxDepth = 1, ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
                            return Json(new { Result = WebUiConstants.Fail, UpdateCaption = "Unable to find Webinar " + parsedOrder.Email + " (Please confirm that the date matches our calendar and update the text as needed.) Please copy the intended webinar's title from our calendar and paste over the title used in the email. " });
                        }

                        migrateOrder.idWebinar = webinar.idWebinar;

                        try
                        {
                            migrateOrder.idRegType =
                                _cartControllerOrchestrator.GetRegTypeById(
                                        _cartControllerOrchestrator.LoadRegistrationForImporter(webinar, regTypeString))
                                    .idRegType;
                        }
                        catch (Exception ex)
                        {
                            _cartControllerOrchestrator.FireMandrillNotificationEvent(
                                "Steve@ttstrain.com",
                                "ERROR! Importer for confSem tossed getting RegType: " + parsedOrder.Email,
                                JsonConvert.SerializeObject(migrateOrder, Formatting.Indented)
                                + "\n Model: \n" + incoming
                            );
                            return Json(new { Result = WebUiConstants.Fail, UpdateCaption = "Unable to determine RegType " + parsedOrder.Email });
                        }
                        if (migrateOrder.idRegType < 1)
                        {
                            _cartControllerOrchestrator.FireMandrillNotificationEvent(
                                "Steve@ttstrain.com",
                                "ERROR! Importer for confSem could not determine RegType: " + parsedOrder.Email,
                                incoming
                                + "\n Model: \n" + JsonConvert.SerializeObject(migrateOrder, Formatting.Indented)
                            );
                        }

                        _logger.Info("Incoming_confSemFromMandrillParsed: "
                                     + JsonConvert.SerializeObject(migrateOrder, Formatting.Indented));

                        if (
                            _cartControllerOrchestrator.CheckIfEmailAlreadyRegisteredForWebinar(
                                Convert.ToInt32(migrateOrder.idWebinar), parsedOrder.Email) > 0)
                        {
                            _logger.Info("Incoming_confSem found dupe: " + parsedOrder.idWebinar + " : " +
                                         parsedOrder.Email);
                            return Json(new { Result = WebUiConstants.Success, UpdateCaption = "Order already exists for " + parsedOrder.Email });
                        }
                        else
                        {
                            var commitOrder = CommitImport(migrateOrder);
                            //_SendOrderConfirmation2();
                            return Json(new { Result = WebUiConstants.Success, Order = commitOrder });
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.FatalException("IncomingConfSem", ex);
                        return Json(new { Result = WebUiConstants.Fail, UpdateCaption = "Importer tossed " + ex.Message });
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { Result = WebUiConstants.Fail, UpdateCaption = "Importer tossed " + ex.Message });

                    _logger.Warn("Incoming_confSem: " + incoming + " exception: " + ex);

                }
            }
        }

        protected JsonResult CommitImport(MigrateOrderModel migrateOrder)
        {
            string incoming;
            double orderTotal;
            ParseOrderModel parsedOrder;
            var PostForm = "";

            PostForm = "idAffiliate=" +
                       HttpUtility.UrlEncode(migrateOrder.idAffiliate.ToString()) + "&BillingAddress.AddressType=Billing";
            PostForm += "&BillingAddress.Name=" +
                        HttpUtility.UrlEncode(migrateOrder.FirstName + " " + HttpUtility.UrlEncode(migrateOrder.LastName));
            PostForm += "&BillingAddress.Phone=" + HttpUtility.UrlEncode(migrateOrder.BillingAddress.Phone);
            PostForm += "&BillingAddress.StreetAddress=" + HttpUtility.UrlEncode(migrateOrder.BillingAddress.StreetAddress);
            PostForm += "&BillingAddress.StreetAddress2=" + HttpUtility.UrlEncode(migrateOrder.BillingAddress.StreetAddress2);
            PostForm += "&BillingAddress.City=" + HttpUtility.UrlEncode(migrateOrder.BillingAddress.City);
            PostForm += "&BillingAddress.Zip=" + HttpUtility.UrlEncode(migrateOrder.BillingAddress.Zip);
            PostForm += "&BillingAddress.State=" + HttpUtility.UrlEncode(migrateOrder.BillingAddress.State);
            PostForm += "&BillingAddress.Country=" + HttpUtility.UrlEncode(migrateOrder.BillingAddress.Country);
            PostForm += "&ShippingAddress.AddressType=Shipping";
            PostForm += "&ShippingAddress.Name=" +
                        HttpUtility.UrlEncode(migrateOrder.FirstName + " " + HttpUtility.UrlEncode(migrateOrder.LastName));
            PostForm += "&ShippingAddress.Phone=" + HttpUtility.UrlEncode(migrateOrder.ShippingAddress.Phone);
            PostForm += "&ShippingAddress.StreetAddress=" + HttpUtility.UrlEncode(migrateOrder.ShippingAddress.StreetAddress);
            PostForm += "&ShippingAddress.StreetAddress2=" + HttpUtility.UrlEncode(migrateOrder.ShippingAddress.StreetAddress2);
            PostForm += "&ShippingAddress.City=" + HttpUtility.UrlEncode(migrateOrder.ShippingAddress.City);
            PostForm += "&ShippingAddress.Zip=" + HttpUtility.UrlEncode(migrateOrder.ShippingAddress.Zip);
            PostForm += "&ShippingAddress.State=" + HttpUtility.UrlEncode(migrateOrder.ShippingAddress.State);
            PostForm += "&ShippingAddress.Country=" + HttpUtility.UrlEncode(migrateOrder.ShippingAddress.Country);
            PostForm += "&Email=" + HttpUtility.UrlEncode(migrateOrder.Email);
            PostForm += "&Title=" + HttpUtility.UrlEncode(migrateOrder.Title);
            PostForm += "&Institution=" + HttpUtility.UrlEncode(migrateOrder.Institution);
            PostForm += "&FirstName=" + HttpUtility.UrlEncode(migrateOrder.FirstName);
            PostForm += "&LastName=" + HttpUtility.UrlEncode(migrateOrder.LastName);
            PostForm += "&idRegType=" + HttpUtility.UrlEncode(migrateOrder.idRegType.ToString());
            PostForm += "&idWebinar=" + HttpUtility.UrlEncode(migrateOrder.idWebinar.ToString());
            PostForm += "&AdditionalLocationsString=" + HttpUtility.UrlEncode(migrateOrder.AdditionalLocationsString);
            PostForm += "&idOrderLegacy=" + HttpUtility.UrlEncode("0");
            PostForm += "&OrderDate=" + HttpUtility.UrlEncode(migrateOrder.OrderDate.ToString());
            PostForm += "&ShippingDate=";
            PostForm += "&DiscountCode=" + HttpUtility.UrlEncode(migrateOrder.DiscountCode);
            PostForm += "&Status=" + (int)migrateOrder.Status;
            PostForm += "&Total=0&Tenant=" + HttpUtility.UrlEncode(migrateOrder.Tenant);
            try
            {

                WebRequest req = WebRequest.Create(_globalConfig.TenantURL + "/order/MigrateOrder");

#if DEBUG
                {
                    _logger.Info("Incoming Running in debug mode");
                    req = WebRequest.Create("http://localhost:3538/order/MigrateOrder");
                }
#endif

                byte[] send = Encoding.Default.GetBytes(PostForm);
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                req.ContentLength = send.Length;

                Stream sout = req.GetRequestStream();
                sout.Write(send, 0, send.Length);
                sout.Flush();
                sout.Close();

                WebResponse res = req.GetResponse();
                StreamReader sr = new StreamReader(res.GetResponseStream());
                string returnvalue1 = sr.ReadToEnd();

                _cartControllerOrchestrator.FireMandrillNotificationEvent(
                    "Steve@ttstrain.com",
                    "ERROR! ConfSem Committed: " + migrateOrder.Email,
                    JsonConvert.SerializeObject(migrateOrder, Formatting.Indented)
                );

                _SendOrderConfirmation2(Convert.ToInt32(returnvalue1.Replace("{\"Result\":\"", "").Replace("\"}", "")));
                return Json(new { Result = WebUiConstants.Success, UpdateCaption = "Imported: " + returnvalue1 });
            }
            catch (Exception e)
            {
                _cartControllerOrchestrator.FireMandrillNotificationEvent(
                    "Steve@ttstrain.com",
                    "ERROR! ConfSem Blew up!: " + migrateOrder.Email,
                    JsonConvert.SerializeObject(migrateOrder, Formatting.Indented) + "\n" + e.Message
                );
                return Json(new { Result = WebUiConstants.Fail, UpdateCaption = "Error: " + e });
            }
        }

        [AllowAnonymous]
        [AcceptVerbs(HttpVerbs.Post), ValidateInput(false)]
        public void Incoming()
        {
            var incoming = HttpContext.Request.Form[0].TrimStart('[').TrimEnd(']');

            try
            {
                StringBuilder sb = new StringBuilder();
                _logger.Info("Incoming Starts");

                var msgHtml = JsonConvert.DeserializeObject<MandrillIncomingMsg.mandrill_events>(incoming.ToString());

                //if ()
                _logger.Info("Incoming " + msgHtml.msg.from_email);
                if (msgHtml.msg.from_email.ToLower().Contains("conferencesandseminars.org") || msgHtml.msg.from_email.ToLower().Contains("steve"))
                {
                    //Incoming_confSem(incoming);
                    return;
                }
                if (msgHtml.msg.from_email.Contains("rate-watch") || msgHtml.msg.from_email.Contains("steve"))
                {
                    Incoming_ratewatch(incoming);
                    return;
                }
                ImportOrderForAcsModel parsedOrder = _appHelper.ParseAcs("<html><body>" + msgHtml.msg.html + "</body></html>",
                    DateTime.Now.ToString());
                _logger.Info("IncomingFromMandrillParsed: " + JsonConvert.SerializeObject(parsedOrder, Formatting.Indented));

                if (
                    _cartControllerOrchestrator.CheckIfEmailAlreadyRegisteredForWebinar(
                        Convert.ToInt32(parsedOrder.BankWebID), parsedOrder.Email) > 0)
                {
                    _logger.Info("Incoming found dupe: " + parsedOrder.BankWebID + " : " + parsedOrder.Email);
                }
                else
                {
                    //var orderController = new OrderController();
                    //var orderControllerContext = new ControllerContext(this.ControllerContext.RequestContext, OrderController);

                    //RedirectToAction("Importorder4Acs", "Order", new {ImportOrderForAcsModel = parsedOrder});
                    var PostForm = "AdditionalLocationsString=" +
                                   HttpUtility.UrlEncode(parsedOrder.AdditionalLocationsString ?? "");


                    PostForm += "&AffiliateComments=" + HttpUtility.UrlEncode("ACSImporter");
                    PostForm += "&AffiliateID=" + HttpUtility.UrlEncode("62");
                    PostForm += "&BankWebID=" + HttpUtility.UrlEncode(parsedOrder.BankWebID);
                    PostForm += "&BillingContact=" +
                                HttpUtility.UrlEncode(parsedOrder.BillingContact ??
                                                      "MissingBillingContact@ttstrain.com");
                    PostForm += "&City=" + HttpUtility.UrlEncode(parsedOrder.City ?? "-ct");
                    PostForm += "&Company=" + HttpUtility.UrlEncode(parsedOrder.Company ?? "_co");
                    PostForm += "&CompanyBillingInformation=" +
                                HttpUtility.UrlEncode(parsedOrder.CompanyBillingInformation ?? "cbi");
                    PostForm += "&Country=" + HttpUtility.UrlEncode(parsedOrder.Country ?? "");
                    PostForm += "&CourseDeliveryType=" + HttpUtility.UrlEncode(parsedOrder.CourseDeliveryType ?? "");
                    PostForm += "&CourseNumber=" + HttpUtility.UrlEncode(parsedOrder.CourseNumber ?? "");
                    PostForm += "&CoursePrice=" + HttpUtility.UrlEncode(parsedOrder.CoursePrice ?? "");
                    PostForm += "&CreditCard=" + HttpUtility.UrlEncode(parsedOrder.CreditCard ?? "");
                    PostForm += "&DateSubmittedToACS=" + HttpUtility.UrlEncode(parsedOrder.DateSubmittedToACS);
                    PostForm += "&DeliveryType=" + HttpUtility.UrlEncode(parsedOrder.DeliveryType);
                    PostForm += "&Email=" + HttpUtility.UrlEncode(parsedOrder.Email);
                    PostForm += "&EmailAddress=" +
                                HttpUtility.UrlEncode(parsedOrder.EmailAddress ?? "_emailAddress@assigned.com");
                    PostForm += "&EmailAddressforCreditCardReceipt=" +
                                HttpUtility.UrlEncode(parsedOrder.EmailAddressforCreditCardReceipt ??
                                                      "_emailAddress@forCreditCardReceipt");
                    PostForm += "&Ext=" + HttpUtility.UrlEncode(parsedOrder.Ext ?? "");
                    PostForm += "&FirstName=" + HttpUtility.UrlEncode(parsedOrder.FirstName);
                    PostForm += "&LastName=" + HttpUtility.UrlEncode(parsedOrder.LastName);
                    PostForm += "&PaymentMethod=" + HttpUtility.UrlEncode(parsedOrder.PaymentMethod ?? "");
                    PostForm += "&Phone=" + HttpUtility.UrlEncode(parsedOrder.Phone);
                    PostForm += "&State=" + HttpUtility.UrlEncode(parsedOrder.State ?? "");
                    PostForm += "&State_Province_Region=" + HttpUtility.UrlEncode(parsedOrder.State_Province_Region);
                    PostForm += "&StreetorP_O_Box=" + HttpUtility.UrlEncode(parsedOrder.StreetorP_O_Box);
                    PostForm += "&Title=" + HttpUtility.UrlEncode(parsedOrder.Title);
                    PostForm += "&WebinarDate=" + HttpUtility.UrlEncode(parsedOrder.WebinarDate);
                    PostForm += "&WebinarTitle=" + HttpUtility.UrlEncode(parsedOrder.WebinarTitle);
                    PostForm += "&ZeroValue=" + HttpUtility.UrlEncode(parsedOrder.ZeroValue);
                    PostForm += "&Zip_PostalCode=" + HttpUtility.UrlEncode(parsedOrder.Zip_PostalCode);

                    //WebRequest req = WebRequest.Create("http://localhost:3538/order/importorder4ACS");
                    WebRequest req = WebRequest.Create("https://www.bankwebinars.com/order/importorder4ACS");

                    byte[] send = Encoding.Default.GetBytes(PostForm);
                    req.Method = "POST";
                    req.ContentType = "application/x-www-form-urlencoded";
                    req.ContentLength = send.Length;

                    Stream sout = req.GetRequestStream();
                    sout.Write(send, 0, send.Length);
                    sout.Flush();
                    sout.Close();

                    WebResponse res = req.GetResponse();
                    StreamReader sr = new StreamReader(res.GetResponseStream());
                    string returnvalue1 = sr.ReadToEnd();
                    var importResult = returnvalue1;
                }
            }
            catch (Exception ex)
            {
                _logger.Warn("Incoming: " + incoming + " exception: " + ex);

            }
            //return null;
        }



        [HttpPost]
        public ActionResult Signup2(CheckoutOptionsViewModel formModel)
        {

            if (ModelState.IsValid)
            {
                //telemetry.Initialize();
                _logger.Info("Signup2 Enters: " + _appHelper.GetUserAuditInfo());

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

            _logger.Info("SignupAffiliate: " +
                         JsonConvert.SerializeObject(formModel, Formatting.None,
                             new JsonSerializerSettings
                             {
                                 MaxDepth = 1,
                                 ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                             }));
            if (ModelState.IsValid)
            {
                var orderAlreadyExists = _cartControllerOrchestrator.GetOrderByUserIdAndWebinar(
                    formModel.idUser, formModel.idWebinar);

                if (orderAlreadyExists != null && orderAlreadyExists.Count > 0)
                {
                    _logger.Warn("SignupAffiliate pulls existing order: " + formModel.idUser + " " + formModel.idWebinar);
                    foreach (var order in orderAlreadyExists)
                    {
                        if (order.OrderStatus == OrderStatus.Billed ||
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
                        if (order.OrderStatus == OrderStatus.Submitted ||
                            order.OrderStatus == OrderStatus.InProcess ||
                            order.OrderStatus == OrderStatus.OutstandingBalance ||
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
                    "There has been an error at the server. Please refresh your page and try again. In the event of repeated problems, please use our Help & Feedback button (lower right corner) for immediate assistance.");
                _logger.FatalException("SignupAffiliate: ", exception);

                ErrorSignal.FromCurrentContext().Raise(exception);
                return Json(new
                {
                    success = "fail",
                    orderId = 0,
                    orderRowId = 0,
                    webinarId = formModel.idWebinar
                }, JsonRequestBehavior.AllowGet);
            }
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
                    return Json(new { shouldShow = showPlusShip.Item1, shippingDetailsRqrd = showPlusShip.Item2 },
                        JsonRequestBehavior.AllowGet);
                }
                catch (Exception exception)
                {
                    _logger.ErrorException(
                        string.Format("CheckIfAddLocShouldHide in cart. | Session{0}", _appHelper.GetUserAuditInfo()),
                        exception);
                    ModelState.AddModelError(string.Empty,
                        "There has been an error. For customer service contact us by using the Online Chat button below or emailing " +
                        _globalConfig.TenantEmail + ".");
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
                    _logger.ErrorException(
                        "RemoveAdditionalLocationsFromOrder|Session=" + _appHelper.GetUserAuditInfo(), exception);
                    ErrorSignal.FromCurrentContext().Raise(exception);
                }

                return Json(new { Result = WebUiConstants.Success });
            }
            return Json(new { });
        }

        [HttpGet]
        public void FixSeriesOrdersWithIncorrectRoyalty(int? idOrder = null)
        {
            int orderIDTracker = 0;
            if (idOrder.HasValue)
            {
                try
                {
                    var order = _cartControllerOrchestrator.GetOrderById(idOrder);

                    var pricesAndDiscounts = _cartControllerOrchestrator.UpdateOrderPricing(order);
                    _logger.Info("FixSeriesOrdersWithIncorrectRoyalty suceeded: " + idOrder);

                }
                catch (Exception exception)
                {
                    _logger.ErrorException(
                        String.Format("FixSeriesOrdersWithIncorrectRoyalty failed on {0} with {1} Session={2}", idOrder,
                            exception.Message, _appHelper.GetUserAuditInfo()), exception);
                    //ErrorSignal.FromCurrentContext().Raise(exception);

                    //return Json(new { Result = WebUiConstants.Fail });

                }
            }
        }

        [HttpPost]
        public ActionResult UpdateOrderDetails(int? idOrderRow = null, int? idRegType = null, string note = null)
        {
            int orderIDTracker = 0;
            if (idOrderRow.HasValue && idRegType.HasValue)
            {
                try
                {
                    var regType = _cartControllerOrchestrator.GetRegTypeById(idRegType.Value);

                    var newRegType = regType.OptionLabel;
                    var oldRegType =
                        _cartControllerOrchestrator.GetOrderRowLoaded(idOrderRow.Value).RegistrationType.OptionLabel;

                    var model = _cartControllerOrchestrator.BuildCheckOutViewModel(idOrderRow);
                    var orgTotal = model.Order.Total;

                    model.Order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).RegistrationType = regType;
                    orderIDTracker = model.Order.idOrder;
                    var pricesAndDiscounts = _cartControllerOrchestrator.UpdateOrderPricing(model.Order);
                    var orderStatusCaption = "";
                    if (model.Order.Total != model.Order.TotalPaid)
                    {
                        model.Order.OrderStatus = OrderStatus.OutstandingBalance;

                    }
                    else
                    {
                        model.Order.OrderStatus = OrderStatus.Paid;

                        model.Order.AdminComments = JsonHelpers.RemoveJObject(model.Order.AdminComments, "OutstandingBalance");
                    }
                    orderStatusCaption =
                        "<span id=\"orderStatusLabel\" class=\"label-important label\">" + model.Order.OrderStatus + "</span>";


                    //just produces a caption - no impact on price
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

                    _logger.Info("UpdateOrderDetails: " + model.Order.idOrder +
                        " changed from: " + oldRegType + " to: " + newRegType +
                        " note: " + note + " by: " + _appHelper.GetUserAuditInfo());

                    var UpdateSuccessCaption = "Order updated to: " + newRegType;
                    var ShippedDate =
                        model.Order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).ShipmentDate;
                    string shippDateString = "";
                    if (ShippedDate != null)
                    {
                        shippDateString = ShippedDate.Value.Month
                                          + "/" + ShippedDate.Value.Day;
                    }


                    return
                        Json(
                            new
                            {
                                DiscountCaption = discountCaption,
                                UpdateSuccessCaption = UpdateSuccessCaption,
                                OrderStatusCaption = orderStatusCaption,
                                regTypeShort = regType.OptionLabelShort,
                                BasePrice = pricesAndDiscounts.UnitPrice,
                                Discount = pricesAndDiscounts.TotalDiscount,
                                OptionsPrice = pricesAndDiscounts.TotalCostOfOptions,
                                Tax = pricesAndDiscounts.TaxAmount,
                                Total = (pricesAndDiscounts.UnitPrice + pricesAndDiscounts.TotalCostOfOptions + pricesAndDiscounts.TaxAmount - pricesAndDiscounts.TotalDiscount),
                                TotalPaid = model.Order.TotalPaid,
                                FlatOff = pricesAndDiscounts.Discount.FlatOff,
                                OutstandingBalance = (pricesAndDiscounts.UnitPrice + pricesAndDiscounts.TotalCostOfOptions + pricesAndDiscounts.TaxAmount - pricesAndDiscounts.TotalDiscount) - model.Order.TotalPaid,
                                OrderStatus = model.Order.OrderStatus,
                                ShippedDateString = shippDateString,
                                PercentOff = pricesAndDiscounts.Discount.PercentOff
                            });
                }
                catch (Exception exception)
                {
                    _logger.ErrorException(
                        String.Format("UpdateOrderDetails failed on {0} with {1} Session={2}", orderIDTracker,
                            exception.Message, _appHelper.GetUserAuditInfo()), exception);
                    ErrorSignal.FromCurrentContext().Raise(exception);

                    return Json(new { Result = WebUiConstants.Fail });

                }


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
                if (status == OrderStatus.Paid)
                {
                    orderRow.Order.TotalPaid = orderRow.Order.Total;
                    _cartControllerOrchestrator.SaveOrder(orderRow.Order);
                }
                return Json(orderRow.Order.OrderStatus.ToString());

            }
            return this.ModelStateJson(ModelState);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public ActionResult UpdateOrderWithUserId(int? orderId, int? userId)
        {

            // Set up some properties:
            var properties = new Dictionary<string, string>
            {
                {"tenant", _globalConfig.Tenant},
                {"affiliate", _stateService.GetValue<Affiliate>(WebUiConstants.CurrentAffiliate).ttsDomain}
            };

            // Send the event:
            telemetry.TrackEvent("UpdateOrderWithUserId", properties);

            if (orderId.HasValue && userId.HasValue)
            {
                try
                {
                    var oOrder = _cartControllerOrchestrator.GetOrderById(orderId);

                    _cartControllerOrchestrator.UpdateOrderWithUserId(orderId.Value, userId.Value);
                    var uOrder = _cartControllerOrchestrator.GetOrderById(orderId);

                    if (oOrder.idAffiliate != uOrder.idAffiliate)
                    {
                        // DetermineAffByAltMeans overwrites aff.
                        _stateService.SetValue(WebUiConstants.CurrentAffiliate, uOrder.Affiliate);
                        _logger.Info("UpdateOrderWithUserId updated the original Affiliate. idOrder: {1}, Affiliate: {2},  Session: {0}",
                            _appHelper.GetUserAuditInfo(),
                            uOrder.idOrder, uOrder.Affiliate.ttsDomain
                        );
                    }
                    return Json(new
                    {
                        Result = WebUiConstants.Success
                    });
                }
                catch (Exception e)
                {
                    telemetry.TrackException(e, properties);
                    return Json(new
                    {
                        Result = WebUiConstants.Fail
                    });
                }
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
        public JsonResult SendHardcopy(int idOrder)
        {
            var order = _cartControllerOrchestrator.GetOrderById(idOrder);
            var updateCaption = "Problem updating the order.";

            try
            {
                var sendHardcopy = order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active).SendHardcopy;
                if (sendHardcopy.HasValue)
                    if (sendHardcopy.Value == true)
                    {
                        order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active).SendHardcopy = false;
                        updateCaption = "Order is updated to not send hardcopies.";
                    }
                    else
                    {
                        order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active).SendHardcopy = true;
                        updateCaption = "Order is updated to send hardcopies.";
                    }
                _logger.Info("SendHardcopy on idOrder: " + idOrder + " is: " + order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active).SendHardcopy.Value);
                _cartControllerOrchestrator.SaveOrder(order);
            }
            catch (Exception)
            {
                return Json(new { Result = WebUiConstants.Fail, UpdateCaption = updateCaption });
            }

            return Json(new { Result = WebUiConstants.Success, UpdateCaption = updateCaption });
        }
        public ActionResult Resume(int id)
        {
            if (Request.IsAuthenticated)
            {
                try
                {
                    var order = _cartControllerOrchestrator.GetOrderById(id);
                    ClaimsIdentity claimsIdentityOfAuthenticatedUser = (ClaimsIdentity)User.Identity;

                    if (!(claimsIdentityOfAuthenticatedUser.HasClaim(
                        (claim) => claim.Type == Business.Constants.ClaimTypes.Affiliate) || claimsIdentityOfAuthenticatedUser.HasClaim(
                        (claim) => claim.Type == Business.Constants.ClaimTypes.Admin)))
                    {
                        if (order.BillingEmail.ToLower() != User.Identity.Name.ToLower())
                        {
                            _logger.Warn("Resume idOrder: " + id + " was not by order's email: " + User.Identity.Name);

                            return RedirectToAction("Index", "Home", new
                            {
                                msg = "Order's email does not match login email.",
                                idOrder = id,
                                source = "Resume"
                            });
                        }
                    }

                    order.Origin = DomainConstants.OriginResume;

                    _logger.Info("Resume idOrder: " + id + " by: " + User.Identity.Name);
                    _cartControllerOrchestrator.SaveOrder(order);

                    return RedirectToAction("Details", "Webinar", new
                    {
                        id = order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active)
                            .Webinar.idWebinar,
                        idOrder = id,
                        source = "Resume"
                    });

                }
                catch (Exception ex)
                {
                    _logger.FatalException("Resume " + id, ex);

                    throw;
                }

            }
            return RedirectToAction("Login", "Account", new { ReturnURL = "/Resume/" + id });
        }

        public ActionResult Express(int id)
        {
            try
            {
                var order = _cartControllerOrchestrator.GetOrderById(id);
                order.Origin = DomainConstants.OriginExpress;

                _logger.Info("Express idOrder: " + id);
                _stateService.SetValue(DomainConstants.OriginExpress, Request.QueryString["idOrder"]);

                return RedirectToAction("Details", "Webinar", new
                {
                    id = order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active)
                        .Webinar.idWebinar,
                    idOrder = id,
                    source = "Express"
                });

            }
            catch (Exception ex)
            {
                _logger.FatalException("ExpressEx: " + id, ex);
                throw;
            }

        }


        [HttpPost]
        public ActionResult ExpressPostback2(ExpressCheckoutModel form)
        {
            try
            {
                if (form.q11_orderid > 0)
                {

                    var order = _cartControllerOrchestrator.GetOrderById(form.q11_orderid);
                    _logger.Info("ExpressPostback from: " + " - " + form.q11_orderid + _appHelper.GetUserAuditInfo());

                    if (order != null)
                    {
                        order.Origin = DomainConstants.OriginExpress;
                        order.OrderStatus = OrderStatus.Submitted;

                        order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active).RegistrationType =
                            _cartControllerOrchestrator.GetRegTypeByLabel(form.q10_registrationType,
                                form.q18_q_webinarid18);
                        _logger.Info("ExpressPostback regtype is: " +
                                     order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active)
                                         .RegistrationType.OptionLabel);
                        _cartControllerOrchestrator.UpdateOrderPricing(order);

                        return View("~/Views/Cart/ThankYou.cshtml", order);
                    }


                    return RedirectToAction("Details", "Webinar",
                        new { id = form.q18_q_webinarid18, idOrder = form.q11_orderid, source = "ExpressPostback2" });

                    //                  return RedirectToAction("OrderComplete", "Account", new { id = order.idOrder });
                }
            }
            catch (Exception ex)
            {
                _logger.FatalException("ExpressCheckoutInternalPostback tossed: " + ex.Message, ex);
            }
            return null;

        }

        public ActionResult CancelPauseOrder(int idOrder, string email)
        {


            var model = new CheckoutPauseViewModel { email = email, idOrder = idOrder };
            _logger.Info("CancelPauseOrder called email: " + email + " idOrder: " + idOrder);

            return View("Partials/_Cancel_PauseOrder", model);
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
                        var newJson = new JProperty(string.Concat(JsonPropertyKeys.OrderCreatedByExpressCheckoutKey),
                            JsonConvert.SerializeObject(_appHelper.GetSessionStartInfo()));

                        JProperty OrderCreatedByExpressCheckout =
                            new JProperty(JsonPropertyKeys.OrderCreatedByExpressCheckoutKey, newJson.Value);

                        order.AdminComments = JsonHelpers.MergeJsonWithStoredField(order.AdminComments,
                            OrderCreatedByExpressCheckout);


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
                        _cartControllerOrchestrator.GetRegTypeByLabel("OnDemand Recording Only", idWebinar: idWebinar)
                            .idRegType;

                    int selectedUser = 0;
                    if (User.Identity.IsAuthenticated)
                    {
                        var identity = ClaimsPrincipal.Current;

                        if (identity != null)
                        {
                            if (identity.Identity.IsAuthenticated)
                            {
                                var userAccount =
                                    _membershipService.GetUserAccountByUserId(ClaimsExtensions.GetUserID(identity));

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
            var _idAffiliate = 19;

            try
            {
                int id;
                if (Request.UrlReferrer.ToString().Contains("&"))
                {
                    id = Convert.ToInt32(Request.UrlReferrer.ToString().Split('&')[1].Split('=')[1]);
                }
                else
                {
                    id = Convert.ToInt32(Request.UrlReferrer.ToString().Split('=')[1]);
                }
                if (!ReferenceEquals(id, null))
                {
                    _idAffiliate = id;
                }
            }
            catch (Exception ex)
            {

                _logger.FatalException("ComplianceSchoolCheckoutreferrer " + Request.UrlReferrer.ToString(), ex);
            }

            var model = new ExpressCheckoutModel
            {
                q15_affiliateid15 = _idAffiliate
            };

            return View(model);
        }


        public ActionResult PayTraceApproved()
        {
            string formFields = Request.Form.ToString().Replace("parmList=", "");
            formFields = HttpUtility.UrlDecode(formFields);

            var ptRedirectSession = _stateService.GetValue<string>(WebUiConstants.PayTraceSubmit);

            var sb = new StringBuilder();
            sb.AppendLine("PayTrace: Approved: " + formFields);
            var nameValuePair = ptRedirectSession.Split('|');
            var payTraceModel = new PayTraceResponse();
            foreach (var name in nameValuePair)
            {
                sb.AppendLine("name :" + name.Split('~')[0] + "  value: " + name.Split('~')[1]);
                if (name.ToUpper().StartsWith("ORDERID"))
                    payTraceModel.Orderid = name.Split('~')[1];

                if (name.ToUpper().StartsWith("TRANSACTIONID"))
                    payTraceModel.Transactionid = name.Split('~')[1];

                if (name.ToUpper().StartsWith("APPCODE"))
                    payTraceModel.Appcode = name.Split('~')[1];

                if (name.ToUpper().StartsWith("APPMSG"))
                    payTraceModel.Appmsg = name.Split('~')[1];

                if (name.ToUpper().StartsWith("AMOUNT"))
                    payTraceModel.Amount = name.Split('~')[1];

                if (name.ToUpper().StartsWith("BNAME"))
                    payTraceModel.Bname = name.Split('~')[1];

            }
            var order = _cartControllerOrchestrator.LoadOrder(Convert.ToInt32(payTraceModel.Orderid));

            payTraceModel.Order = order;

            if (order.OrderStatus != OrderStatus.Paid)
            {
                payTraceModel.Appcode =
                    "PayTrace reports that your transaction succeeded however, we have not yet received a confirmation" +
                    "code but will update your order status as soon as that arrives.";
            }

            return View(payTraceModel);
        }

        public ActionResult PayTraceDeclined()
        {

            string formFields = Request.Form.ToString().Replace("parmList=", "");
            formFields = HttpUtility.UrlDecode(formFields);

            var ptRedirectSession = _stateService.GetValue<string>(WebUiConstants.PayTraceSubmit);

            var sb = new StringBuilder();
            sb.AppendLine("PayTrace: Declined: " + formFields);
            var nameValuePair = ptRedirectSession.Split('|');
            var payTraceModel = new PayTraceResponse();
            foreach (var name in nameValuePair)
            {
                sb.AppendLine("name :" + name.Split('~')[0] + "  value: " + name.Split('~')[1]);
                if (name.ToUpper().StartsWith("ORDERID"))
                    payTraceModel.Orderid = name.Split('~')[1];

                if (name.ToUpper().StartsWith("TRANSACTIONID"))
                    payTraceModel.Transactionid = name.Split('~')[1];

                if (name.ToUpper().StartsWith("APPCODE"))
                    payTraceModel.Appcode = name.Split('~')[1];

                if (name.ToUpper().StartsWith("APPMSG"))
                    payTraceModel.Appmsg = name.Split('~')[1];

                if (name.ToUpper().StartsWith("AMOUNT"))
                    payTraceModel.Amount = name.Split('~')[1];

                if (name.ToUpper().StartsWith("BNAME"))
                    payTraceModel.Bname = name.Split('~')[1];

            }
            var order = _cartControllerOrchestrator.LoadOrder(Convert.ToInt32(payTraceModel.Orderid));

            payTraceModel.Order = order;

            if (order.OrderStatus == OrderStatus.Paid)
            {
                _logger.Fatal("PayTrace returned user to 'Declined' but order is marked paid on order: " + order.idOrder);
                payTraceModel.Appcode = "PayTrace reports that your transaction was declined.";
            }
            _logger.Warn("PayTrace returns user to 'Declined' on order: " + order.idOrder);
            return View(payTraceModel);
        }

        public ActionResult PayTraceCanceled()
        {
            string formFields = Request.QueryString.ToString();
            _logger.Info("PayTraceCanceled postback: " + formFields);

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



        //[HttpPost]
        public string PayTracePostBack()
        {
            string formFields = Request.Form.ToString().Replace("parmList=", "");
            formFields = HttpUtility.UrlDecode(formFields);
            _logger.Info("PayTracePostBack: " + formFields);
            try
            {
                var nameValuePair = formFields.Split('|');
                var payTraceModel = new PayTraceResponse();
                foreach (var name in nameValuePair)
                {

                    if (name.ToUpper().StartsWith("ORDERID"))
                        payTraceModel.Orderid = name.Split('~')[1];


                    if (name.ToUpper().StartsWith("CARDTYPE"))
                        payTraceModel.CartType = name.Split('~')[1];


                    if (name.ToUpper().StartsWith("TRANSACTIONID"))
                        payTraceModel.Transactionid = name.Split('~')[1];


                    if (name.ToUpper().StartsWith("APPCODE"))
                        payTraceModel.Appcode = name.Split('~')[1];


                    if (name.ToUpper().StartsWith("APPMSG"))
                        payTraceModel.Appmsg = name.Split('~')[1];


                    if (name.ToUpper().StartsWith("AMOUNT"))
                        payTraceModel.Amount = name.Split('~')[1];


                    if (name.ToUpper().StartsWith("BNAME"))
                        payTraceModel.Bname = name.Split('~')[1];

                }
                var order = _cartControllerOrchestrator.LoadOrder(Convert.ToInt32(payTraceModel.Orderid));

                if (order == null)
                {
                    _logger.Fatal("orderAtPaytracePostback");
                    throw new ArgumentNullException("orderAtPaytracePostback");
                }

                if (payTraceModel.Appmsg.StartsWith("Your TEST transaction was successfully processed.") ||
                    payTraceModel.Appmsg.Contains("Approv")
                    || payTraceModel.CartType.ToLower() == "check")
                {
                    order.AdminComments = order.AdminComments.Replace("\"PendingPaytraceResponse\"", JsonConvert.SerializeObject(payTraceModel));

                    bool multi = order.AdminComments != null
                        && order.AdminComments.Contains("Multi_OrderCheckout");

                    var row = order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active);

                    if (row == null)
                    {
                        _logger.Fatal("rowAtPaytracePostback is null");
                        throw new ArgumentNullException("rowAtPaytracePostback");
                    }

                    var createdSeriesOrders = "";

                    if (!multi)
                    {
                        try
                        {

                            order.AdminComments = order.AdminComments.Replace("Single_OrderCheckout", "PaidByCC");

                            order.OrderStatus = OrderStatus.Paid;
                            order.TotalPaid = order.Total;

                            _cartControllerOrchestrator.AddClaimForPostEventMaterials(order.BillingEmail,
                                order.OrderRows.FirstOrDefault());

                            _cartControllerOrchestrator.UpdateOrderPricing(order);
                            _logger.Info("PayTrace: " + order.idOrder + " is Approved");

                            //M4Gen

                            if (row.Webinar.SeriesInfo.Contains("Children"))
                                _cartControllerOrchestrator.CreateSeriesOrders(row);

                            if (row.Webinar.idWebinar == 2520)
                            {
                                //is a WSP order
                                _cartControllerOrchestrator.CreateWspCode(order);
                            }

                            if (row.Webinar.Title.Contains("Compliance Perspectives")
                                //test if CP code needs to be created by ensuring it's not a trial
                                && row.RowPrice > 200)
                                _cartControllerOrchestrator.CreateCompliancePerspectivesSubscription(row);

                            _SendOrderConfirmation2(order.idOrder);

                        }
                        catch (Exception e)
                        {
                            _cartControllerOrchestrator.FireMandrillNotificationEvent(
                                "2afda898.ttstrain.com@amer.teams.ms" // Errors channel of teams
                                , "PayTracePostback Error on: " + order.idOrder, "PaytracePostback hit error: " + e.Message);
                        }
                    }
                    else
                    {
                        var orderExcp = order.idOrder;
                        JObject o = JObject.Parse(order.AdminComments);

                        var nullChecked = o.Properties().FirstOrDefault(p => p.Name.StartsWith("Multi_OrderCheckout"));
                        var list = o.SelectToken("Multi_OrderCheckout[0].OrderList").ToList();
                        _logger.Info("PayTrace: " + order.idOrder + "postback hits multi" + order.idOrder);
                        foreach (var odr in list)
                        {
                            try
                            {
                                var orderMulti = _cartControllerOrchestrator.LoadOrder(Convert.ToInt32(odr.ToString()));
                                orderExcp = orderMulti.idOrder;
                                var rowMulti = orderMulti.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active);

                                _logger.Info("PaytracePostback: " + order.idOrder + "  Postback Multi-OrderCheckout loops: " + order.idOrder);

                                orderMulti.OrderStatus = OrderStatus.Paid;
                                orderMulti.TotalPaid = orderMulti.Total;
                                orderMulti.AdminComments = orderMulti.AdminComments.Replace("Multi_OrderCheckout", "PaidByCC");

                                _cartControllerOrchestrator.AddClaimForPostEventMaterials(order.BillingEmail,
                                    orderMulti.OrderRows.FirstOrDefault());

                                _cartControllerOrchestrator.UpdateOrderPricing(orderMulti);

                                //_cartControllerOrchestrator.FireOrderSubmittedNotification(order, userCreatedInCart: false);
                                if (row.Webinar.SeriesInfo.Contains("Children"))
                                    createdSeriesOrders = _cartControllerOrchestrator.CreateSeriesOrders(row);

                                if (rowMulti.Webinar.idWebinar == 2520)
                                {
                                    //is a WSP order
                                    _cartControllerOrchestrator.CreateWspCode(orderMulti);
                                }

                                if (rowMulti.Webinar.Title.Contains("Compliance Perspectives")
                                    //test if CP code needs to be created by ensuring it's not a trial
                                    && rowMulti.RowPrice > 200)
                                    _cartControllerOrchestrator.CreateCompliancePerspectivesSubscription(rowMulti);

                                _SendOrderConfirmation2(orderMulti.idOrder);

                            }
                            catch (Exception ex)
                            {
                                _cartControllerOrchestrator.FireMandrillNotificationEvent(
                                    "2afda898.ttstrain.com@amer.teams.ms" // Errors channel of teams
                                    , "PayTracePostback Multi Error on: " + orderExcp, "PaytracePostback hit error: " + ex.Message);

                                _logger.FatalException("Paytrace: " + order.idOrder + "  Multi-OrderCheckout", ex);
                                throw;
                            }
                        }
                    }
                }
                else
                {
                    _logger.Fatal("Paytrace: " + order.idOrder + "  declines payment: {0}",
                        order.idOrder);
                }
            }
            catch (Exception exception)
            {
                _cartControllerOrchestrator.FireMandrillNotificationEvent(
                    "2afda898.ttstrain.com@amer.teams.ms" // Errors channel of teams
                    , "PayTrace Error!!", "PaytracePostback hit error: " + exception.Message + " |submitted values:" + formFields.ToString());

                ModelState.AddModelError(string.Empty,
                    "Connection Error #552. Please contact the administrator to complete transaction.");
                _logger.ErrorException("PayTrace postback returned APPROVED but controller failed ", exception);
                ErrorSignal.FromCurrentContext().Raise(exception);

                return $"Ok";
            }
            //        _logger.Error("ConfirmOrder Action | Id parameter was null");
            //        _logger.Error(string.Format("ConfirmOrder Action | {0}", _appHelper.GetSessionStartInfo()));
            //    }
            //        else
            //        {
            //            _logger.Info("Moneris declined with msg {0}: order {1}", form.message, JsonConvert.SerializeObject(order, Formatting.None, new JsonSerializerSettings { MaxDepth = 1, ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            //}));
            return $"Ok";


        }


        [HttpPost]

        public void PostBackWPS(FormCollection form)
        {
            //wps = webinar package subscription
            string formFields = Request.Form.ToString();
            _logger.Info("PostBackWPS: " + formFields);

        }


        [HttpGet]
        public JsonResult ContinueShopping(int idWebinar)
        {

            ContinueShoppingModel model = _cartControllerOrchestrator.BuildContinueShoppingModel(idWebinar);

            var topicButtons = "";

            foreach (var _topic in model.SelectedTopics)
            {
                topicButtons += "<button class='btn btn-mini topic_'  id='topic_" + _topic.idTopic + "' name='topic_" + _topic.idTopic + "'  value='" + _topic.Topic.topicDesc + "'>" + _topic.Topic.topicDesc + "</button>&nbsp;&nbsp;";
            }

            topicButtons.TrimEnd(new Char[] { '<', 'b', 'r', '>' });

            return Json(new { success = "success", topicButtons, presenterName = model.SelectedPresenter }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ContinueShoppingJumper(FormCollection form)
        {

            try
            {
                if (form["searchTerm"] != null && form["searchTerm"].Length > 1)
                {
                    _logger.Info("continueShoppingSearchTerm: " + form["searchTerm"]);
                    return RedirectToAction("Search", controllerName: "Webinar", routeValues: new { searchTerm = form["searchTerm"] });
                }

                if (form["relatedTag"] != null && form["relatedTag"] != string.Empty)
                {
                    _logger.Info("continueShoppingTopic: " + form["relatedTag"]);
                    return RedirectToAction("Search", controllerName: "Webinar", routeValues: new { searchTerm = form["relatedTag"] });
                }

                if (form["continueShoppingBySpeaker"] != null && form["continueShoppingBySpeaker"] != string.Empty)
                {
                    _logger.Info("continueShoppingPresenter: " + form["continueShoppingBySpeaker"]);
                    return RedirectToAction("Search", controllerName: "Webinar", routeValues: new { searchTerm = form["continueShoppingBySpeaker"] });
                }

                return Json(new { Result = WebUiConstants.Success });
            }
            catch (Exception exception)
            {
                ModelState.AddModelError(string.Empty, "The operation failed.");
                _logger.ErrorException("AdjustUserDetails|AdjustUserDetails failed ", exception);
                return this.ModelStateJson(ModelState);
            }
        }


        public ActionResult Checkout()
        {
            // do not allow anonymous (it errors due to null/empty list of orders)
            if (User == null || !User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account", new { ReturnURL = "/cart/checkout" });


            List<Order> orders = _cartControllerOrchestrator.GetOrdersByUser(User.Identity.Name)
                .Where(o => o.OrderStatus == OrderStatus.InProcess).ToList();

            IList<int> iDsToRemove = new List<int>();

            var result =
                    orders
                        .SelectMany(o => o.OrderRows.Where(r => r.RowStatus == OrderRowStatus.Active))
                        .OrderBy(o => o.Order.OrderDate)
                        .GroupBy(y => y.idWebinar)
                        .Where(g => g.Skip(1).Any())
                        .Select(g => g.Key)
                        .ToList()
                ;
            if (result.Any())
            {
                foreach (var i in result)
                {
                    var _orders =
                        orders.Where(o => o.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active).idWebinar == i)
                        .Skip(1).ToList();
                    foreach (var _orderId in _orders)
                    {
                        iDsToRemove.Add(_orderId.idOrder);
                    }
                }
            }

            foreach (var id in iDsToRemove)
            {
                var itemToRemove = orders.SingleOrDefault(o => o.idOrder == id);
                itemToRemove.OrderStatus = OrderStatus.Canceled;
                _cartControllerOrchestrator.SaveOrder(itemToRemove);
                if (itemToRemove != null)
                    orders.Remove(itemToRemove);
                _logger.Warn("Checkout found and canceled duped order: " + id);
            }

            RegistrationSummaryMultiViewModel model = BuildRegistrationSummaryMultiViewModel(orders);

            return View(model);
        }


        public ActionResult MyWebinars(int? idOrder)
        {
            if (User != null && User.Identity.IsAuthenticated)
            {
                ClaimsIdentity claimsIdentityOfAuthenticatedUser = (ClaimsIdentity)User.Identity;

                if (claimsIdentityOfAuthenticatedUser.HasClaim((claim) => claim.Type == ClaimTypes.Admin))
                {
                    return RedirectToAction("Index", "Admin");
                }
                ViewBag.OnDemandClaim = "";

                if (claimsIdentityOfAuthenticatedUser.HasClaim(
                    (claim) => claim.Type == CUWebinars.Business.Constants.ClaimTypes.Affiliate))
                {
                    var claimTTSDomain =
                        claimsIdentityOfAuthenticatedUser.Claims
                            .Where(c => c.Type == CUWebinars.Business.Constants.ClaimTypes.Affiliate)
                            .First().Value;
                    _stateService.SetValue(WebUiConstants.CurrentAffiliate,
                        _cartControllerOrchestrator.LoadByTTSDomain(claimTTSDomain));
                    return RedirectToAction("Index", "Admin");

                }

                CompliancePerspectivesModel compPersectivesModel =
                    _cartControllerOrchestrator.BuildCompPersectivesModel();
                var discountModel = _cartControllerOrchestrator.BuildDiscountModelForUser();
                var myWebinarsDTO = _cartControllerOrchestrator.BuildMyWebinarsDTO
                    (discountModel, claimsIdentityOfAuthenticatedUser);
                if (compPersectivesModel != null)
                    myWebinarsDTO.CompliancePerspectives = compPersectivesModel;

                ViewBag.idUser = myWebinarsDTO.WebUser.idUser;
                return View("MyWebinars", myWebinarsDTO);
            }
            else
            {
                if (idOrder.HasValue && idOrder.Value > 0)
                {
                    return RedirectToAction("Login", "Account", new { ReturnURL = "MyWebinars?idOrder=" + idOrder.Value });
                }
            }

            return RedirectToAction("Login", "Account", new { ReturnURL = "MyWebinars" });
        }



        private RegistrationSummaryMultiViewModel BuildRegistrationSummaryMultiViewModel(List<Order> orders)
        {
            RegistrationSummaryMultiViewModel model = new RegistrationSummaryMultiViewModel();

            model.RegistrationSummaryViewModels = new List<RegistrationSummaryViewModel>();

            foreach (Order order in orders)
            {
                OrderRow orderRowForOrder = order.OrderRows.SingleOrDefault(or => or.RowStatus == OrderRowStatus.Active);
                if (orderRowForOrder == null)
                    continue;
                int checkForDupedOrderId = _cartControllerOrchestrator.CheckIfEmailAlreadyRegisteredForWebinar(orderRowForOrder.idWebinar, order.BillingEmail);

                if (checkForDupedOrderId > 0)
                {

                    _cartControllerOrchestrator.SetOrderStatus(checkForDupedOrderId, order.BillingEmail,
                        OrderStatus.Canceled);
                    _logger.Warn("BuildRegistrationSummaryMultiViewModel found duped order: " + checkForDupedOrderId + "_" + order.BillingEmail);
                }
                if (orderRowForOrder.AdditionalLocation == null)
                    orderRowForOrder.AdditionalLocation = new List<AdditionalLocation>();

                if (orderRowForOrder.Webinar == null)
                    orderRowForOrder.Webinar = _cartControllerOrchestrator.LoadWebinar(orderRowForOrder.idWebinar);

                if (orderRowForOrder.RegistrationType == null)
                    orderRowForOrder.RegistrationType = _cartControllerOrchestrator.GetRegTypeById(orderRowForOrder.idRegType);

                if (orderRowForOrder.Order.Affiliate == null)
                    orderRowForOrder.Order.Affiliate = _cartControllerOrchestrator.GetAffiliateById(orderRowForOrder.Order.idAffiliate);

                RegistrationSummaryViewModel registrationSummaryViewModel = new RegistrationSummaryViewModel
                {
                    AdditionalLocationsViewModel = _cartControllerOrchestrator.BuildAdditionalLocationsViewModel(orderRowForOrder, orderRowForOrder.idOrder),
                    OrderRow = order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active),
                    RecordingLink =
                        "<a href='" + GlobalConfig.GlobalConfigSingleton.WMVRepository + orderRowForOrder.Webinar.RecordingUrl +
                        "' target=_blank /> Recording Playback</a>",
                    WebinarStatus = orderRowForOrder.Webinar.Status
                };


                if (orderRowForOrder.Discount != null &&
                    orderRowForOrder.Discount.DiscountType == DiscountType.Subscription)
                {
                    registrationSummaryViewModel.DiscountCaption = "WSP Credit Cost: " +
                                                                    orderRowForOrder.RegistrationType.CreditCost
                                                                        .ToString().Replace(".00", "");
                }

                model.RegistrationSummaryViewModels.Add(registrationSummaryViewModel);
            }

            string discountCaptionMultiMsg = "";
            string grandTotalCaptionMultiMsg = "";

            BuildCaptionsMulti(orders, out discountCaptionMultiMsg, out grandTotalCaptionMultiMsg);

            model.DiscountCaptionMulti = discountCaptionMultiMsg;
            model.GrandTotalCaptionMulti = grandTotalCaptionMultiMsg;

            return model;

        }



        private void BuildCaptionsMulti(List<Order> orders, out string discountCaptionMultiMsg, out string grandTotalCaptionMultiMsg)
        {
            decimal totalCostInCredits = 0M;
            decimal discountTotal = 0M;
            decimal creditsRemain = 0M;
            decimal grandTotal = 0M;
            string notEnoughCreditsCaption = "";

            discountCaptionMultiMsg = "";
            grandTotalCaptionMultiMsg = "";

            if (orders.Count == 0)
                return;

            foreach (Order order in orders)
            {
                OrderRow orderRowForOrder = order.OrderRows.SingleOrDefault(or => or.RowStatus == OrderRowStatus.Active);
                if (orderRowForOrder == null)
                    continue;




                grandTotal += order.Total;


                System.Diagnostics.Debug.WriteLine("idOrder: {0}, idOrderRow: {1}, running grandTotal: {2}", order.idOrder, orderRowForOrder.idOrderRow, grandTotal);

                // special subscription processing
                if (orderRowForOrder.Discount != null &&
                    orderRowForOrder.Discount.DiscountType == DiscountType.Subscription)
                {

                    System.Diagnostics.Debug.Write("Discount processing ");

                    decimal creditCost = orderRowForOrder.RegistrationType.CreditCost; // occasionally null???
                    totalCostInCredits += creditCost;

                    System.Diagnostics.Debug.WriteLine("running totalCostInCredits: {0}, orderRow.CreditCost: {1}", totalCostInCredits, creditCost);

                    Discount subDiscount = orderRowForOrder.Discount;
                    creditsRemain = _cartControllerOrchestrator.CalculateCreditsRemaining(subDiscount);

                    System.Diagnostics.Debug.WriteLine("creditsRemain: {0}", creditsRemain);
                    System.Diagnostics.Debug.WriteLine("discount.PercentOff: {0}", subDiscount.PercentOff);

                    if (creditsRemain >= totalCostInCredits)
                    {
                        decimal orderRowCost = orderRowForOrder.RowPrice * subDiscount.PercentOff / 100;
                        System.Diagnostics.Debug.WriteLine("enough credits to cover, adding on orderRow price: {0}", orderRowCost);

                        discountTotal += orderRowCost;
                        System.Diagnostics.Debug.WriteLine("running discountTotal: {0}", discountTotal);

                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("not enough credits to cover");

                        if (creditsRemain > 0)
                        {
                            System.Diagnostics.Debug.WriteLine("partial credits to apply: {0}", creditsRemain);

                            discountTotal += 265 * creditsRemain;

                            System.Diagnostics.Debug.WriteLine("running discountTotal: {0}", discountTotal);
                        }

                        //else
                        //{
                        //    discountTotal = 0;
                        //}
                    }
                } // end special subscription processing
            }


            if (creditsRemain < 0)
            {
                notEnoughCreditsCaption =
                    "The credits available to your Webinar Subscription Package do not " +
                    "completely cover the required cost of your orders. We've pro-rated " +
                    "the total amount required to " +
                    discountTotal + ".";

                System.Diagnostics.Debug.WriteLine("notEnoughCreditsCaption: {0}", notEnoughCreditsCaption);

            }

            //if (orders.Count == 0)
            //    creditsRemain = _cartControllerOrchestrator.CalculateCreditsRemaining(???);  // promote that they have credits??


            if (totalCostInCredits > 0)
            {
                discountCaptionMultiMsg =
                    "Your subscription package has " + creditsRemain + " credits. This cart uses " +
                    totalCostInCredits + ". " + notEnoughCreditsCaption;
            }

            System.Diagnostics.Debug.WriteLine("final discountCaptionMultiMsg: {0}", discountCaptionMultiMsg);

            grandTotalCaptionMultiMsg = "Included below are all currently pending orders. Any orders removed from here are permanently deleted. Edit any remaining orders for your exact needs and use our 'Bill Me' or credit card processing when you are ready for final checkout.<br><br><b> Grand Total: " + grandTotal.ToString("c").Replace(".00", "") + "</b>";

            System.Diagnostics.Debug.WriteLine("final grandTotalCaptionMultiMsg: {0}", grandTotalCaptionMultiMsg);

        }

        public JsonResult RemoveOrderJson(int idOrder, int idOrderRow)
        {
            return UpdateOrderStatus(idOrder, idOrderRow, OrderStatus.Canceled);
        }
        public JsonResult UndoRemoveOrderJson(int idOrder, int idOrderRow)
        {
            return UpdateOrderStatus(idOrder, idOrderRow, OrderStatus.InProcess);
        }

        private JsonResult UpdateOrderStatus(int idOrder, int idOrderRow, OrderStatus orderStatus)
        {
            _cartControllerOrchestrator.SetOrderStatus(idOrder, User.Identity.Name, orderStatus); // validates that the user owns this orderid

            List<Order> orders = _cartControllerOrchestrator.GetOrdersByUser(User.Identity.Name)
                .Where(o => o.OrderStatus == OrderStatus.InProcess).ToList();
            // used a couple of places, may want to make a function that just returns these...

            string discountCaptionMultiMsg = "";
            string grandTotalCaptionMultiMsg = "";

            BuildCaptionsMulti(orders, out discountCaptionMultiMsg, out grandTotalCaptionMultiMsg);

            return Json(new { Result = WebUiConstants.Success, discountCaptionMultiMsg = discountCaptionMultiMsg, grandTotalCaptionMultiMsg = grandTotalCaptionMultiMsg });
        }

        public JsonResult CheckoutConfirmOrderBillMeJson()
        {
            string currentUserEmail = User.Identity.Name;

            // get list of orders in process for this user
            List<Order> orders = _cartControllerOrchestrator.GetOrdersByUser(currentUserEmail)
                .Where(o => o.OrderStatus == OrderStatus.InProcess).ToList(); // used a couple of places, may want to make a function that just returns these...

            RegistrationSummaryMultiViewModel model = BuildRegistrationSummaryMultiViewModel(orders);

            StringBuilder sbOrdersSummary = new StringBuilder();
            StringBuilder sbHeaderSummary = new StringBuilder();
            sbHeaderSummary.Append("This confirmation includes summaries for the following orders:" + Environment.NewLine);
            foreach (RegistrationSummaryViewModel registrationSummaryViewModel in model.RegistrationSummaryViewModels)
            {
                _logger.Info("Multi-event checkout: " + registrationSummaryViewModel.OrderRow.idOrder);

                var createdSeriesOrders = "";
                if (registrationSummaryViewModel.OrderRow.Webinar.SeriesInfo.Contains("Children"))
                    if (registrationSummaryViewModel.OrderRow != null)
                        createdSeriesOrders =
                            _cartControllerOrchestrator.CreateSeriesOrders(
                                registrationSummaryViewModel.OrderRow);

                // update the rows to submitted status
                _cartControllerOrchestrator.SetOrderStatus(registrationSummaryViewModel.OrderRow.idOrder, currentUserEmail, OrderStatus.Submitted); // validates that the user owns this orderid

                _cartControllerOrchestrator.AddClaimForPostEventMaterials(registrationSummaryViewModel.OrderRow.Order.BillingEmail, registrationSummaryViewModel.OrderRow);

                if (createdSeriesOrders != "")
                {
                    var seriesOrders = createdSeriesOrders.Split(',');
                    foreach (var order in seriesOrders)
                    {
                        if (order != "")
                        {
                            _cartControllerOrchestrator.SetOrderStatus(Convert.ToInt32(order), currentUserEmail,
                                OrderStatus.Submitted); // validates that the user owns this orderid
                            var row = _cartControllerOrchestrator.GetOrderById(Convert.ToInt32(order)).OrderRows
                                .SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active);
                            _cartControllerOrchestrator.AddClaimForPostEventMaterials(
                                registrationSummaryViewModel.OrderRow.Order.BillingEmail, row);
                        }
                    }
                }
                // generate order summary email verbiage via RenderViewToString
                sbOrdersSummary.Append(ViewHelpers.RenderViewToString(ControllerContext, "~/Views/Shared/Partials/_OrderSumUser2.cshtml", registrationSummaryViewModel, true));
                sbHeaderSummary.Append("  " + registrationSummaryViewModel.OrderRow.idOrder + ",");
            }

            // send email via azure / 
            string subject = _globalConfig.OrderSubmittedMultiEmailSubject; // could do it in handler, could add dynamic content

            // take the summery and merge it into our HTML email template.  doing this early since we are reusing the generic-ish AzureCuwWebJobSmtpMessageDelivery code
            OrderSubmittedMultiViewModel orderSubmittedMultiViewModel = new OrderSubmittedMultiViewModel()
            {
                Subject = subject,
                OrderSummaryHtml = sbOrdersSummary.ToString(),
                DiscountCaption = model.DiscountCaptionMulti,
                //GrandTotalCaption = model.GrandTotalCaptionMulti,
                HeaderSummaryCaption = sbHeaderSummary.ToString().TrimEnd(',')
            };

            string htmlEmailBody = ViewHelpers.RenderViewToString(ControllerContext, "~/Notification/Templates/OrderSubmittedMulti.cshtml", orderSubmittedMultiViewModel, true);

            _cartControllerOrchestrator.FireOrderSubmittedMultiNotification(currentUserEmail, subject, htmlEmailBody);

            // send back enough to make the user comfortable, adjust the screen (remove buttons, say thanks, etc.)
            return Json(new { Result = WebUiConstants.Success, msg = "Order successfully submitted - thank you! Please visit <b>My Webinars</b> (link above) for detailed information of all your events." });
        }
        public JsonResult CheckoutConfirmOrderPayTraceJson()
        {
            string currentUserEmail = User.Identity.Name;

            // get list of orders in process for this user
            List<Order> orders = _cartControllerOrchestrator.GetOrdersByUser(currentUserEmail)
                .Where(o => o.OrderStatus == OrderStatus.InProcess).ToList(); // used a couple of places, may want to make a function that just returns these...

            RegistrationSummaryMultiViewModel model = BuildRegistrationSummaryMultiViewModel(orders);

            StringBuilder sbOrdersSummary = new StringBuilder();
            StringBuilder sbHeaderSummary = new StringBuilder();
            sbHeaderSummary.Append("Payment will be submitted for the following orders:" + Environment.NewLine);
            foreach (RegistrationSummaryViewModel registrationSummaryViewModel in model.RegistrationSummaryViewModels)
            {
                _logger.Info("Multi-event checkout to PayTrace: " + registrationSummaryViewModel.OrderRow.idOrder);

                // update the rows to submitted status
                //_cartControllerOrchestrator.SetOrderStatus(registrationSummaryViewModel.OrderRow.idOrder, currentUserEmail, OrderStatus.Submitted); // validates that the user owns this orderid

                //_cartControllerOrchestrator.AddClaimForPostEventMaterials(registrationSummaryViewModel.OrderRow.Order.BillingEmail, registrationSummaryViewModel.OrderRow);


                // generate order summary email verbiage via RenderViewToString
                sbOrdersSummary.Append(ViewHelpers.RenderViewToString(ControllerContext, "~/Views/Shared/Partials/_OrderSumUser2.cshtml", registrationSummaryViewModel, true));
                sbHeaderSummary.Append("  " + registrationSummaryViewModel.OrderRow.idOrder + ",");
            }

            // send email via azure / mandrill
            string subject = _globalConfig.OrderSubmittedMultiEmailSubject; // could do it in handler, could add dynamic content

            // take the summery and merge it into our HTML email template.  doing this early since we are reusing the generic-ish AzureCuwWebJobSmtpMessageDelivery code
            OrderSubmittedMultiViewModel orderSubmittedMultiViewModel = new OrderSubmittedMultiViewModel()
            {
                Subject = subject,
                OrderSummaryHtml = sbOrdersSummary.ToString(),
                DiscountCaption = model.DiscountCaptionMulti,
                GrandTotalCaption = model.GrandTotalCaptionMulti,
                HeaderSummaryCaption = sbHeaderSummary.ToString().TrimEnd(',')
            };

            //string htmlEmailBody = ViewHelpers.RenderViewToString(ControllerContext, "~/Notification/Templates/OrderSubmittedMulti.cshtml", orderSubmittedMultiViewModel, true);

            //_cartControllerOrchestrator.FireOrderSubmittedMultiNotification(currentUserEmail, subject, htmlEmailBody);

            // send back enough to make the user comfortable, adjust the screen (remove buttons, say thanks, etc.)
            return Json(new { Result = WebUiConstants.Success, msg = "Order successfully submitted - thank you! Please visit <b>My Webinars</b> (link above) for detailed information of all your events." });
        }

        [HttpPost]
        public ActionResult UpdateAdditionalLocations(IEnumerable<AdditionalLocation> additionalLocations, int? newOrderRowId)
        {
            try
            {
                var model = _cartControllerOrchestrator.BuildCheckOutViewModel(newOrderRowId.Value);
                var row = model.Order.OrderRows.SingleOrDefault(o => o.RowStatus == OrderRowStatus.Active);
                _cartControllerOrchestrator.UpdateAdditionalLocationsForOrderRow(additionalLocations.Where(a => a.Email != null), newOrderRowId.Value);

                var updateSuccessCaption = "Updated Additional Locations";

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

                var regType = _cartControllerOrchestrator.GetRegTypeById(model.Order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active).idRegType);
                StringBuilder updateAddLocCaption = new StringBuilder();
                var orderStatusCaption = "";
                if (model.Order.OrderStatus == OrderStatus.Paid && model.Order.Total != model.Order.TotalPaid)
                {
                    model.Order.OrderStatus = OrderStatus.OutstandingBalance;
                    orderStatusCaption =
                        "<span id=\"orderStatusLabel\" class=\"label-important label\">Outstanding Balance</span>";
                    _cartControllerOrchestrator.SaveOrder(model.Order);
                }

                if (
                    row.AdditionalLocation.Any())
                {
                    var _addLocs =
                        row.AdditionalLocation;

                    if (_addLocs.Any())
                    {
                        if (_addLocs.Count == 1)
                        {
                            updateAddLocCaption.Clear();
                            updateAddLocCaption.AppendLine("1 Additional Location: (" +
                                                           _addLocs.FirstOrDefault().Price.ToString("C0") + ") " +
                                                           _addLocs.FirstOrDefault().Email);
                            updateSuccessCaption = "Order updated with an Additional Location for " +
                                                   (_addLocs.FirstOrDefault().Email + " at a cost of " +
                                                    _addLocs.FirstOrDefault().Price.ToString("C0"));
                            ;
                        }
                        else
                        {
                            var totalCost = _addLocs.Sum(a => a.Price);
                            {
                                updateAddLocCaption.Clear();
                                updateAddLocCaption.Append(_addLocs.Count + " Additional Locations: (" +
                                                           totalCost.ToString("C0"));
                            }
                            foreach (var addloc in _addLocs)
                            {
                                updateAddLocCaption.AppendLine(addloc.Email);
                            }
                            updateSuccessCaption = "Order updated to " + (_addLocs.Count + " Additional Locations costing " +
                                                    totalCost.ToString("c0"));
                        }
                    }
                    else
                    {
                        updateSuccessCaption = "Additional Locations are removed.";
                    }

                }
                return
                    Json(
                        new
                        {
                            DiscountCaption = discountCaption,
                            UpdateSuccessCaption = updateSuccessCaption,
                            UpdateAddLocCaption = updateAddLocCaption,
                            regTypeShort = regType.OptionLabelShort,
                            BasePrice = pricesAndDiscounts.UnitPrice,
                            Discount = pricesAndDiscounts.TotalDiscount,
                            OptionsPrice = pricesAndDiscounts.TotalCostOfOptions,
                            Tax = pricesAndDiscounts.TaxAmount,
                            Total = (pricesAndDiscounts.UnitPrice + pricesAndDiscounts.TotalCostOfOptions + pricesAndDiscounts.TaxAmount - pricesAndDiscounts.TotalDiscount),
                            TotalPaid = model.Order.TotalPaid,
                            FlatOff = pricesAndDiscounts.Discount.FlatOff,
                            OutstandingBalance = (pricesAndDiscounts.UnitPrice + pricesAndDiscounts.TotalCostOfOptions + pricesAndDiscounts.TaxAmount - pricesAndDiscounts.TotalDiscount) - model.Order.TotalPaid,
                            //ShippedDateString = shippDateString,
                            PercentOff = pricesAndDiscounts.Discount.PercentOff
                        });
            }
            catch (Exception ex)
            {
                _logger.FatalException("UpdateAdditionalLocations", ex);
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