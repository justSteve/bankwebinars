using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Mvc;
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
        private readonly GlobalConfig _globalConfig = GlobalConfig.GlobalConfigSingleton;

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

                    var msg = myDiscount.Notes;
                    //_cartControllerOrchestrator.UpdateOrderPricing(row.Order);
                    var newJson = new JProperty(
                        "DiscountIsApplied",
                        new JObject(new JProperty("DiscountID", myDiscount.idDiscount)
                            , new JProperty("By", _appHelper.GetUserAuditInfo())));


                    row.Order.AdminComments = JsonHelpers.ReplaceJsonWithStoredField(row.Order.AdminComments, newJson, "DiscountIsApplied");
                    //row.Order.AffiliateComments = JsonHelpers.AddObjectToJsonArray(row.Order.AffiliateComments, newJson);

                    var pricesAndDiscounts = _cartControllerOrchestrator.UpdateOrderPricingReadOnly(row.Order);

                    return
                        Json(
                            new
                            {
                                msg = msg,
                                regTypeShort = row.RegistrationType.OptionLabelShort,
                                BasePrice = pricesAndDiscounts.UnitPrice,
                                Discount = pricesAndDiscounts.TotalDiscount,
                                OptionsPrice = pricesAndDiscounts.TotalCostOfOptions,
                                Tax = pricesAndDiscounts.TaxAmount,
                                Total = pricesAndDiscounts.TotalOrderPrice,
                                FlatOff = (pricesAndDiscounts.Discount != null) ? pricesAndDiscounts.Discount.FlatOff : 0,
                                PercentOff = (pricesAndDiscounts.Discount != null) ? pricesAndDiscounts.Discount.PercentOff : 0,
                                CreditsRemain = _cartControllerOrchestrator.CalculateCreditsRemaining(myDiscount).ToString()
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
                _logger.ErrorException("ApplyDiscountCode|ApplyDiscountCode failed ", exception);
                return Json(new { Result = 0, Code = code, Order = orderRowId });
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

                if (_cartControllerOrchestrator.UserHasMultipleEvents(id))
                {
                    return Json(new
                    {
                        Result = "UserHasMulti"
                    }, JsonRequestBehavior.AllowGet);
                }
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
                        if (model.Order.Origin == "Express")
                        {
                            _cartControllerOrchestrator.FireOrderSubmittedNotification(model.Order, userCreatedInCart: false);
                        }

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
                    //_cartControllerOrchestrator.UpdateOrderPricing(model.Order);
                    if (model.Order.Total == 0) model.Order.OrderStatus = OrderStatus.Paid;

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
                    if (id == null)
                    {
                        id = Convert.ToInt32(Request.UrlReferrer.ToString().Split('=')[1].Split('&')[0]);
                        id =
                            _cartControllerOrchestrator.GetOrderById(id)
                                .OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active)
                                .idOrderRow;
                    }

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

                    var order = _cartControllerOrchestrator.LoadOrder(ID.Value);
                    ViewBag.Order = order;

                    var row = order.OrderRows.Single(r => r.RowStatus == OrderRowStatus.Active);
                    ViewBag.TaxAmount = model.DisplayRowPriceViewModel.PricesAndDiscounts.TaxAmount;
                    if (row.Discount != null)
                    {
                        ViewBag.DiscountCaption = _cartControllerOrchestrator.GetDiscountCaption(
                            row.Discount, row, null, 1);
                    }

                }
                else
                {
                    _logger.FatalException("CheckoutConfirm was passed a null or zero value: ", new Exception("null or zero ID passed to CheckoutConfirm partial"));
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
        public JsonResult IniMonerisModal(int idOrder)
        {
            //ensures that the price passed to moneris reflects order price with discount applied.
            var order = _cartControllerOrchestrator.GetOrderById(idOrder);

            var newPrice = _cartControllerOrchestrator.UpdateOrderPricing(order);

            return Json(new
            {
                success = "success",
                total = newPrice.TotalOrderPrice
            }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult PayTraceModalIni(int idOrder, string multi)
        {
            //ensures that the price passed to PayTrace reflects order price with discount applied.
            var order = _cartControllerOrchestrator.GetOrderById(idOrder);

            var newPrice = _cartControllerOrchestrator.UpdateOrderPricing(order);

            string wTitle = order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active).Webinar.Title;

            string regType =
                order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active).RegistrationType.OptionLabel;
            decimal totalAmt = newPrice.TotalOrderPrice;

            string[] orderList = null;
            var ProdDesc = "";
            if (multi != null)
            {
                orderList = multi.TrimEnd(',').Split(',');
            }
            if (orderList != null && orderList.Length > 0)
            {
                ProdDesc += "<tr><td colspan=3 align=left><font size=2><b>Registration Details - " + _globalConfig.Tenant + ".</b><br></font></td></tr>";
                ProdDesc += "<tr><td colspan=3 height=1 bgcolor=000000></td></tr>";
                ProdDesc += "<tr bgcolor=CCCCCC>";
                ProdDesc += "    <td align='Center'><b>Title</b></td>";
                ProdDesc += "    <td align='center'><b>Type</b></td>";
                ProdDesc += "    <td align='center'><b>Price</b></td>";
                ProdDesc += "</tr>";

                foreach (var _idOrder in orderList)
                {
                    var _order = _cartControllerOrchestrator.GetOrderById(Convert.ToInt32(_idOrder));
                    var _newPrice = _cartControllerOrchestrator.UpdateOrderPricing(_order);



                    totalAmt = totalAmt + _newPrice.TotalOrderPrice;


                    wTitle = _order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active).Webinar.Title;
                    wTitle = wTitle.Substring(0, Math.Min(wTitle.Length, 40)) + " <font size=2>(" + _globalConfig.TenantPrefix +
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
                }
            }
            else
            {
                ProdDesc += "<tr><td colspan=3 align=left><font size=2><b>Registration Details - " + _globalConfig.Tenant + "</b></font></td></tr>";
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
            }

            //format parameters for request 
            // to get an approval amount set: AMOUNT~1.00
            // to get a declined amount set: AMOUNT~1.12
            totalAmt = 1.00M;
            //string parameters = "UN~shuener|PSWD~PttAWka2|TERMS~Y|TRANXTYPE~Sale|";
            string parameters = "UN~demo123|PSWD~demo123|TERMS~Y|TRANXTYPE~Sale|";
            parameters += "ORDERID~" + idOrder + "|AMOUNT~" + totalAmt + "|";

            string return_url = @"http://" + Request.Url.Authority;

            parameters += "ApproveURL~https://bwdev.azurewebsites.net/cart/PayTraceApproved/|";
            parameters += "DeclineURL~https://bwdev.azurewebsites.net/cart/PayTraceDeclined/|";

            parameters += "ReturnURL~https://bwdev.azurewebsites.net/cart/PostBackPayTrace/|";
            //parameters += "ReturnURL~" + _globalConfig.TenantURL + "/cart/PostBackPayTrace/|";

            string parameter_list = "PARMLIST=";

            parameter_list = parameter_list + parameters;
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] bytes = encoding.GetBytes(parameter_list);

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
            string strResponse = reader.ReadToEnd();
            var responseString = strResponse;

            string authKey;

            // if we have errors if so output to ui
            if (!strResponse.Contains("ERROR"))
            {
                string[] _parameters = strResponse.Split('|');

                authKey = _parameters[1].Split('~')[1];
            }
            else
            {
                responseString = strResponse;
                authKey = "failed";
            }

            string paramList = string.Format("DISPLAYTRUSTLOGO~Y|DISABLETERMS~Y|ENABLEREDIRECT~N|RETURNPARIS~Y|authKey~{0}|disablelogin~y|disableoptional~y|showbname~y|hideinvoice~y|hidepassword~y|orderid~{1}|bname~{2}", authKey, idOrder, order.FirstName + ' ' + order.LastName);
            paramList += "|ProductDetails~" + ProdDesc.Replace(System.Environment.NewLine, "");
            //paramList += "|test~y";
            paramList += "|baddress~" + order.BillingAddress;
            paramList += "|bcity~" + order.BillingCity;
            paramList += "|bstate~" + order.BillingState;
            paramList += "|bzip~" + order.BillingZip;
            paramList += "|bcountry~US";
            paramList += "|email~" + order.BillingEmail;
            paramList += "|phone~" + order.BillingPhone;
            paramList += "|CUSTOMDBA~" + _globalConfig.TenantDomain;
            paramList += "|IMAGEURL~" + _globalConfig.TenantLogo;
            paramList += "|CANCELURL~https://bwdev.azurewebsites.net/cart/PayTraceCanceled";


            return Json(new
            {
                success = "success",
                responseString,
                paramList

            }, JsonRequestBehavior.AllowGet);
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
                if (incoming != null)
                {
                    var msgHtml = JsonConvert.DeserializeObject<MandrillIncomingMsg.mandrill_events>(incoming.ToString());
                    _logger.Info("Incoming " + msgHtml);
                    var parsedOrder = ParseMandrillMsg.ParseAcs("<html><body>" + msgHtml.msg.html + "</body></html>", DateTime.Now.ToString());
                    _logger.Info("IncomingFromMandrillParsed: " + JsonConvert.SerializeObject(parsedOrder));

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
                                    HttpUtility.UrlEncode(parsedOrder.BillingContact ?? "MissingBillingContact@ttstrain.com");
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

                    //_cartControllerOrchestrator.UpdateRegTypeOnLegacy(idRegType.Value, model.Order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).idWebinar, model.Order.BillingEmail);

                    _logger.Info("EditRegTypeTo: " + newRegType + " From: " + oldRegType + " on orderId: " + model.Order.idOrder);
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
                                regTypeShort = regType.OptionLabelShort,
                                BasePrice = pricesAndDiscounts.UnitPrice,
                                Discount = pricesAndDiscounts.TotalDiscount,
                                OptionsPrice = pricesAndDiscounts.TotalCostOfOptions,
                                Tax = pricesAndDiscounts.TaxAmount,
                                Total = pricesAndDiscounts.TotalOrderPrice,
                                FlatOff = pricesAndDiscounts.Discount.FlatOff,
                                ShippedDateString = shippDateString,
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


        public ActionResult Resume(int id)
        {
            if (Request.IsAuthenticated)
            {
                try
                {
                    var order = _cartControllerOrchestrator.GetOrderById(id);
                    order.Origin = DomainConstants.OriginResume;

                    _logger.Info("Resume idOrder: " + id + " by: " + User.Identity.Name);

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


        [HttpPost]
        public ActionResult ExpressPostback2(ExpressCheckoutModel form)
        {
            try
            {
                if (form.q11_orderid > 0)
                {
                    var order = _cartControllerOrchestrator.GetOrderById(form.q11_orderid);

                    order.Origin = DomainConstants.OriginExpress;

                    order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active).RegistrationType =
                        _cartControllerOrchestrator.GetRegTypeByLabel(form.q10_registrationType, form.q18_q_webinarid18);

                    _cartControllerOrchestrator.UpdateOrderPricing(order);

                    _logger.Info("ExpressPostback from: " + " - " + form.q11_orderid + _appHelper.GetUserAuditInfo());

                    return RedirectToAction("OrderComplete", "Account", new { id = order.idOrder});

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
            string formFields = Request.QueryString.ToString();
            _logger.Info("PayTraceApproved postback: " + formFields);

            return View();
        }

        public ActionResult PayTraceDeclined()
        {
            string formFields = Request.QueryString.ToString();
            _logger.Info("PayTraceDeclined postback: " + formFields);

            return View();
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

        [HttpPost]
        public ActionResult PostBackPayTrace()
        {
            string formFields = Request.Form.ToString().Replace("parmList=", "");
            //string formFields = HttpUtility.UrlDecode("BNAME%257EStephen%2bHueners%257CNAME%257EStephen%2bHueners%257CBADDRESS%257E7136%2bHeram%257CBADDRESS2%257E%257CBCITY%257EHolmen%257CBSTATE%257EWI%257CBCOUNTRY%257EUS%257CBZIP%257E54636%257CPHONE%257E%257CDESCRIPTION%257E%252A%252ATEST%252A%252Athis%2bis%2bfor%2bspecial%2binstructions%257CINVOICE%257E%257CAMOUNT%257E1%252E00%257C&cmdSecureCheckout=Click+here+to+return+to+the+TOTAL+TRAINING+SOLUTIONS+web+site.");
            formFields = HttpUtility.UrlDecode(formFields.Replace("parmList=", ""));
            _logger.Info("PostBackPayTrace: " + formFields);
            //PostBackPayTrace: parmList=ORDERID~131598|TRANSACTIONID~136100879|APPCODE~123456|APPMSG~Your TEST transaction was successfully processed. HOWEVER, NO FUNDS WILL BE transferred.|AVSRESPONSE~Full Exact Match|CSCRESPONSE~Match|EMAIL~user@ttstrain.com|BNAME~Stephen Hueners|CARDTYPE~MasterCard|EXPMNTH~08|EXPYR~19|LAST4~6285|AMOUNT~1.00|
            //var order = _cartControllerOrchestrator.LoadOrder(Convert.ToInt32(form.order_no.Split('-')[1]));

            //if (order == null) throw new ArgumentNullException("order");

            //JProperty monerisResponse = new JProperty(JsonPropertyKeys.MonerisResponse, JsonConvert.SerializeObject(form));

            //order.AdminComments = JsonHelpers.MergeJsonWithStoredField(order.AdminComments, monerisResponse);
            ////TODO: Fix Json formatter forJsonbrowser (jquery plugin)
            //// the comment being written here is valid Json but is not displayed nicely by the Json reader:
            //// original: { "MonerisResponse":"{\"FormId\":null,\"order_no\":\"BW-129285\",\"ref_num\":\"642120820012660120\",\"message\":\"APPROVED*\",\"result\":\"1\",\"auth_code\":\"062449\",\"txn_time\":\"14:06:26\",\"txn_date\":\"2016-09-23\",\"response_code\":\"001\",\"note\":\"BankWebinars thanks you! Watch your email for complete details. \"}"}
            ////  better: {"MonerisResponse":{"FormId":null,"order_no":"BW-129285","ref_num":"642120820012660120","message":"APPROVED*","result":"1","auth_code":"062449","txn_time":"14:06:26","txn_date":"2016-09-23","response_code":"001","note":"BankWebinars thanks you! Watch your email for complete details. "}}
            //_cartControllerOrchestrator.UpdateOrderPricing(order);

            //if (form.message.StartsWith("APPROVED"))
            //{
            //    try
            //    {
            //        _logger.Info("Confirming Moneris submission with Id BW-{0}", JsonConvert.SerializeObject(order, Formatting.None, new JsonSerializerSettings { MaxDepth = 1, ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));

            //        order.OrderStatus = OrderStatus.Paid;

            //        //_cartControllerOrchestrator.CreatePostEventClaim(order);
            //        _cartControllerOrchestrator.AddClaimForPostEventMaterials(order.BillingEmail, order.OrderRows.FirstOrDefault());

            //        if (User.Identity.IsAuthenticated)
            //        {
            //            _cartControllerOrchestrator.FireOrderSubmittedNotification(order, userCreatedInCart: false);
            //        }
            //        else
            //        {
            //            _cartControllerOrchestrator.FireOrderSubmittedNotification(order, userCreatedInCart: true);
            //        }

            //        return RedirectToAction("OrderComplete", "Account", new { id = order.idOrder, message = form.message });
            //    }
            //    catch (Exception exception)
            //    {
            //        ModelState.AddModelError(string.Empty,
            //            "Connection Error #552. Please contact the administrator to complete transaction.");
            //        _logger.ErrorException("MonerisConfirmOrder returned APPROVED but controller failed ", exception);
            //        ErrorSignal.FromCurrentContext().Raise(exception);
            //    }
            //    _logger.Error("ConfirmOrder Action | Id parameter was null");
            //    _logger.Error(string.Format("ConfirmOrder Action | {0}", _appHelper.GetSessionStartInfo()));
            //}
            //else
            //{
            //    _logger.Info("Moneris declined with msg {0}: order {1}", form.message, JsonConvert.SerializeObject(order, Formatting.None, new JsonSerializerSettings { MaxDepth = 1, ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
            //    return Json(new
            //    {
            //        Result = WebUiConstants.Fail,
            //        OrderRowID = order.idOrder,
            //        Msg = string.Format("Your credit card transaction did not complete successfully. Our processor replied with this message: {0}", form.message)
            //    }, JsonRequestBehavior.AllowGet);

            //}
            return this.ModelStateJson(ModelState);

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
            //TODO: Fix Json formatter for Jsonbrowser (jquery plugin)
            // the comment being written here is valid Json but is not displayed nicely by the Json reader:
            // original: { "MonerisResponse":"{\"FormId\":null,\"order_no\":\"BW-129285\",\"ref_num\":\"642120820012660120\",\"message\":\"APPROVED*\",\"result\":\"1\",\"auth_code\":\"062449\",\"txn_time\":\"14:06:26\",\"txn_date\":\"2016-09-23\",\"response_code\":\"001\",\"note\":\"BankWebinars thanks you! Watch your email for complete details. \"}"}
            //  better: {"MonerisResponse":{"FormId":null,"order_no":"BW-129285","ref_num":"642120820012660120","message":"APPROVED*","result":"1","auth_code":"062449","txn_time":"14:06:26","txn_date":"2016-09-23","response_code":"001","note":"BankWebinars thanks you! Watch your email for complete details. "}}
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

            RegistrationSummaryMultiViewModel model = BuildRegistrationSummaryMultiViewModel(orders);

            return View(model);
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

            grandTotalCaptionMultiMsg = "Grand Total: " + grandTotal.ToString("c").Replace(".00", "");

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
                .Where(o => o.OrderStatus == OrderStatus.InProcess).ToList(); // used a couple of places, may want to make a function that just returns these...

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


                // update the rows to submitted status
                _cartControllerOrchestrator.SetOrderStatus(registrationSummaryViewModel.OrderRow.idOrder, currentUserEmail, OrderStatus.Submitted); // validates that the user owns this orderid

                _cartControllerOrchestrator.AddClaimForPostEventMaterials(registrationSummaryViewModel.OrderRow.Order.BillingEmail, registrationSummaryViewModel.OrderRow);


                // generate order summary email verbiage via RenderViewToString
                sbOrdersSummary.Append(ViewHelpers.RenderViewToString(ControllerContext, "~/Views/Shared/Partials/_OrderSumUser2.cshtml", registrationSummaryViewModel, true));
                sbHeaderSummary.Append("  " + registrationSummaryViewModel.OrderRow.idOrder + ",");
            }

            // send email via azure / mailchimp
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

            // send email via azure / mailchimp
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
                _cartControllerOrchestrator.UpdateAdditionalLocationsForOrderRow(additionalLocations, newOrderRowId.Value);

                var model = _cartControllerOrchestrator.BuildCheckOutViewModel(newOrderRowId.Value);

                var UpdateSuccessCaption = "Updated Additional Locations";

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

                return
                    Json(
                        new
                        {
                            Result = WebUiConstants.Success,
                            DiscountCaption = discountCaption,
                            UpdateSuccessCaption = UpdateSuccessCaption,
                            //regTypeShort = regType.OptionLabelShort,
                            BasePrice = pricesAndDiscounts.UnitPrice,
                            Discount = pricesAndDiscounts.TotalDiscount,
                            OptionsPrice = pricesAndDiscounts.TotalCostOfOptions,
                            Tax = pricesAndDiscounts.TaxAmount,
                            Total = pricesAndDiscounts.TotalOrderPrice,
                            FlatOff = pricesAndDiscounts.Discount.FlatOff,
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