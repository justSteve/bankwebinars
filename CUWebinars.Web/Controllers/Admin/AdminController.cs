using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using AutoMapper;
using BrockAllen.MembershipReboot;
using CUWebinars.Business.AccountService;
using CUWebinars.Business.Constants;
using CUWebinars.Business.Core;
using CUWebinars.Business.Core.Extensions;
using CUWebinars.Business.Core.Helpers;
using CUWebinars.Business.Models;
using CUWebinars.Business.Notification;
using CUWebinars.Business.Notification.Formatters;
using CUWebinars.Business.Notification.ViewModel;
using CUWebinars.Business.Repository;
using CUWebinars.Business.Services;
using CUWebinars.Web.Core;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Infrastructure.Attributes;
using CUWebinars.Web.Infrastructure.Extensions;
using CUWebinars.Web.Models;
using CUWebinars.Web.Models.JsonModels;
using CUWebinars.Web.Services;
using CUWebinars.Web.ViewModel;
using Elmah;
using GemBox.Document;
using GemBox.Document.MailMerging;
using GemBox.Document.Tables;
using HtmlAgilityPack;
using MailChimp.Net;
using MailChimp.Net.Core;
using MailChimp.Net.Interfaces;
using MailChimp.Net.Models;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ninject.Extensions.Logging;
using Thinktecture.IdentityModel.Authorization;
using Thinktecture.IdentityModel.Authorization.Mvc;

using ClaimTypes = System.Security.Claims.ClaimTypes;
using ClaimTypes1 = CUWebinars.Business.Constants.ClaimTypes;
using DateTimeHelper = CUWebinars.Web.Helpers.DateTimeHelper;
using Formatting = Newtonsoft.Json.Formatting;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Thinktecture.IdentityModel.Http.Cors;
using Address = CUWebinars.Business.Models.Address;
using Order = CUWebinars.Business.Models.Order;
using PostEventClaim = CUWebinars.Business.Services.PostEventClaim;

namespace CUWebinars.Web.Controllers.Admin
{

    [ElmahHandleError]
    [ClaimsAuthorize]

    public class AdminController : Controller
    {
        public const string HtmlMimeType = "text/html";
        private readonly IOrderManagementService _orderManagementService;
        private readonly IDataTablesService _dataTablesService;
        private readonly IAppHelper _appHelper;
        private readonly IMembershipService _membershipService;
        private readonly ILogger _logger;
        private readonly IWebinarManagementService _webinarManagementService;
        private readonly IStateService _stateService;
        private readonly GlobalConfig _globalConfig = GlobalConfig.GlobalConfigSingleton;
        private readonly IAffiliateManagementService _affiliateManagementService;

        private readonly IFormatter _generalFormatter;

        //private bool _disposed;
        //
        // GET: /Admin/
        public AdminController(
            IMembershipService membershipService,
            ILogger logger,
            IStateService stateService,
            IWebinarManagementService webinarManagementService,
            IOrderManagementService orderManagementService,
            IDataTablesService dataTablesService,
            IAppHelper appHelper,
            IAffiliateManagementService affiliateManagementService,
            IFormatter generalFormatter)
        {
            _membershipService = membershipService;
            _logger = logger;
            _stateService = stateService;
            _webinarManagementService = webinarManagementService;
            _orderManagementService = orderManagementService;
            _dataTablesService = dataTablesService;
            _appHelper = appHelper;
            _affiliateManagementService = affiliateManagementService;
            _generalFormatter = generalFormatter;
        }

        public ActionResult GetPromoLinks(int? idWebinar, int? idAffiliate)
        {
            PromoLinks model = new PromoLinks();
            if (idWebinar.HasValue && idAffiliate.HasValue)
            {
                ClaimsIdentity claimsIdentityOfAuthenticatedUser = (ClaimsIdentity)User.Identity;
                var currentAffiliate = _orderManagementService.GetAffiliateById(19);

                if (claimsIdentityOfAuthenticatedUser.HasClaim(
                    (claim) => claim.Type == Business.Constants.ClaimTypes.Affiliate))
                {
                    currentAffiliate =
                        _orderManagementService.GetAffiliateByDomain(claimsIdentityOfAuthenticatedUser.Claims
                            .Where(c => c.Type == Business.Constants.ClaimTypes.Affiliate).Select(c => c.Value).Single());
                }

                model.Affiliate = currentAffiliate;
                model.Links =
                    _affiliateManagementService.GetPromosByAffiliate(_globalConfig.Tenant, currentAffiliate.idUserAff,
                        idWebinar.Value);

            }
            return View("~/Views/Admin/Partials/_ShowPromoLinks.cshtml", model);
        }

        [HttpGet]
        public ActionResult EditAffiliate(int idAffiliate)
        {

            ViewBag.PageStyleType = "index-flex-dark";
            ClaimsIdentity claimsIdentityOfAuthenticatedUser = (ClaimsIdentity)User.Identity;

            if (claimsIdentityOfAuthenticatedUser.HasClaim(
                (claim) => claim.Type == CUWebinars.Business.Constants.ClaimTypes.Admin))
            {
                Affiliate affiliate = _affiliateManagementService.FindById(idAffiliate);

                WebUser user = _membershipService.GetWebUserById(idAffiliate);

                var model = new AffiliateSettingsViewModel { Affiliate = affiliate, WebUser = user };
                return View(model);
            }
            return View(new AffiliateSettingsViewModel { Affiliate = null, WebUser = null });
        }

        [ValidateInput(false)]
        [HttpPost]
        public JsonResult AffiliateUpdate(AffiliateSettingsViewModel model)
        {
            try
            {
                _affiliateManagementService.UpdateAffiliate(model.Affiliate);

                return Json(new { result = "Success" });
            }
            catch (Exception ex)
            {
                _logger.FatalException("AffiliateUpdate", ex);
                return Json(new { result = "Failed" });
            }
        }


        //
        // GET: /Admin/
        //[ClaimsAuthorize(IdentityConstants.Access, IdentityConstants.AffiliateFunction)]
        public ActionResult Index()
        {
            var aff = _affiliateManagementService.FindById(19);

            string searchTerm = Request["searchTerm"];

            ViewBag.Title = "Search Results";

            ClaimsIdentity claimsIdentityOfAuthenticatedUser = (ClaimsIdentity)User.Identity;
            var affiliate = _stateService.GetValue<Affiliate>(WebUiConstants.CurrentAffiliate);
            var user = _membershipService.GetWebUserById(affiliate.idUserAff);
            var model = new AdminDTO
            {
                ShowWebinarsViewModel = new ShowWebinarsViewModel
                {
                    SearchTerm = searchTerm,
                    UserIsAdmin = true

                },
                UserDetailsViewModel = new UserDetailsViewModel()
                {
                    Affiliate = aff,
                    UserIsAdmin = true
                },
                AffiliateSettingsViewModel = new AffiliateSettingsViewModel()
                {
                    Affiliate = affiliate,
                    WebUser = user
                },

                //WpsViewModel = new WpsViewModel
                //{
                //    Discounts = _affiliateManagementService.GetSubscriptionsByAffiliate(19)
                //    .Where(d => d.DiscountType == DiscountType.Subscription).ToList()
                //},

                //CompPersSubscriptionsModel = new CompPersSubscriptionsModel
                //{
                //    Discounts = _affiliateManagementService.GetSubscriptionsByAffiliate(19)
                //}


            };

            if (claimsIdentityOfAuthenticatedUser.HasClaim(
                (claim) => claim.Type == CUWebinars.Business.Constants.ClaimTypes.Affiliate))
            {
                model.ShowWebinarsViewModel = new ShowWebinarsViewModel
                {
                    SearchTerm = searchTerm,
                    UserIsAdmin = false,
                    Affiliate = affiliate

                };

                model.InvoicesModel =
                    new InvoicesModel
                    {
                        Affiliate = affiliate,
                        Links =
                            _affiliateManagementService.GetInvoicesByAffiliate(_globalConfig.Tenant, affiliate.idUserAff)
                    };

                model.WpsViewModel = new WpsViewModel
                {
                    Discounts =
                        _affiliateManagementService.GetSubscriptionsByAffiliate(affiliate.idUserAff)
                            .Where(d => d.DiscountType == DiscountType.Subscription)
                            .ToList()
                };

                model.CompPersSubscriptionsModel = new CompPersSubscriptionsModel
                {
                    Discounts = _affiliateManagementService.GetSubscriptionsByAffiliate(affiliate.idUserAff)
                };

                model.UserDetailsViewModel = new UserDetailsViewModel()
                {
                    Affiliate = affiliate,
                    UserIsAdmin = false
                };
                model.AffiliateSettingsViewModel = new AffiliateSettingsViewModel()
                {
                    Affiliate = affiliate,
                    WebUser = user
                };
            }
            ;


            return View("~/Views/Admin/Home/Index.cshtml", model);

        }

        public PartialViewResult GetAffiliates()
        {
            var affiliates = _orderManagementService.GetAffiliatesForDisplayList();

            var affiliatesChooserModel = new AffiliatesChooserModel
            {
                AffiliatesList = affiliates.Select(a => new SelectListItem
                {
                    Text = a.Value,
                    Value = a.Key.ToString()
                })
            };

            return PartialView("~/Views/Admin/Partials/_AffiliatesChooser.cshtml", affiliatesChooserModel);
        }

        public ActionResult SetAffiliateAssignedToOrder(int? idAffiliate, int? idOrder)
        {
            var order = _orderManagementService.GetOrderById(idOrder.Value);
            var originalAffiliate = new AffiliateRepository().FindByIdWithIncluding(order.idAffiliate);
            order.idAffiliate = idAffiliate.Value;

            var newAffiliate = new AffiliateRepository().FindByIdWithIncluding(idAffiliate.Value);

            string buildMessage = "Affiliate changed for order " + order.idOrder +
                                  " from " + originalAffiliate.ttsDomain + " to " +
                                  newAffiliate.ttsDomain + " by " + User.Identity.Name +
                                  " on " + TtsConfig.UtcNowAsCts.ToShortDateString();
            try
            {
                if (order.InvoiceDetail != null && order.InvoiceDetail.StartsWith("{\"OrderIsInvoiced"))
                {
                    _logger.Warn("Invoiced Order changed affiliate: " + order.idOrder + " from " +
                                 originalAffiliate.ttsDomain + " to " +
                                 newAffiliate.ttsDomain);

                    var toJson = JObject.Parse(order.InvoiceDetail);
                    var nullChecked = toJson.Properties().FirstOrDefault(p => p.Name.StartsWith("OrderIsInvoiced"));

                    if (nullChecked != null)
                    {
                        //properties saved when invoiced:
                        //"", invoice.InvoiceID),
                        //"", TtsConfig.UtcNowAsCts,
                        //"AmountOfOrder", order.Total),
                        //"AmountOfRoyalty", row.Royalty),
                        //"", row.PercentPaid),
                        //"Affiliate", _affiliateRepository.FindById(order.idAffiliate).ttsDomain)
                        var row = order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active);
                        //
                        var dataOperations = new DataOperations(TtsConfig.DefaultConnectionString);
                        string preSaveValues = dataOperations.GetPreSaveValues(order.idOrder);

                        row.Royalty = row.RowPrice * (decimal)nullChecked.First()["PercentPaid"];
                        if (order.idAffiliate != originalAffiliate.idUserAff)
                        {
                            //order will be re-invoiced when processed as via the postevent-orders branch
                            order.InvoiceDetail = order.InvoiceDetail.Replace("OrderIsInvoiced",
                                "AffiliateReassignedNeedsNewInvoice");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.FatalException("SetAffiliateAssignedToOrder Json Merge: ", ex);
            }

            _orderManagementService.SaveChanges();

            _logger.Info(buildMessage);

            return Json(new { Result = WebUiConstants.Success, NewAffiliate = newAffiliate.ttsDomain });
        }

        private void SyncAdditionalLocations(ManageOrderEditModel model, OrderRow orderRow)
        {
            IList<AdditionalLocation> deletedAdditionalLocations = new List<AdditionalLocation>();
            IList<AdditionalLocation> addedAdditionalLocations = new List<AdditionalLocation>();

            if (model.AdditionalLocations == null || !model.AdditionalLocations.Any())
            {
                CollectionExtensions.AddRange(deletedAdditionalLocations, orderRow.AdditionalLocation);
                model.AdditionalLocations = new List<AdditionalLocation>();
            }
            else
            {
                foreach (var additionalLocation in
                    model.AdditionalLocations.Where(
                        al => !orderRow.AdditionalLocation.Select(eal => eal.Email).Contains(al.Email)))
                {
                    if (_appHelper.CheckIsEmailValid(additionalLocation.Email))
                        addedAdditionalLocations.Add(additionalLocation);
                }
            }

            if (!orderRow.AdditionalLocation.Any())
            {
                CollectionExtensions.AddRange(addedAdditionalLocations, model.AdditionalLocations);
            }
            else
            {
                foreach (var additionalLocation
                    in
                    orderRow.AdditionalLocation.Where(
                        al => !model.AdditionalLocations.Select(mal => mal.Email).Contains(al.Email)))
                {
                    deletedAdditionalLocations.Add(additionalLocation);
                }
            }

            foreach (var deletedAdditionalLocation in deletedAdditionalLocations)
            {
                orderRow.AdditionalLocation.Remove(deletedAdditionalLocation);
                _orderManagementService.RemoveAndDeleteAdditionalLocation(deletedAdditionalLocation);
            }

            foreach (var addedAdditionalLocation in addedAdditionalLocations)
            {
                if (_appHelper.CheckIsEmailValid(addedAdditionalLocation.Email))
                {
                    orderRow.AdditionalLocation.Add(addedAdditionalLocation);
                    _orderManagementService.AddAdditionalLocation(addedAdditionalLocation);
                }
            }
        }


        [AllowAnonymous]
        public JsonResult GetEditOrderStatusDropdownHtml()
        {
            string html = "";

            _logger.Info("GetEditOrderStatusDropdownHtml called");
            //if (ClaimsAuthorization.CheckAccess(IdentityConstants.Access, IdentityConstants.GetGridDataFeature))
            //{
            html = ViewHelpers.RenderViewToString(ControllerContext,
                "~/Views/Shared/EditorTemplates/DataTablesEditorTemplates/EditOrderStatus_Compact.cshtml",
                null, true);
            //}

            return Json(new { html = html });

        }

        [AllowAnonymous]
        public JsonResult GetEditDiscountDropdownHtml()
        {
            string html = "";
            if (ClaimsAuthorization.CheckAccess(IdentityConstants.Access, IdentityConstants.GetGridDataFeature))
            {
                html = ViewHelpers.RenderViewToString(ControllerContext,
                    "~/Views/Shared/EditorTemplates/DataTablesEditorTemplates/EditDiscount_Compact.cshtml",
                    null, true);
            }

            return Json(new { html = html });

        }

        [HttpPost]
        //[ValidateAntiForgeryToken(Order = 0)]
        [HandleAjaxException]
        public ActionResult UpdateOrderStatus(ManageOrderEditModel model, int? discountId)
        {
            var _discountId = 0;
            if (discountId.HasValue)
                _discountId = discountId.Value;

            if (ModelState.IsValid)
            {
                try
                {

                    var order = _orderManagementService.GetOrderById(model.Id);
                    _logger.Info("Updating OrderStatus " + order.idOrder + " from: " + order.OrderStatus + " to: " +
                                 model.DisplayRowPriceViewModel.OrderStatus + " by: " + _appHelper.GetUserAuditInfo());

                    var dataOperationsV3 = new DataOperations(TtsConfig.DefaultConnectionString);

                    string preSaveValues = dataOperationsV3.GetPreSaveValues(order.idOrder);


                    if ((model.DisplayRowPriceViewModel.OrderStatus == OrderStatus.Billed ||
                         model.DisplayRowPriceViewModel.OrderStatus == OrderStatus.Paid ||
                         model.DisplayRowPriceViewModel.OrderStatus == OrderStatus.Submitted)
                        && order.idAffiliate == 62
                        && order.OrderStatus == OrderStatus.AwaitingVerification)
                    {
                        var newDate = TtsConfig.UtcNowAsCts.ToShortDateString();
                        JProperty NewOrderDateApplied = new JProperty(JsonPropertyKeys.NewOrderDateAppliedKey,
                       "'OrderIsUpdated': {' OrderDate changed from " + order.OrderDate + " to " + newDate + " '}");

                        //                    order.AdminComments = JsonHelpers.MergeJsonWithStoredField(null,
                        //                        createdByExpressCheckout);

                        order.AffiliateComments = JsonHelpers.MergeJsonWithStoredField(order.AffiliateComments, NewOrderDateApplied);


                        order.OrderDate = DateTime.Now;

                    }
                    order.OrderStatus = model.DisplayRowPriceViewModel.OrderStatus;
                    // the only field that we are updating at this time
                    try
                    {
                        if (!string.IsNullOrEmpty(order.InvoiceDetail) &&
                            order.OrderStatus == OrderStatus.Canceled)
                        {
                            _logger.Warn("Invoiced Order is canceled: " + order.idOrder);

                            var toJson = JObject.Parse(order.InvoiceDetail);
                            var thisInvoice =
                                toJson.Properties().FirstOrDefault(p => p.Name.StartsWith("OrderIsInvoiced"));

                            if (thisInvoice != null)
                            {
                                var row = order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active);
                                //
                                row.Royalty = 0;

                                var regTypeShortened =
                                    preSaveValues.Split(',')[4].Replace(" Package", "")
                                        .Replace("Live Plus Six", "Live+6")
                                        .Replace(" and Hardcopy Handouts", "")
                                        .Replace(" Recording Only", "")
                                        .Replace(" Plus Five", "+5");
                                StringBuilder sb = new StringBuilder();

                                sb.Append(order.idOrder + " was first invoiced on " + thisInvoice.First()["InvoiceId"] +
                                          " as type '" + regTypeShortened + "' for $" +
                                          preSaveValues.Split(',')[0].ToString().Replace(".0000", "").Replace(".00", "") +
                                          ") ");
                                sb.Append(" but was canceled " +
                                          TtsConfig.UtcNowAsCts.ToString(DomainConstants.DateTimeShortFormat) + ". ");

                                var adustmentAmount = 0 - (decimal)thisInvoice.First()["AmountOfRoyalty"];

                                //set default direction
                                var adjustmentDirection = "Royalty is decreased";

                                sb.Append(adjustmentDirection + " by " +
                                          adustmentAmount.ToString("C").Replace(".00", ""));

                                row.RowPrice = 0;
                                var newJson4Invoice = new JProperty(
                                    "ChangedOrderNeedsNewInvoice",
                                    new JObject(
                                        new JProperty("OriginalInvoice", thisInvoice.First()["InvoiceId"].ToString()),
                                        new JProperty("OriginalDateOfInvoice",
                                            thisInvoice.First()["DateOfInvoice"].ToString()),
                                        new JProperty("OriginalTotal", thisInvoice.First()["AmountOfOrder"].ToString()),
                                        new JProperty("OriginalPercentPaid",
                                            thisInvoice.First()["PercentPaid"].ToString()),
                                        new JProperty("OriginalRoyaltyPaid",
                                            thisInvoice.First()["AmountOfRoyalty"].ToString()),
                                        new JProperty("OriginalAffiliate", thisInvoice.First()["Affiliate"].ToString()),
                                        new JProperty(adjustmentDirection, adustmentAmount),
                                        new JProperty("DateOfChange",
                                            TtsConfig.UtcNowAsCts.ToString(DomainConstants.DateTimeShortFormat)),
                                        new JProperty("Message", sb.ToString())
                                    ));


                                order.InvoiceDetail = JsonHelpers.ReplaceJsonWithStoredField(
                                    order.InvoiceDetail, newJson4Invoice, "OrderIsInvoiced");

                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        _logger.FatalException("UpdateOrderChanged Json Merge: ", ex);
                    }

                    _orderManagementService.UpdateOrderByAdmin(order);

                    return
                        Json(
                            new
                            {
                                Result = WebUiConstants.Success,
                                orderStatus = model.DisplayRowPriceViewModel.OrderStatus.ToString(),
                                msgFromLegacy = "not currently implemented"
                            });
                }
                catch (Exception exception)
                {
                    _logger.ErrorException("In UpdateOrderStatus Action: ", exception);
                    ErrorSignal.FromCurrentContext().Raise(exception);
                    throw;
                }
            }
            return this.ModelStateJson(ModelState);
        }

        [HttpPost]
        public ActionResult Delete(int idOrder)
        {

            return null;
        }


        [HttpPost]
        public ActionResult UnDelete(int idOrder)
        {

            return null;
        }

        [HttpPost]
        [HandleAjaxException]
        public ActionResult SetPriceOfOrder(int orderID, string note, string targetPrice)
        {
            _logger.Info("SetPriceOfOrder: " + orderID + " note: " + note + " to: " + targetPrice);
            var order = _orderManagementService.GetOrderById(orderID);
            var dataOperations = new DataOperations(TtsConfig.DefaultConnectionString);
            var amtToDiscount = "";
            if (targetPrice == "0")
            {
                amtToDiscount = "100%";
            }
            else
            {
                amtToDiscount = (order.Total - Convert.ToInt32(targetPrice)).ToString(CultureInfo.CurrentCulture);
            }

            int idDiscount = dataOperations.CreateAdjustmentDiscount(note, amtToDiscount, orderID);

            try
            {

                var msg = "";
                order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active).Discount =
                    _orderManagementService.GetDiscountById(idDiscount);

                JProperty createdByExpressCheckout = new JProperty(JsonPropertyKeys.PriceAdjusted,
                    "'PriceAdjusted': {'" + _appHelper.GetUserAuditInfo() + "'}");

                order.AdminComments = JsonHelpers.MergeJsonWithStoredField(order.AdminComments,
                    createdByExpressCheckout);

                _orderManagementService.SaveOrderChanges(order, string.Empty, string.Empty);


                msg = order.idOrder + " update price " + targetPrice + " via Discount: " + idDiscount;


                if (!string.IsNullOrEmpty(order.InvoiceDetail))
                {

                }
                _orderManagementService.SaveChanges();


                return
                    Json(
                        new
                        {
                            Result = WebUiConstants.Success,
                            message = msg
                        });
            }
            catch (Exception ex)
            {
                _logger.FatalException("SetPriceOfOrder", ex);
                Json(
                    new
                    {
                        Result = WebUiConstants.Fail,
                        Reason =
                        "Server Error on SetPriceOfOrder"
                    });
                throw;
            }

        }

        [HttpPost]
        [HandleAjaxException]
        public ActionResult SetUserAssignedToOrder(int orderID, string migrateOrder, string targetUserEmail)
        {
            try
            {

                var order = _orderManagementService.GetOrderByIdThin(orderID);
                var webUser =
                    _membershipService.GetWebUserById(_membershipService.GetWebUserIdByEmail(targetUserEmail).Value);

                if (ReferenceEquals(null, webUser))
                {
                    return
                        Json(new { Result = WebUiConstants.Fail, Reason = targetUserEmail + " was not found." });
                }

                Debug.Assert(webUser.Addresses.Single(a => a.AddressType == WebUiConstants.BillingAddress) != null,
                    "There must be at least a Billing Address or we have bad data.");

                var msg = "";
                if (migrateOrder.Equals("moveAll", StringComparison.OrdinalIgnoreCase))
                {
                    IList<Order> ordersToMove = _orderManagementService.FindOrdersByUserId(order.idUser).ToList();

                    for (int j = 0; j < ordersToMove.Count; j++)
                    {
                        ordersToMove[j].BillingEmail = webUser.email;
                        ordersToMove[j].idUser = webUser.idUser;
                        ordersToMove[j].FirstName = webUser.FirstName;
                        ordersToMove[j].LastName = webUser.LastName;
                        msg += ordersToMove[j].idOrder + ", ";

                    }
                    _orderManagementService.SaveChanges();
                    msg = msg.TrimEnd(',') + "moved to " + targetUserEmail;
                }
                else
                {
                    order.idUser = webUser.idUser;
                    order.BillingEmail = webUser.email;
                    order.FirstName = webUser.FirstName;
                    order.LastName = webUser.LastName;
                    msg = order.idOrder + " moved to " + targetUserEmail;
                    _orderManagementService.SaveChanges();
                }

                return
                    Json(
                        new
                        {
                            Result = WebUiConstants.Success,
                            message = msg
                        });
            }
            catch (Exception exception)
            {
                _logger.ErrorException(
                    string.Format("SetUserAssignedToOrder. Session | {0}", _appHelper.GetUserAuditInfo()), exception);
            }

            return
                Json(
                    new
                    {
                        Result = WebUiConstants.Fail,
                        Reason =
                        "Server Error - Set user assigned to Order: For customer service contact us by using the Online Chat button below or emailing Support@ttsTrain.com."
                    });
        }


        public ActionResult ClaimsManagement()
        {
            var frameworkTypesAsSelectList = GetClaimTypesFromClass(typeof(ClaimTypes), ConstantType.Constant);
            var customTypesAsSelectList = GetClaimTypesFromClass(typeof(Business.Constants.ClaimTypes),
                ConstantType.Readonly);

            var model = new AddClaimInputModel
            {
                ClaimTypes = frameworkTypesAsSelectList,
                TtsClaimTypes = customTypesAsSelectList
            };

            return View(model);
        }

        [HttpPost]
        [ValidateJsonAntiForgeryToken(Order = 0)]
        [HandleAjaxException(Order = 1)]

        public ActionResult ClaimsManagement(AddClaimInputModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var webUser = _membershipService.GetUserByEmail(model.UserEmail);

                    if (ReferenceEquals(webUser, null))
                    {
                        ModelState.AddModelError(string.Empty, "There is no user with that email address on file.");
                        return this.ModelStateJson(ModelState);
                    }
                    var userAccount = _membershipService.GetUserAccountByEmail(_globalConfig.Tenant, webUser.email);

                    if (ReferenceEquals(userAccount, null))
                    {
                        ModelState.AddModelError(string.Empty, "There is no user with that email address on file.");
                        return this.ModelStateJson(ModelState);
                    }

                    if (model.NewClaimType.Equals(
                        Business.Constants.ClaimTypes.PostEventMaterials, StringComparison.OrdinalIgnoreCase))
                    {
                        var result = _membershipService.ValidatePostEventMaterialsAccessClaimValue(
                            model.NewClaimValue,
                            model.NewClaimType
                        );

                        if (result.IsValid)
                        {
                            var claimArray = model.NewClaimValue.Split(':');
                            var order = _orderManagementService.GetOrderById(int.Parse(claimArray[0]));
                            var onDemandCode =
                                order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).OnDemandCode;

                            var orderIdProperty = new JProperty(JsonPropertyKeys.OrderId, order.idOrder);
                            var expiryDateProperty = new JProperty(JsonPropertyKeys.ExpiryDate,
                                DateTime.Parse(claimArray[1]));
                            var onDemandCodeProperty = new JProperty(JsonPropertyKeys.OnDemandCode, onDemandCode);

                            var claimValue = new JObject(
                                orderIdProperty,
                                expiryDateProperty,
                                onDemandCodeProperty
                            );

                            _membershipService.AddClaim(
                                userAccount,
                                model.NewClaimType,
                                claimValue.ToString(Formatting.None)
                            );
                            return Json(new { Result = WebUiConstants.Success });
                        }
                        return Json(new { Result = WebUiConstants.Fail, Msg = result.Errors.ElementAt(0).ErrorMessage });
                    }

                    _membershipService.AddClaim(
                        userAccount,
                        model.NewClaimType,
                        model.NewClaimValue
                    );

                    return Json(new { Result = WebUiConstants.Success });
                }
                catch (Exception ex)
                {
                    _logger.FatalException("ClaimsManagement error on " + model.UserEmail, ex);
                }
            }

