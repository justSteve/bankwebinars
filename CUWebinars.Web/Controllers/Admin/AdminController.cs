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
using System.Web.UI.WebControls;
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
using CUWebinars.Web.Models.DataTablesModels;

using CUWebinars.Web.Services;
using CUWebinars.Web.ViewModel;
using Elmah;
using GemBox.Document;
using GemBox.Document.Tables;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ninject.Extensions.Logging;
using Thinktecture.IdentityModel.Authorization;
using Thinktecture.IdentityModel.Authorization.Mvc;
using WinSCP;
using ClaimTypes = System.Security.Claims.ClaimTypes;
using ClaimTypes1 = CUWebinars.Business.Constants.ClaimTypes;
using DateTimeHelper = CUWebinars.Web.Helpers.DateTimeHelper;
using Formatting = Newtonsoft.Json.Formatting;

// would be needed if we add PromoGenerateForAffiliate action
//using Newtonsoft.Json.Converters;
//using System.Dynamic;

using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Calendar = System.Web.UI.WebControls.Calendar;
using Table = GemBox.Document.Tables.Table;

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

        //
        // GET: /Admin/
        //[ClaimsAuthorize(IdentityConstants.Access, IdentityConstants.AffiliateFunction)]
        public ActionResult Index()
        {
            var aff = _affiliateManagementService.FindById(19);
            //uncomment to use Updated DataTable code
            string searchTerm = Request["searchTerm"];
            ShowWebinarsViewModel gridModel = new ShowWebinarsViewModel
            {
                SearchTerm = searchTerm
            };
            ViewBag.Title = "Search Results";

            ClaimsIdentity claimsIdentityOfAuthenticatedUser = (ClaimsIdentity)User.Identity;
            var affiliate = _stateService.GetValue<Affiliate>(WebUiConstants.CurrentAffiliate);
            var user = _membershipService.GetWebUserById(affiliate.idUserAff);
            var model = new AdminDTO
            {
                ShowWebinarsViewModel = gridModel,
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
                DiscountSubscriptionsModel = _affiliateManagementService.GetSubscriptionsByAffiliate(62) as IList<DiscountDTO>,

            };

            if (claimsIdentityOfAuthenticatedUser.HasClaim(
                (claim) => claim.Type == CUWebinars.Business.Constants.ClaimTypes.Affiliate))
            {

                model = new AdminDTO
                {
                    DiscountSubscriptionsModel = _affiliateManagementService.GetSubscriptionsByAffiliate(affiliate.idUserAff) as IList<DiscountDTO>,
                    UserDetailsViewModel = new UserDetailsViewModel()
                    {
                        Affiliate = affiliate,
                        UserIsAdmin = false
                    },
                    AffiliateSettingsViewModel = new AffiliateSettingsViewModel()
                    {
                        Affiliate = affiliate,
                        WebUser = user
                    }
                };
            }

            return View("~/Views/Admin/Home/Index.cshtml", model);
        }

        public ActionResult GetAppLog2()
        {
            ServicePointManager.SetTcpKeepAlive(true, 30000, 20000);

            // Get the object used to communicate with the server.

            FtpWebRequest request =
                (FtpWebRequest)
                    WebRequest.Create(
                        "ftp://waws-prod-ch1-005.ftp.azurewebsites.windows.net/LogFiles/log4netCSV.log");

            request.Method = WebRequestMethods.Ftp.DownloadFile;


            request.Credentials = new NetworkCredential("$BankWebinars33",
                "FDcehM4K2WbSuxEplrG2B7uJxqrMbeqJ6MdDm3GLyraYTmzWnmLDQAkllu0t", "BankWebinars33");

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();


            Stream responseStream = response.GetResponseStream();

            StreamReader reader = new StreamReader(responseStream);


            StreamWriter writer = new StreamWriter("C:\\Users\\Steve\\Desktop\\logfiles\\resultStream.log");
            writer.Write(reader.ReadToEnd());

            Console.WriteLine("Download Complete, status {0}", response.StatusDescription);



            reader.Close();

            response.Close();


            return View();
        }

        public ActionResult GetAppLog()
        {
            // Setup session options
            SessionOptions sessionOptions = new SessionOptions
            {
                Protocol = Protocol.Ftp,
                HostName = "waws-prod-ch1-005.ftp.azurewebsites.windows.net",
                PortNumber = 21,
                UserName = @"BankWebinars33\$BankWebinars33",
                Password = "FDcehM4K2WbSuxEplrG2B7uJxqrMbeqJ6MdDm3GLyraYTmzWnmLDQAkllu0t",
                FtpMode = FtpMode.Active,
            };


            try
            {
                using (Session session = new Session())
                {
                    // Connect
                    session.DebugLogLevel = 1;
                    session.DebugLogPath = "d:\\log.log";

                    session.Open(sessionOptions);
                    // Upload files
                    TransferOptions transferOptions = new TransferOptions();
                    transferOptions.TransferMode = TransferMode.Ascii;

                    TransferOperationResult transferResult;
                    transferResult = session.GetFiles("LogFiles/log4netCSV.log", "D:\\log4net.log", false,
                        transferOptions);


                    // Throw on any error
                    transferResult.Check();

                    // Print results
                    foreach (TransferEventArgs transfer in transferResult.Transfers)
                    {
                        Console.WriteLine("Upload of {0} succeeded", transfer.FileName);
                    }
                }

                View();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
                View();
            }

            return View();
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
            var order = _orderManagementService.GetOrderByIdThin(idOrder.Value);
            var originalAffiliate = new AffiliateRepository().FindByIdWithIncluding(order.idAffiliate);
            order.idAffiliate = idAffiliate.Value;
            //_orderManagementService.SaveChanges();

            var newAffiliate = new AffiliateRepository().FindByIdWithIncluding(idAffiliate.Value);
            //            var orderUser = _membershipService.GetUserByEmail(order.BillingEmail);

            string buildMessage = "<div class=\"affiliateChanged\">Affiliate changed for order " + order.idOrder +
                                  " from " + originalAffiliate.ttsDomain + " to " +
                                  newAffiliate.ttsDomain + " by " + User.Identity.Name +
                                  " on " + TtsConfig.UtcNowAsCts.ToShortDateString() +
                                  "</div>";
            try
            {
                //following copies pattern found at WebinarController | Identify
                JObject existingJObject = null;

                string comments = string.Empty;


                if (!ReferenceEquals(null, order.AdminComments))
                {
                    comments = order.AdminComments.Trim();
                }

                var newJson =
                    new JProperty(
                        string.Concat("ChangeAffiliate-",
                            TtsConfig.UtcNowAsCts.ToString(DomainConstants.DateTimeLongFormat)),
                        new JObject(
                            new JProperty("ChangeAffiliateTo", newAffiliate.ttsDomain),
                            new JProperty("Details", buildMessage)
                            ));

                if (string.IsNullOrWhiteSpace(comments))
                {
                    existingJObject = new JObject(newJson);
                }
                else
                {
                    existingJObject = JObject.Parse(comments);
                    existingJObject.Add(newJson);
                }

                order.AdminComments = existingJObject.ToString(Formatting.None);
                //_membershipService.AddClaim(_membershipService.GetUserAccountByEmail(_globalConfig.Tenant, _globalConfig.TenantEmail)
                //                ,CUWebinars.Business.Constants.ClaimTypes.CommentAdmin, "SetAffiliateAssignedToOrder: {" + newJson + "}");

                _orderManagementService.SaveChanges();


            }
            catch (Exception ex)
            {
                _logger.Fatal("SetAffiliateAssigedToOrder: ", ex);
                throw;
            }

            _logger.Info(buildMessage);


            return Json(new { Result = WebUiConstants.Success, NewAffiliate = newAffiliate.ttsDomain });
        }

        //public ActionResult ManageOrder()
        //{
        //    return View();
        //}


        //[HttpPost]
        //[ValidateAntiForgeryToken(Order = 0)]
        //[HandleAjaxException(Order = 1)]
        //public ActionResult ManageOrder(ManageOrderEditModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var newOrder = ApplyModelChangesToOrder(model);
        //        var oldOrder = model.Order;

        //        //var orderchanges = new OrderChanges
        //        //{

        //        //}

        //        //model.PostEventAccessExpires = _membershipService.SetPostEventAccessExpireyDate(userAccount, id.Value);

        //        _orderManagementService.UpdateOrderByAdmin(newOrder);
        //    }

        //    return Json(new { Result = WebUiConstants.Success });
        //}

        //public ActionResult ManageOrderFromDetails(int? id)
        //{
        //    if (id.HasValue)
        //    {
        //        if (id == 0)
        //            return RedirectToAction("ManageOrder");

        //        var userAccount = _membershipService.GetUserAccountByEmail(_globalConfig.Tenant,
        //            _orderManagementService.GetOrderById(id.Value).BillingEmail);
        //        var claimsViewModel = new ClaimsViewModel { UserClaims = userAccount.Claims };

        //        var model = BuildManageOrderEditModel(id.Value);

        //        if (model.Order.WebUser.email != model.Order.BillingEmail)
        //        {
        //            model.Order.WebUser = _membershipService.GetUserByEmail(model.Order.BillingEmail);
        //            if (model.Order.WebUser != null)
        //                _orderManagementService.SaveChanges();
        //        }

        //        var onDemandClaim = new PostEventClaim();
        //        foreach (
        //            var claim in
        //                claimsViewModel.UserClaims.Where(
        //                    c => c.Type == "http://ttstrain.com/ws/2014/01/identity/claims/DisplayPostEventMaterials"))
        //        {
        //            var singleOrDefault = model.Order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active);
        //            if (singleOrDefault != null && claim.Value.Contains(singleOrDefault.OnDemandCode))
        //            {
        //                var thisClaim = JsonConvert.DeserializeObject<PostEventClaim>(claim.Value);

        //                onDemandClaim.OrderId = model.Order.idOrder;
        //                onDemandClaim.OnDemandCode = thisClaim.OnDemandCode;
        //                onDemandClaim.ExpiryDate = thisClaim.ExpiryDate;

        //                model.ClaimByOrderViewModel = onDemandClaim;
        //            }
        //        }

        //        model.PostEventAccessExpires = onDemandClaim.ExpiryDate;

        //        return View(model);
        //    }

        //    //  should never reach here as RouteConfig will not route here with anything but an integer > 0.
        //    throw new NullReferenceException(
        //        "Query string parameter has to be an positive integer for the ManageOrderFromDetails action.");
        //}

        //private Order ApplyModelChangesToOrder(ManageOrderEditModel model)
        //{
        //    var order = _orderManagementService.GetOrderById(model.Id);
        //    var orderRow = order.OrderRows.Single(o => o.RowStatus == OrderRowStatus.Active);

        //    order.Total = model.DisplayRowPriceViewModel.PricesAndDiscounts.TotalOrderPrice;
        //    order.OrderStatus = model.DisplayRowPriceViewModel.OrderStatus;

        //    orderRow.RowPrice = order.Total;
        //    orderRow.UnitPrice = model.DisplayRowPriceViewModel.PricesAndDiscounts.UnitPrice;

        //    SyncAdditionalLocations(model, orderRow);

        //    return order;
        //}

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

        public JsonResult getResendInfoHtml()
        {

            string html = "";

            //var orderToEdit = (full.idOrderLegacy != 0) ? full.idOrderLegacy : full.idOrder;

            //var resendMsg = "<button data-orderId=\"" + orderToEdit + "\" class=\"ResendOrderConfirmationButton btn btn-mini\">Send Confirmation</button>";
            //if (full.Webinar_IsActive)
            //{
            //    resendMsg += "<button data-orderId=\"" + orderToEdit + "\" class=\"ResendConnectionInfoButton btn btn-mini\">Connection Info</button>";
            //}

            return Json(new { html = html });

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
                    if (discountId > 0)
                    {
                        _orderManagementService.ApplyDiscountCode(discountId);
                    }
                    var order = _orderManagementService.GetOrderById(model.Id);
                    _logger.Info("Updating OrderStatus from " + order.OrderStatus + " to: " + model.DisplayRowPriceViewModel.OrderStatus + " by: " + _appHelper.GetUserAuditInfo());

                    var dataOperations = new DataOperations(TtsConfig.LegacyConnectionString);

                    var UpdateOrderStatusOnLegacy = dataOperations.UpdateOrderStatusOnLegacy(order.OrderRows.Single(r => r.RowStatus == OrderRowStatus.Active).idWebinar, order.BillingEmail, Convert.ToInt32(model.DisplayRowPriceViewModel.OrderStatus));

                    //order.OrderStatus = model.DisplayRowPriceViewModel.OrderStatus; // the only field that we are updating at this time
                    order.OrderStatus = model.DisplayRowPriceViewModel.OrderStatus; // the only field that we are updating at this time

                    _orderManagementService.UpdateOrderByAdmin(order);

                    return Json(new { Result = WebUiConstants.Success, orderStatus = model.DisplayRowPriceViewModel.OrderStatus.ToString(), msgFromLegacy = UpdateOrderStatusOnLegacy });
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
        public ActionResult SetUserAssignedToOrder(int orderID, string migrateOrder, string targetUserEmail)
        {
            try
            {

                var order = _orderManagementService.GetOrderByIdThin(orderID);
                var webUser = _membershipService.GetWebUserById(_membershipService.GetWebUserIdByEmail(targetUserEmail).Value);

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
                    _logger.Fatal("ClaimsManagement error on " + model.UserEmail + " " + ex);
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
        public String AuditDiscount(DiscountAudit posted)
        {

            Discount discount = _orderManagementService.GetDiscountById(Convert.ToInt32(posted.idDiscount));
            Discount discountLegacy = _orderManagementService.GetDiscountByIdLegacy(Convert.ToInt32(posted.idDiscount));

            IList<WebUser> users = _orderManagementService.GetWebUsersOfDiscount(discount.idDiscount);
            IList<Order> ordersByDiscount = _orderManagementService.GetOrdersByDiscount(posted.idDiscount);
            IList<Order> ordersWithDiscount = new List<Order>();

            var sb = new StringBuilder();
            var authUsers = new StringBuilder();

            if (!ReferenceEquals(users, null) && users.Count == 1)
            {
                authUsers.AppendLine("The only address authorized for AutoCheckout is " + users.Single().email);
            }
            else
            {
                if (!ReferenceEquals(users, null))
                {
                    authUsers.AppendLine("These addresses are authorized for AutoCheckout: ");
                    foreach (var user in users)
                    {
                        authUsers.AppendLine(user.email);
                    }
                }
                else
                {
                    authUsers.AppendLine("No authorized users were found.");
                }
            }

            try
            {
                sb.AppendLine("Starting Discount: " + discount.idDiscount + " Used: " + posted.CreditsUsed + " Remain: " + posted.CreditsRemain);
                var trackUsed = posted.CreditsUsed;
                var trackRemain = posted.CreditsRemain;

                foreach (var order in ordersByDiscount)
                {

                    var DiscountCaption = _orderManagementService.CalculateDiscountRedemption(discount,
                         order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active), null, 0);

                    var thisUse = DiscountCaption.Split(',')[0];
                    var used = DiscountCaption.Split(',')[1];
                    var remain = DiscountCaption.Split(',')[2];

                    trackUsed = trackUsed + Convert.ToDecimal(thisUse);
                    trackRemain = trackRemain - Convert.ToDecimal(thisUse);

                    sb.AppendLine("    Order " + order.idOrder + " deducts: " + thisUse);

                    ordersWithDiscount.Add(order);

                }
                sb.AppendLine("Calculated usage = " + trackUsed + "-" + trackRemain + " Stored = " + discount.CreditsUsed + "-" + discount.CreditsRemain);

                return authUsers.ToString() + sb.ToString();
                //return Json(new { Result = "{" + sb.ToString() + "}" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var errString = string.Format("Discount audit failed on {0} - {1} with msg: {2}", posted.idDiscount, posted.DiscountCode, ex.Message);
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                _logger.ErrorException(errString, ex);

                return errString + sb;
                //return Json(new { Result = errString + sb.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        [AllowAnonymous]
        public void ExpressCheckout(JotFormWebHook postback)
        {
            string formFields = Request.Form.ToString();
            _logger.Info("ExpressCheckoutPostBack all fields submitted to: " + formFields.ToString());

            ExpressCheckoutModel form
                = JsonConvert.DeserializeObject<ExpressCheckoutModel>(postback.RawRequest);
            _logger.Info("ExpressCheckoutPostBack all fields submitted to " + form.q11_orderid + ". " + formFields.ToString());

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

                        _orderManagementService.ApplyDiscountCode(discount.DiscountCode, row);

                    }

                    RegType regType = _orderManagementService.GetRegTypeByLabel(regTypeLable, form.q18_q_webinarid18);

                    var dataOperations = new DataOperations(TtsConfig.DefaultConnectionString);

                    var additionalLocationsPricing = dataOperations.GetAdditionalLocationsPricing(row.idWebinar);


                    JProperty createdByExpressCheckout = new JProperty(JsonPropertyKeys.OrderCreatedByExpressCheckoutKey,
                        "'ExpressCheckoutPostBack': {'" + postback.Pretty + "'}");

                    expressOrder.AdminComments = JsonHelpers.MergeJsonWithStoredField(null,
                        createdByExpressCheckout);

                    //_membershipService.AddClaim(
                    // _membershipService.GetUserAccountByEmail(_globalConfig.Tenant, _globalConfig.TenantEmail)
                    // ,CUWebinars.Business.Constants.ClaimTypes.CommentAdmin, JsonConvert.SerializeObject(createdByExpressCheckout, Formatting.None, new JsonSerializerSettings { MaxDepth = 1, ReferenceLoopHandling = ReferenceLoopHandling.Ignore })

                    //    );

                    expressOrder.OrderStatus = OrderStatus.Submitted;
                    expressOrder.Origin = DomainConstants.OriginExpress;

                    OrderGenesis og = OrderGenesis.CreatedViaExpressCheckout;

                    row.RegistrationType = regType;
                    row.idRegType = regType.idRegType;
                    _orderManagementService.CalculateOrderCost(expressOrder, additionalLocationsPricing.Single().Price);
                    _orderManagementService.SaveChanges();
                    _orderManagementService.FireOrderSubmittedEvent(expressOrder, userCreatedByCheckout);

                    expressOrder.idOrderLegacy = _orderManagementService.SynchExpressCheckoutOrder(expressOrder);
                    _orderManagementService.SaveChanges();
                }
                catch (Exception ex)
                {
                    _logger.Fatal("ExpressCheckout tossed exception: " + ex.Message);
                }

            }
            else
            {
                _logger.Warn("ExpressCheckout webhook {0} order NOT FOUND: " + form.q11_orderid);

                //var idRegType = _webinarManagementService.GetRegTypeByLableAndWebinar(regTypeLable,
                //    form.q18_q_webinarid18);
                //var newOrderRow = _orderManagementService.CreateOrderRow(null, null, idRegType);

                //newOrderRow.idWebinar = form.q18_q_webinarid18;

                //newOrderRow.idRegType = idRegType;
                //_orderManagementService.LoadWebinarIntoOrderRow(newOrderRow);


                //var affiliate = _stateService.GetValue<Affiliate>(WebUiConstants.CurrentAffiliate);
                //_orderManagementService.SetUserStatusToUnChanged(user);

                //Order newOrder = _orderManagementService.CreateNewOrder(
                //    affiliate.idUserAff,
                //    user,
                //    newOrderRow.Webinar,
                //    newOrderRow
                //    );

                //newOrderRow.Order.OrderStatus = OrderStatus.Submitted;

                //newOrderRow.Order.Origin = "ExpressCheckout";

                //JProperty createdByExpressCheckout = new JProperty(
                //    JsonPropertyKeys.OrderCreatedByExpressCheckoutKey,
                //    ""
                //    );

                //newOrderRow.Order.AdminComments = JsonHelpers.MergeJsonWithStoredField(newOrderRow.Order.AdminComments,
                //    createdByExpressCheckout);
                ////_orderManagementService.AttachAffiliate(affiliate);

                //_orderManagementService.SaveChanges();


                //newOrder.idOrderLegacy = _orderManagementService.SynchExpressCheckoutOrder(newOrder);

                //_orderManagementService.SynchIds(newOrder);

                //_orderManagementService.SaveChanges();

                //_logger.Info("ExpressCheckout created order: " + newOrder.idOrder);

                //_orderManagementService.FireOrderSubmittedEvent(newOrderRow.Order, true);


            }
            //return RedirectToAction("OrderComplete", "Account", new { id = expressOrder.idOrder });
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

        //private ManageOrderEditModel BuildManageOrderEditModel(int id)
        //{
        //    var order = _orderManagementService.GetOrderById(id);
        //    var orderRow = order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active);
        //    var userAccount = _membershipService.GetUserAccountByEmail(_globalConfig.Tenant, order.BillingEmail);
        //    var additionalLocationsPricing =
        //        _orderManagementService.GetCostOfAdditionalLocations(
        //            orderRow.AdditionalLocation,
        //            orderRow.idWebinar
        //            );
        //    var additionalLocations = orderRow.AdditionalLocation;
        //    additionalLocations = _appHelper.CheckAdditionalLocationsForValidEmail(additionalLocations).ToList();

        //    var additionalLocationsCount = additionalLocations.Count;

        //    var manageOrderEditModel = new ManageOrderEditModel
        //    {
        //        //EditOrder_Compact should look to 
        //        AdditionalLocations = additionalLocations,
        //        AdditionalLocationsAvailableOnLoad = false, // see below for where this is properly decided.
        //        AdditionalLocationsRenderer = ViewHelpers.GetRendererOfAdditionalLocations(additionalLocations.Select(al => al.Email).ToList()),
        //        CostPerAdditionalLocation = additionalLocationsPricing.Item2,
        //        ClaimsViewModel = new ClaimsViewModel { UserClaims = userAccount.Claims },

        //        DisplayOptionsInDropDownViewModel = new DisplayOptionsInDropDownViewModel
        //        {
        //            Options = _orderManagementService.GetAllPossibleOptionsByWebinarId(orderRow.idWebinar, false),
        //            OrderRowId = orderRow.idOrderRow,
        //            OrderRowRegistrationType = orderRow.RegistrationType
        //        },
        //        DisplayRowPriceViewModel = new DisplayRowPriceViewModel
        //        {
        //            Discount = orderRow.Discount,
        //            NumberOfAdditionalLocations = additionalLocationsCount,
        //            OrderStatus = order.OrderStatus,
        //            Price = Convert.ToDecimal(orderRow.RegistrationType.Price),
        //            PricesAndDiscounts =
        //                _orderManagementService.CalculateOrderCost(order, additionalLocationsPricing.Item2),
        //            RegistrationType = orderRow.RegistrationType,
        //            RowPrice = orderRow.RowPrice
        //        },
        //        Id = order.idOrder,
        //        AffiliateName = _appHelper.GetAffiliateName(order.idAffiliate),
        //        JoinCode = orderRow.TtsJoinUrl,
        //        OnDemandCode = orderRow.OnDemandCode,
        //        Order = order,
        //        NumberOfAdditionalLocations = additionalLocationsCount,
        //        PhoneNumber = order.BillingPhone,
        //        UserId = order.idUser,
        //        WebinarId = orderRow.idWebinar,
        //        WebUser = order.WebUser
        //    };

        //    // Need to decide whether the selected option in the DropDownList can add additional locations
        //    var regType = manageOrderEditModel.DisplayOptionsInDropDownViewModel.OrderRowRegistrationType;

        //    {
        //        var option = CheckIfAddLocAvailable(regType.idRegType);
        //        if (option.HasValue)
        //        {
        //            manageOrderEditModel.AdditionalLocationsAvailableOnLoad = option.Value;
        //        }
        //    }
        //    return manageOrderEditModel;
        //}

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

        //public PartialViewResult ResendConnectionInfo()
        //{
        //    var model = new ResendOrderInformationViewModel
        //    {
        //        OrderId = string.Empty
        //    };

        //    return PartialView("~/Views/Admin/Home/_resendConnectionInfo.cshtml", model);
        //}

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

        //public PartialViewResult SendRecordingPosted()
        //{
        //    var model = new AdhocNotificationViewModel
        //    {
        //        Webinars = EventInvokerHelpers.GetRecordedWebinarsAsSelectListItems(_webinarManagementService)
        //    };

        //    return PartialView("~/Views/Admin/Home/_SendRecordingPosted.cshtml", model);
        //}

        //[System.Web.Mvc.HttpPost]
        //public JsonResult SendRecordingPosted(int webinarId)
        //{
        //    var orders = _orderManagementService.GetOrdersForRecordedNotifications(webinarId);
        //    _logger.Info(string.Join(",", orders.Select(o => o.idOrder.ToString())));

        //    var ordersWhichSatisfyClaim = GetOrdersWhichAreEligibleForMaterials(orders);

        //    if (ordersWhichSatisfyClaim.Any())
        //    {
        //        _orderManagementService.FireSendRecordingIsPostedEvent(ordersWhichSatisfyClaim.ToList());

        //        return Json(new { Result = WebUiConstants.Success });
        //    }

        //    return Json(new { Result = WebUiConstants.NoOrdersForWebinar });
        //}

        //private IEnumerable<Order> GetOrdersWhichAreEligibleForMaterials(IEnumerable<Order> orders)
        //{
        //    IList<Order> eligableOrders = new Order[0];
        //    foreach (var order in orders)
        //    {
        //        var userAccount = _membershipService.GetUserAccountByEmail(_globalConfig.Tenant, order.WebUser.email);

        //        DateTime? expiryDate = _orderManagementService.CalculatePostEventMaterialsAccessExpiry(order);

        //        if (expiryDate.HasValue && expiryDate > TtsConfig.UtcNowAsCts)
        //        {
        //            eligableOrders.Add(order);
        //        }
        //    }
        //    return eligableOrders;
        //}


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


        //[HttpPost]
        //[AllowAnonymous]
        //public JsonResult PromoGenerateForAffiliate(WebinarPromoViewModel model)
        //{

        //    // this method could be just an "if affiliate id" block in the main method...
        //    //  would save on the JSON / object expander complexity

        //    JsonResult ret = PromoGenerate(model);
        //    string json = ret.Data.ToJsonNet();
        //    //var x = JsonConvert.DeserializeObject<xobj>(json);
        //    //System.Diagnostics.Debug.WriteLine(x.masterText);

        //    var converter = new ExpandoObjectConverter();
        //    dynamic x = JsonConvert.DeserializeObject<ExpandoObject>(json, converter);
        //    string masterMarkup = x.masterText;
        //    System.Diagnostics.Debug.WriteLine(masterMarkup);

        //    // do affiliate substitution
        //    Affiliate aff = new AffiliateRepository().FindByIdWithIncluding(model.Affiliate.idUserAff);
        //    string affiliateMarkup = replaceMasterTokensForAffiliate(masterMarkup, aff);

        //    // return complete affiliate-specific version
        //    return Json(new { result = WebUiConstants.Success, affiliateCopy = affiliateMarkup });
        //}

        ////private class xobj
        ////{
        ////    public string masterText { get; set; }
        ////}

        //// unforatunately there is a client-side version of this function in the promo-generator.js file as well...
        //private string replaceMasterTokensForAffiliate(string masterCopy, Affiliate aff)
        //{
        //    StringBuilder copy = new StringBuilder(masterCopy);

        //    copy.Replace("{aff_ttsdomain}", aff.ttsDomain);
        //    copy.Replace("{aff_idUserAff}", aff.idUserAff.ToString());
        //    copy.Replace("{aff_ContactPerson}", aff.ContactPerson);
        //    copy.Replace("{aff_ContactEmail}", aff.ContactEmail);
        //    copy.Replace("{aff_ContactPhone}", aff.ContactPhone);
        //    copy.Replace("{aff_EmailFooter}", aff.EmailFooter);

        //    return copy.ToString();

        //}
        [ValidateInput(false)]
        [HttpPost]
        public JsonResult WritePromoToStorage(WebinarPromoViewModel model)
        {
            string eventBodyText = System.Uri.UnescapeDataString(model.EventBody);

            // refactor??? don't want to create a new connection to azure for each file though...

            // based on CUMailer\CUWebinars.Azure.OrderConfirmNotifier\CUWebinars.Azure.OrderConfirmNotifier\OrderConfirmationHandler.cs

            // almost certainly should be extracted to a function in the Business project
            var storageCredentials = new StorageCredentials(_globalConfig.StorageAccountName, _globalConfig.StorageAccessKey);
            var cloudStorageAccount = new CloudStorageAccount(storageCredentials, false);
            CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference("v3generator");
            container.CreateIfNotExists();

            string webinarTitleEnc = Server.UrlEncode(model.Webinar.Title);
            string strAffId = model.Affiliate.idUserAff.ToString();
            string filenameBase = string.Concat(strAffId, "/", model.Webinar.idWebinar, "/", webinarTitleEnc);

            // Create the blobs
            string filename = "";
            string ret = "";
            bool overwriteFlag = false; // do we want to think of a way to let the user tell us we should (or should not) overwrite?

            // html file
            filename = string.Concat(filenameBase, ".html");
            byte[] byteArrayHTML = Encoding.UTF8.GetBytes(eventBodyText); // "full" markup from WIJMO editor, may want to add doctype and body tags...
            ret += UploadToAzure(container, filename, byteArrayHTML, overwriteFlag);

            // text file
            filename = string.Concat(filenameBase, ".txt");
            byte[] byteArrayTXT = Encoding.UTF8.GetBytes(eventBodyText.Replace("<br>", "\r\n")); // decidedly NOT robust, yet!!
            ret += UploadToAzure(container, filename, byteArrayTXT, overwriteFlag);

            return Json(new
            {
                result = (string.IsNullOrWhiteSpace(ret) ? WebUiConstants.Success : WebUiConstants.Fail),
                returnMessage = ret
            });
        }

        private string UploadToAzure(CloudBlobContainer container, string filename, byte[] content, bool overwriteIfExists)
        {
            string ret = "";

            using (var memoryStream = new MemoryStream(content))
            {
                _logger.Info(string.Format("Persisting blob now. Named: {0}", filename));
                CloudBlockBlob blob = container.GetBlockBlobReference(filename);
                if (overwriteIfExists ||
                    !blob.Exists())
                {
                    blob.UploadFromStream(memoryStream);
                    _logger.Info("Blob successfully persisted");
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
        public JsonResult PromoGenerate(WebinarPromoViewModel model)
        {

            if (model.TemplateType == "Daily")
            {
                model.Webinar = _webinarManagementService.GetWebinar(model.Webinar.idWebinar);

                // populate the dropdown selector (not currently implemented)
                TempData["ListOfWebinarsForUpcoming"] =
                    _webinarManagementService.GetUpcomingWebinars().OrderBy(w => w.Date).Take(10).ToList();


                if (model.ListOfWebinarsForUpcoming == null)
                {
                    model.ListOfWebinarsForUpcoming =
                   _webinarManagementService.GetUpcomingWebinars().OrderBy(w => w.Date).Take(10).Select(w => w.idWebinar).ToArray();
                }

                var _upcoming = _webinarManagementService.GetUpcomingWebinars().Where(w => w.Date > model.SendDate && w.idWebinar != model.Webinar.idWebinar).OrderBy(w => w.Date).Take(5);


                var doc = new HtmlDocument();
                doc.LoadHtml(model.Webinar.Presenter.BiographyLong);

                var root = doc.DocumentNode;
                root.SelectSingleNode("(//img)[1]").Remove();

                model.PresenterW_OutPic = root.InnerHtml;

                var upcomingWebinars = (from w in _upcoming
                                        select
                                            "<p style=\"color: bisque; text-decoration: none; \" ><a style=\" color: bisque; border-bottom: 1px dotted bisque;\" href=\"http://www.bankwebinars.com/Webinar/Details/" +
                                            w.idWebinar + "?idaff={aff_idUserAff}\">" + w.Title + "</a><br>" +
                                            w.Date.ToLongDateString() + "</p>"
                    ).ToArray();

                if (upcomingWebinars.Count() > 0)
                    model.ListOfWebinarsUpcomingRendered = String.Join("\r\n", upcomingWebinars);

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

                        string eDate = "<b>" + DateTimeHelper.FormatDate(webinar.Date) + " - " + DateTimeHelper.FormatTimeWithDuration(webinar.Date, model.TimeZone, false,
                                    webinar.Duration) + "</b><br />";

                        upcomingDetail.Append(
                            "<p><span style='font-size:20px; font-weight:bold; font-family:trebuchet ms;'><a href=\"http://www.bankwebinars.com/Webinar/Details/" + webinar.idWebinar + "?idaff={aff_idUserAff}\">" + webinar.Title + "</a></span><br />");
                        upcomingDetail.Append("<span style='font-family:trebuchet ms;'><b>" +
                      webinar.Presenter.WebUser.FullName + "</b><br /></span>");
                        upcomingDetail.Append("<span style='font-size:12px;'>" + eDate + "</span></p>");
                        upcomingDetail.Append("<div style='font-family:trebuchet ms;'>" + webinar.Description + "</div>");

                        upcomingDetail.Append(
                            "<p font-family:trebuchet ms;'><a href='http://www.bankwebinars.com/Webinar/Details/" +
                            webinar.idWebinar + "?idaff={aff_idUserAff}'>Click here for more info!</a></p>");
                        upcomingDetail.Append("<hr style='width:50%; ' />");
                    }
                }
                var q = _webinarManagementService.GetUpcomingWebinars().Where(a => a.idWebinar != model.Webinar.idWebinar && a.Date > model.SendDate).Where(
                         a => !(featureWebinarIDs.Any(item2 => item2 == a.idWebinar))).OrderBy(a => a.Date).Take(12).Select(a =>
                         new
                         {
                             featuredItem =
                             "<p style=\"color: bisque; text-decoration: none; \"><a style=\"color: bisque; \" href=\"http://www.bankwebinars.com/Webinar/Details/" +
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
            return null;
        }

        [HttpPost]
        public ActionResult PerWeekPromo(WebinarPromoViewModel model)
        {
            IList<Webinar> webinars = new List<Webinar>();
            //
            foreach (var _id in model.ListOfWebinarsForWeekly)
            {
                webinars.Add(_webinarManagementService.GetWebinar(_id));
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
        public ActionResult PerDayPromo(int id)
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

                model.Affiliates = new AffiliateRepository().GetAffiliatesByPromoType("Daily").Where(a => a.idUserAff == affId).ToList();
            }

            return View(model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendSinglePromo(int affiliateId, string messageBodyHtml) // WebinarPromoViewModel model
        {
            //var model = null;
            WebinarPromoViewModel model = new WebinarPromoViewModel(); // TEMP FOR INITIAL TESTING
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
                var adminUserEmail = adminUser.Claims.Single(c => c.Type == System.IdentityModel.Claims.ClaimTypes.Email).Value;
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

        [HttpGet]
        public ActionResult SynchWebinars()
        {

            _webinarManagementService.SynchToLegacy();

            return View();
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

                    var resetKey = _globalConfig.TenantURL + "/Account/PasswordResetConfirm/" + userAccount.VerificationKey;

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


        ////        public PartialViewResult GetJsonTextArea()
        ////        {
        ////            const string importOrderViaDashboardViewModel = @"{""AffiliateComments"": ""Affiliate comments"",
        ////                                  ""BillingAddress.AddressType"": ""Billing"",
        ////                                  ""BillingAddress.Name"": ""Alan Turing"",
        ////                                  ""BillingAddress.Phone"": ""555-555-5555"",
        ////                                  ""BillingAddress.StreetAddress"": ""968 Wildcat Dr"",
        ////                                  ""BillingAddress.StreetAddress2"": """",
        ////                                  ""BillingAddress.City"": ""Del Rio"",
        ////                                  ""BillingAddress.Zip"": ""5000"",
        ////                                  ""BillingAddress.State"": ""Tx"",
        ////                                  ""BillingAddress.Country"": ""USA"",
        ////                                  ""Email"": ""alanbturingy@turing.com"",
        ////                                  ""FirstName"": ""Alan"",
        ////                                  ""LastName"": ""Turing"",
        ////                                  ""idAffiliate"": 19,
        ////                                  ""idRegType"": 88,
        ////                                  ""idWebinar"": 437,
        ////                                  ""Institution"": ""Some Institution"",
        ////                                  ""ShippingAddress.AddressType"": ""Shipping"",
        ////                                  ""ShippingAddress.Name"": ""Alan Turing"",
        ////                                  ""ShippingAddress.Phone"": ""555-555-5555"",
        ////                                  ""ShippingAddress.StreetAddress"": ""968 Wildcat Dr"",
        ////                                  ""ShippingAddress.StreetAddress2"": """",
        ////                                  ""ShippingAddress.City"": ""Del Rio"",
        ////                                  ""ShippingAddress.Zip"": ""5000"",
        ////                                  ""ShippingAddress.State"": ""Tx"",
        ////                                  ""ShippingAddress.Country"": ""USA"",
        ////                                  ""SendNotification"": ""true"",
        ////                                  ""Title"": ""Mr""}";

        ////            ViewBag.Payload = importOrderViaDashboardViewModel;

        ////            return PartialView("~/Views/Admin/Home/_ImportOrder.cshtml", importOrderViaDashboardViewModel);
        ////        }

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
                    _logger.Warn("CreateOnDemandClaimByAdjuster failed on orderId: " + order.idOrder);
                }
            }
            return null;
        }

        //[ValidateJsonAntiForgeryToken(Order = 0)]
        [HandleAjaxException(Order = 1)]
        [HttpPost]
        [AllowAnonymous]
        public ActionResult UpdateAdditionalLocations(int orderRowId, IEnumerable<AdditionalLocation> additionalLocations)
        {
            //submitted by edit-forms-in-grid.js | submitUpdateAddLocsForm

            additionalLocations = _appHelper.CheckAdditionalLocationsForValidEmail(additionalLocations.ToList()).ToList();

            var orderRow = _orderManagementService.GetOrderRowById(orderRowId);


            if (additionalLocations != null)
            {
                var manageOrderEditModel = new ManageOrderEditModel
                {
                    AdditionalLocations = additionalLocations
                };

                var addLocEmails = "UpdateAdditionalLocations: " + orderRow.idOrder;
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


            PricesAndDiscounts pricesAndDiscounts = default(PricesAndDiscounts);
            _orderManagementService.UpdateOrderChanges(orderRow.Order, ref pricesAndDiscounts);

            if (pricesAndDiscounts.Discount == null)
                pricesAndDiscounts.Discount = new Discount();

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
        public ActionResult SynchOrders(int? webinarId, int? idAffiliate)
        {

            _orderManagementService.SynchOrders(webinarId.Value);
            return Json(new
            {
                data = webinarId,
                affiliate = idAffiliate,
                Success = "Success"
            });
        }

        [HandleAjaxException]
        [AllowAnonymous]
        public ActionResult SynchOrdersBatch(int? webinarId, int? idAffiliate)
        {
            var totalNumberOrders = 0;

            _orderManagementService.SynchOrders(webinarId.Value);

            return Content("Ok");
        }


        [HandleAjaxException]
        [AllowAnonymous]
        public ActionResult GenerateRoyaltiesBatch(int? webinarId, int? idAffiliate)
        {
            var totalNumberOrders = 0;

            var dtsource = _orderManagementService.GetOrdersByWebinar(webinarId.Value).ToList();

            _affiliateManagementService.BuildAffiliateReport(dtsource, webinarId.Value);

            return Content("Ok");
        }


        [AllowAnonymous]
        [HttpGet]
        public ActionResult SynchOrdersWhereLegacyIsZero(int idOrderLegacy, int idOrderV3)
        {

            _orderManagementService.SynchOrdersWhereLegacyIsZero(idOrderLegacy, idOrderV3);
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

                    var onDemandCode = order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active).OnDemandCode;
                    foreach (var claim in claimsViewModel.UserClaims.Where(c => c.Type == "http://ttstrain.com/ws/2014/01/identity/claims/DisplayPostEventMaterials"))
                    {
                        var thisClaim = JsonConvert.DeserializeObject<PostEventClaim>(claim.Value);

                        if (!ReferenceEquals(onDemandCode, null))
                        {
                            if (claim.Value.Contains(onDemandCode))
                            {
                                var needsCreatedClaim = true;
                                if (thisClaim.OrderId != order.idOrder)
                                {
                                    _logger.Warn("CheckOnDemandCodes finds Claim mismatch! Claim: {0} vs Order: {1}", thisClaim.OrderId,
                                        order.idOrder);
                                    _logger.Warn("Alert User of changed claim = '{0}' ", thisClaim.OrderId);
                                    _logger.Warn("DELETE dbo.UserClaims WHERE Value = '{0}' --Order: {1}", claim.Value, order.idOrder);

                                    //CreatePostEventClaim(order, thisClaim.ExpiryDate);
                                }
                                else
                                {
                                    needsCreatedClaim = false;
                                    _logger.Info("CheckOnDemandCodes found Match on: " + order.OrderRows.SingleOrDefault().idOrder);
                                }
                                if (needsCreatedClaim)
                                {
                                    //{"OrderId":30706,"ExpiryDate":"2015-12-23","OnDemandCode":"oybo"}
                                    _logger.Info("INSERT dbo.UserClaims ( ParentKey, Type, Value ) VALUES	( (SELECT [Key] FROM dbo.UserAccounts WHERE " +
                                                 "Email = '{0}'), 'http://ttstrain.com/ws/2014/01/identity/claims/DisplayPostEventMaterials', " +
                                                 "'{\"OrderId\":{1},\"ExpiryDate\":\"{2}\"' )", order.BillingEmail, order.idOrder, thisClaim.ExpiryDate);
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
                                _logger.Info("UPDATE dbo.OrderRow SET OnDemandCode = '{0}' WHERE idOrder ={1}", RandomHelpers.GetUniqueCode(4), order.idOrder);
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


        //[HandleAjaxException]
        //[HttpPost]
        //[AllowAnonymous]
        //public void BuildAffiliateInvoice(int? idWebinar, string aff)
        //{
        //    if (idWebinar != null)
        //    {
        //        AffiliateInvoiceDTO model = _affiliateManagementService.GetAffiliateInvoice(idWebinar.Value, aff);

        //        //JsonResult jsonresult = Json(model.Rows);
        //        //jsonresult.MaxJsonLength = int.MaxValue;  // needed if/when the data is > 4mb
        //        //string html = "";

        //        //try
        //        //{
        //        //    WebUser editingUser = null;

        //        //    html = ViewHelpers.RenderViewToString(ControllerContext,
        //        //            "~/Views/Shared/EditorTemplates/DataTablesEditorTemplates/EditOrder_Compact.cshtml",
        //        //            model, true);
        //        //}
        //        //catch (Exception ex)
        //        //{
        //        //    _logger.Fatal("GetOrderInfoForm error", ex);
        //        //}

        //        //return Json(new { success = "success" });

        //    }
        //}


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
                _logger.Fatal("GetOrderCompact error on " + idWebinar, ex);
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
                    var shouldBuildAffRpt = true;
                    // custom filtering by Webinar Id
                    if (!showAllEvents)
                    {
                        if (searchTerm != null)
                        {
                            shouldBuildAffRpt = false;
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
                                dtsource = _dataTablesService.GetOrdersByPending(affiliateId, out totalNumberOrders).ToList();
                            }
                            else if (searchTerm.StartsWith("l "))
                            {
                                dtsource = _orderManagementService.GetOrdersAll(affiliateId, out totalNumberOrders).Where(o => String.Equals(o.LastName, searchTerm.Replace("l ", ""), StringComparison.CurrentCultureIgnoreCase))
                                            .ToList();
                            }
                            else if (searchTerm == "inprocess")
                            {
                                dtsource = _dataTablesService.GetOrdersByInProcess(affiliateId, out totalNumberOrders).ToList();
                            }
                        }
                        else
                        {
                            dtsource = _dataTablesService.GetOrdersByWebinar(webinarId, affiliateId, out totalNumberOrders).ToList();
                        }
                    }
                    else
                    {
                        dtsource = _orderManagementService.GetOrdersAll(affiliateId, out totalNumberOrders).ToList();
                    }
                    if (param.Search.Value != null)
                    {
                        shouldBuildAffRpt = false;
                    }
                    if (shouldBuildAffRpt)
                        _affiliateManagementService.BuildAffiliateReport(dtsource, webinarId);

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

                    List<OrderDTO> data = new DTResultSetOrders().GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtoSource, columnSearch);
                    int count = new DTResultSetOrders().Count(param.Search.Value, dtoSource, columnSearch);


                    DataTableService<OrderDTO> result = new DataTableService<OrderDTO>
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
        [HttpGet]
        [AllowAnonymous]
        public ActionResult GenerateWeeklyInvoicesEvent(DateTime startDate)
        {
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            DateTime date1 = startDate;
            System.Globalization.Calendar cal = dfi.Calendar;

            var weekNumber = cal.GetWeekOfYear(date1, dfi.CalendarWeekRule,
                                                dfi.FirstDayOfWeek) + "-" + cal.GetYear(DateTime.Now);
            var endDate = startDate.AddDays(7);

            IList<Webinar> webinars = _webinarManagementService.GetWebinarsForWeeklyInvoices(startDate);

            var affiliates = _affiliateManagementService.GetAffiliates();

            foreach (var affiliate in affiliates)
            {
                int totalNumberOrders = 0;
                int totalNumberDiscounts = 1;
                AffiliateInvoiceDTO invoice = new AffiliateInvoiceDTO
                {
                    Affiliate = affiliate,
                    InvoiceID = weekNumber + "-" + affiliate.idUserAff,
                    RoyaltyTier = affiliate.CommissionModel

                };
                StringBuilder discountNotes = new StringBuilder();

                //had webinar orders or postevent orders ini
                var hadWOrder = false;
                var hadPOrder = false;
                var hadDiscount = false;

                int thisAffiliate = affiliate.idUserAff;

                DocumentModel document =
                    DocumentModel.Load(Server.MapPath(@"~/App_Data/mergeTemplates/WeeklyInvoice1.docx"));


                decimal GrandTotalOnBilled = 0;
                decimal GrandTotalOnPaid = 0;
                decimal GrandTotalDiscounts = 0;
                decimal GrandTotalRoyalties = 0;
                decimal GrandTotalNetDue = 0;
                var dataSource =
                    new
                    {
                        RoyaltyTier = invoice.RoyaltyTier,
                        invoice.InvoiceID,
                        DateRange = startDate.ToShortDateString() + " - " + endDate.ToShortDateString(),
                        DisplayTitle = affiliate.DisplayTitle
                    };

                document.MailMerge.Execute(dataSource);


                DataSet ds = new DataSet();
                DataSet ds1 = new DataSet();

                DataTable webinarsPerAff = new DataTable("Webinars");

                webinarsPerAff.Columns.Add("Title", typeof(string));
                webinarsPerAff.Columns.Add("WebinarDate", typeof(string));
                webinarsPerAff.Columns.Add("TotalOnBilled", typeof(string));
                webinarsPerAff.Columns.Add("TotalOnPaid", typeof(string));
                webinarsPerAff.Columns.Add("TotalDiscounts", typeof(string));
                webinarsPerAff.Columns.Add("TotalRoyalties", typeof(string));
                webinarsPerAff.Columns.Add("TotalNetDue", typeof(string));
                webinarsPerAff.Columns.Add("Id", typeof(int));
                ds.Tables.Add(webinarsPerAff);

                DataTable postEventOrders = new DataTable("PostEventOrders");

                postEventOrders.Columns.Add("Title", typeof(string));
                postEventOrders.Columns.Add("WebinarDate", typeof(DateTime));
                postEventOrders.Columns.Add("TotalOnBilled", typeof(string));
                postEventOrders.Columns.Add("TotalOnPaid", typeof(string));
                postEventOrders.Columns.Add("TotalDiscounts", typeof(string));
                postEventOrders.Columns.Add("TotalRoyalties", typeof(string));
                postEventOrders.Columns.Add("TotalNetDue", typeof(string));
                postEventOrders.Columns.Add("Id", typeof(int));
                ds1.Tables.Add(postEventOrders);

                //orders
                DataTable ordersPerAff = new DataTable("Orders");

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
                ds.Tables.Add(ordersPerAff);


                //orders
                DataTable pOrders = new DataTable("pOrders");

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
                ds1.Tables.Add(pOrders);
                // Add parent-child relation 

                ds.Relations.Add("Orders", webinarsPerAff.Columns["Id"], ordersPerAff.Columns["Id"]);
                ds1.Relations.Add("pOrders", postEventOrders.Columns["Id"], pOrders.Columns["Id"]);

                foreach (var webinar in webinars)
                {
                    try
                    {
                        List<Order> theseOrders =
                            _orderManagementService.GetOrdersByWebinar(webinar.idWebinar)
                                .Where(o => o.idAffiliate == thisAffiliate)
                                .ToList();
                        if (theseOrders.Any())
                        {
                            hadWOrder = true;
                            invoice = _affiliateManagementService.BuildAffiliateInvoice(invoice, theseOrders, webinar.idWebinar,
                                thisAffiliate);
                            int rowNumber = 0;
                            webinarsPerAff.Rows.Add(
                                webinar.Title
                                , webinar.Date.ToShortDateString()
                                , invoice.TotalOnBilled.ToString("C").Replace(".00", "")
                                , invoice.TotalOnPaid.ToString("C").Replace(".00", "")
                                , invoice.TotalDiscounts.ToString("C").Replace(".00", "")
                                , invoice.TotalRoyalties.ToString("C").Replace(".00", "")
                                , invoice.TotalNetDue.ToString("C").Replace(".00", "")
                                , webinar.idWebinar
                                );


                            foreach (var order in theseOrders)
                            {
                                string _price = order.Total.ToString("C");
                                var row = order.OrderRows.FirstOrDefault(r => r.RowStatus == OrderRowStatus.Active);
                                rowNumber++;

                                string _percent = (row.PercentPaid * 100).ToString().Replace(".00", "").Replace(".0", "") + "%";
                                var totalToShow = order.Total.ToString("c");
                                if (row.Discount != null)
                                {
                                    hadDiscount = true;
                                    var discount = row.Discount;
                                    _price = "See Note #" + totalNumberDiscounts;
                                    discountNotes.Append(totalNumberDiscounts + " " + discount.DiscountCode + ": (" +
                                                         discount.DiscountType.ToString().Replace("DiscountType.", "") +
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
                                    //discountNotes.Append(discountAmount);
                                    discountNotes.Append(Environment.NewLine);
                                }
                                ordersPerAff.Rows.Add(
                                    rowNumber
                                    , order.FirstName + ' ' + order.LastName
                                    , order.BillingEmail
                                    ,
                                    order.Institution + Environment.NewLine + order.BillingAddress + Environment.NewLine +
                                    order.BillingCity + ", " + order.BillingState + " " + order.BillingZip
                                    , _price
                                    , _percent
                                    , row.Royalty.ToString("C").Replace(".00", "")
                                    , order.OrderStatus
                                    , order.idOrder
                                    , webinar.idWebinar
                                    );
                            }
                            GrandTotalOnBilled = GrandTotalOnBilled + invoice.TotalOnBilled;
                            GrandTotalOnPaid = GrandTotalOnPaid + invoice.TotalOnPaid;
                            GrandTotalDiscounts = GrandTotalDiscounts + invoice.TotalDiscounts;
                            GrandTotalRoyalties = GrandTotalRoyalties + invoice.TotalRoyalties;
                            GrandTotalNetDue = GrandTotalNetDue + invoice.TotalNetDue;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.ErrorException("GenerateWeeklyInvoicesEvent", ex);
                    }

                }

                try
                {

                    List<Order> theseOrders =
                        _orderManagementService.GetOrdersAll(thisAffiliate, out totalNumberOrders)
                            .Where(o => o.idAffiliate == thisAffiliate)
                            .Where(
                                o =>
                                    o.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active).Webinar.Date <
                                    startDate && o.OrderDate > startDate && o.OrderDate < endDate)
                            .ToList();

                    if (theseOrders.Count() > 0)
                    {
                        hadPOrder = true;

                        invoice = _affiliateManagementService.BuildAffiliateInvoiceForPostEventOrders(invoice, theseOrders,
                            thisAffiliate);
                        int rowNumber = 0;
                        postEventOrders.Rows.Add(
                            "Post Event Orders"
                            , startDate
                            , invoice.TotalOnBilled.ToString("C").Replace(".00", "")
                            , invoice.TotalOnPaid.ToString("C").Replace(".00", "")
                            , invoice.TotalDiscounts.ToString("C").Replace(".00", "")
                            , invoice.TotalRoyalties.ToString("C").Replace(".00", "")
                            , invoice.TotalNetDue.ToString("C").Replace(".00", "")
                            , 99
                            );


                        foreach (var order in theseOrders)
                        {

                            string _price = order.Total.ToString("C");
                            var row = order.OrderRows.FirstOrDefault(r => r.RowStatus == OrderRowStatus.Active);
                            rowNumber++;
                            string _percent = (row.PercentPaid * 100).ToString().Replace(".00", "").Replace(".0", "") + "%";
                            if (row.Discount != null)
                            {
                                hadDiscount = true;
                                var discount = row.Discount;
                                _price = "See Note #" + totalNumberDiscounts;
                                discountNotes.Append(totalNumberDiscounts + " " + discount.DiscountCode + ": (" +
                                                     discount.DiscountType.ToString().Replace("DiscountType.", "") +
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
                                //discountNotes.Append(discountAmount);
                                discountNotes.Append(Environment.NewLine);
                            }
                            pOrders.Rows.Add(
                                rowNumber
                                , order.FirstName + ' ' + order.LastName
                                , order.BillingEmail
                                , order.Institution + Environment.NewLine + order.BillingAddress + Environment.NewLine +
                                order.BillingCity + ", " + order.BillingState + " " + order.BillingZip
                                , _price
                                , _percent
                                , row.Royalty.ToString("C").Replace(".00", "")
                                , order.OrderStatus
                                , order.idOrder
                                , row.Webinar.Title
                                , 99
                                );

                        }

                        GrandTotalOnBilled = GrandTotalOnBilled + invoice.TotalOnBilled;
                        GrandTotalOnPaid = GrandTotalOnPaid + invoice.TotalOnPaid;
                        GrandTotalDiscounts = GrandTotalDiscounts + invoice.TotalDiscounts;
                        GrandTotalRoyalties = GrandTotalRoyalties + invoice.TotalRoyalties;
                        GrandTotalNetDue = GrandTotalNetDue + invoice.TotalNetDue;
                    }
                }
                catch (Exception ex)
                {
                    _logger.ErrorException("GenerateWeeklyInvoicesEvent", ex);
                }

                document.MailMerge.FieldMerging += (sender, e) =>
                {
                    if (e.IsValueFound)
                    {

                        switch (e.FieldName)
                        {

                            case "Date":

                                ((Run)e.Inline).Text = ((DateTime)e.Value).ToString("dddd, MMMM d, yyyy");

                                break;


                            case "Percent":

                            case "Royalty":

                                //((Run)e.Inline).Text = ((double)e.Value).ToString("0.00");

                                break;

                        }

                    }

                    else if (e.RangeName == "Webinars" && e.FieldName == "TotalPrice")
                    {

                        // For each project calculate Total.

                        //e.Inline = new Run(e.Document, ds.Tables[e.RangeName].Rows[e.RecordNumber - 1].GetChildRows("Items").Sum(item => (double)item["Total"]).ToString("0.00"));

                        e.Cancel = false;

                    }

                };
                if (!hadWOrder)
                {

                }

                if (!hadPOrder)
                {
                    postEventOrders.Rows.Add(
                        "Post Event Orders"
                        , startDate
                        , ""
                        , ""
                        , ""
                        , ""
                        , ""
                        , 99
                        );
                    pOrders.Rows.Add(
                        0
                        , "No Orders Found"
                        , ""
                        , ""
                        , ""
                        , ""
                        , ""
                        , ""
                        , 0
                        , ""
                        , 99
                        );
                }

                document.MailMerge.Execute(ds, null);
                document.MailMerge.Execute(ds1, null);

                var grandTotalSource = new
                    {
                        GrandTotalOnBilled = GrandTotalOnBilled.ToString("C").Replace(".00", ""),
                        GrandTotalOnPaid = GrandTotalOnPaid.ToString("C").Replace(".00", ""),
                        GrandTotalDiscounts = GrandTotalDiscounts.ToString("C").Replace(".00", ""),
                        GrandTotalRoyalties = GrandTotalRoyalties.ToString("C").Replace(".00", ""),
                        GrandTotalNetDue = GrandTotalNetDue.ToString("C").Replace(".00", ""),
                        DiscountNotes = discountNotes.ToString()
                    };

                document.MailMerge.Execute(grandTotalSource);


                if (hadWOrder || hadPOrder)
                {
                    _logger.Info("begins write to file: " + invoice.InvoiceID);
                    document.Save(
                        Server.MapPath(@"~/App_Data/mergeTemplates/" + invoice.InvoiceID + ".pdf"));
                    //document.Save(
                    //    Server.MapPath(@"~/App_Data/mergeTemplates/" + weekNumber + "-" + affiliate.idUserAff + ".html"));


                    var storageCredentials = new StorageCredentials(_globalConfig.StorageAccountName, _globalConfig.StorageAccessKey);
                    var cloudStorageAccount = new CloudStorageAccount(storageCredentials, false);
                    CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();

                    // Retrieve reference to a previously created container.
                    CloudBlobContainer container = blobClient.GetContainerReference("affiliateinvoices");
                    container.CreateIfNotExists();

                    CloudBlockBlob blob = container.GetBlockBlobReference(startDate.ToShortDateString().Replace("/", "-") + "/" + invoice.InvoiceID + ".pdf");
                    blob.UploadFromFile(Server.MapPath(@"~/App_Data/mergeTemplates/" + invoice.InvoiceID + ".pdf"), FileMode.Open);

                }
            }

            var model = new GenerateWeeklyInvoicesViewModel
            {
                Webinars = webinars,
                Affiliates = affiliates.ToList(),
                PosteventOrders = null,
                AdjustedtOrders = null
            };
            return View("~/Views/Admin/GenerateWeeklyInvoices.cshtml", model);
        }


        public PartialViewResult GetWeeklyInvoicesEvent()
        {
            var model = new GenerateWeeklyInvoicesViewModel();
            {

            };

            return PartialView("~/Views/Admin/Home/_generateWeeklyInvoices.cshtml", model);
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
        public JsonResult DTDataHandlerRoyaltySummary(DTParametersOrders param)
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
                    dtsource = _dataTablesService.GetOrdersByWebinar(webinarId, affiliateId, out totalNumberOrders).OrderBy(o => o.OrderDate).ToList();

                    // use automapper to flatten out the order records, in this specific case the data 
                    //  model has circular references which cause problems with JSON serialization
                    List<OrderDTO> dtoSource = new List<OrderDTO>();
                    Mapper.Map(dtsource, dtoSource);

                    // custom filtering by Order Status
                    if (param.selectedOrderStatuses != null)
                    {
                        dtoSource = dtoSource.Where(x => param.selectedOrderStatuses.Contains(x.OrderStatus)).ToList();

                    }
                    _affiliateManagementService.BuildAffiliateReport(dtsource, webinarId);

                    List<String> columnSearch = new List<string>();
                    foreach (var col in param.Columns)
                    {
                        columnSearch.Add(col.Search.Value);
                    }

                    List<OrderDTO> data = new DTResultSetOrders().GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtoSource, columnSearch);
                    int count = new DTResultSetOrders().Count(param.Search.Value, dtoSource, columnSearch);


                    DataTableService<OrderDTO> result = new DataTableService<OrderDTO>
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

        //[HandleAjaxException]
        //[HttpPost]
        //[AllowAnonymous]
        //public JsonResult SubscriptionsDataHandler(DTParametersSubscriptions param)
        //{
        //    if (ClaimsAuthorization.CheckAccess(IdentityConstants.Access, IdentityConstants.GetGridDataFeature))
        //    {
        //        // based heavily on https://www.echosteg.com/jquery-datatables-asp.net-mvc5-server-side
        //        int totalNumberOrders = 0;
        //        int orderId = param.orderId;
        //        string searchTerm = param.searchTerm;
        //        int affiliateId = param.affiliateId ?? 19; // 19 is magic internal / house affiliate id
        //        bool showAllEvents = param.showAllEvents ?? false;

        //        List<Discount> dtsource = null;

        //        try
        //        {
        //            dtsource = _orderManagementService.GetSubscriptionsAll(affiliateId, out totalNumberOrders).ToList();

        //            // use automapper to flatten out the order records, in this specific case the data 
        //            //  model has circular references which cause problems with JSON serialization
        //            List<DiscountDTO> dtoSource = new List<DiscountDTO>();
        //            Mapper.Map(dtsource, dtoSource);

        //            List<String> columnSearch = new List<string>();
        //            foreach (var col in param.Columns)
        //            {
        //                columnSearch.Add(col.Search.Value);
        //            }

        //            List<DiscountDTO> data = new DTResultSetDiscounts.GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtoSource, columnSearch);
        //            int count = new DTResultSetDiscounts.Count(param.Search.Value, dtoSource, columnSearch);


        //            DataTableService<Discount> result = new DataTableService<Discount>
        //            {
        //                draw = param.Draw,
        //                data = data,
        //                recordsFiltered = count,
        //                recordsTotal = count
        //            };

        //            JsonResult jsonresult = Json(result);
        //            jsonresult.MaxJsonLength = int.MaxValue;  // needed if/when the data is > 4mb

        //            return jsonresult;
        //        }
        //        catch (Exception ex)
        //        {
        //            return Json(new { error = ex.Message });
        //        }
        //    }

        //    return Json(new { NotAuthorized = true });

        //}


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

        //[HandleAjaxException]
        //[HttpPost]
        //[AllowAnonymous]

        //public JsonResult GetGridUserData(DTParameters param)
        //{
        //    var aff = _affiliateManagementService.LoadByTTSDomain("bankwebinars");

        //    if (ClaimsAuthorization.CheckAccess(IdentityConstants.Access, IdentityConstants.GetGridDataFeature))
        //    {
        //        //
        //        try
        //        {
        //            var dtsource = new List<WebUser>();

        //            List<String> columnSearch = new List<string>();

        //            foreach (var col in param.Columns)
        //            {
        //                columnSearch.Add(col.Search.Value);
        //            }

        //            List<WebUser> data = new DTResultSetUsers().GetResult(param.Search.Value, param.SortOrder, param.Start, param.Length, dtsource, columnSearch);
        //            int count = new DTResultSetUsers().Count(param.Search.Value, dtsource, columnSearch);

        //            DataTableService<WebUser> result = new DataTableService<WebUser>
        //            {
        //                draw = param.Draw,
        //                data = data,
        //                recordsFiltered = count,
        //                recordsTotal = count
        //            };

        //            return Json(result);

        //        }
        //        catch (Exception ex)
        //        {
        //            return Json(new { error = ex.Message });
        //        }
        //    }

        //    return Json(new { NotAuthorized = true });

        //}


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