            return this.ModelStateJson(ModelState);
        }

        public ActionResult GenerateClickToJoinForAdHocCaller()
        {
            var generateClickToJoinViewModel = new GenerateClickToJoinViewModel
            {

                Webinars = EventInvokerHelpers.GetUpcomingWebinarsAsSelectListItems(_webinarManagementService)
            };

            return View("GenerateClickToJoin", generateClickToJoinViewModel);
        }



        [HttpPost]
        [AllowAnonymous]
        public void ExpressCheckout(JotFormWebHook postback)
        {
            string formFields = Request.Form.ToString();
            _logger.Info("ExpressCheckoutPostBack all fields submitted to: " + formFields.ToString());

            ExpressCheckoutModel form
                = JsonConvert.DeserializeObject<ExpressCheckoutModel>(postback.RawRequest);
            _logger.Info("ExpressCheckoutPostBack all fields submitted to " + form.q11_orderid + ". " +
                         formFields.ToString());

            var user = _membershipService.GetUserByEmail(form.q5_email5);
            bool userCreatedByCheckout = false;
            if (ReferenceEquals(null, user))
            {
                userCreatedByCheckout = true;
                _logger.Info("ExpressCheckout user must be created: " + form.q5_email5);
                //(string city, string state, string zip, string streetAdd1, string streetAdd2
                //, string tenant, string email, string firstName, string lastName, string phone, string institution, string title)
                user = _membershipService.CreateWebUserFromExpressCheckout(
                    form.q14_address14.city
                    , form.q14_address14.state
                    , form.q14_address14.postal
                    , form.q14_address14.addr_line1
                    , form.q14_address14.addr_line2
                    , _globalConfig.Tenant
                    , form.q5_email5
                    , form.q4_name.first
                    , form.q4_name.last
                    , form.q6_phoneNumber6.area + "=" + form.q6_phoneNumber6.phone
                    , form.q8_institution
                    , form.q9_title);
                _orderManagementService.SaveChanges();
            }

            Order expressOrder = _orderManagementService.GetOrderById(form.q11_orderid);


            var regTypeLable = form.q10_registrationType.Replace(" (months)", "").Replace(" (days)", "");
            if (!ReferenceEquals(expressOrder, null))
            {
                try
                {
                    _logger.Info("ExpressCheckout webhook {0} found order: {1}", postback.FormId, expressOrder.idOrder);
                    _orderManagementService.AssignWebUserToOrder(user, expressOrder);

                    var row = expressOrder.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active);

                    if (!ReferenceEquals(form.q20_discountCode20, ""))
                    {
                        var discount = _orderManagementService.GetDiscountByCode(form.q20_discountCode20);

                        //should no longer be attempting to persist discount tally
                        //_orderManagementService.ApplyDiscountCode(discount.DiscountCode, row);

                    }

                    RegType regType = _orderManagementService.GetRegTypeByLabel(regTypeLable, form.q18_q_webinarid18);

                    var additionalLocationsPricing = _orderManagementService.GetAdditionalLocationsPricing(row.idWebinar);


                    JProperty createdByExpressCheckout = new JProperty(
                        JsonPropertyKeys.OrderCreatedByExpressCheckoutKey,
                        postback.Pretty);

                    expressOrder.AdminComments = JsonHelpers.AddObjectToJsonArray(expressOrder.AdminComments,
                        JsonPropertyKeys.OrderCreatedByExpressCheckoutKey, createdByExpressCheckout);

                    expressOrder.OrderStatus = OrderStatus.Submitted;
                    expressOrder.Origin = DomainConstants.OriginExpress;

                    //OrderGenesis og = OrderGenesis.CreatedViaExpressCheckout;

                    row.RegistrationType = regType;
                    row.idRegType = regType.idRegType;
                    _orderManagementService.CalculateOrderCost(expressOrder, additionalLocationsPricing);
                    _orderManagementService.SaveChanges();
                    _orderManagementService.FireOrderSubmittedEvent(expressOrder
                        , userCreatedByCheckout);


                    _orderManagementService.SaveChanges();
                }
                catch (Exception ex)
                {
                    _logger.FatalException("ExpressCheckout tossed exception: ", ex);
                }

            }
            else
            {
                _logger.Warn("ExpressCheckout webhook {0} order NOT FOUND: " + form.q11_orderid);

            }

        }

        [HttpPost]
        public ActionResult GenerateClickToJoinForAdHocCaller(GenerateClickToJoinViewModel generateClickToJoinViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (generateClickToJoinViewModel.OrderId.HasValue)
                    {
                        var orderRow =
                            _orderManagementService.GetOrderById(generateClickToJoinViewModel.OrderId.Value).OrderRows
                                .Single(or => or.RowStatus == OrderRowStatus.Active);

                        return Json(new { Result = WebUiConstants.Success, Code = orderRow.TtsJoinUrl });
                    }

                    var user = _membershipService.GetUserByEmail(generateClickToJoinViewModel.Email);

                    if (ReferenceEquals(null, user))
                    {
                        user = _membershipService.CreateBareUserFromEmail(generateClickToJoinViewModel.Email);
                        _orderManagementService.SaveChanges();
                    }
                    var newOrderRow = _orderManagementService.CreateOrderRow(null, null, 1);

                    newOrderRow.idWebinar = generateClickToJoinViewModel.SelectedWebinarId;
                    newOrderRow.idRegType = 1;
                    _orderManagementService.LoadWebinarIntoOrderRow(newOrderRow);


                    var affiliate = _stateService.GetValue<Affiliate>(WebUiConstants.CurrentAffiliate);
                    _orderManagementService.SetUserStatusToUnChanged(user);
                    //_orderManagementService.SetAffiliateStatusToUnChanged(affiliate);


                    _orderManagementService.CreateNewOrder(
                        _orderManagementService.GetAffiliateById(affiliate.idUserAff),
                        user,
                        newOrderRow.Webinar,
                        newOrderRow
                    );

                    _orderManagementService.SaveChanges();

                    return Json(new { Result = WebUiConstants.Success, Code = newOrderRow.TtsJoinUrl });
                }
                catch (DbEntityValidationException dbEntityValidationException)
                {
                    var stringBuilder = new StringBuilder();

                    foreach (var validationErrors in dbEntityValidationException.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            //Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName,validationError.ErrorMessage);
                            stringBuilder.AppendFormat("Property: {0} Error: {1} ", validationError.PropertyName,
                                validationError.ErrorMessage);
                        }
                    }
                    _logger.Error("GenerateClickToJoingForAdHocCaller dbEntityValidationException errors | {0}",
                        stringBuilder.ToString());
                }
            }

            return Json(new { Result = WebUiConstants.Fail });
        }

        [HttpPost]
        [ValidateJsonAntiForgeryToken(Order = 0)]
        [HandleAjaxException(Order = 1)]
        public PartialViewResult GetClaimsForUser(string email)
        {
            try
            {
                var userAccount = _membershipService.GetUserAccountByEmail(_globalConfig.Tenant, email);

                if (ReferenceEquals(userAccount, null))
                    return PartialView("~/Views/Admin/Partials/_ServerError.cshtml",
                        string.Format("There's no User in the system with the email {0}", email)
                    );

                var claimsViewModel = new ClaimsViewModel { UserClaims = userAccount.Claims };

                return PartialView("~/Views/Admin/Partials/_ViewClaims.cshtml", claimsViewModel);
            }
            catch (Exception exception)
            {
                _logger.ErrorException(string.Format("GetClaimsForUser. Session | {0}", _appHelper.GetUserAuditInfo()),
                    exception);
                return PartialView("~/Views/Admin/Partials/_ServerError.cshtml");
            }
        }

        public PartialViewResult GetAdditionalLocationByOrderId(int? id = null)
        {
            if (id.HasValue)
            {
                var order = _orderManagementService.GetOrderById(id.Value);
                var additionalLocations =
                    order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).AdditionalLocation.ToList();
                additionalLocations = _appHelper.CheckAdditionalLocationsForValidEmail(additionalLocations).ToList();

                var addAdditionalLocationViewModel = new AdditionalLocationOfferViewModel
                {
                    AdditionalLocations = additionalLocations,
                    OrderExists = true,
                    Emails = additionalLocations.Select(al => al.Email).ToList()
                };

                return PartialView(
                    "~/Views/Webinar/Partials/_AdditionalLocationsModal.cshtml",
                    addAdditionalLocationViewModel
                );
            }
            return null;
        }
        public ActionResult GetOrdersByTypeahead(int? id)
        {
            if (id.HasValue)
            {
                //var results = _orderManagementService.GetOrderIdsByPartialId(id.Value);
                var results = _orderManagementService.GetOrderIdsByPartialId(id.Value);
                //var userIds = _orderManagementService.GetUserIdsByPartialId(id.Value);


                return Json(new { results }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Error = WebUiConstants.NullValueParameter });
        }


        public ActionResult GetOrdersByEmailTypeahead(string email)
        {
            if (!string.IsNullOrWhiteSpace(email))
            {
                var aff = 0;
                var currentUser = User.Identity as ClaimsIdentity;

                if (currentUser.HasClaim(
                    (claim) => claim.Type == Business.Constants.ClaimTypes.Affiliate))
                {
                    aff = _membershipService.GetUserByEmail(User.Identity.Name).idUser;

                }


                var results = _orderManagementService.GetOrdersByEmail(email, aff).Select(o =>
                    new
                    {
                        id = o.WebUser.idUser,
                        billingEmail = o.WebUser.email,
                        firstName = o.WebUser.FirstName,
                        lastName = o.WebUser.LastName,
                        institution = o.Institution
                    }).Distinct();

                return Json(new { results }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Error = WebUiConstants.NullValueParameter });
        }


        [AllowAnonymous]
        public ActionResult GetOrdersByLastName(string lastName)
        {
            if (!string.IsNullOrWhiteSpace(lastName))
            {
                if (ClaimsAuthorization.CheckAccess(IdentityConstants.Access,
                    IdentityConstants.GetOrdersByLastNameFeature))
                {
                    var aff = 0;
                    var currentUser = User.Identity as ClaimsIdentity;

                    if (currentUser.HasClaim(
                        (claim) => claim.Type == Business.Constants.ClaimTypes.Affiliate))
                    {
                        aff = _membershipService.GetUserByEmail(User.Identity.Name).idUser;

                    }

                    var results = _orderManagementService.GetOrdersByLastName(lastName, aff)
                        .Select(o => new
                        {
                            //AffiliateName = GetAffiliateName(o.idAffiliate),
                            id = o.WebUser.idUser,
                            billingEmail = o.WebUser.email,
                            lastName = o.WebUser.LastName,
                            firstName = o.WebUser.FirstName,
                            institution = o.Institution
                        }).Distinct();


                    return Json(new { results }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { NotAuthorized = "true" });
            }

            return Json(new { Error = WebUiConstants.NullValueParameter });
        }

        public PartialViewResult ResendOrderConfirmation()
        {
            var model = new ResendOrderInformationViewModel
            {
                OrderId = string.Empty
            };

            return PartialView("~/Views/Admin/Home/_resendOrderConfirmation.cshtml", model);
        }

        [HttpPost]
        public ActionResult ResendOrderConfirmation(int orderId)
        {
            var order = _orderManagementService.GetOrderById(orderId);

            if (ReferenceEquals(null, order))
            {
                return Json(new { Result = WebUiConstants.Fail });
            }

            var orderRow = order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active);
            // perf bump by assigning to local variable

            if (string.IsNullOrEmpty(orderRow.CitrixJoinUrl) && orderRow.Webinar.CitrixJoinInfoAvailable())
            {
                _orderManagementService.GenerateRegistrantKey(order);

                if (orderRow.AdditionalLocation.Any())
                {
                    foreach (var additionalLocation in orderRow.AdditionalLocation)
                    {
                        _orderManagementService.GenerateRegistrantKey(order, additionalLocation);
                    }
                }
            }

            //TODO: orders that have been migrated are going to have the OrderGenisis over-written by FireOrderSubmittedEvent
            _orderManagementService.FireOrderSubmittedEvent(order, resending: true);

            return Json(new { Result = WebUiConstants.Success });
        }

        [HttpPost]
        public ActionResult ResendConnectionInfo(int orderId)
        {
            var order = _orderManagementService.GetOrderById(orderId);

            if (ReferenceEquals(null, order))
            {
                return Json(new { Result = WebUiConstants.Fail });
            }

            var orderRow = order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active);
            // perf bump by assigning to local variable

            _orderManagementService.FireSendConnectionInfoNotificationEvent(new[] { order }, resending: true);

            return Json(new { Result = WebUiConstants.Success });
        }

        [HttpPost]
        public ActionResult ResendPostEventMaterial(int orderId)
        {
            var order = _orderManagementService.GetOrderById(orderId);

            if (ReferenceEquals(null, order))
            {
                return Json(new { Result = WebUiConstants.Fail });
            }

            var orderRow = order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active);
            // perf bump by assigning to local variable

            _orderManagementService.FireSendConnectionInfoNotificationEvent(new[] { order }, resending: true);

            return Json(new { Result = WebUiConstants.Success });
        }

        public PartialViewResult SendAdhocEvent()
        {
            var model = new AdhocNotificationViewModel
            {
                NotificationBody = string.Empty,
                Webinars = EventInvokerHelpers.GetUpcomingWebinarsAsSelectListItems(_webinarManagementService)
            };

            return PartialView("~/Views/Admin/Home/_adHocNotification.cshtml", model);
        }

        [HttpPost]
        public ActionResult SendAdhocEvent(int webinarId)
        {
            var regTypes = EventInvokerHelpers.GetRegTypesForWebinarAsSelectListItems(webinarId,
                _webinarManagementService);

            return Json(regTypes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendAdhocNotification(string emails, string subject, string body)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _orderManagementService.SendAdhocNotification(emails, subject, body);
                    return Json(new { Result = WebUiConstants.Success });
                }
                catch (Exception exception)
                {
                    _logger.ErrorException("SendAdhocNotification action", exception);
                }

            }

            return this.ModelStateJson(ModelState);
        }

        public PartialViewResult SendReminder()
        {
            var model = new AdhocNotificationViewModel
            {
                Webinars = EventInvokerHelpers.GetUpcomingWebinarsAsSelectListItems(_webinarManagementService)
            };

            return PartialView("~/Views/Admin/Home/_SendReminder.cshtml", model);
        }

        [HttpPost]
        public JsonResult SendReminder(int webinarId)
        {
            var orders = _orderManagementService.GetOrdersForLiveNotifications(webinarId);

            if (orders.Any())
            {
                _orderManagementService.FireSendReminderNotificationEvent(orders);

                return Json(new { Result = WebUiConstants.Success });
            }

            return Json(new { Result = WebUiConstants.NoOrdersForWebinar });
        }

        public PartialViewResult SendConnectionInfo()
        {
            var model = new AdhocNotificationViewModel
            {
                Webinars = EventInvokerHelpers.GetUpcomingWebinarsAsSelectListItems(_webinarManagementService)
            };

            return PartialView("~/Views/Admin/Home/_SendConnectionInfo.cshtml", model);
        }

        [HttpPost]
        public JsonResult SendConnectionInfo(int webinarId)
        {
            //_logger.Info("Begins SendConnectionInfo");
            var orders = _orderManagementService.GetOrdersForLiveNotifications(webinarId);
            var citrixRegistrants = _orderManagementService.GetCitrixRegistrantsByWebinar(webinarId);
            foreach (var order in orders)
            {
                var orderRow = order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active);
                // perf bump by assigning to local variable
                //need to refine logic in the CitrixJoinInfoAvailable a little bit.
                if (string.IsNullOrEmpty(orderRow.CitrixJoinUrl)) // && orderRow.Webinar.CitrixJoinInfoAvailable())
                {
                    _orderManagementService.GenerateRegistrantKey(order);

                    if (orderRow.AdditionalLocation.Any())
                    {
                        foreach (var additionalLocation in orderRow.AdditionalLocation)
                        {
                            _orderManagementService.GenerateRegistrantKey(order, additionalLocation);
                        }
                    }
                }
            }

            orders.ToList().ForEach((order) =>
            {
                var notificationStorage = new NotificationStorage
                {
                    idOrder = order.idOrder,
                    SessionStartInfo = _appHelper.GetSessionStartInfo()
                };

                if (string.IsNullOrWhiteSpace(order.NotificationStorage))
                {
                    order.NotificationStorage = JsonConvert.SerializeObject(notificationStorage);
                }
            });

            _orderManagementService.FireSendConnectionInfoNotificationEvent(orders, resending: false);

            var numRows = _orderManagementService.SaveChanges();

            _logger.Info("Concludes SendConnectionInfo. {0} rows updated.", numRows);
            return Json(new { Result = WebUiConstants.Success });
        }


        public PartialViewResult SendShippedOrder()
        {
            var ordersShipped = EventInvokerHelpers.GetShippedWebinarsAsSelectListItems(_orderManagementService);

            var model = new AdhocNotificationViewModel
            {
                OrdersList = ordersShipped
            };

            return PartialView("~/Views/Admin/Home/_SendShippedOrder.cshtml", model);
        }

        [HttpPost]
        public JsonResult SendShippedOrder(int orderId)
        {
            var order = _orderManagementService.GetOrderById(orderId);

            _orderManagementService.FireSendOrderShippedNotificationEvent(new Order[] { order });

            return Json(new { Result = WebUiConstants.Success });
        }


        public ActionResult PreviewShippedOrder(int id)
        {
            var order = _orderManagementService.GetOrderById(id);

            if (ReferenceEquals(null, order))
                return File("<html>fail</html>".GenerateStreamFromString(), HtmlMimeType);


            var formatter = new PreviewFormatter(new EnvironmentInformation { BaseUrl = HttpRuntime.AppDomainAppPath });

            return File(formatter.FormatToString(order, "PreviewShippedOrder").GenerateStreamFromString(), HtmlMimeType);
        }

        public ActionResult PreviewRecordingPosted(int id)
        {
            //  id is a WebinarId
            var firstRetrievedOrderForWebinar = _orderManagementService.GetOrdersForRecordedNotifications(id)
                .FirstOrDefault(
                    o =>
                        o.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active)
                            .RegistrationType.ShowLiveNotifications.ToLower() == "yes"
                );

            if (ReferenceEquals(null, firstRetrievedOrderForWebinar))
                return File("<html>fail</html>".GenerateStreamFromString(), HtmlMimeType);


            var formatter = new PreviewFormatter(new EnvironmentInformation { BaseUrl = HttpRuntime.AppDomainAppPath });

            return
                File(
                    formatter.FormatToString(firstRetrievedOrderForWebinar, "PreviewRecordingPosted")
                        .GenerateStreamFromString(), HtmlMimeType);
        }

        public ActionResult PreviewConnectionInfo(int id)
        {
            //  id is a WebinarId
            var firstRetrievedOrderForWebinar = _orderManagementService.GetOrdersForLiveNotifications(id)
                .FirstOrDefault(
                    o =>
                        o.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active)
                            .RegistrationType.ShowLiveNotifications.ToLower() == "yes"
                );

            if (ReferenceEquals(null, firstRetrievedOrderForWebinar))
                return File("<html>fail</html>".GenerateStreamFromString(), HtmlMimeType);

            var formatter = new PreviewFormatter(new EnvironmentInformation { BaseUrl = HttpRuntime.AppDomainAppPath });

            return
                File(
                    formatter.FormatToString(firstRetrievedOrderForWebinar, "PreviewConnectionInfo")
                        .GenerateStreamFromString(), HtmlMimeType);
        }

        [ClaimsAuthorize(IdentityConstants.Access, IdentityConstants.ImpersonateFeature)]
        public PartialViewResult LogInAsUser()
        {
            var logInAsOtherUserViewModel = new LogInAsOtherUserViewModel
            {
                Email = string.Empty,
                Password = string.Empty
            };

            return PartialView("~/Views/Admin/Home/_LogInAsUser.cshtml", logInAsOtherUserViewModel);
        }

        [ValidateInput(false)]
        [HttpPost]
        public JsonResult WritePromoToStorage(WebinarPromoViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.EventBody))
            {
                // TODO: zzz ALS think about this.  It shouldn't ever happen, but do we want/need to know if it does?
                return Json(new
                {
                    result = WebUiConstants.Success,
                    returnMessage = "" // could return a message that would show in alert on client
                });
            }

            var timeString = GetAffiliateTimeZoneAndList(model.Affiliate.idUserAff, model.Webinar.idWebinar).ToString();

            model.EventBody = model.EventBody.Replace("{timeString}", timeString);
            string eventBodyText = System.Uri.UnescapeDataString(model.EventBody);

            //GeneratePromoDocuments(model);

            var storageCredentials = new StorageCredentials(_globalConfig.StorageAccountName,
                _globalConfig.StorageAccessKey);
            var cloudStorageAccount = new CloudStorageAccount(storageCredentials, false);
            CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();

            // Retrieve reference to desired Azure storage container.
            string containerRoot = "v3generator";
            // could also be configuration-driven, so we could have different spots for dev/qa, etc. if we wanted
            CloudBlobContainer container = blobClient.GetContainerReference(containerRoot);
            container.CreateIfNotExists();

            string strAffId = model.Affiliate.idUserAff.ToString();
            string filenameBase = string.Concat(strAffId, "/", model.Webinar.idWebinar + "_promo");

            List<string> hrefsForEmail = new List<string>();

            // Create the blobs
            string filename = "";
            string ret = "";
            bool overwriteFlag = true;
            // do we want to think of a way to let the user tell us we should (or should not) overwrite?

            // html file
            filename = string.Concat(filenameBase, ".html");
            byte[] byteArrayHTML = Encoding.UTF8.GetBytes(eventBodyText);
            // "full" markup from WIJMO editor, may want to add doctype and body tags...

            ret += UploadToAzure(container, filename, byteArrayHTML, overwriteFlag);
            if (string.IsNullOrWhiteSpace(ret)) // no error, add to list for email
                hrefsForEmail.Add(string.Format("https://siteroot/mypromos/{0}/{1}", containerRoot, filename));

            return Json(new
            {
                result = (string.IsNullOrWhiteSpace(ret) ? WebUiConstants.Success : WebUiConstants.Fail),
                returnMessage = ret
            });
        }


        private string UploadToAzure(CloudBlobContainer container, string filename, byte[] content,
            bool overwriteIfExists = true)
        {
            string ret = "";

            using (var memoryStream = new MemoryStream(content))
            {

                CloudBlockBlob blob = container.GetBlockBlobReference(filename);
                if (overwriteIfExists ||
                    !blob.Exists())
                {
                    blob.UploadFromStream(memoryStream);
                    _logger.Info("Promo named: {0}", filename);
                }
                else
                {
                    ret = string.Format("File exists and overwrite flag was false ({0}).\r\n\r\n", filename);
                    _logger.Info(ret);
                }
            }

            return ret;
        }


        [HttpPost]
        public async Task<JsonResult> GetAffiliateTimeZoneAndList(int idAffiliate, int idWebinar)
        {
            var idMailChimpList = _affiliateManagementService.FindById(idAffiliate).idMailChimpList;
            var listName = "";
            if (idMailChimpList != null)
            {
                IMailChimpManager manager = new MailChimpManager("9e623830aa054e8fc5b2bf18473d482f-us10");
                List list = await manager.Lists.GetAsync(idMailChimpList).ConfigureAwait(false);
                listName = list.Name;
            }
            Webinar webinar = _webinarManagementService.GetWebinar(idWebinar);
            var timeZone = _affiliateManagementService.FindById(idAffiliate).WebUser.timeZone;
            var TimeFormatDisplay = "<i>" + DateTimeHelper.FormatTime(webinar.Date, timeZone, false) +
                          " - " +
                          DateTimeHelper.FormatTime(
                              webinar.Date.AddHours((double)webinar.Duration), timeZone, true) + "<br /></i>";
            return Json(new { timeFormatDisplay = TimeFormatDisplay, listName = listName });
        }


        [HttpPost]
        public JsonResult GeneratePromo(WebinarPromoViewModel model)
        {

            if (model.TemplateType == "Daily")
            {
                model.Webinar = _webinarManagementService.GetWebinar(model.Webinar.idWebinar);
                model.SubscriptionPackURL = "http://ttstrain.com/webinar-subscription-packages-for-credit-unions/";
                if (model.Affiliate.idUserAff != null && model.Affiliate.idUserAff > 0)
                {
                    model.Affiliate = _affiliateManagementService.FindById(model.Affiliate.idUserAff);
                    model.TimeZone = model.Affiliate.WebUser.timeZone;

                }
                if (_globalConfig.Tenant == "BankWebinars")
                    model.SubscriptionPackURL = "http://ttstrain.com/webinar-subscription-packages-for-banks/";

                model.BasePrice = "$265";
                if (model.Webinar.Duration == 1)
                    model.BasePrice = "$165";
                if ((double)model.Webinar.Duration == 1.5)
                    model.BasePrice = "$195";

                // populate the dropdown selector (not currently implemented)
                TempData["ListOfWebinarsForUpcoming"] =
                    _webinarManagementService.GetUpcomingWebinars().OrderBy(w => w.Date).Take(10).ToList();


                if (model.ListOfWebinarsForUpcoming == null)
                {
                    model.ListOfWebinarsForUpcoming =
                        _webinarManagementService.GetUpcomingWebinars()
                            .OrderBy(w => w.Date)
                            .Take(10)
                            .Select(w => w.idWebinar)
                            .ToArray();
                }

                var _upcoming =
                    _webinarManagementService.GetUpcomingWebinars()
                        .Where(w => w.Date > model.SendDate && w.idWebinar != model.Webinar.idWebinar)
                        .OrderBy(w => w.Date)
                        .Take(5);


                var doc = new HtmlDocument();
                doc.LoadHtml(model.Webinar.Presenter.BiographyLong);

                var root = doc.DocumentNode;
                root.SelectSingleNode("(//img)[1]").Remove();

                model.PresenterW_OutPic = root.InnerHtml;

                var upcomingWebinars = (from w in _upcoming
                                        select
                                        "<p style=\"color: whitesmoke; text-decoration: none; \" ><a style=\" color: whitesmoke; border-bottom: 1px dotted bisque;\" href=\"" +
                                        _globalConfig.TenantURL + "/Webinar/Details/" +
                                        w.idWebinar + "?idaff={aff_idUserAff}\">" + w.Title + "</a><br>" +
                                        w.Date.ToLongDateString() + "</p>"
                ).ToArray();

                if (upcomingWebinars.Count() > 0)
                    model.ListOfWebinarsUpcomingRendered = String.Join("\r\n", upcomingWebinars);

                model.TimeFormatDisplay = "<i>" + DateTimeHelper.FormatTime(model.Webinar.Date, model.TimeZone, false) +
                                          " - " +
                                          DateTimeHelper.FormatTime(
                                              model.Webinar.Date.AddHours((double)model.Webinar.Duration),
                                              model.TimeZone,
                                              true) + "<br /></i>";

                model.EventBody =
                    HttpUtility.HtmlDecode(
                        _generalFormatter.FormatV2(model, "~/Notification/Templates/SendPerDayPromoMaster.cshtml").Body);
                // get Template with new method

                return Json(new { masterText = model.EventBody });
            }
            else
            {
                List<int> featureWebinarIDs = new List<int>();
                var upcomingDetail = new StringBuilder();
                if (TempData["ListOfWebinarsForWeekly"] != null)
                {
                    foreach (var webinar in (IList<Webinar>)TempData["ListOfWebinarsForWeekly"])
                    {
                        featureWebinarIDs.Add(webinar.idWebinar);

                        string eDate = "<b>" + DateTimeHelper.FormatDate(webinar.Date) + " - " +
                                       DateTimeHelper.FormatTimeWithDuration(webinar.Date, model.TimeZone, false,
                                           webinar.Duration) + "</b><br />";

                        upcomingDetail.Append(
                            "<p><span style='font-size:20px; font-weight:bold; font-family:trebuchet ms;'><a href=\"" +
                            _globalConfig.TenantURL + "/Webinar/Details/" + webinar.idWebinar +
                            "?idaff={aff_idUserAff}\">" + webinar.Title + "</a></span><br />");
                        upcomingDetail.Append("<span style='font-family:trebuchet ms;'><b>" +
                                              webinar.Presenter.WebUser.FullName + "</b><br /></span>");
                        upcomingDetail.Append("<span style='font-size:12px;'>" + eDate + "</span></p>");
                        upcomingDetail.Append("<div style='font-family:trebuchet ms;'>" + webinar.Description + "</div>");

                        upcomingDetail.Append(
                            "<p font-family:trebuchet ms;'><a href='" + _globalConfig.TenantURL + "/Webinar/Details/" +
                            webinar.idWebinar + "?idaff={aff_idUserAff}'>Click here for more info!</a></p>");
                        upcomingDetail.Append("<hr style='width:50%; ' />");
                    }
                }
                var q = _webinarManagementService.GetUpcomingWebinars()
                    .Where(a => a.idWebinar != model.Webinar.idWebinar && a.Date > model.SendDate)
                    .Where(
                        a => !(featureWebinarIDs.Any(item2 => item2 == a.idWebinar)))
                    .OrderBy(a => a.Date)
                    .Take(12)
                    .Select(a =>
                        new
                        {
                            featuredItem =
                            "<p style=\"color: whitesmoke; text-decoration: none; \"><a style=\"color: whitesmoke; \" href=\"" +
                            _globalConfig.TenantURL + "/Webinar/Details/" +
                            a.idWebinar + "?idaff={aff_idUserAff}\">" + a.Title + "</a><br>" +
                            a.Date.ToLongDateString() + "</p>"
                        });
                int i1 = 0;
                var upcoming = new StringBuilder();
                foreach (var ID in q)
                {
                    if (i1 < 5)
                    {
                        upcoming.Append(ID.featuredItem.ToString());
                    }
                    i1++;
                }
                model.ListOfWebinarsUpcomingRendered = "" + upcoming.ToString();

                model.EventBody = upcomingDetail.ToString();

                model.EventBody =
                    HttpUtility.HtmlDecode(
                        _generalFormatter.FormatV2(model, "~/Notification/Templates/SendPerWeekPromoMaster.cshtml").Body);

                return Json(new { masterText = model.EventBody });
            }
            //return null;
        }

        private void GeneratePromoDocuments(WebinarPromoViewModel model)
        {
            var webinar = _webinarManagementService.GetWebinar(model.Webinar.idWebinar);
            model.Webinar = webinar;
            model.Affiliate = _affiliateManagementService.FindById(model.Affiliate.idUserAff);
            model.Affiliate.WebUser = _membershipService.GetWebUserById(model.Affiliate.idUserAff);
            model.TimeZone = model.Affiliate.WebUser.timeZone;
            model.Webinar.Presenter.WebUser = _membershipService.GetWebUserById(model.Webinar.idPresenter);

            var doc = new HtmlDocument();
            doc.LoadHtml(model.Webinar.Presenter.BiographyLong);
            var root = doc.DocumentNode;
            root.SelectSingleNode("(//img)[1]").Remove();
            double bodyWidth = 0;
            double sideBarWidth = 0;
            model.PresenterW_OutPic = root.InnerHtml;

            doc.LoadHtml(HttpUtility.UrlDecode(model.EventBody));


            var _upcoming =
                _webinarManagementService.GetUpcomingWebinars()
                    .Where(w => w.Date > model.SendDate && w.idWebinar != model.Webinar.idWebinar)
                    .OrderBy(w => w.Date)
                    .Take(5);

            var upcomingWebinars = (from w in _upcoming
                                    select
                                    "<p style=\"color: whitesmoke; text-decoration: none; \" ><a style=\" color: whitesmoke; border-bottom: 1px dotted bisque;\" href=\"" +
                                    _globalConfig.TenantURL + "/Webinar/Details/" +
                                    w.idWebinar + "?idaff=" + model.Affiliate.idUserAff + "\">" + w.Title + "</a><br>" +
                                    w.Date.ToLongDateString() + "</p>").ToArray();

            if (upcomingWebinars.Count() > 0)
                model.ListOfWebinarsUpcomingRendered = String.Join("\r\n", upcomingWebinars);

            model.TimeFormatDisplay = "" + DateTimeHelper.FormatDate(webinar.Date) + "<br>" +
               DateTimeHelper.FormatTimeWithDuration(webinar.Date, model.TimeZone, false,
                   webinar.Duration);

            model.BasePrice = "$265";
            if (model.Webinar.Duration == 1)
                model.BasePrice = "$165";
            if ((double)model.Webinar.Duration == 1.5)
                model.BasePrice = "$195";
            if (model.TemplateType == "")
            {
                model.Webinar.Description = doc.DocumentNode.SelectSingleNode("//*[@id='bodyLeft']").InnerHtml;
            }
            var bodyRight = "<h3 style=\"color: whitesmoke\">Upcoming Webinars</h3>" + model.ListOfWebinarsUpcomingRendered;
            bodyRight += "<p style=\"color: whitesmoke\" align=\"center\"><b>OnDemand Webinars Available</b></p>";
            bodyRight += "<p style=\"color: whitesmoke\">Unable to attend the live session, or interested in a topic of a past webinar? Not a problem. Get the recording that includes online access to the webinar for six months, you can even add a CD-ROM and materials for offline viewing. </p>";
            bodyRight += "<p style=\"color: whitesmoke\" align=\"center\"><b>Webinar Subscription Packages</b></p>";
            bodyRight += "<p style=\"color: whitesmoke\">Would you and your colleagues like to attend webinars at a lower price?&nbsp; With a Webinar Subscription Package, we can help you greatly reduce that expense.&nbsp; </p>";

            model.BodyRight = bodyRight;

            TableRow insetRow;
            var document = GemBoxHelpers.BuildPromoReplica(model);

            CloudBlockBlob blob;
            var container = BuildCloudBlobContainer(model, out blob);

            using (MemoryStream output = new MemoryStream())
            {
                document.Save(output, SaveOptions.DocxDefault);
                output.Position = 0; // reset to beginning so Upload operation can work correctly
                blob.UploadFromStream(output);

                blob = container.GetBlockBlobReference(string.Concat(model.Affiliate.idUserAff, "/", model.Webinar.idWebinar) + ".pdf");
                document.Save(output, SaveOptions.PdfDefault);
                output.Position = 0; // reset to beginning so Upload operation can work correctly
                blob.UploadFromStream(output);

                blob = container.GetBlockBlobReference(string.Concat(model.Affiliate.idUserAff, "/", model.Webinar.idWebinar) + ".txt");
                document.Save(output, SaveOptions.TxtDefault);
                output.Position = 0; // reset to beginning so Upload operation can work correctly
                blob.UploadFromStream(output);
            }

            var bodyLeft = "<h1 style =\"font-size: 18px;\" align=\"center\">" + model.Webinar.Title + "</h1>";
            bodyLeft += "<h3 align=\"center\"><b>A web-based Seminar<br /></b></h3>";
            bodyLeft += model.TimeFormatDisplay;
            bodyLeft += "<p><b>Recommended for" + model.CEUValue + " CE Credits</b></p>";
            bodyLeft += "<b>Program Content: </b>" + model.Webinar.Description;
            bodyLeft += "<h3>" + model.Webinar.LearnCaption + "</h3>";
            bodyLeft += model.Webinar.LearnBody;
            bodyLeft += "<h3>Who Should Attend</h3>";
            bodyLeft += model.Webinar.WhoAttend;
            bodyLeft += "<h3>" + model.Webinar.Presenter.WebUser.FullName + "</h3>";
            bodyLeft += model.PresenterW_OutPic;
            bodyLeft += "<h3>Cancellation Policy:</h3><p>Refunds will be given only for cancellations received in written form 3 business days prior to the program.  If your bank is unable to participate after registering, you can also select to receive an OnDemand website link to see the information online of the seminar at no additional charge.</p><p><b>If you are unable to attend the webinar but would like to have this information for training purposes, you may also purchase an OnDemand website link and/or CD-ROM. </b></p>";

            //create graphic for right side https://jsfiddle.net/justSteve/8mp40fbu/6/

            document = GemBoxHelpers.BuildPromoForms(model, bodyLeft, bodyRight);

            container = BuildCloudBlobContainer(model, out blob);

            blob =
                container.GetBlockBlobReference(string.Concat(model.Affiliate.idUserAff, "/", model.Webinar.idWebinar) + "_form.docx");

            using (MemoryStream output = new MemoryStream())
            {
                document.Save(output, SaveOptions.DocxDefault);
                output.Position = 0; // reset to beginning so Upload operation can work correctly
                blob.UploadFromStream(output);

                blob = container.GetBlockBlobReference(string.Concat(model.Affiliate.idUserAff, "/", model.Webinar.idWebinar) + "_form.pdf");
                document.Save(output, SaveOptions.PdfDefault);
                output.Position = 0; // reset to beginning so Upload operation can work correctly
                blob.UploadFromStream(output);

                blob = container.GetBlockBlobReference(string.Concat(model.Affiliate.idUserAff, "/", model.Webinar.idWebinar) + "_form.txt");
                document.Save(output, SaveOptions.TxtDefault);
                output.Position = 0; // reset to beginning so Upload operation can work correctly
                blob.UploadFromStream(output);
            }

        }


        private CloudBlobContainer BuildCloudBlobContainer(WebinarPromoViewModel model, out CloudBlockBlob blob)
        {
            var storageCredentials = new StorageCredentials(_globalConfig.StorageAccountName,
                _globalConfig.StorageAccessKey);

            var cloudStorageAccount = new CloudStorageAccount(storageCredentials, false);
            CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference("v3generator");
            container.CreateIfNotExists();

            blob =
                container.GetBlockBlobReference(string.Concat(model.Affiliate.idUserAff, "/", model.Webinar.idWebinar) + ".docx");
            return container;
        }

        [HttpPost]
        public ActionResult PerWeekPromo(WebinarPromoViewModel model)
        {
            IList<Webinar> webinars = new List<Webinar>();
            //

            var listOfIds = model.sListOfWebinarsForWeekly.Split(',');
            foreach (var _id in listOfIds)
            {
                webinars.Add(_webinarManagementService.GetWebinar(Convert.ToInt32(_id)));
            }
            TempData["ListOfWebinarsForWeekly"] = webinars;

            //
            TempData["ListOfWebinarsForUpcoming"] =
                _webinarManagementService.GetUpcomingWebinars().OrderBy(w => w.Date).Take(10).ToList();
            model.SendDate = TtsConfig.UtcNowAsCts;
            model.Webinar = _webinarManagementService.GetWebinar(Convert.ToInt32(Request["idWebinar"]));
            model.TimeZone = USTimeZone.Central;
            model.Affiliates = new AffiliateRepository().GetAffiliatesByPromoType("Weekly").ToList();
            model.TemplateType = "Weekly";

            return View(model);

        }

        [HttpGet]
        public async Task<ActionResult> PerDayPromo(int id)
        {

            var webinarsForUpcoming = _webinarManagementService.GetUpcomingWebinars().Take(15).OrderBy(w => w.Date);

            TempData["ListOfWebinarsForWeekly"] = webinarsForUpcoming;

            WebinarPromoViewModel model = new WebinarPromoViewModel()
            {
                TimeZone = USTimeZone.Eastern,
                SendDate = TtsConfig.UtcNowAsCts,
                Webinar = _webinarManagementService.GetWebinar(id),
                TemplateType = "Daily"
            };


            // check for Admins vs Affiliates and limit if Affiliate
            ClaimsIdentity claimsIdentityOfAuthenticatedUser = (ClaimsIdentity)User.Identity;
            if (claimsIdentityOfAuthenticatedUser.HasClaim((claim) => claim.Type == Business.Constants.ClaimTypes.Admin))
            {
                model.Affiliates = new AffiliateRepository().GetAffiliatesByPromoType("Daily").ToList();
            }
            else
            {
                // limit affiliates (non-admins)...
                int affId = -1;
                var affiliate = _stateService.GetValue<Affiliate>(WebUiConstants.CurrentAffiliate);
                if (affiliate != null)
                {
                    model.Affiliate = affiliate; // used on View to manipulate displayed verbiage
                    affId = affiliate.idUserAff;
                }

                model.Affiliates = new AffiliateRepository().GetAffiliatesByPromoType("Daily")
                    .Where(a => a.idUserAff == affId).ToList();
            }
            return View(model);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult SendSinglePromo(int affiliateId, string messageBodyHtml, int webinarId, string subject)
        // WebinarPromoViewModel model
        {

            var timeString = GetAffiliateTimeZoneAndList(affiliateId, webinarId).ToString();

            WebinarPromoViewModel model = new WebinarPromoViewModel
            {
                Affiliate = _affiliateManagementService.FindById(affiliateId),
                EventBody = messageBodyHtml.Replace("{timeString}", timeString),
                Webinar = _webinarManagementService.GetWebinar(webinarId),
                Subject = subject
            };

            if (ModelState.IsValid)
            {
                try
                {
                    _orderManagementService.FireSendPerDayPromoEvent(model);
                    return Json(new { Result = WebUiConstants.Success });
                }
                catch (Exception exception)
                {
                    _logger.ErrorException("SendSinglePromo action", exception);
                }
            }
            return this.ModelStateJson(ModelState);
        }


        [ValidateInput(false)]
        [HttpPost]
        public async Task<JsonResult> CreateCampaignSingle(int affiliateId, string messageBodyHtml, int webinarId, string sendDate, string sendTime)
        {

            Webinar webinar = _webinarManagementService.GetWebinar(webinarId);
            Affiliate affiliate = _affiliateManagementService.FindById(affiliateId);
            sendDate = sendDate.Split('/')[2] + "-" + sendDate.Split('/')[0] + "-" + sendDate.Split('/')[1] + "T" + sendTime + ":00:00" + (int)affiliate.WebUser.timeZone;//"T10:00:00-05:00";

            McCampaign campaign = new McCampaign { AffiliateId = affiliate.idUserAff, WebinarId = webinar.idWebinar };

            IMailChimpManager manager = new MailChimpManager("9e623830aa054e8fc5b2bf18473d482f-us10");

            List list = await manager.Lists.GetAsync(affiliate.idMailChimpList).ConfigureAwait(false);

            var recp = new Recipient { ListId = affiliate.idMailChimpList };

            var seg = recp.SegmentText;
            //https://github.com/brandonseydel/MailChimp.Net/issues/157
            // or find a way to nav to URL

            var newCamp = new Campaign
            {
                ContentType = "html",
                Type = CampaignType.Regular,
                Recipients = recp,

                Settings = new Setting
                {
                    SubjectLine = "Webinar: " + webinar.Title,
                    Title = affiliate.ttsDomain + "_" + webinar.Title,
                    FolderId = "30aca5892b",
                    InlineCss = true,
                    Authenticate = true,
                    AutoFooter = true,
                    AutoTweet = false,
                    ToName = "*|FNAME|* *|LNAME|* ",
                    FromName = list.CampaignDefaults.FromName,
                    ReplyTo = list.CampaignDefaults.FromEmail,
                    UseConversation = true,
                },
                Tracking = new Tracking
                {
                    GoogleAnalytics = affiliate.idUserAff + "_" + webinar.idWebinar + "_" + sendDate,
                    HtmlClicks = true,
                    TextClicks = true,
                    Opens = true
                }

            };
            try
            {
                var mkCamp = await manager.Campaigns.AddAsync(campaign: newCamp);
                campaign.CampaignId = mkCamp.Id;
                var content = new Content
                {
                    Html = messageBodyHtml
                };

                _logger.Info("CreateCampaign | mkCamp.Id: " + mkCamp.Id);
                var putContent = await manager.Content.AddOrUpdateAsync(mkCamp.Id, new ContentRequest
                {
                    Html = messageBodyHtml
                });


                _logger.Info("CreateCampaign | SendChecklistAsync: " + mkCamp.Id);
                var checkList = await manager.Campaigns.SendChecklistAsync(mkCamp.Id);

                _logger.Info("CreateCampaign | sendDate: " + mkCamp.Id);
                await manager.Campaigns.ScheduleAsync(mkCamp.Id, new CampaignScheduleRequest
                {
                    Timewarp = false,
                    BatchDelivery = new BatchDelivery
                    {
                        Count = 2,
                        Delay = 15
                    },
                    ScheduleTime = sendDate
                });

                CampaignTestRequest emails = new CampaignTestRequest
                {
                    EmailType = "html",
                    Emails = new string[] { "all.of.us@ttstrain.com", affiliate.NotiPromos }
                };

                await manager.Campaigns.TestAsync(mkCamp.Id, emails);

                var campMsg = new JProperty(JsonPropertyKeys.MailChimpCampaign, JsonConvert.SerializeObject(campaign, Formatting.None));
                _logger.Info("Webinar.Campaigns += " + campMsg);
                webinar.Campaigns = JsonHelpers.MergeJsonWithStoredField(webinar.Campaigns, campMsg);

                _webinarManagementService.SaveChanges();




                return Json(new { success = "success" });

            }
            catch (Exception exception)
            {
                _logger.FatalException("CreateCampaignSingle: ", exception);
                return Json(new { error = exception.Message });
            }
        }

        public ActionResult RssFeedOfAddedEvents()
        {
            //http://office.microsoft.com/en-us/office365-sharepoint-online-small-business-help/basic-tasks-in-sharepoint-online-for-office-365-for-professionals-and-small-businesses-HA101988906.aspx#_Toc272147708

            string strFeed =
                "https://totaltrainingsolutions-public.sharepoint.com/_layouts/15/listfeed.aspx?List={09364DB1-2255-406E-9CB6-45FE64C0D341}";

            using (XmlReader reader = XmlReader.Create(strFeed))
            {
                SyndicationFeed rssData = SyndicationFeed.Load(reader);

                return PartialView(rssData);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogInAsUser(LogInAsOtherUserViewModel model)
        {
            model.Email = model.Email.Trim();
            if (ModelState.IsValid)
            {
                var adminUser = User.Identity as ClaimsIdentity;
                var adminUserEmail =
                    adminUser.Claims.Single(c => c.Type == System.IdentityModel.Claims.ClaimTypes.Email).Value;
                var impersonatedUserAccount = _membershipService.GetUserAccountByEmail(_globalConfig.Tenant, model.Email);

                if (ReferenceEquals(null, impersonatedUserAccount))
                {
                    var nullReferenceException =
                        new NullReferenceException(string.Format("There is no webuser with the email address {0}",
                            model.Email));
                    _logger.ErrorException("LogInAsUser | No User Found For Email", nullReferenceException);
                    ModelState.AddModelError(string.Empty, nullReferenceException);
                    return View("", "", "");
                }
                var dataOperations =
                    new DataOperations(ConfigurationManager.ConnectionStrings["MembershipReboot"].ConnectionString);

                dataOperations.RemoveImpersonatedClaimsByCurrentAdmin(adminUserEmail);


                _membershipService.AddClaim(impersonatedUserAccount, Business.Constants.ClaimTypes.BeingImpersonated,
                    impersonatedUserAccount.Email + "|" + adminUserEmail);
                //adminUserEmail);
                _membershipService.LogOutUser();

                if (_membershipService.LogInAdminUserAsOtherUser(_globalConfig.Tenant,
                    adminUserEmail.Trim(), model.Password.Trim(),
                    impersonatedUserAccount
                ))
                {
                    return RedirectToAction("Index", "Home");
                }

                return RedirectToRoute("Default", new { Action = "IncorrectPassword", Controller = "StaticContent" });
            }

            return RedirectToAction("Index", "Home");
        }

        public PartialViewResult ManualPasswordReset()
        {
            var manualPasswordResetViewModel = new ManualPasswordResetViewModel
            {
                ConfirmPassword = string.Empty,
                Email = string.Empty,
                NewPassword = string.Empty
            };

            return PartialView("~/Views/Admin/Home/_ManualPasswordReset.cshtml", manualPasswordResetViewModel);

        }



        [HttpPost]
        [ValidateAntiForgeryToken(Order = 0)]
        [HandleAjaxException(Order = 1)]
        public ActionResult ManualPasswordReset(ManualPasswordResetViewModel model)
        {
            model.Email = model.Email.Trim();
            if (ModelState.IsValid)
            {
                var userAccount = _membershipService.GetUserAccountByEmail(_globalConfig.Tenant, model.Email.Trim());

                try
                {
                    if (userAccount.HasClaim(Business.Constants.ClaimTypes.HasNotVerified))
                    {
                        _membershipService.RemoveClaim(_globalConfig.Tenant, model.Email,
                            Business.Constants.ClaimTypes.HasNotVerified);

                    }
                }
                catch (Exception exception)
                {
                    _logger.Error("ManualPasswordReset did not find existing account: {0}",
                        _appHelper.GetUserAuditInfo());
                    _logger.Error("ManualPasswordReset did not find existing account: {0}", exception.Message);
                }

                try
                {
                    _membershipService.CleanUser(_globalConfig.Tenant, model.Email, model.NewPassword);
                    _logger.Info("Manual Password Reset for {0} by: {1}. ", model.Email, _appHelper.GetUserAuditInfo());

                    var resetKey = _globalConfig.TenantURL + "/Account/PasswordResetConfirm/" +
                                   userAccount.VerificationKey;

                    return Json(new { Result = WebUiConstants.Success, ResetKey = resetKey });

                }
                catch (NullReferenceException nullReferenceException)
                {
                    if (nullReferenceException.Message.Equals(DomainConstants.UserNotFound))
                        return Json(new { Result = WebUiConstants.InvalidEmail });
                    ModelState.AddModelError(string.Empty, "Reset Failed");
                    _logger.Error("ManualPasswordReset: {0}", _appHelper.GetUserAuditInfo());
                    _logger.Error("ManualPasswordReset: {0}", nullReferenceException.Message);
                }
                catch (Exception exception)
                {
                    _logger.Error("ManualPasswordReset: {0}", _appHelper.GetUserAuditInfo());
                    _logger.Error("ManualPasswordReset: {0}", exception.Message);
                }
            }
            _logger.Error("Invalid ManualPasswordResetViewModel {0}", _appHelper.GetUserAuditInfo());
            return this.ModelStateJson(ModelState);
        }

        [AllowAnonymous]
        public PartialViewResult PasswordResetOperation()
        {
            var resetPasswordModel = new ResetPasswordModel
            {
                Email = string.Empty,
                EmailSent = false
            };

            return PartialView("~/Views/Admin/Home/_ResetPasswordPartial.cshtml", resetPasswordModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult ResetPassword(string email)
        {
            var globals = GlobalConfig.GlobalConfigSingleton;
            _logger.Info("Resetting password for: {0}", email);

            var dataOperations =
                new DataOperations(ConfigurationManager.ConnectionStrings["MembershipReboot"].ConnectionString);


            try
            {
                var userAccount = _membershipService.GetUserAccountByEmail(_globalConfig.Tenant, email);
                if (userAccount.HasClaim(Business.Constants.ClaimTypes.HasNotVerified))
                {
                    _membershipService.RemoveClaim(_globalConfig.Tenant, email,
                        Business.Constants.ClaimTypes.HasNotVerified);

                }
                dataOperations.SetFieldsConsistantWithVerifiedUser(userAccount);

                _membershipService.ResetPassword(globals.Tenant, email);

                var resetKey = globals.TenantURL + "/Account/PasswordResetConfirm/" + userAccount.VerificationKey;

                return Json(new { Result = WebUiConstants.Success, ResetKey = resetKey });
            }
            catch (Exception exception)
            {
                _logger.ErrorException("ResetPassword exception on: " + email, exception);
                ErrorSignal.FromCurrentContext().Raise(exception);
            }

            return Json(new { Result = WebUiConstants.Fail });
        }

        public PartialViewResult GetPasswordResetConfirmFields()
        {
            var changePasswordFromResetKeyInputModel = new ChangePasswordFromResetKeyInputModel
            {
                ChangePasswordSucceeded = false
            };

            return PartialView("~/Views/Admin/Home/_PasswordResetConfirm.cshtml", changePasswordFromResetKeyInputModel);
        }

        [HttpPost]
        public ActionResult DeleteClaim(string email, string claim, string claimValue)
        {
            _membershipService.RemoveClaim(_globalConfig.Tenant, email, claim, claimValue);

            return Json(new { Result = WebUiConstants.Success });
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult EmailOrder(int? id, string emails)
        {
            if (id.HasValue)
            {
                try
                {
                    var order = _orderManagementService.GetOrderById(id.Value);

                    if (emails.Contains(","))
                        _orderManagementService.FireAdminEmailSendShippedOrderEvent(order,
                            emails.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
                    else if (emails.Contains(";"))
                        _orderManagementService.FireAdminEmailSendShippedOrderEvent(order,
                            emails.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
                    else
                        _orderManagementService.FireAdminEmailSendShippedOrderEvent(order, new string[] { emails });
                    // this is where a single email is specified


                    return Json(new
                    {
                        Result = WebUiConstants.Success,
                        Msg =
                        string.Format(
                            "<p>Your Order ID is {0}. Please check your email for connection information for the webinar.</p>",
                            id)
                    });
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty,
                        "There was a problem at the server. Please contact the administrator.");
                    _logger.ErrorException("ConfirmOrder|ConfirmOrder failed ", exception);
                    ErrorSignal.FromCurrentContext().Raise(exception);
                }
                return this.ModelStateJson(ModelState);
            }

            return Json(new
            {
                Result = WebUiConstants.Fail,
                Msg = "You need to select an Order from the DropDownList"
            });
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult EmailOrderConnectionInfo(int? id, string emails)
        {
            if (id.HasValue)
            {
                try
                {
                    //  id is a WebinarId
                    var firstRetrievedOrderForWebinar = _orderManagementService.GetOrdersForLiveNotifications(id.Value)
                        .FirstOrDefault(
                            o =>
                                o.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active)
                                    .RegistrationType.ShowLiveNotifications.ToLower() == "yes"
                        );

                    if (emails.Contains(","))
                        _orderManagementService.FireAdminEmailConnectionInfoHandler(firstRetrievedOrderForWebinar,
                            emails.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
                    else if (emails.Contains(";"))
                        _orderManagementService.FireAdminEmailConnectionInfoHandler(firstRetrievedOrderForWebinar,
                            emails.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
                    else
                        _orderManagementService.FireAdminEmailConnectionInfoHandler(firstRetrievedOrderForWebinar,
                            new string[] { emails }); // this is where a single email is specified

                    return Json(new
                    {
                        Result = WebUiConstants.Success,
                        Msg =
                        string.Format(
                            "<p>Your Order ID is {0}. Please check your email for connection information for the webinar.</p>",
                            id)
                    });
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty,
                        "There was a problem at the server. Please contact the administrator.");
                    _logger.ErrorException("ConfirmOrder|ConfirmOrder failed ", exception);
                    ErrorSignal.FromCurrentContext().Raise(exception);
                }
                return this.ModelStateJson(ModelState);
            }

            return Json(new
            {
                Result = WebUiConstants.Fail,
                Msg = "You need to select an Order from the DropDownList"
            });
        }

        [HttpPost]
        public ActionResult EmailRecordingPosted(int? id, string emails)
        {
            if (id.HasValue)
            {
                try
                {
                    //  id is a WebinarId
                    var firstRetrievedOrderForWebinar = _orderManagementService.GetOrdersForRecordedNotifications(
                        id.Value).FirstOrDefault(
                        o =>
                            o.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active)
                                .RegistrationType.ShowLiveNotifications.ToLower() == "yes"
                    );

                    if (emails.Contains(","))
                        _orderManagementService.FireAdminEmailConnectionInfoHandler(firstRetrievedOrderForWebinar,
                            emails.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
                    else if (emails.Contains(";"))
                        _orderManagementService.FireAdminEmailConnectionInfoHandler(firstRetrievedOrderForWebinar,
                            emails.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
                    else
                        _orderManagementService.FireAdminEmailConnectionInfoHandler(firstRetrievedOrderForWebinar,
                            new string[] { emails }); // this is where a single email is specified

                    return Json(new
                    {
                        Result = WebUiConstants.Success,
                        Msg =
                        string.Format(
                            "<p>Your Order ID is {0}. Please check your email for connection information for the webinar.</p>",
                            id)
                    });
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty,
                        "There was a problem at the server. Please contact the administrator.");
                    _logger.ErrorException("ConfirmOrder|ConfirmOrder failed ", exception);
                    ErrorSignal.FromCurrentContext().Raise(exception);
                }
                return this.ModelStateJson(ModelState);
            }

            return Json(new
            {
                Result = WebUiConstants.Fail,
                Msg = "You need to select an Order from the DropDownList"
            });
        }

        [HttpPost]
        [ValidateJsonAntiForgeryToken(Order = 0)]
        [HandleAjaxException(Order = 1)]
        public ActionResult ExtendPostEventAccess(int? orderID, string newExpiryDate)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(newExpiryDate))
                    throw new ValidationException("You need to enter a value.");

                var order = _orderManagementService.GetOrderById(orderID.Value);
                if (order != null)
                {

                    var onDemandCode = order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).OnDemandCode;

                    if (onDemandCode == null)
                    {
                        onDemandCode = RandomHelpers.GetUniqueCode(5);
                        order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).OnDemandCode = onDemandCode;
                        _orderManagementService.SaveChanges();
                    }
                    var orderIdProperty = new JProperty(JsonPropertyKeys.OrderId, orderID.Value);
                    var expiryDateProperty = new JProperty(JsonPropertyKeys.ExpiryDate, newExpiryDate);
                    var OnDemandCodeProperty = new JProperty(JsonPropertyKeys.OnDemandCode, onDemandCode);

                    var claimValue = new JObject(
                        orderIdProperty,
                        expiryDateProperty,
                        OnDemandCodeProperty
                    );



                    _membershipService.UpdatePostEventMaterialsClaim(
                        _globalConfig.Tenant,
                        order.BillingEmail,
                        DateTime.Parse(newExpiryDate),
                        order
                    );
                    //_logger.Info("Added expiryDate claim for: {0}. Date: {1}", orderID.Value, DateTime.Parse(newExpiryDate));
                    return Json(new { Result = WebUiConstants.Success });
                }
            }
            catch (Exception exception)
            {
                _logger.ErrorException(
                    string.Format("ExtendPostEventAccess | Session {0}", _appHelper.GetUserAuditInfo()), exception);

                ModelState.AddModelError(string.Empty, exception.Message);
            }

            return this.ModelStateJson(ModelState);
        }

        [HttpPost]
        public JsonResult FirePasswordResetEvent(ChangePasswordFromResetKeyInputModel model, string verificationKey)
        {
            model.Email = model.Email.Trim();
            if (_membershipService.ChangePasswordFromResetKey(_globalConfig.Tenant, verificationKey, model.Password))
                model.ChangePasswordSucceeded = true;
            _logger.Info("FirePasswordResetEvent ");
            return Json(model);
        }

        public string CreatePostEventClaim(Order order, DateTime expiryDate)
        {

            if (!ReferenceEquals(null, order))
            {
                var orderRow = order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active);
                if (orderRow.Webinar.Status != WebinarStatus.Recorded) return null;
                try
                {
                    var orderIdProperty = new JProperty(JsonPropertyKeys.OrderId, order.idOrder);
                    var expiryDateProperty = new JProperty(JsonPropertyKeys.ExpiryDate, expiryDate.ToShortDateString());
                    var OnDemandCodeProperty = new JProperty(JsonPropertyKeys.OnDemandCode, orderRow.OnDemandCode);

                    var claimValue = new JObject(
                        orderIdProperty,
                        expiryDateProperty,
                        OnDemandCodeProperty
                    );

                    //orderRow.OnDemandCode = onDemandCode;
                    //_orderManagementService.SaveChanges();

                    var userAccountOfOrderer = _membershipService.GetUserAccountByEmail(
                        _globalConfig.Tenant,
                        order.WebUser.email
                    );

                    _membershipService.AddClaim(
                        userAccountOfOrderer,
                        ClaimTypes1.PostEventMaterials
                        , claimValue.ToString(Formatting.None)
                    );


                    //_membershipService.UpdatePostEventMaterialsClaim(
                    //    _globalConfig.Tenant,
                    //    order.BillingEmail,
                    //    DateTime.Parse(newExpiryDate),
                    //    order
                    //    );
                    //_logger.Info("INSERT dbo.UserClaims ( ParentKey, Type, Value ) VALUES	( (SELECT [Key] FROM dbo.UserAccounts WHERE Email = '{0}'), http://ttstrain.com/ws/2014/01/identity/claims/DisplayPostEventMaterials,'{1}')", userAccountOfOrderer.Email, claimValue);
                    _logger.Info("CreatePostEventClaim: {0}", claimValue.ToString(Formatting.None));

                }
                catch (Exception ex)
                {
                    _logger.FatalException("CreateOnDemandClaimByAdjuster failed on orderId: " + order.idOrder, ex);
                }
            }
            return null;
        }

        //[ValidateJsonAntiForgeryToken(Order = 0)]
        [HandleAjaxException(Order = 1)]
        [HttpPost]
        [AllowAnonymous]
        public ActionResult UpdateAdditionalLocations(int orderRowId,
            IEnumerable<AdditionalLocation> additionalLocations)
        {
            //submitted by edit-forms-in-grid.js | submitUpdateAddLocsForm

            if (additionalLocations != null)
            {
                additionalLocations =
                    _appHelper.CheckAdditionalLocationsForValidEmail(additionalLocations.ToList()).ToList();
            }
            var orderRow = _orderManagementService.GetOrderRowById(orderRowId);


            if (additionalLocations != null)
            {
                var manageOrderEditModel = new ManageOrderEditModel
                {
                    AdditionalLocations = additionalLocations
                };

                var addLocEmails = "UpdateAdditionalLocations: " + orderRow.idOrder + "_";
                foreach (var addLoc in additionalLocations)
                {
                    if (_appHelper.CheckIsEmailValid(addLoc.Email.Trim()))
                        addLocEmails += addLoc.Email.Trim() + ",";
                }

                _logger.Info(addLocEmails.TrimEnd(','));


                SyncAdditionalLocations(manageOrderEditModel, orderRow);
            }
            else
            {
                var manageOrderEditModel = new ManageOrderEditModel
                {
                    AdditionalLocations = null
                };

                var addLocEmails = "UpdateAdditionalLocations | Removed : " + orderRow.idOrder;

                _logger.Info(addLocEmails.TrimEnd(','));
                SyncAdditionalLocations(manageOrderEditModel, orderRow);
            }
            Order order = orderRow.Order;
            if (!string.IsNullOrEmpty(order.InvoiceDetail) &&
                            order.OrderStatus == OrderStatus.Canceled)
            {

                _logger.Warn("Invoiced Order changes additional locations: " + order.idOrder);

                var toJson = JObject.Parse(order.InvoiceDetail);
                var thisInvoice =
                    toJson.Properties().FirstOrDefault(p => p.Name.StartsWith("OrderIsInvoiced"));

                if (thisInvoice != null)
                {
                    var row = order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active);
                    //

                    StringBuilder sb = new StringBuilder();

                    //sb.Append(order.idOrder + " was first invoiced on " + thisInvoice.First()["InvoiceId"] +
                    //          " as type '" + regTypeShortened + "' for $" +
                    //          preSaveValues.Split(',')[0].ToString().Replace(".0000", "").Replace(".00", "") +
                    //          ") ");
                    sb.Append(" but was canceled " +
                              TtsConfig.UtcNowAsCts.ToString(DomainConstants.DateTimeShortFormat) + ". ");

                    var adustmentAmount = 0 - (decimal)thisInvoice.First()["AmountOfRoyalty"];

                    //set default direction
                    var adjustmentDirection = "Royalty is decreased";

                    sb.Append(adjustmentDirection + " by " +
                              adustmentAmount.ToString("C").Replace(".00", ""));

                    row.RowPrice = 0;
                    var newJson4Invoice = new JProperty(
                        "ChangedOrderNeedsNewInvoice",
                        new JObject(
                            new JProperty("OriginalInvoice", thisInvoice.First()["InvoiceId"].ToString()),
                            new JProperty("OriginalDateOfInvoice",
                                thisInvoice.First()["DateOfInvoice"].ToString()),
                            new JProperty("OriginalTotal", thisInvoice.First()["AmountOfOrder"].ToString()),
                            new JProperty("OriginalPercentPaid",
                                thisInvoice.First()["PercentPaid"].ToString()),
                            new JProperty("OriginalRoyaltyPaid",
                                thisInvoice.First()["AmountOfRoyalty"].ToString()),
                            new JProperty("OriginalAffiliate", thisInvoice.First()["Affiliate"].ToString()),
                            new JProperty(adjustmentDirection, adustmentAmount),
                            new JProperty("DateOfChange",
                                TtsConfig.UtcNowAsCts.ToString(DomainConstants.DateTimeShortFormat)),
                            new JProperty("Message", sb.ToString())
                        ));


                    order.InvoiceDetail = JsonHelpers.ReplaceJsonWithStoredField(
                        order.InvoiceDetail, newJson4Invoice, "OrderIsInvoiced");

                }
            }
            PricesAndDiscounts pricesAndDiscounts = default(PricesAndDiscounts);
            _orderManagementService.UpdateOrderChanges(orderRow.Order, ref pricesAndDiscounts);

            if (pricesAndDiscounts.Discount == null)
                pricesAndDiscounts.Discount = new Discount();
            var ShippedDate =
                orderRow.ShipmentDate;
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
                        regTypeShort = orderRow.RegistrationType.OptionLabelShort,
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



        [HandleAjaxException]
        [HttpPost]
        public ActionResult GetOrdersByUser(string email)
        {
            int totalNumberOrders;

            return Json(new
            {
                data = BuildDisplayOrdersByUserViewModel(email, out totalNumberOrders)
            });
        }


        [HandleAjaxException]
        [AllowAnonymous]
        public ActionResult CheckOrderCommentsBatch()
        {
            var totalNumberOrders = 0;

            string result = _orderManagementService.CheckOrderComments();

            return Content("Ok");
        }




        [HandleAjaxException]
        [AllowAnonymous]
        public ActionResult CheckOnDemandCodes(int? webinarId, int? idAffiliate)
        {
            IList<Order> v3Orders = _orderManagementService.GetV3OrdersByOnDemandClaim();

            foreach (var order in v3Orders)
            {
                try
                {
                    var userAccount = _membershipService.GetUserAccountByEmail(_globalConfig.Tenant, order.BillingEmail);
                    var claimsViewModel = new ClaimsViewModel { UserClaims = userAccount.Claims };

                    var onDemandCode =
                        order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active).OnDemandCode;
                    foreach (
                        var claim in
                        claimsViewModel.UserClaims.Where(
                            c => c.Type == "http://ttstrain.com/ws/2014/01/identity/claims/DisplayPostEventMaterials"))
                    {
                        var thisClaim = JsonConvert.DeserializeObject<PostEventClaim>(claim.Value);

                        if (!ReferenceEquals(onDemandCode, null))
                        {
                            if (claim.Value.Contains(onDemandCode))
                            {
                                var needsCreatedClaim = true;
                                if (thisClaim.OrderId != order.idOrder)
                                {
                                    _logger.Warn("CheckOnDemandCodes finds Claim mismatch! Claim: {0} vs Order: {1}",
                                        thisClaim.OrderId,
                                        order.idOrder);
                                    _logger.Warn("Alert User of changed claim = '{0}' ", thisClaim.OrderId);
                                    _logger.Warn("DELETE dbo.UserClaims WHERE Value = '{0}' --Order: {1}", claim.Value,
                                        order.idOrder);

                                    //CreatePostEventClaim(order, thisClaim.ExpiryDate);
                                }
                                else
                                {
                                    needsCreatedClaim = false;
                                    _logger.Info("CheckOnDemandCodes found Match on: " +
                                                 order.OrderRows.SingleOrDefault().idOrder);
                                }
                                if (needsCreatedClaim)
                                {
                                    //{"OrderId":30706,"ExpiryDate":"2015-12-23","OnDemandCode":"oybo"}
                                    _logger.Info(
                                        "INSERT dbo.UserClaims ( ParentKey, Type, Value ) VALUES	( (SELECT [Key] FROM dbo.UserAccounts WHERE " +
                                        "Email = '{0}'), 'http://ttstrain.com/ws/2014/01/identity/claims/DisplayPostEventMaterials', " +
                                        "'{\"OrderId\":{1},\"ExpiryDate\":\"{2}\"' )", order.BillingEmail, order.idOrder,
                                        thisClaim.ExpiryDate);
                                }
                            }
                        }
                        else
                        {
                            if (claim.Value.Contains(order.idOrder.ToString()))
                            {
                                //_orderManagementService.SaveChanges();
                                _logger.Info("UPDATE dbo.OrderRow SET OnDemandCode = '{0}' WHERE idOrder ={1}",
                                    thisClaim.OnDemandCode, order.idOrder);
                                //var expiryDate = _orderManagementService.CalculatePostEventMaterialsAccessExpiry(order);

                                //CreatePostEventClaim(order, expiryDate);
                                //_logger.Warn("Null OnDemandCode is updated " + order.OrderRows.SingleOrDefault().idOrder);
                            }
                            else
                            {
                                _logger.Info("UPDATE dbo.OrderRow SET OnDemandCode = '{0}' WHERE idOrder ={1}",
                                    RandomHelpers.GetUniqueCode(4), order.idOrder);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.ErrorException("Checks ondemand claim failed: " + order.idOrder, ex);
                }
            }
            return Content("Ok");
        }


        public JsonResult getorderscompact(int? idWebinar)
        {
            string html = "";

            try
            {
                var orders = _orderManagementService.GetOrdersByWebinar(idWebinar.Value);

                SummaryOfOrdersPerWebinar model = new SummaryOfOrdersPerWebinar();

                html = ViewHelpers.RenderViewToString(ControllerContext,
                    "~/Views/Shared/DisplayTemplates/DataTablesDisplayTemplates/GetOrders_Compact.cshtml",
                    orders, true);
            }
            catch (Exception ex)
            {
                _logger.FatalException("GetOrderCompact error on " + idWebinar, ex);
            }

            return Json(new { html = html });


        }



        [HandleAjaxException]
        [HttpPost]
        [AllowAnonymous]
        public JsonResult DTHandlerOrders(DTParametersOrders param)
        {
            if (ClaimsAuthorization.CheckAccess(IdentityConstants.Access, IdentityConstants.GetGridDataFeature))
            {
                // based heavily on https://www.echosteg.com/jquery-datatables-asp.net-mvc5-server-side
                int totalNumberOrders = 0;
                int webinarId = param.webinarId;
                string searchTerm = param.searchTerm;
                int affiliateId = param.affiliateId ?? 19; // 19 is magic internal / house affiliate id
                bool showAllEvents = param.showAllEvents ?? false;

                List<Order> dtsource = null;

                try
                {

                    // custom filtering by Webinar Id
                    if (!showAllEvents)
                    {
                        if (searchTerm != null)
                        {

                            ViewBag.IsSearchResult = true;
                            int orderId = 0;

                            if (searchTerm.All(Char.IsDigit))
                            {
                                orderId = Convert.ToInt32(searchTerm);
                                var user = _orderManagementService.GetOrderById(orderId).WebUser;
                                dtsource =
                                    _dataTablesService.GetOrdersByUser(user.email, affiliateId, out totalNumberOrders)
                                        .ToList();
                            }
                            else if (searchTerm.Contains("@"))
                            {
                                if (searchTerm.StartsWith("@"))
                                {
                                    dtsource =
                                        _dataTablesService.GetOrdersByDomain(searchTerm, affiliateId,
                                                out totalNumberOrders)
                                            .ToList();
                                }
                                else
                                {
                                    dtsource =
                                        _dataTablesService.GetOrdersByUser(searchTerm, affiliateId,
                                                out totalNumberOrders)
                                            .ToList();
                                }
                            }
                            else if (searchTerm == "aa")
                            {
                                dtsource =
                                    _dataTablesService.GetOrdersByPending(affiliateId, out totalNumberOrders).ToList();
                            }
                            else if (searchTerm.StartsWith("l "))
                            {
                                dtsource =
                                    _orderManagementService.GetOrdersAll(affiliateId, out totalNumberOrders)
                                        .Where(
                                            o =>
                                                String.Equals(o.LastName, searchTerm.Replace("l ", ""),
                                                    StringComparison.CurrentCultureIgnoreCase))
                                        .ToList();
                            }
                            else if (searchTerm == "inprocess")
                            {
                                dtsource =
                                    _dataTablesService.GetOrdersByInProcess(affiliateId, out totalNumberOrders).ToList();
                            }
                        }
                        else
                        {
                            dtsource =
                                _dataTablesService.GetOrdersByWebinar(webinarId, affiliateId, out totalNumberOrders)
                                    .ToList();
                        }
                    }
                    else
                    {
                        dtsource = _orderManagementService.GetOrdersAll(affiliateId, out totalNumberOrders).ToList();
                    }

                    //disabled pending stableization of affiliate reports
                    //if (shouldBuildAffRpt)
                    //    _affiliateManagementService.BuildAffiliateReport(dtsource, webinarId);

                    // use automapper to flatten out the order records, in this specific case the data 
                    //  model has circular references which cause problems with JSON serialization
                    List<OrderDTO> dtoSource = new List<OrderDTO>();
                    Mapper.Map(dtsource, dtoSource);

                    // custom filtering by Order Status
                    if (param.selectedOrderStatuses != null)
                    {
                        dtoSource = dtoSource.Where(x => param.selectedOrderStatuses.Contains(x.OrderStatus)).ToList();
                    }

                    List<String> columnSearch = new List<string>();
                    foreach (var col in param.Columns)
                    {
                        columnSearch.Add(col.Search.Value);
                    }

                    List<OrderDTO> data = new DTResultSetOrders().GetResult(param.Search.Value, param.SortOrder,
                        param.Start, param.Length, dtoSource, columnSearch);
                    int count = new DTResultSetOrders().Count(param.Search.Value, dtoSource, columnSearch);


                    DataTableService<OrderDTO> result = new DataTableService<OrderDTO>
                    {
                        draw = param.Draw,
                        data = data,
                        recordsFiltered = count,
                        recordsTotal = count
                    };

                    JsonResult jsonresult = Json(result);
                    jsonresult.MaxJsonLength = int.MaxValue; // needed if/when the data is > 4mb

                    return jsonresult;
                }
                catch (Exception ex)
                {
                    return Json(new { error = ex.Message });
                }
            }

            return Json(new { NotAuthorized = true });

        }

        [HandleAjaxException]
        [HttpGet]
        //[AuthorizeHeaders]  
        [AllowAnonymous]
        public JsonResult GenerateWeeklyInvoicesEvent(DateTime startDate, int? _idAffiliate)
        {
            Stopwatch openingCall = new Stopwatch();
            openingCall.Start();

            if (!_idAffiliate.HasValue)
            {
                return Json(new { Result = WebUiConstants.Fail, OrdersFound = false }, JsonRequestBehavior.AllowGet);
            }
            int idAffiliate = _idAffiliate.Value;
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            DateTime date1 = startDate;
            System.Globalization.Calendar cal = dfi.Calendar;

            var weekNumber = cal.GetWeekOfYear(date1, dfi.CalendarWeekRule,
                                 dfi.FirstDayOfWeek) + "-" + cal.GetYear(DateTime.Now);
            var endDate = startDate.AddDays(7);

            var existingInvoice = CheckForExistingInvoice(startDate.ToShortDateString().Replace("/", "-"), weekNumber,
                idAffiliate);

            if (existingInvoice != null)
            {
                var __idAffiliate =
                    Convert.ToInt32(existingInvoice.Split('/')[5].Split('-')[2].ToString().Replace(".pdf", ""));

                var affiliate = _affiliateManagementService.FindById(__idAffiliate);

                return Json(new
                {
                    Result = WebUiConstants.Success,
                    InvoicesFound = true,
                    AffiliateId = affiliate.idUserAff,
                    InvoiceId = existingInvoice.Split('/')[5].Split('.')[0],
                    InvoiceUrl = existingInvoice,
                    AffiliateContactEmail = affiliate.ContactEmail,
                    DateRange = startDate.ToShortDateString() + " thru " + endDate.ToShortDateString(),
                    AffiliateLabel =
                    affiliate.DisplayTitle + " (" + affiliate.ttsDomain + " - " + affiliate.idUserAff + ")",
                    Link = new { PrimaryUri = existingInvoice },

                }, JsonRequestBehavior.AllowGet);
            }
            else
            {

                IList<Webinar> webinars = _webinarManagementService.GetWebinarsForWeeklyInvoices(startDate);
                var affiliateHasAnyOrders = CheckForAnyOrders(startDate, idAffiliate);
                var affiliate = _affiliateManagementService.FindById(idAffiliate);

                if (affiliateHasAnyOrders.StartsWith("none found"))
                {

                    return Json(new
                    {
                        Result = WebUiConstants.Fail,
                        OrdersFound = false,
                        AffiliateLabel =
                            affiliate.DisplayTitle + " (" + affiliate.ttsDomain + " - " + affiliate.idUserAff + ")",
                        ErrorMsg =
                            "Found no orders: " + affiliate.DisplayTitle + " (" + affiliate.ttsDomain + " - " +
                            affiliate.idUserAff + ")"
                    },
                        JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var listOrdersAdjusted = affiliateHasAnyOrders.Split(':')[3].TrimEnd(',').TrimStart(' ');

                    var listOrdersPostEvent = affiliateHasAnyOrders.Split(':')[2].Replace(" AjustedOrders", "").TrimEnd(',').TrimStart(' ');

                    _logger.Info("GenerateWeeklyInvoicesEvent | affiliateHasAnyOrders " + affiliateHasAnyOrders);

                    // clears the has any orders hurdle - 
                    int totalNumberOrders = 0;
                    int totalNumberDiscounts = 1;

                    StringBuilder discountNotes = new StringBuilder();

                    //had webinar orders or postevent orders ini
                    var hadWOrder = false;
                    var hadPOrder = false;
                    var hadUOrder = false;


                    int thisAffiliate = affiliate.idUserAff;

                    var InvoiceID = weekNumber + "-" + affiliate.idUserAff;
                    var RoyaltyTier = affiliate.CommissionModel;

                    decimal GrandTotalOnBilled = 0;
                    decimal GrandTotalOnPaid = 0;
                    decimal GrandTotalDiscounts = 0;
                    decimal GrandTotalRoyalties = 0;
                    decimal GrandTotalNetDue = 0;
                    var dsHeader =
                        new
                        {
                            RoyaltyTier = RoyaltyTier,
                            InvoiceID,
                            DateRange = startDate.ToShortDateString() + " - " + endDate.ToShortDateString(),
                            DisplayTitle = affiliate.DisplayTitle
                        };


                    DataSet dsPostEvent;
                    DataSet dsUpgradedOrders;
                    DataTable webinarsPerAff;
                    DataTable PostEventOrders;
                    DataTable upgradedOrders;
                    DataTable ordersPerAff;
                    DataTable pOrders;
                    DataTable uOrders;

                    var dsWebinars = IniDataTables(out dsPostEvent, out webinarsPerAff, out PostEventOrders,
                        out ordersPerAff, out pOrders, out upgradedOrders, out uOrders, out dsUpgradedOrders);

                    foreach (var webinar in webinars)
                    {

                        AffiliateInvoiceDTO invoice = new AffiliateInvoiceDTO
                        {
                            Affiliate = affiliate,
                            InvoiceID = weekNumber + "-" + affiliate.idUserAff,
                            RoyaltyTier = affiliate.CommissionModel
                        };

                        try
                        {
                            List<Order> webinarsOrders =
                                _orderManagementService.GetOrdersByWebinarForInvoice(webinar.idWebinar)
                                    .Where(o => o.idAffiliate == thisAffiliate)
                                    // filter is already applied .Where(o => o.OrderStatus == OrderStatus.Paid || o.OrderStatus == OrderStatus.Submitted || o.OrderStatus == OrderStatus.Billed)
                                    .ToList();
                            if (webinarsOrders.Any())
                            {
                                foreach (var o in webinarsOrders)
                                {
                                    o.InvoiceDetail = null;
                                }
                                hadWOrder = true;
                                invoice = _affiliateManagementService.BuildAffiliateInvoice(invoice, webinarsOrders,
                                    webinar.idWebinar,
                                    thisAffiliate);
                                int rowNumber = 0;

                                webinarsPerAff.Rows.Add(
                                    webinar.Title
                                    , webinar.Date.ToShortDateString()
                                    , invoice.TotalOnBilled.ToString("C").Replace(".00", "") + Environment.NewLine
                                    , invoice.TotalOnPaid.ToString("C").Replace(".00", "") + Environment.NewLine
                                    , invoice.TotalDiscounts.ToString("C").Replace(".00", "") + Environment.NewLine
                                    , invoice.TotalRoyalties.ToString("C").Replace(".00", "") + Environment.NewLine
                                    , invoice.TotalNetDue.ToString("C").Replace(".00", "") + Environment.NewLine
                                    , "Total Net Due: "
                                    , "Total Royalties: "
                                    , "Total Revenue Billed: "
                                    , "Total Revenue Paid: "
                                    , webinar.idWebinar
                                );


                                foreach (var order in webinarsOrders)
                                {
                                    try
                                    {
                                        var row =
                                            order.OrderRows.FirstOrDefault(r => r.RowStatus == OrderRowStatus.Active);

                                        string _price = row.RowPrice.ToString("C").Replace(".00", "");
                                        rowNumber++;

                                        string _percent =
                                            (row.PercentPaid * 100).ToString().Replace(".00", "").Replace(".0", "") +
                                            "%";

                                        //var totalToShow = order.Total.ToString("c");
                                        if (row.Discount != null)
                                        {

                                            var discount = row.Discount;
                                            _price = "See Note #" + totalNumberDiscounts;

                                            if (discount.DiscountType == DiscountType.Subscription &&
                                                discount.DateValidFrom != discount.DateValidTo)
                                            {
                                                discountNotes.Append(totalNumberDiscounts + " " + discount.DiscountCode +
                                                    ": (Unlimited Subscription) ");
                                            }
                                            else
                                            {
                                                discountNotes.Append(totalNumberDiscounts + " " + discount.DiscountCode +
                                                                     ": (" +
                                                                     discount.DiscountType.ToString()
                                                                         .Replace("DiscountType.", "") +
                                                                     ") ");
                                            }


                                            totalNumberDiscounts++;
                                            var discountAmount = "";
                                            if (row.Discount.PercentOff > 0)
                                            {
                                                discountAmount =
                                                    (row.RowPrice * (row.Discount.PercentOff / 100)).ToString("c");
                                            }
                                            if (row.Discount.FlatOff > 0)
                                            {
                                                discountAmount = (row.RowPrice - row.Discount.FlatOff).ToString("C");
                                            }
                                            //discountNotes.Append(discountAmount);
                                            discountNotes.Append(Environment.NewLine);
                                        }
                                        ordersPerAff.Rows.Add(
                                            rowNumber
                                            , order.FirstName + ' ' + order.LastName
                                            , order.BillingEmail
                                            ,
                                            order.Institution + Environment.NewLine + order.BillingAddress +
                                            Environment.NewLine +
                                            order.BillingCity + ", " + order.BillingState + " " + order.BillingZip
                                            , _price
                                            , _percent
                                            , row.Royalty.ToString("C").Replace(".00", "")
                                            , abbrevRegType(row.idRegType) + Environment.NewLine + order.OrderStatus
                                            , order.idOrder
                                            , webinar.idWebinar
                                        );
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.ErrorException(
                                            "GenerateWeeklyInvoicesEvent | WebinarsOrders looper: " + order.idOrder, ex);
                                    }
                                }

                                GrandTotalOnBilled += invoice.TotalOnBilled;
                                GrandTotalOnPaid += invoice.TotalOnPaid;
                                GrandTotalDiscounts += invoice.TotalDiscounts;
                                GrandTotalRoyalties += invoice.TotalRoyalties;
                                GrandTotalNetDue += invoice.TotalNetDue;
                            }

                            _logger.Info("GenerateWeeklyInvoicesEvent | ends processing: " + webinar.idWebinar);
                        }

                        catch (Exception ex)
                        {
                            _logger.ErrorException("GenerateWeeklyInvoicesEvent", ex);
                        }
                    }

                    //Post Event Orders
                    try
                    {
                        List<Order> _postEventOrders = new List<Order>();

                        foreach (var id in listOrdersPostEvent.Split(','))
                        {
                            //for why the test against static ID see DataOperations | 
                            if (id != "19983")
                            {
                                int variable = 0;
                                int.TryParse(id, out variable);
                                if (variable > 0)
                                    _postEventOrders.Add(_orderManagementService.GetOrderById(Convert.ToInt32(id)));
                            }
                        }

                        if (_postEventOrders.Any())
                        {
                            _logger.Info("GenerateWeeklyInvoices found " + _postEventOrders.Count + " for " + idAffiliate);
                            foreach (var postEventOrder in _postEventOrders)
                            {
                                _logger.Info("GenerateWeeklyInvoices | PostEvent  order " + postEventOrder.idOrder + " for " + idAffiliate);
                                postEventOrder.InvoiceDetail = null;
                            }
                            hadPOrder = true;
                            AffiliateInvoiceDTO invoice = new AffiliateInvoiceDTO
                            {
                                Affiliate = affiliate,
                                InvoiceID = weekNumber + "-" + affiliate.idUserAff,
                                RoyaltyTier = affiliate.CommissionModel
                            };
                            invoice = _affiliateManagementService.BuildAffiliateInvoiceForPostEventOrders(invoice,
                                                            _postEventOrders, thisAffiliate);
                            int rowNumber = 0;
                            PostEventOrders.Rows.Add(
                                                            "Post Event Orders"
                                                            , startDate
                                                            , invoice.TotalOnBilled.ToString("C").Replace(".00", "") + Environment.NewLine
                                                            , invoice.TotalOnPaid.ToString("C").Replace(".00", "") + Environment.NewLine
                                                            , invoice.TotalDiscounts.ToString("C").Replace(".00", "") + Environment.NewLine
                                                            , invoice.TotalRoyalties.ToString("C").Replace(".00", "") + Environment.NewLine
                                                            , invoice.TotalNetDue.ToString("C").Replace(".00", "") + Environment.NewLine
                                                            , "Total Net Due: "
                                                            , "Total Royalties: "
                                                            , "Total Revenue Billed: "
                                                            , "Total Revenue Paid: "
                                                            , 99
                                                        );


                            foreach (var order in _postEventOrders)
                            {
                                try
                                {
                                    var row = order.OrderRows.FirstOrDefault(r => r.RowStatus == OrderRowStatus.Active);
                                    string _price = row.RowPrice.ToString("C").Replace(".00", "");
                                    rowNumber++;
                                    string _percent =
                                        (row.PercentPaid * 100).ToString().Replace(".00", "").Replace(".0", "") +
                                        "%";
                                    if (row.Discount != null)
                                    {
                                        var discount = row.Discount;
                                        _price = "See Note #" + totalNumberDiscounts;
                                        discountNotes.Append(totalNumberDiscounts + " " + discount.DiscountCode + ": (" +
                                                             discount.DiscountType.ToString()
                                                                 .Replace("DiscountType.", "") +
                                                             ") ");

                                        totalNumberDiscounts++;
                                        var discountAmount = "";
                                        if (row.Discount.PercentOff > 0)
                                        {
                                            discountAmount = (row.UnitPrice * (row.Discount.PercentOff / 100)).ToString("c");
                                        }
                                        if (row.Discount.FlatOff > 0)
                                        {
                                            discountAmount = (row.UnitPrice - row.Discount.FlatOff).ToString("C");
                                        }

                                        discountNotes.Append(Environment.NewLine);
                                    }

                                    pOrders.Rows.Add(
                                        rowNumber
                                        , order.FirstName + ' ' + order.LastName
                                        , order.BillingEmail
                                        ,
                                        order.Institution + Environment.NewLine + order.BillingAddress +
                                        Environment.NewLine +
                                        order.BillingCity + ", " + order.BillingState + " " + order.BillingZip
                                        , _price
                                        , _percent
                                        , row.Royalty.ToString("C").Replace(".00", "")
                                        , abbrevRegType(row.idRegType) + Environment.NewLine + order.OrderStatus
                                        , order.idOrder
                                        , row.Webinar.Title
                                        , 99
                                    );
                                }
                                catch (Exception ex)
                                {
                                    _logger.ErrorException(
                                        "GenerateWeeklyInvoicesEvent | PostEventOrders looper: " + order.idOrder, ex);
                                }
                            }

                            GrandTotalOnBilled += invoice.TotalOnBilled;
                            GrandTotalOnPaid += invoice.TotalOnPaid;
                            GrandTotalDiscounts += invoice.TotalDiscounts;
                            GrandTotalRoyalties += invoice.TotalRoyalties;
                            GrandTotalNetDue += invoice.TotalNetDue;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.ErrorException("GenerateWeeklyInvoicesEvent | PostEventOrders: " + idAffiliate, ex);
                    }

                    try
                    {

                        List<Order> adjustedOrders = new List<Order>();

                        foreach (var id in listOrdersAdjusted.Split(','))
                        {
                            if (id != "19983")
                            {
                                int variable = 0;
                                int.TryParse(id, out variable);
                                if (variable > 0)
                                    adjustedOrders.Add(_orderManagementService.GetOrderById(Convert.ToInt32(id)));
                            }
                        }

                        if (adjustedOrders.Any())
                        {
                            hadUOrder = true;
                            AffiliateInvoiceDTO invoice = new AffiliateInvoiceDTO
                            {
                                Affiliate = affiliate,
                                InvoiceID = weekNumber + "-" + affiliate.idUserAff,
                                RoyaltyTier = affiliate.CommissionModel
                            };
                            invoice = _affiliateManagementService.BuildAffiliateInvoiceForAdjustedOrders(invoice,
                                                            adjustedOrders,
                                                            thisAffiliate);
                            int rowNumber = 0;
                            upgradedOrders.Rows.Add(
                                                            "Adjusted Orders"
                                                            , startDate
                                                            , invoice.TotalOnBilled.ToString("C").Replace(".00", "") + Environment.NewLine
                                                            , invoice.TotalOnPaid.ToString("C").Replace(".00", "") + Environment.NewLine
                                                            , invoice.TotalDiscounts.ToString("C").Replace(".00", "") + Environment.NewLine
                                                            , invoice.TotalRoyalties.ToString("C").Replace(".00", "") + Environment.NewLine
                                                            , invoice.TotalNetDue.ToString("C").Replace(".00", "") + Environment.NewLine
                                                            , "Adjusted Net Due: "
                                                            , "Adjusted Royalties: "
                                                            , "Adjusted Billed: "
                                                            , "Adjusted Paid: "
                                                            , 99
                                                        );


                            foreach (var order in adjustedOrders)
                            {
                                try
                                {
                                    StringBuilder sbL = new StringBuilder();
                                    sbL.Append("ChangedOrderNeedsNewInvoice logs: " + order.idOrder + " on aff: " + affiliate.idUserAff);
                                    var row = order.OrderRows.FirstOrDefault(r => r.RowStatus == OrderRowStatus.Active);
                                    rowNumber++;

                                    var obj = (JObject)JsonConvert.DeserializeObject(order.InvoiceDetail);
                                    var dict = obj.First.First.Children()
                                        .Cast<JProperty>()
                                        .ToDictionary(p => p.Name, p => p.Value);

                                    var message = (string)dict["Message"];
                                    var adjustmentDirection = "Royalty is increased";
                                    try
                                    {
                                        if (order.InvoiceDetail.Contains("Royalty is decreased"))
                                        {
                                            adjustmentDirection = "Royalty is decreased";
                                        }

                                        var adjustedTotal = row.RowPrice - (decimal)dict["OriginalTotal"];
                                        var adjustedRoyalty = (decimal)dict[adjustmentDirection];

                                        if (row.Discount != null)
                                        {

                                            var discount = row.Discount;
                                            //_price = "See Note #" + totalNumberDiscounts;
                                            discountNotes.Append(totalNumberDiscounts + " " + discount.DiscountCode + ": (" +
                                                                                                             discount.DiscountType.ToString()
                                                                                                                 .Replace("DiscountType.", "") +
                                                                                                             ") ");

                                            totalNumberDiscounts++;
                                            var discountAmount = "";
                                            if (row.Discount.PercentOff > 0)
                                            {
                                                discountAmount = (row.UnitPrice * (row.Discount.PercentOff / 100)).ToString("c");
                                            }
                                            if (row.Discount.FlatOff > 0)
                                            {
                                                discountAmount = (row.UnitPrice - row.Discount.FlatOff).ToString("C");
                                            }
                                            discountNotes.Append(Environment.NewLine);
                                        }

                                        uOrders.Rows.Add(
                                            rowNumber
                                            , order.FirstName + ' ' + order.LastName
                                            , order.BillingEmail
                                            ,
                                            order.Institution + Environment.NewLine + order.BillingAddress +
                                            Environment.NewLine +
                                            order.BillingCity + ", " + order.BillingState + " " + order.BillingZip
                                            , adjustedTotal
                                            , adjustedRoyalty
                                            , message
                                            , order.OrderStatus
                                            , order.idOrder
                                            , row.Webinar.Title
                                            , 99
                                            );

                                        order.InvoiceDetail = order.InvoiceDetail.Replace("ChangedOrderNeedsNewInvoice",
                                            "ChangedOrderWasReinvoiced-" + InvoiceID);
                                        _orderManagementService.SaveChanges();
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.ErrorException(
                                            "GenerateWeeklyInvoicesEvent | AdjustedOrders looper: " + order.idOrder, ex);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _logger.ErrorException("GenerateWeeklyInvoicesEvent | AdjustedOrders outer looper: " + order.idOrder, ex);
                                }
                            }

                            GrandTotalOnBilled += invoice.TotalOnBilled;
                            GrandTotalOnPaid += invoice.TotalOnPaid;
                            GrandTotalDiscounts += invoice.TotalDiscounts;
                            GrandTotalRoyalties += invoice.TotalRoyalties;
                            GrandTotalNetDue += invoice.TotalNetDue;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.ErrorException("GenerateWeeklyInvoicesEvent | Upgrades: ", ex);
                    }

                    DocumentModel document =
                        DocumentModel.Load(Server.MapPath(@"~/App_Data/mergeTemplates/WeeklyInvoice.docx"));

                    //if (affiliate.CommissionModel != 1)
                    //    document = DocumentModel.Load(Server.MapPath(@"~/App_Data/mergeTemplates/WeeklyInvoiceForFlatPercent.docx"));


                    document.MailMerge.FieldMerging += (sender, e) =>
                                        {
                                            if (affiliate.BillingModel == "aff")
                                            {
                                                if (e.Inline != null && e.FieldName == "TotalRevenueBilledLabel")
                                                    ((Run)e.Inline).Text = "";
                                                if (e.Inline != null && e.FieldName == "TotalRevenuePaidLabel")
                                                    ((Run)e.Inline).Text = "";
                                                if (e.Inline != null && e.FieldName == "TotalNetDueLabel")
                                                    ((Run)e.Inline).Text = "";
                                                if (e.Inline != null && e.FieldName == "TotalAdjustedRevenueBilledLabel")
                                                    ((Run)e.Inline).Text = "";
                                                if (e.Inline != null && e.FieldName == "TotalAdjustedRevenuePaidLabel")
                                                    ((Run)e.Inline).Text = "";
                                                if (e.Inline != null && e.FieldName == "TotalAdjustedNetDueLabel")
                                                    ((Run)e.Inline).Text = "";
                                                if (e.Inline != null && e.FieldName == "TotalOnBilled")
                                                    ((Run)e.Inline).Text = "";
                                                if (e.Inline != null && e.FieldName == "TotalOnPaid")
                                                    ((Run)e.Inline).Text = "";
                                                if (e.Inline != null && e.FieldName == "TotalNetDue")
                                                    ((Run)e.Inline).Text = "";
                                                if (e.Inline != null && e.FieldName == "GrandTotalOnBilled")
                                                    ((Run)e.Inline).Text = "";
                                                if (e.Inline != null && e.FieldName == "GrandTotalOnPaid")
                                                    ((Run)e.Inline).Text = "";
                                                if (e.Inline != null && e.FieldName == "GrandTotalNetDue")
                                                    ((Run)e.Inline).Text = "";
                                                if (e.Inline != null && e.FieldName == "GrandTotalRevenueBilledLabel")
                                                    ((Run)e.Inline).Text = "";
                                                if (e.Inline != null && e.FieldName == "GrandTotalRevenuePaidLabel")
                                                    ((Run)e.Inline).Text = "";
                                                if (e.Inline != null && e.FieldName == "GrandTotalNetDueLabel")
                                                    ((Run)e.Inline).Text = "";
                                            }

                                            if (e.IsValueFound)
                                            {
                                                switch (e.FieldName)
                                                {
                                                    case "Date":
                                                        ((Run)e.Inline).Text = ((DateTime)e.Value).ToString("dddd, MMMM d, yyyy");
                                                        break;

                                                    case "Percent":
                                                    case "Royalty":
                                                        break;
                                                }
                                            }
                                        };

                    var dsGrandTotals = new
                    {
                        GrandTotalOnBilled = GrandTotalOnBilled.ToString("C").Replace(".00", "") + Environment.NewLine,
                        GrandTotalOnPaid = GrandTotalOnPaid.ToString("C").Replace(".00", "") + Environment.NewLine,
                        GrandTotalDiscounts = GrandTotalDiscounts.ToString("C").Replace(".00", "") + Environment.NewLine,
                        GrandTotalRoyalties = GrandTotalRoyalties.ToString("C").Replace(".00", "") + Environment.NewLine,
                        GrandTotalNetDueLabel = "Grand Total Net Due: ",
                        GrandTotalRevenueBilledLabel = "Grand Total Revenue Billed: ",
                        GrandTotalRevenuePaidLabel = "Grand Total Revenue Paid: ",
                        GrandTotalNetDue = GrandTotalNetDue.ToString("C").Replace(".00", "") + Environment.NewLine,
                        DiscountNotes = discountNotes.ToString()
                    };


                    document.MailMerge.Execute(dsHeader);
                    if (hadWOrder)
                        document.MailMerge.Execute(dsWebinars, null);
                    // needs to be explicitly set to null since we are using a dataset
                    if (hadPOrder)
                        document.MailMerge.Execute(dsPostEvent, null);
                    // needs to be explicitly set to null since we are using a dataset
                    if (hadUOrder)
                        document.MailMerge.Execute(dsUpgradedOrders, null);
                    // needs to be explicitly set to null since we are using a dataset
                    //document.MailMerge.Execute(dsUpgrades, null);

                    // ONLY set this RemoveEmptyRanges option before the final merge execution or it 
                    //  causes problems (blank docs, probably due to the ranges getting removed before they are populated)
                    //  see also: http://www.gemboxsoftware.com/support-center/kb/articles/9-how-does-mailmergeclearoptions-removeemptyranges-work
                    document.MailMerge.ClearOptions = MailMergeClearOptions.RemoveEmptyRanges;
                    document.MailMerge.Execute(dsGrandTotals);

                    try
                    {
                        if (hadWOrder || hadPOrder || hadUOrder)
                        {
                            _logger.Info("begins write to file: " + InvoiceID);

                            //// SAVE LOCALLY if needed for easier testing
                            //document.Save(
                            //    Server.MapPath(@"~/App_Data/mergeTemplates/" + InvoiceID + ".pdf"), SaveOptions.PdfDefault);

                            var storageCredentials = new StorageCredentials(_globalConfig.StorageAccountName,
                                _globalConfig.StorageAccessKey);

                            var cloudStorageAccount = new CloudStorageAccount(storageCredentials, false);
                            CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();

                            // Retrieve reference to a previously created container.
                            CloudBlobContainer container = blobClient.GetContainerReference("affiliateinvoices");
                            container.CreateIfNotExists();

                            CloudBlockBlob blob =
                                container.GetBlockBlobReference(startDate.ToShortDateString().Replace("/", "-") + "/" +
                                                                InvoiceID + ".pdf");

                            using (MemoryStream output = new MemoryStream())
                            {
                                document.Save(output, SaveOptions.PdfDefault);
                                output.Position = 0; // reset to beginning so Upload operation can work correctly
                                blob.UploadFromStream(output);
                            }

                            return Json(new
                            {
                                Result = WebUiConstants.Success,
                                OrdersFound = true,
                                AffiliateId = affiliate.idUserAff,
                                InvoiceId = InvoiceID,
                                AffiliateContactEmail = affiliate.ContactEmail,
                                DateRange = startDate.ToShortDateString() + " thru " + endDate.ToShortDateString(),
                                AffiliateLabel =
                                    affiliate.DisplayTitle + " (" + affiliate.ttsDomain + " - " + affiliate.idUserAff +
                                    ")",
                                Link = new { PrimaryUri = blob.StorageUri.PrimaryUri },
                                //Link = new { PrimaryUri = "http://fakeurl.com/" + affiliate.idUserAff }, //TEMP:ALS
                                GrandTotalOnBilled = GrandTotalOnBilled.ToString("c"),
                                GrandTotalOnPaid = GrandTotalOnPaid.ToString("c"),
                                GrandTotalDiscounts = GrandTotalDiscounts.ToString("c"),
                                GrandTotalRoyalties = GrandTotalRoyalties.ToString("c"),
                                GrandTotalNetDue = GrandTotalNetDue.ToString("c")

                            }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new
                            {
                                Result = WebUiConstants.Fail,
                                OrdersFound = false,
                                AffiliateLabel =
                                    affiliate.DisplayTitle + " (" + affiliate.ttsDomain + " - " + affiliate.idUserAff +
                                    ")",
                            },
                                JsonRequestBehavior.AllowGet);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.ErrorException("building Invoice: " + InvoiceID, ex);
                    }
                }
                return Json(new
                {
                    Result = WebUiConstants.Fail,
                    OrdersFound = false,
                    AffiliateLabel =
                        affiliate.DisplayTitle + " (" + affiliate.ttsDomain + " - " + affiliate.idUserAff + ")",
                    ErrorMsg = "Error processing " + affiliate.DisplayTitle + " (" + affiliate.ttsDomain + " - " + affiliate.idUserAff + ")"
                },
        JsonRequestBehavior.AllowGet);
            }
        }

        private string CheckForAnyOrders(DateTime startDate, int idAffiliate)
        {
            var dataOperations = new DataOperations(TtsConfig.DefaultConnectionString);

            return dataOperations.CheckForAnyOrders(startDate, idAffiliate);

        }

        private string abbrevRegType(int idRegType)
        {
            return _orderManagementService.GetRegTypeOfOrderRow(idRegType).OptionLabelShort;
        }

        private string CheckForExistingInvoice(string startDate, string weekNumber, int idAffiliate)
        {
            string blob = null;

            try
            {
                var blobTest = BlobHelper.GetBlob("affiliateinvoices/", startDate, weekNumber + "-" + idAffiliate + ".pdf");
                if (blobTest != null)
                    blob = blobTest.BlobUri;
            }
            catch (Exception ex)
            {
                _logger.FatalException("CheckForExistingInvoice: ", ex);
            }
            return blob;
        }


        private static DataSet IniDataTables(out DataSet dsPostEvent, out DataTable webinarsPerAff, out DataTable postEventOrders,
            out DataTable ordersPerAff, out DataTable pOrders, out DataTable upgradedOrders, out DataTable uOrders, out DataSet dsUpgradedOrders)
        {
            DataSet dsWebinars = new DataSet("dsWebinars"); // dataset name doesn't play an active role in nested scenario, although example had same name as data relation
            dsPostEvent = new DataSet("dsPEOrders"); // dataset name doesn't seem to play an active role in nested scenario, although example had same name as data relation
            dsUpgradedOrders = new DataSet("dsUpgradedOrders"); // dataset name doesn't seem to play an active role in nested scenario, although example had same name as data relation


            upgradedOrders = new DataTable("Upgrades"); // parent table name needs to match outer range name in doc

            upgradedOrders.Columns.Add("Title", typeof(string));
            upgradedOrders.Columns.Add("WebinarDate", typeof(string));
            upgradedOrders.Columns.Add("TotalOnBilled", typeof(string));
            upgradedOrders.Columns.Add("TotalOnPaid", typeof(string));
            upgradedOrders.Columns.Add("TotalDiscounts", typeof(string));
            upgradedOrders.Columns.Add("TotalRoyalties", typeof(string));
            upgradedOrders.Columns.Add("TotalNetDue", typeof(string));
            upgradedOrders.Columns.Add("TotalAdjustedNetDueLabel", typeof(string));
            upgradedOrders.Columns.Add("TotalAdjustedRoyaltiesLabel", typeof(string));
            upgradedOrders.Columns.Add("TotalAdjustedBilledLabel", typeof(string));
            upgradedOrders.Columns.Add("TotalAdjustedPaidLabel", typeof(string));
            upgradedOrders.Columns.Add("Id", typeof(int));
            dsUpgradedOrders.Tables.Add(upgradedOrders);

            uOrders = new DataTable("dtUpgradeOrders"); // inner range, but don't want to have a name conflict with relation below

            uOrders.Columns.Add("RowNumber", typeof(int));
            uOrders.Columns.Add("Name", typeof(string));
            uOrders.Columns.Add("Email", typeof(string));
            uOrders.Columns.Add("Address", typeof(string));
            uOrders.Columns.Add("AdjustedPrice", typeof(string));
            uOrders.Columns.Add("AdjustedRoyalty", typeof(string));
            uOrders.Columns.Add("Message", typeof(string));
            uOrders.Columns.Add("Status", typeof(string));
            uOrders.Columns.Add("OrderID", typeof(int));
            uOrders.Columns.Add("WebinarTitle", typeof(string));
            uOrders.Columns.Add("Id", typeof(int));
            dsUpgradedOrders.Tables.Add(uOrders);



            webinarsPerAff = new DataTable("Webinars"); // parent table name needs to match outer range name in doc

            webinarsPerAff.Columns.Add("Title", typeof(string));
            webinarsPerAff.Columns.Add("WebinarDate", typeof(string));
            webinarsPerAff.Columns.Add("TotalOnBilled", typeof(string));
            webinarsPerAff.Columns.Add("TotalOnPaid", typeof(string));
            webinarsPerAff.Columns.Add("TotalDiscounts", typeof(string));
            webinarsPerAff.Columns.Add("TotalRoyalties", typeof(string));
            webinarsPerAff.Columns.Add("TotalNetDue", typeof(string));
            webinarsPerAff.Columns.Add("TotalNetDueLabel", typeof(string));
            webinarsPerAff.Columns.Add("TotalRoyaltiesLabel", typeof(string));
            webinarsPerAff.Columns.Add("TotalRevenueBilledLabel", typeof(string));
            webinarsPerAff.Columns.Add("TotalRevenuePaidLabel", typeof(string));
            webinarsPerAff.Columns.Add("Id", typeof(int));
            dsWebinars.Tables.Add(webinarsPerAff);

            postEventOrders = new DataTable("PostEvent"); // parent table name needs to match outer range name in doc

            postEventOrders.Columns.Add("Title", typeof(string));
            postEventOrders.Columns.Add("WebinarDate", typeof(DateTime));
            postEventOrders.Columns.Add("TotalOnBilled", typeof(string));
            postEventOrders.Columns.Add("TotalOnPaid", typeof(string));
            postEventOrders.Columns.Add("TotalDiscounts", typeof(string));
            postEventOrders.Columns.Add("TotalRoyalties", typeof(string));
            postEventOrders.Columns.Add("TotalNetDue", typeof(string));
            postEventOrders.Columns.Add("TotalNetDueLabel", typeof(string));
            postEventOrders.Columns.Add("TotalRoyaltiesLabel", typeof(string));
            postEventOrders.Columns.Add("TotalRevenueBilledLabel", typeof(string));
            postEventOrders.Columns.Add("TotalRevenuePaidLabel", typeof(string));
            postEventOrders.Columns.Add("Id", typeof(int));
            dsPostEvent.Tables.Add(postEventOrders);

            //orders
            ordersPerAff = new DataTable("dtOrders"); // inner range, but don't want to have a name conflict with relation below

            ordersPerAff.Columns.Add("RowNumber", typeof(int));
            ordersPerAff.Columns.Add("Name", typeof(string));
            ordersPerAff.Columns.Add("Email", typeof(string));
            ordersPerAff.Columns.Add("Address", typeof(string));
            ordersPerAff.Columns.Add("Price", typeof(string));
            ordersPerAff.Columns.Add("Percent", typeof(string));
            ordersPerAff.Columns.Add("Royalty", typeof(string));
            ordersPerAff.Columns.Add("Status", typeof(string));
            ordersPerAff.Columns.Add("OrderID", typeof(int));
            ordersPerAff.Columns.Add("Id", typeof(int));
            dsWebinars.Tables.Add(ordersPerAff);


            //orders
            pOrders = new DataTable("dtPEOrders"); // inner range, but don't want to have a name conflict with relation below

            pOrders.Columns.Add("RowNumber", typeof(int));
            pOrders.Columns.Add("Name", typeof(string));
            pOrders.Columns.Add("Email", typeof(string));
            pOrders.Columns.Add("Address", typeof(string));
            pOrders.Columns.Add("Price", typeof(string));
            pOrders.Columns.Add("Percent", typeof(string));
            pOrders.Columns.Add("Royalty", typeof(string));
            pOrders.Columns.Add("Status", typeof(string));
            pOrders.Columns.Add("OrderID", typeof(int));
            pOrders.Columns.Add("WebinarTitle", typeof(string));
            pOrders.Columns.Add("Id", typeof(int));
            dsPostEvent.Tables.Add(pOrders);
            // Add parent-child relation 

            dsWebinars.Relations.Add("Orders", webinarsPerAff.Columns["Id"], ordersPerAff.Columns["Id"]); // relation name needs to match nested range name in doc
            dsPostEvent.Relations.Add("pOrders", postEventOrders.Columns["Id"], pOrders.Columns["Id"]); // relation name needs to match nested range name in doc
            dsUpgradedOrders.Relations.Add("uOrders", upgradedOrders.Columns["Id"], uOrders.Columns["Id"]); // relation name needs to match nested range name in doc
            return dsWebinars;
        }



        public PartialViewResult SendWeeklyInvoicesEvent()
        {
            var model = new GenerateWeeklyInvoicesViewModel
            {
                Affiliates = _affiliateManagementService.GetAffiliates().ToList()

            };

            return PartialView("~/Views/Admin/Home/_sendWeeklyInvoicesEvent.cshtml", model);
        }

        [HandleAjaxException]
        [HttpPost]
        //[AuthorizeHeaders]  
        [AllowAnonymous]
        public JsonResult SendWeeklyInvoicesEvent(List<SendWeeklyInvoiceViewModel> model)
        {
            int emailsQueued = 0;

            // all necessary data has been populated via MVC / form post
            foreach (SendWeeklyInvoiceViewModel invoiceData in model)
            {
                // model automatically populated by data from UI via MVC, just fire the event, which queues the message to azure, which triggers the webjob on azure, which sends the email via mandrill/mailchimp

                // business project:
                //<Compile Include="Notification\Email\AzureWeeklyInvoiceWebJobSmtpMessageDelivery.cs" />
                //<Compile Include="Notification\Events\SendWeeklyInvoiceEvent.cs" />
                //<Compile Include="Notification\Handlers\SendWeeklyInvoiceHandler.cs" />
                //<Compile Include="Notification\ViewModel\SendWeeklyInvoiceViewModel.cs" />

                // web project:
                //<Content Include="Notification\Templates\SendWeeklyInvoice.cshtml" />
                // new FROM address and subject in web.config (and GlobalConfir.cs and TtsConfigHelper.cs)

                System.Diagnostics.Debug.WriteLine(string.Format("Queuing email for affiliate {0} | invoice {1} | url {2}", invoiceData.Affiliate.idUserAff, invoiceData.InvoiceId, invoiceData.InvoiceStorageUri));

                // email will be sent to Affiliate.ContactEmail, any reason to override here?
                invoiceData.Subject = _globalConfig.WeeklyInvoiceEmailSubject; // or in event handler / web.config?
                invoiceData.EmailBody = HttpUtility.HtmlDecode(
                    _generalFormatter.FormatV2(invoiceData, "~/Notification/Templates/SendWeeklyInvoice.cshtml").Body
                    );

                // is there any risk of this not working and us needing to deal with problems?
                _orderManagementService.FireSendWeeklyInvoiceEvent(invoiceData);
                emailsQueued++;

            }

            return Json(new { Result = WebUiConstants.Success, EmailsQueued = emailsQueued });
        }

        public ActionResult GetWeeklyInvoicesEvent()
        {
            var model = new GenerateWeeklyInvoicesViewModel
            {
                Affiliates = _affiliateManagementService.GetAffiliates().Where(a => a.idUserAff != 379 && a.idUserAff != 380 && a.idUserAff != 384 && a.idUserAff != 395 && a.idUserAff != 396 && a.idUserAff != 963).ToList()
            };

            return View("~/Views/Admin/Home/GenerateWeeklyInvoices.cshtml", model);
        }
        [HandleAjaxException]
        [HttpPost]
        [AllowAnonymous]
        public JsonResult DTDataHandlerAffiliateReport(int idWebinar)
        {
            if (ClaimsAuthorization.CheckAccess(IdentityConstants.Access, IdentityConstants.GetGridDataFeature))
            {
                // based heavily on https://www.echosteg.com/jquery-datatables-asp.net-mvc5-server-side
                int totalNumberOrders = 0;


                IList<Order> dtsource = null;

                try
                {

                    dtsource = _dataTablesService.GetOrdersByWebinarForRevenueReport(idWebinar, out totalNumberOrders).ToList();

                    var registrations =
                        from r in dtsource
                        where r.Affiliate != null
                        orderby r.OrderDate
                        group r by r.Affiliate into u
                        select new AffiliateReportDTO
                        {
                            WebinarId = idWebinar,
                            Affiliate = u.Key,
                            Orders = u.ToList()
                        };

                    // use automapper to flatten out the order records, in this specific case the data 
                    //  model has circular references which cause problems with JSON serialization
                    List<AffiliateReportDTO> dtoSource = new List<AffiliateReportDTO>();
                    Mapper.Map(registrations, dtoSource);


                    List<AffiliateReportDTO> data = dtoSource.ToList<AffiliateReportDTO>();

                    //List<AffiliateReportDTO> data = new DTResultSetAffiliates().GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtoSource, columnSearch);
                    int count = 10;


                    DataTableService<AffiliateReportDTO> result = new DataTableService<AffiliateReportDTO>
                    {
                        draw = 10,
                        data = data,
                        recordsFiltered = count,
                        recordsTotal = count
                    };

                    JsonResult jsonresult = Json(result);
                    jsonresult.MaxJsonLength = int.MaxValue;  // needed if/when the data is > 4mb


                    //return Json("result");
                    return Json(result);
                }
                catch (Exception ex)
                {
                    return Json(new { error = ex.Message });
                }
            }

            return Json(new { NotAuthorized = true });

        }



        [HandleAjaxException]
        [HttpPost]
        [AllowAnonymous]
        public JsonResult UsersDataHandler(DTParametersUsers param)
        {
            if (ClaimsAuthorization.CheckAccess(IdentityConstants.Access, IdentityConstants.GetGridDataFeature))
            {
                // based heavily on https://www.echosteg.com/jquery-datatables-asp.net-mvc5-server-side

                int totalNumberOrders = 0;
                int webinarId = param.webinarId;
                int? affiliateId = param.affiliateId;

                try
                {
                    List<WebUser> dtsource = _dataTablesService.GetWebUsers(affiliateId ?? 19, out totalNumberOrders).ToList();

                    // use automapper to flatten out the order records, in this specific case the data 
                    //  model has circular references which cause problems with JSON serialization
                    List<UserDTO> dtoSource = new List<UserDTO>();
                    Mapper.Map(dtsource, dtoSource);


                    List<String> columnSearch = new List<string>();
                    foreach (var col in param.Columns)
                    {
                        columnSearch.Add(col.Search.Value);
                    }

                    List<UserDTO> data = new DTResultSetUsers().GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtoSource, columnSearch);
                    int count = new DTResultSetUsers().Count(param.Search.Value, dtoSource, columnSearch);

                    DataTableService<UserDTO> result = new DataTableService<UserDTO>
                    {
                        draw = param.Draw,
                        data = data,
                        recordsFiltered = count,
                        recordsTotal = count
                    };

                    JsonResult jsonresult = Json(result);
                    jsonresult.MaxJsonLength = int.MaxValue;  // needed if/when the data is > 4mb

                    return jsonresult;
                }
                catch (Exception ex)
                {
                    return Json(new { error = ex.Message });
                }
            }

            return Json(new { NotAuthorized = true });

        }


        [HandleAjaxException]
        [HttpPost]
        [AllowAnonymous]
        public ActionResult GetWebinarSearchGridData(string searchTerm, int? affiliateId)
        {
            if (ClaimsAuthorization.CheckAccess(IdentityConstants.Access, IdentityConstants.GetGridDataFeature))
            {
                int totalNumberWebinars;

                return Json(new
                {
                    data = BuildWebinarsSearchViewModel(searchTerm, affiliateId, out totalNumberWebinars)
                });
            }

            return Json(new { NotAuthorized = true });
        }

        [HandleAjaxException]
        [HttpPost]
        [AllowAnonymous]
        public ActionResult GetGridDataForConnInfo(int? webinarId, int? affiliateId)
        {
            if (ClaimsAuthorization.CheckAccess(IdentityConstants.Access, IdentityConstants.GetGridDataFeature))
            {
                //

                int totalNumberOrders;

                return Json(new
                {
                    data = BuildConnInfoViewModel(webinarId.Value, affiliateId, out totalNumberOrders)
                });
            }

            return Json(new { NotAuthorized = true });
        }


        private IList<IDictionary<string, string>> BuildWebinarsSearchViewModel(string searchTerm, int? affiliateId, out int totalNumberWebinars)
        {
            var webinars = _dataTablesService.SearchWebinars(searchTerm, affiliateId ?? 19, out totalNumberWebinars);

            IList<IDictionary<string, string>> responsePayload = new List<IDictionary<string, string>>();
            IDictionary<string, string> responsePayloadInner = new Dictionary<string, string>();

            foreach (Webinar webinar in webinars)
            {

                var numDaysLeft = (webinar.Date.AddMonths(6) - DateTime.Now).Days;
                var showDateValue = DateTimeHelper.FormatDate(webinar.Date) + " - " +
                                    DateTimeHelper.FormatTime(webinar.Date) + " Central Time Zone";
                if (webinar.Status == WebinarStatus.Recorded && webinar.idWebinar != 842)
                {
                    //showDateValue += "<br><i>This OnDemand Webinar has <b>" + numDaysLeft + "</b> days of access remaining.</i>";
                    //if (numDaysLeft > 45)
                    //{
                    //    showDateValue = "About " + Convert.ToInt32(numDaysLeft / (365.25 / 12)) + " months of access remain.";
                    //}
                }
                var allTopics = webinar.WebinarTopicXrefs
                    .Select(webinarTopic => webinarTopic.Topic.topicDesc)
                    .Aggregate((s1, s2) => s1 + ", " + s2);

                string title = "<a href='/Webinar/Details/" +
                               webinar.idWebinar + "' target='_blank'>" + webinar.Title + "</a>";
                var desc = webinar.DescriptionLong.Length > 600
                    ? webinar.DescriptionLong.Substring(0, 600) + "... <a class=\"btn  btn-primary btn-small\" href='/Webinar/Details/" + webinar.idWebinar + "'>More ?</a></p>"
                    : webinar.DescriptionLong;

                var relatedTopics = "Related Topics: <b><i>" + string.Join(", ", allTopics) + "</i></b>";
                var descString = new StringBuilder();


                descString.Append("<div class=\"wTitle\">");
                descString.Append("    <h3>" + title + "</h3>");
                descString.Append("    <div id=\"eventBody_" + webinar.idWebinar + "\" style=\"width: 90%; margin-left: auto; margin-right: auto;\">");
                descString.Append("        <p>");
                descString.Append("            Presented by <img class=\"alignleft\" src=" + webinar.Presenter.PhotoThumb + " alt=\"Photo of " + webinar.Presenter.WebUser.FullName + "/> " + webinar.Presenter.WebUser.FullName + "<br/>");
                descString.Append("            <i>" + showDateValue + "</i>");
                descString.Append("        </p>");
                descString.Append("        <span style=\"margin-right: 20%;\">" + desc + "</span>");
                descString.Append("        <p>" + relatedTopics + "</p>");
                descString.Append("    </div>");
                descString.Append("</div>");


                string MediaColumn = desc;

                string webinarColumn = "descString.ToString()";
                string presenterColumn = webinar.Presenter.WebUser.FullName;
                string shareColumn = relatedTopics;
                string addToCartColumn = " ";
                string topicsColumn = " ";
                int webinarToEdit = 0;
                string ordersColumn = " ";
                responsePayloadInner = new Dictionary<string, string>();


                responsePayloadInner.Add("idWebinar", webinarColumn);
                responsePayloadInner.Add("OrdersColumn", ordersColumn);
                responsePayloadInner.Add("PresenterColumn", presenterColumn);
                responsePayloadInner.Add("TopicsColumn", topicsColumn);
                responsePayloadInner.Add("AddToCartColumn", addToCartColumn);
                responsePayloadInner.Add("MediaColumn", MediaColumn);
                responsePayloadInner.Add("ShareColumn", shareColumn);
                responsePayloadInner.Add("StatusColumn", "<a href='/Admin/manageWebinar" + webinarToEdit + "' target='_new' />");

                responsePayload.Add(responsePayloadInner);
            }

            return responsePayload;
        }


        private IList<IDictionary<string, string>> BuildConnInfoViewModel(int webinarId, int? affiliateId, out int totalNumberOrders)
        {

            var orders = _dataTablesService.GetOrdersByWebinar(webinarId, affiliateId ?? 19, out totalNumberOrders);

            IList<IDictionary<string, string>> responsePayload = new List<IDictionary<string, string>>();
            IDictionary<string, string> responsePayloadInner = new Dictionary<string, string>();

            foreach (var order in orders.Where(o => o.OrderStatus == OrderStatus.Paid || o.OrderStatus == OrderStatus.Billed || o.OrderStatus == OrderStatus.Submitted))
            {

                var orderRow = order.OrderRows.Single(o => o.RowStatus == OrderRowStatus.Active);
                if (orderRow.RegistrationType.ShowLiveNotifications == "No") continue;

                int orderToEdit = order.idOrderLegacy;
                if (orderToEdit == 0) orderToEdit = order.idOrder;

                var orderColumn = orderToEdit.ToString() + ", " + orderRow.TtsJoinUrl;
                var userColumn = "<a href='/account/edituser/" + order.idUser + "' target='_new' />" + order.LastName + ", " + order.FirstName + "</a><br>" + order.BillingEmail;
                var institutionColumn = order.Institution;
                var showDiscount = "";
                if (!ReferenceEquals(orderRow.Discount, null) && orderRow.Discount.FlatOff > 0)
                {
                    showDiscount = "<br><span class=\"DisplayDiscount\">Discounted by: $" + orderRow.Discount.FlatOff.ToString().Replace(".00", "") + "</span>";
                }
                else if ((!ReferenceEquals(orderRow.Discount, null) && orderRow.Discount.PercentOff > 0))
                {
                    showDiscount = "<br><span class=\"DisplayDiscount\">Discounted by: " + orderRow.Discount.PercentOff.ToString() + "%</span>";
                }
                var billingColumn = orderRow.RegistrationType.OptionLabel.Replace(" and Hardcopy Handouts", "").Replace("Plus Five", "Only") + showDiscount + "<br>Total: $" + order.Total.ToString().Replace(".00", "");
                var discountColumn = orderRow.Discount.DiscountCode;


                responsePayloadInner = new Dictionary<string, string>();

                responsePayloadInner.Add("OrderId", orderColumn);
                responsePayloadInner.Add("OrderColumn", orderColumn);
                responsePayloadInner.Add("UserColumn", userColumn);
                responsePayloadInner.Add("InstitutionColumn", institutionColumn);
                responsePayloadInner.Add("BillingColumn", billingColumn);
                responsePayloadInner.Add("DiscountColumn", discountColumn);

                responsePayloadInner.Add("AffiliateColumn", order.Affiliate.ttsDomain);

                responsePayloadInner.Add("OrderDateColumn", order.OrderDate.ToShortDateString());
                responsePayloadInner.Add("StatusColumn", "<a href='/Admin/manageOrder/" + orderToEdit + "' target='_new' />" + order.OrderStatus.ToString() + "</a>");

                responsePayload.Add(responsePayloadInner);
            }

            return responsePayload;
        }


        private IList<IDictionary<string, string>> BuildDisplayOrdersByUserViewModel(string email, out int totalNumberOrders)
        {
            var orders = _dataTablesService.GetOrdersByUser(email, 19, out totalNumberOrders);

            IList<IDictionary<string, string>> responsePayload = new List<IDictionary<string, string>>();
            IDictionary<string, string> responsePayloadInner = new Dictionary<string, string>();

            foreach (var order in orders)
            {
                var orderRow = order.OrderRows.Single(o => o.RowStatus == OrderRowStatus.Active);

                var orderColumn = order.idOrderLegacy.ToString() + ", " + order.Origin;
                var userColumn = "<a href='/account/edituser/" + order.idUser + "' target='_new' />" + order.LastName + ", " + order.FirstName + "</a>";
                var institutionColumn = order.Institution;
                var billingColumn = orderRow.RegistrationType.OptionLabel.Replace(" and Hardcopy Handouts", "").Replace("Plus Five", "Only") + "<br>Total: " + order.Total.ToString().Replace(".00", "");

                var resendMsg = "Resend Confirmation";
                if (orderRow.Webinar.Status == WebinarStatus.Active)
                {
                    resendMsg = "<button id='fireResendConfirmation' data-orderid='" + order.idOrder + "' type='button' class='btn btn-success btn-small' >Resend Connection Info</button>";
                }

                responsePayloadInner = new Dictionary<string, string>();

                responsePayloadInner.Add("OrderId", orderColumn);
                responsePayloadInner.Add("OrderColumn", orderColumn);
                responsePayloadInner.Add("UserColumn", userColumn);
                responsePayloadInner.Add("InstitutionColumn", institutionColumn);
                responsePayloadInner.Add("BillingColumn", billingColumn);
                //responsePayloadInner.Add("DiscountColumn", discountColumn);

                responsePayloadInner.Add("AffiliateColumn", order.Affiliate.ttsDomain);

                responsePayloadInner.Add("OrderDateColumn", order.OrderDate.ToShortDateString());
                responsePayloadInner.Add("StatusColumn", "<a href='/Admin/manageOrder/" + order.idOrderLegacy + "' target='_new' />" + order.OrderStatus.ToString() + "</a><br/>" + resendMsg);

                responsePayload.Add(responsePayloadInner);
            }

            return responsePayload;
        }

        private List<Address> ProcessAddresses(RegisterViewModel registerViewModel)
        {
            var billingAddress = new Address
            {
                AddressType =
                    Enum.GetName(typeof(AddressType), registerViewModel.RegisterFields.BillingAddress.TypeOfAddress),
                City = registerViewModel.RegisterFields.BillingAddress.City.Trim(),
                Country = registerViewModel.RegisterFields.BillingAddress.Country.Trim(),
                Name =
                    registerViewModel.RegisterFields.FirstName.Trim() + ' ' +
                    registerViewModel.RegisterFields.LastName.Trim(),
                Phone = registerViewModel.RegisterFields.BillingAddress.Phone.Trim(),
                State = registerViewModel.RegisterFields.BillingAddress.State.Trim(),
                StreetAddress = registerViewModel.RegisterFields.BillingAddress.StreetAddress.Trim(),
                StreetAddress2 =
                    registerViewModel.RegisterFields.BillingAddress.StreetAddress2 == null
                        ? registerViewModel.RegisterFields.BillingAddress.StreetAddress2
                        : registerViewModel.RegisterFields.BillingAddress.StreetAddress2.Trim(),
                Zip = registerViewModel.RegisterFields.BillingAddress.Zip.Trim()
            };

            var shippingAddress = new Address
            {
                AddressType =
                    Enum.GetName(typeof(AddressType), registerViewModel.RegisterFields.ShippingAddress.TypeOfAddress),
                City = registerViewModel.RegisterFields.ShippingAddress.City.Trim(),
                Country = registerViewModel.RegisterFields.ShippingAddress.Country.Trim(),
                Name =
                    registerViewModel.RegisterFields.FirstName.Trim() + ' ' +
                    registerViewModel.RegisterFields.LastName.Trim(),
                Phone = registerViewModel.RegisterFields.ShippingAddress.Phone.Trim(),
                State = registerViewModel.RegisterFields.ShippingAddress.State.Trim(),
                StreetAddress = registerViewModel.RegisterFields.ShippingAddress.StreetAddress.Trim(),
                StreetAddress2 =
                    registerViewModel.RegisterFields.ShippingAddress.StreetAddress2 == null
                        ? registerViewModel.RegisterFields.ShippingAddress.StreetAddress2
                        : registerViewModel.RegisterFields.ShippingAddress.StreetAddress2.Trim(),
                Zip = registerViewModel.RegisterFields.ShippingAddress.Zip.Trim()
            };

            return new List<Address> { billingAddress, shippingAddress };
        }


        private static IEnumerable<SelectListItem> GetClaimTypesFromClass(Type type, ConstantType typeOfConstant)
        {
            IEnumerable<FieldInfo> claimTypes = null;

            switch (typeOfConstant)
            {
                case ConstantType.Constant:
                    {
                        claimTypes = type.GetFields().Where(f => f.IsLiteral).ToList();
                        break;
                    }
                case ConstantType.Readonly:
                    {
                        claimTypes = type.GetFields(BindingFlags.Public | BindingFlags.Static)
                            .Where(f => f.FieldType == typeof(string) && f.IsInitOnly)
                            .ToList();
                        break;
                    }
                default:
                    {
                        throw new NotSupportedException(string.Format("An unsupported option for {0} was passed in.", typeOfConstant));
                    }
            }


            var typesAsSelectList = new List<SelectListItem>();
            string rawConstant;

            foreach (var claimType in claimTypes)
            {
                switch (typeOfConstant)
                {
                    case ConstantType.Constant:
                        {
                            rawConstant = claimType.GetRawConstantValue().ToString();
                            break;
                        }
                    case ConstantType.Readonly:
                        {
                            rawConstant = claimType.GetValue(claimType).ToString();
                            break;
                        }
                    default:
                        {
                            throw new NotSupportedException(string.Format("An unsupported option for {0} was passed in.", typeOfConstant));
                        }
                }

                typesAsSelectList.Add(new SelectListItem
                {
                    Text =
                        rawConstant.SubstringFromRight(rawConstant.Length -
                                                       rawConstant.LastIndexOf(Path.AltDirectorySeparatorChar) - 1),
                    Value = rawConstant
                });
            }
            return typesAsSelectList;
        }

        private bool? CheckIfAddLocAvailable(int optionId)
        {
            var firstOrDefault = _orderManagementService.GetRegTypeOption(optionId)
                .Select(o => new { Show = o.ShowLiveNotifications, Ship = o.ShowShippedNotifications })
                .FirstOrDefault();

            if (firstOrDefault != null)
            {
                return firstOrDefault.Show.Equals("Yes", StringComparison.OrdinalIgnoreCase);
            }

            return null;
        }



    }
}