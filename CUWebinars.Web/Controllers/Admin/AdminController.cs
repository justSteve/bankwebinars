using System.Configuration;
using BrockAllen.MembershipReboot.Ef;
using BrockAllen.MembershipReboot.WebHost;
using CUWebinars.Business.AccountService;
using CUWebinars.Business.Constants;
using CUWebinars.Business.Core;
using CUWebinars.Business.Core.Extensions;
using CUWebinars.Business.Core.Helpers;
using CUWebinars.Business.Models;
using CUWebinars.Business.Notification;
using CUWebinars.Business.Notification.Formatters;
using CUWebinars.Business.Notification.ViewModel;
using CUWebinars.Business.Services;
using CUWebinars.Web.App_Start;
using CUWebinars.Web.Core;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Infrastructure;
using CUWebinars.Web.Infrastructure.Attributes;
using CUWebinars.Web.Infrastructure.Extensions;
using CUWebinars.Web.Membership.Email;
using CUWebinars.Web.Models;
using CUWebinars.Web.Services;
using CUWebinars.Web.ViewModel;
using DataTables.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ninject.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.ServiceModel.Syndication;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using BrockAllen.MembershipReboot;
using CUWebinars.Business.Core.Helpers;
using Microsoft.AspNet.Identity;
using Ninject.Extensions.Logging.Log4net.Infrastructure;
using Thinktecture.IdentityModel.Authorization;
using Thinktecture.IdentityModel.Authorization.Mvc;
using ClaimTypes = System.Security.Claims.ClaimTypes;
using DateTimeHelper = CUWebinars.Web.Helpers.DateTimeHelper;
using Formatting = Newtonsoft.Json.Formatting;

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
        //private bool _disposed;

        //
        // GET: /Admin/
        public ActionResult Index()
        {
            return View("~/Views/Admin/Home/Index.cshtml");
        }

        //
        // GET: /Admin/
        public AdminController(
            IMembershipService membershipService,
            ILogger logger,
            IStateService stateService,
            IWebinarManagementService webinarManagementService,
            IOrderManagementService orderManagementService,
            IDataTablesService dataTablesService,
            IAppHelper appHelper)
        {
            _membershipService = membershipService;
            _logger = logger;
            _stateService = stateService;
            _webinarManagementService = webinarManagementService;
            _orderManagementService = orderManagementService;
            _dataTablesService = dataTablesService;
            _appHelper = appHelper;
        }

        public ActionResult AddQuiz()
        {
            var addQuizEditModel = new AddQuizEditModel();
            return View(addQuizEditModel);
        }

        [HttpPost]
        public ActionResult AddQuiz(AddQuizEditModel addQuizEditModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _webinarManagementService.AddQuiz(addQuizEditModel.SelectedWebinar, addQuizEditModel.Questions);

                    return Json(new { Result = WebUiConstants.Success });
                }
                catch (Exception exception)
                {
                    _logger.ErrorException(string.Format("AddQuiz. Session | {0}",
                        _appHelper.GetUserAuditInfo()),
                        exception);

                    return Json(new { Result = WebUiConstants.Fail });
                }
            }
            return this.ModelStateJson(ModelState);
        }
        
        public PartialViewResult BatchPasswordReset()
        {
            if (ClaimsAuthorization.CheckAccess(IdentityConstants.Access, IdentityConstants.BatchPasswordResetFeature))
            {
                var batchPasswordResetViewModel = new BatchPasswordResetViewModel {UserEmails = string.Empty};
                return PartialView("~/Views/Admin/Partials/_BatchPasswordReset.cshtml", batchPasswordResetViewModel);
            }

            return PartialView("~/Views/Admin/Partials/_NotAuthorized.cshtml");
        }

        [HttpPost]
        [ValidateJsonAntiForgeryToken(Order = 0)]
        [HandleAjaxException(Order = 1)]
        public ActionResult BatchPasswordReset(BatchPasswordResetViewModel batchPasswordResetViewModel)
        {
            const string CommaDelimiter = "|,|";
            const string SemiColonDelimiter = "|;|";

            if (ModelState.IsValid)
            {
                try
                {
                    var emailsPlusPasswordsCollection = 
                        batchPasswordResetViewModel.UserEmails.Split(new[] {SemiColonDelimiter}, StringSplitOptions.RemoveEmptyEntries);

                    var membershipRebootConfiguration = MembershipRebootConfigInert.Create(
                        HttpRuntime.AppDomainAppPath,
                        _stateService
                        );

                    var userAccountService = new UserAccountService(
                        membershipRebootConfiguration,
                        new DefaultUserAccountRepository(new DefaultMembershipRebootDatabase())
                        );

                    var dataOperations = new DataOperations(ConfigurationManager.ConnectionStrings["MembershipReboot"].ConnectionString);


                    foreach (var emailPwdPairing in emailsPlusPasswordsCollection)
                    {
                        if(string.IsNullOrWhiteSpace(emailPwdPairing))
                            continue;

                        var pairingAsArray = emailPwdPairing.Split(
                            new[] {CommaDelimiter},
                            StringSplitOptions.RemoveEmptyEntries
                            );

                        var email = pairingAsArray[0].Trim();
                        var password = pairingAsArray[1].Trim();

                        var userAccount = _membershipService.GetUserAccountByEmail(_globalConfig.Tenant, email);

                        if (ReferenceEquals(null, userAccount))
                        {
                            throw new NullReferenceException(DomainConstants.UserNotFound);
                        }

                        dataOperations.SetFieldsConsistantWithVerifiedUser(userAccount);

                        userAccountService.ResetPassword(_globalConfig.Tenant, email);

                        var verificationKey = _stateService.GetValue<string>(DomainConstants.VerificationKeyForBatchChangePwd);

                        userAccountService.ChangePasswordFromResetKey(verificationKey, password);

                        _stateService.ClearValue(DomainConstants.VerificationKeyForBatchChangePwd);
                    }
                    return Json(new {Result = WebUiConstants.Success});
                }
                catch (Exception exception)
                {
                    _logger.ErrorException(string.Format("BatchPasswordReset | Session {0}", _appHelper.GetUserAuditInfo()), exception);
                    ModelState.AddModelError(string.Empty, string.Format("There was an error at the server. The exception message is {0}", exception.Message));
                }
            }

            return this.ModelStateJson(ModelState);
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

        public ActionResult UpdateOrderAffiliate(int? idAffiliate, int? idOrder)
        {
            var order = _orderManagementService.GetOrderByIdThin(idOrder.Value);
            order.idAffiliate = idAffiliate.Value;
            //_orderManagementService.SaveChanges();

            var originalAffiliate = new CUWebinars.Business.Repository.AffiliateRepository().FindByIdWithIncluding(order.idAffiliate);
            var newAffiliate = new CUWebinars.Business.Repository.AffiliateRepository().FindByIdWithIncluding(idAffiliate.Value);
            //            var orderUser = _membershipService.GetUserByEmail(order.BillingEmail);

            string buildMessage = "<div class=\"affiliateChanged\">Affiliate changed for order " + order.idOrder + " from " + originalAffiliate.ttsDomain + " to " +
                                  newAffiliate.ttsDomain + " by " + User.Identity.Name +
                                  " on " + TtsConfig.UtcNowAsCts.ToShortDateString() +
                                  "</div>";

            if (!ReferenceEquals(null, order))
            {
                //following copies pattern found at WebinarController | Identify
                JObject existingJObject = null;

                string comments = string.Empty;


                if (!ReferenceEquals(null, order.AdminComments))
                {
                    comments = order.AdminComments.Trim();
                }

                var newJson = new JProperty(string.Concat("ChangeAffiliate-", TtsConfig.UtcNowAsCts.ToString(DomainConstants.DateTimeLongFormat)),
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

                _orderManagementService.SaveChanges();

            }

            _logger.Info(buildMessage);


            return Json(new { Result = WebUiConstants.Success });
        }

        public ActionResult ManageOrder()
        {
            return View();
        }

        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken(Order = 0)]
        [HandleAjaxException(Order = 1)]
        public ActionResult ManageOrder(ManageOrderEditModel model)
        {
            if (ModelState.IsValid)
            {
                var newOrder = ApplyModelChangesToOrder(model);
                var oldOrder = model.Order;

                //var orderchanges = new OrderChanges
                //{

                //}

                //model.PostEventAccessExpires = _membershipService.SetPostEventAccessExpireyDate(userAccount, id.Value);

                _orderManagementService.UpdateOrderByAdmin(newOrder);
            }

            return Json(new { Result = WebUiConstants.Success });
        }

        public ActionResult ManageOrderFromDetails(int? id)
        {
            if (id.HasValue)
            {
                if (id == 0)
                    return RedirectToAction("ManageOrder");

                var model = BuildManageOrderEditModel(id.Value);
                var userAccount = _membershipService.GetUserAccountByEmail(_globalConfig.Tenant, _orderManagementService.GetOrderById(id.Value).WebUser.email);
                if (userAccount == null) throw new NullReferenceException(string.Format("UserAccount does not exist in system for OrderId {0}", id.Value));

                model.PostEventAccessExpires = _membershipService.GetPostEventAccessExpireyDate(userAccount, id.Value);
                return View(model);
            }

            //  should never reach here as RouteConfig will not route here with anything but an integer > 0.
            throw new NullReferenceException("Query string parameter has to be an positive integer for the ManageOrderFromDetails action.");
        }

        private Order ApplyModelChangesToOrder(ManageOrderEditModel model)
        {
            var order = _orderManagementService.GetOrderById(model.Id);
            var orderRow = order.OrderRows.Single(o => o.RowStatus == OrderRowStatus.Active);

            order.Total = model.DisplayRowPriceViewModel.PricesAndDiscounts.TotalOrderPrice;
            order.OrderStatus = model.DisplayRowPriceViewModel.OrderStatus;

            orderRow.RowPrice = order.Total;
            orderRow.UnitPrice = model.DisplayRowPriceViewModel.PricesAndDiscounts.UnitPrice;

            SyncAdditionalLocations(model, orderRow);

            return order;
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
                orderRow.AdditionalLocation.Add(addedAdditionalLocation);
                _orderManagementService.AddAdditionalLocation(addedAdditionalLocation);
            }
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult Delete(int idOrder)
        {

            return null;
        }


        [System.Web.Mvc.HttpPost]
        public ActionResult UnDelete(int idOrder)
        {

            return null;
        }

        [System.Web.Mvc.HttpPost]
        [ValidateJsonAntiForgeryToken(Order = 0)]
        [HandleAjaxException(Order = 1)]

        public ActionResult SetUserAssignedToOrder(int orderID, string migrateOrder, int targetUserID)
        {
            try
            {
                if (targetUserID < 2)
                {
                    TempData["EditResult"] = "Invalid UserID";
                    TempData["alertType"] = "alert-error";
                    return RedirectToAction("Edit", new { ID = orderID });
                }

                var order = _orderManagementService.GetOrderByIdThin(orderID);
                WebUser webUser = _membershipService.GetWebUserById(targetUserID);


                if (ReferenceEquals(null, webUser))
                {
                    return Json(new { Result = WebUiConstants.Fail, Reason = "No user with that Id exists in our system." });
                }

                var linkToNewUser = string.Concat(
                    "/account/edituser/",
                    targetUserID,
                    "?returnUrl=/Admin/ManageOrder/",
                    orderID
                    );

                var email = webUser.email;
                var fullName = string.Concat(webUser.FirstName, " ", webUser.LastName);

                Debug.Assert(webUser.Addresses.Single(a => a.AddressType == WebUiConstants.BillingAddress) != null, "There must be at least a Billing Address or we have bad data.");

                var phone = webUser.Addresses.Single(a => a.AddressType == WebUiConstants.BillingAddress).Phone;

                if (migrateOrder.Equals("moveAll", StringComparison.OrdinalIgnoreCase))
                {
                    IList<Order> ordersToMove = _orderManagementService.FindOrdersByUserId(order.idUser).ToList();

                    for (int j = 0; j < ordersToMove.Count; j++)
                    {
                        ordersToMove[j].BillingEmail = webUser.email;
                        ordersToMove[j].idUser = targetUserID;
                        ordersToMove[j].FirstName = webUser.FirstName;
                        ordersToMove[j].LastName = webUser.LastName;
                    }

                    _orderManagementService.SaveChanges();

                    //var i = 0;
                    //string numOrdersMigrated = string.Empty;
                    //string emailOfTargetUser = string.Empty;
                    //foreach (var order in ordersToMove)
                    //{
                    //    i++;
                    //    TempData["migratingOrder"] += TempData["migratingOrder"] + "Migrating " + order.idOrder;
                    //    //intent of next line not clear
                    //    //if (!UserAssignedToOrder(order.idOrder, newUserID)) return RedirectToAction("Edit", new { ID = orderID });
                    //    emailOfTargetUser = order.BillingEmail;
                    //    TempData["migratingOrder"] += TempData["migratingOrder"] + " was successful. ";
                    //}
                    //if (i == 1)
                    //{
                    //    numOrdersMigrated = "One order was ";
                    //}
                    //else
                    //{
                    //    numOrdersMigrated = i + " orders were ";
                    //}
                }
                else
                {
                    order.idUser = targetUserID;
                    order.BillingEmail = webUser.email;
                    order.FirstName = webUser.FirstName;
                    order.LastName = webUser.LastName;
                    _orderManagementService.SaveChanges();
                }

                return
                    Json(
                        new
                        {
                            Result = WebUiConstants.Success,
                            NewHref = linkToNewUser,
                            Email = email,
                            FullName = fullName,
                            Phone = phone
                        });
            }
            catch (Exception exception)
            {
                _logger.ErrorException(string.Format("SetUserAssignedToOrder. Session | {0}", _appHelper.GetUserAuditInfo()), exception);
            }

            return
                Json(
                    new
                    {
                        Result = WebUiConstants.Fail,
                        Reason = "There has been an error at the server. Please call us at 800-831-0678 ext. 3 to resolve."
                    });
        }


        public ActionResult ClaimsManagement()
        {
            var frameworkTypesAsSelectList = GetClaimTypesFromClass(typeof(ClaimTypes), ConstantType.Constant);
            var customTypesAsSelectList = GetClaimTypesFromClass(typeof(Business.Constants.ClaimTypes), ConstantType.Readonly);

            var model = new AddClaimInputModel
            {
                ClaimTypes = frameworkTypesAsSelectList,
                TtsClaimTypes = customTypesAsSelectList
            };

            return View(model);
        }

        [System.Web.Mvc.HttpPost]
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
                        Business.Constants.ClaimTypes.DisplayPostEventMaterials, StringComparison.OrdinalIgnoreCase))
                    {
                        var result = _membershipService.ValidatePostEventMaterialsAccessClaimValue(
                            model.NewClaimValue,
                            model.NewClaimType
                            );

                        if (result.IsValid)
                        {
                            var claimArray = model.NewClaimValue.Split(':');
                            var order = _orderManagementService.GetOrderById(int.Parse(claimArray[0]));
                            var onDemandCode = order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).OnDemandCode;

                            var orderIdProperty = new JProperty(JsonPropertyKeys.OrderId, order.idOrder);
                            var expiryDateProperty = new JProperty(JsonPropertyKeys.ExpiryDate, DateTime.Parse(claimArray[1]));
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
                catch (Exception exception)
                {

                }
            }

            return this.ModelStateJson(ModelState);
        }

        public ActionResult GenerateClickToJoinForAdHocCaller()
        {
            var generateClickToJoinViewModel = new GenerateClickToJoinViewModel();

            return View("GenerateClickToJoin", generateClickToJoinViewModel);
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

                    newOrderRow.idWebinar = generateClickToJoinViewModel.WebinarId.Value;
                    newOrderRow.idRegType = 1;
                    _orderManagementService.LoadWebinarIntoOrderRow(newOrderRow);


                    var affiliate = _stateService.GetValue<Affiliate>(WebUiConstants.CurrentAffiliate);
                    _orderManagementService.SetUserStatusToUnChanged(user);
                    //_orderManagementService.SetAffiliateStatusToUnChanged(affiliate);


                    _orderManagementService.CreateNewOrder(
                        affiliate.idUserAff,
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
                _logger.ErrorException(string.Format("GetClaimsForUser. Session | {0}", _appHelper.GetUserAuditInfo()), exception);
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

        public ActionResult GetOrderDetails(int? id = null)
        {
            if (id.HasValue)
            {
                try
                {
                    var manageOrderEditModel = BuildManageOrderEditModel(id.Value);

                    return PartialView("~/Views/Admin/Home/_OrderEditDetails.cshtml", manageOrderEditModel);
                }
                catch (Exception exception)
                {
                    _logger.ErrorException(string.Format("GetOrderDetails | Session {0}", _appHelper.GetUserAuditInfo()), exception);
                    return new HttpStatusCodeResult(500); // if reach here, we are in error state.
                }
            }

            _logger.Error("The value posted to the server was not a valid interger. GetOrderDetails {0}", _appHelper.GetUserAuditInfo());

            return new HttpStatusCodeResult(500, "The value posted to the server was not a valid integer."); // if reach here, we are in error state.
        }

        private ManageOrderEditModel BuildManageOrderEditModel(int id)
        {
            var order = _orderManagementService.GetOrderById(id);
            var orderRow = order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active);
            var additionalLocationsPricing =
                _orderManagementService.GetCostOfAdditionalLocations(
                    orderRow.AdditionalLocation,
                    orderRow.idWebinar
                    );
            var additionalLocations = orderRow.AdditionalLocation;
            var additionalLocationsCount = additionalLocations.Count;

            var manageOrderEditModel = new ManageOrderEditModel
            {
                AdditionalLocations = additionalLocations,
                AdditionalLocationsAvailableOnLoad = false, // see below for where this is properly decided.
                AdditionalLocationsRenderer = GetRenderer(additionalLocations.Select(al => al.Email).ToList()),
                CostPerAdditionalLocation = additionalLocationsPricing.Item2,
                DisplayOptionsInDropDownViewModel = new DisplayOptionsInDropDownViewModel
                {
                    Options = _orderManagementService.GetAllPossibleOptionsByWebinarId(orderRow.idWebinar, false),
                    OrderRowId = orderRow.idOrderRow,
                    OrderRowRegistrationType = orderRow.RegistrationType
                },
                DisplayRowPriceViewModel = new DisplayRowPriceViewModel
                {
                    Discount = orderRow.Discount,
                    NumberOfAdditionalLocations = additionalLocationsCount,
                    OrderStatus = order.OrderStatus,
                    Price = orderRow.RegistrationType.Price,
                    PricesAndDiscounts =
                        _orderManagementService.CalculateOrderCost(order, additionalLocationsPricing.Item2),
                    RegistrationType = orderRow.RegistrationType,
                    RowPrice = orderRow.RowPrice
                },
                Id = order.idOrder,
                AffiliateName = _appHelper.GetAffiliateName(order.idAffiliate),
                JoinCode = orderRow.TtsJoinUrl,
                OnDemandCode = orderRow.OnDemandCode,
                Order = order,
                NumberOfAdditionalLocations = additionalLocationsCount,
                PhoneNumber = order.BillingPhone,
                UserId = order.idUser,
                WebinarId = orderRow.idWebinar,
                WebUser = order.WebUser
            };

            // Need to decide whether the selected option in the DropDownList can add additional locations
            var regType = manageOrderEditModel.DisplayOptionsInDropDownViewModel.OrderRowRegistrationType;

            {
                var option = CheckIfAddLocAvailable(regType.idRegType);
                if (option.HasValue)
                {
                    manageOrderEditModel.AdditionalLocationsAvailableOnLoad = option.Value;
                }
            }
            return manageOrderEditModel;
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

        public ActionResult GetOrdersByEmailDomain(string email)
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


                var results = _orderManagementService.GetOrdersByEmailDomain(email, aff).Select(o =>
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

        public ActionResult GetOrdersByLastName(string lastName)
        {
            if (!string.IsNullOrWhiteSpace(lastName))
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

            return Json(new { Error = WebUiConstants.NullValueParameter });
        }


        private TagBuilder GetRenderer(IList<string> emailAddresses)
        {
            const string locationsSpanPrefix = "LocationSpan-";
            const string breakSuffix = "-break";
            const string additionalLocationDeleteSuffix = "-AdditionLocationEmail-delete";
            const string additionalLocationEmailPrefix = "AdditionalLocationEmail-";
            const string nonBreakingSpace = "&nbsp;";
            var stringBuilder = new StringBuilder();


            var spanBuilder = new TagBuilder("span");
            var inputBuilder = new TagBuilder("input");
            var iconBuilder = new TagBuilder("i");

            /***************** The generataed element will look like this: *****************/
            /*
                <span id="LocationSpan-0">
                   <input aria-describedby="AdditionalLocationEmail_0-error" aria-invalid="false" class="valid" id="AdditionalLocationEmail-0" name="AdditionalLocations[0].Email" placeholder="Enter email address" type="email" value="test@yahoo.com">&nbsp;
                   <i class="icon-white icon-trash" id="0-AdditionLocationEmail-delete" style="cursor: pointer"></i>
                   <br id="0-break">
                </span>             
             */

            if (emailAddresses.Any())
            {
                for (var i = 0; i < emailAddresses.Count; i++)
                {
                    spanBuilder = new TagBuilder("span");
                    inputBuilder = new TagBuilder("input");
                    iconBuilder = new TagBuilder("i");

                    inputBuilder.GenerateId(additionalLocationEmailPrefix + i);
                    inputBuilder.MergeAttributes(new Dictionary<string, string>
                    {
                        {"name", "AdditionalLocations[" + i + "].Email"},
                        {"type", "email"},
                        {"placeholder", "Enter email address"},
                        {"aria-invalid", "false"},
                        {"aria-describedby", "AdditionalLocationEmail_" + i + "-error"},
                        {"value", emailAddresses[i]},
                    });
                    inputBuilder.AddCssClass("valid");

                    iconBuilder.AddCssClass("icon-trash");
                    iconBuilder.AddCssClass("icon-white");
                    iconBuilder.MergeAttribute("style", "cursor: pointer");
                    iconBuilder.MergeAttribute("id", string.Concat(i, additionalLocationDeleteSuffix));

                    spanBuilder.GenerateId(locationsSpanPrefix + i);
                    spanBuilder.InnerHtml = string.Concat(
                        inputBuilder.ToString(TagRenderMode.SelfClosing),
                        nonBreakingSpace,
                        iconBuilder.ToString(TagRenderMode.Normal),
                        "<br id=" + i + breakSuffix + ">"
                        );

                    stringBuilder.Append(spanBuilder.ToString(TagRenderMode.Normal));
                }
            }
            else
            {
                spanBuilder = new TagBuilder("span");
                spanBuilder.GenerateId("noLocationsText");
                spanBuilder.InnerHtml = "No additionalLocations yet";
                spanBuilder.AddCssClass("text");
                spanBuilder.AddCssClass("text-info");
            }


            var div = new TagBuilder("div");
            div.GenerateId("collectAdditionalLocations");
            div.AddCssClass("addLocsBox");
            div.InnerHtml = emailAddresses.Any() ? stringBuilder.ToString() : spanBuilder.ToString(TagRenderMode.Normal);

            return div;

        }

        public PartialViewResult ResendOrderConfirmation()
        {
            var model = new ResendOrderInformationViewModel
            {
                OrderId = string.Empty
            };

            return PartialView("~/Views/Admin/Home/_resendOrderConfirmation.cshtml", model);
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult ResendOrderConfirmation(int orderId)
        {
            var order = _orderManagementService.GetOrderById(orderId);

            if (ReferenceEquals(null, order))
            {
                return Json(new { Result = WebUiConstants.Fail });
            }

            var orderRow = order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active); // perf bump by assigning to local variable

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

        public PartialViewResult ResendConnectionInfo()
        {
            var model = new ResendOrderInformationViewModel
            {
                OrderId = string.Empty
            };

            return PartialView("~/Views/Admin/Home/_resendConnectionInfo.cshtml", model);
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult ResendConnectionInfo(int orderId)
        {
            var order = _orderManagementService.GetOrderById(orderId);

            if (ReferenceEquals(null, order))
            {
                return Json(new { Result = WebUiConstants.Fail });
            }

            var orderRow = order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active); // perf bump by assigning to local variable

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

            _orderManagementService.FireSendConnectionInfoNotificationEvent(new[] { order }, resending: true);

            return Json(new { Result = WebUiConstants.Success });
        }

        public PartialViewResult SendAdhocEvent()
        {
            var model = new AdhocNotificationViewModel
            {
                Webinars = EventInvokerHelpers.GetUpcomingWebinarsAsSelectListItems(_webinarManagementService)
            };

            return PartialView("~/Views/Admin/Home/_adHocNotification.cshtml", model);
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult SendAdhocEvent(int webinarId)
        {
            var regTypes = EventInvokerHelpers.GetRegTypesForWebinarAsSelectListItems(webinarId, _webinarManagementService);

            return Json(regTypes);
        }

        public PartialViewResult SendReminder()
        {
            var model = new AdhocNotificationViewModel
            {
                Webinars = EventInvokerHelpers.GetUpcomingWebinarsAsSelectListItems(_webinarManagementService)
            };

            return PartialView("~/Views/Admin/Home/_SendReminder.cshtml", model);
        }

        [System.Web.Mvc.HttpPost]
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

        [System.Web.Mvc.HttpPost]
        public JsonResult SendConnectionInfo(int webinarId)
        {
            //_logger.Info("Begins SendConnectionInfo");
            var orders = _orderManagementService.GetOrdersForLiveNotifications(webinarId);

            foreach (var order in orders)
            {
                var orderRow = order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active); // perf bump by assigning to local variable
                //need to refine logic in the CitrixJoinInfoAvailable a little bit.
                if (string.IsNullOrEmpty(orderRow.CitrixJoinUrl))// && orderRow.Webinar.CitrixJoinInfoAvailable())
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

        private IEnumerable<Order> GetOrdersWhichAreEligibleForMaterials(IEnumerable<Order> orders)
        {
            IList<Order> eligableOrders = new Order[0];
            foreach (var order in orders)
            {
                var userAccount = _membershipService.GetUserAccountByEmail(_globalConfig.Tenant, order.WebUser.email);

                DateTime? expiryDate = _membershipService.GetPostEventAccessExpireyDate(userAccount, order.idOrder);

                if (expiryDate.HasValue && expiryDate > TtsConfig.UtcNowAsCts)
                {
                    eligableOrders.Add(order);
                }
            }
            return eligableOrders;
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

        [System.Web.Mvc.HttpPost]
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
            var firstRetrievedOrderForWebinar = _orderManagementService.GetOrdersForRecordedNotifications(id).FirstOrDefault(
                    o => o.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).RegistrationType.ShowLiveNotifications.ToLower() == "yes"
                );

            if (ReferenceEquals(null, firstRetrievedOrderForWebinar))
                return File("<html>fail</html>".GenerateStreamFromString(), HtmlMimeType);


            var formatter = new PreviewFormatter(new EnvironmentInformation { BaseUrl = HttpRuntime.AppDomainAppPath });

            return File(formatter.FormatToString(firstRetrievedOrderForWebinar, "PreviewRecordingPosted").GenerateStreamFromString(), HtmlMimeType);
        }

        public ActionResult PreviewConnectionInfo(int id)
        {
            //  id is a WebinarId
            var firstRetrievedOrderForWebinar = _orderManagementService.GetOrdersForLiveNotifications(id)
                .FirstOrDefault(
                    o => o.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).RegistrationType.ShowLiveNotifications.ToLower() == "yes"
                );

            if (ReferenceEquals(null, firstRetrievedOrderForWebinar))
                return File("<html>fail</html>".GenerateStreamFromString(), HtmlMimeType);

            var formatter = new PreviewFormatter(new EnvironmentInformation { BaseUrl = HttpRuntime.AppDomainAppPath });

            return File(formatter.FormatToString(firstRetrievedOrderForWebinar, "PreviewConnectionInfo").GenerateStreamFromString(), HtmlMimeType);
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


        [HttpPost]
        public JsonResult PromoGenerate(WebinarPromoViewModel model)
        {
            model.Webinars = _webinarManagementService.GetUpcomingWebinars().ToList();

            var includedAffiliates = Request.Params.AllKeys
                  .Where(x => x.StartsWith("cb_"))
                  .Where(x => Request.Params[x] != null && Request.Params[x].ToString().Length > 0)
                  .Select(x => new { key = x.Replace(@"cb_", ""), value = Request.Params[x] })
                  .ToDictionary(x => int.Parse(x.key), x => x.value);

            //if (UserFacade.Instance.GetCurrentUser().UserType == UserType.Affiliate)
            //{
            //    includedAffiliates.Clear();
            //    includedAffiliates.Add(UserFacade.Instance.GetCurrentUser().ID, "on");

            //}
            int i = model.Webinar.idWebinar;
            model.SendDate = Convert.ToDateTime(model.SendDate);

            //model.GeneratedMessages = new List<AffPromoMsgViewModel>();
            foreach (var aff in includedAffiliates)
            {
                if (aff.Value != "on") break;

                model.TimeZone = USTimeZone.Central; // UserFacade.Instance.Load(aff.Key).TimeZone;
                model.Affiliate = new CUWebinars.Business.Repository.AffiliateRepository().FindByIdWithIncluding(Convert.ToInt32(aff.Key));
                model.Webinar = _webinarManagementService.GetWebinar(i);
                model.From = model.Affiliate.ContactEmail;
                model.Affiliates = new List<Affiliate>();

                IEnumerable<int> featured1 = AppHelper.StringToIntList(Request.Form["listOfEvents"]);

                StringBuilder upcoming = new StringBuilder();
                int i1 = 0;

                var q = from a in model.Webinars.Take(15)
                        where a.idWebinar != model.Webinar.idWebinar && a.Date > model.SendDate
                        //where !(list2.Any(item2 => item2.Email == item1.Email))
                        orderby a.Date
                        select new
                        {
                            featuredItem =
                         "<p><a style=\"color: bisque; text-decoration: none; border-bottom: 1px dotted bisque;\" href=\"http://www.bankwebinars.com/Webinar/Details/" +
                         a.idWebinar + "?idaff=" + model.Affiliate.idUserAff + "\">" + a.Title + "</a><br><font size='-3'> (" + a.Date.ToLongDateString() + ")</font></p>"
                        };

                string wDate = "<b>" + DateTimeHelper.FormatDate(model.Webinar.Date) + "</b><br>" +
                               DateTimeHelper.FormatTimeWithDuration(model.Webinar.Date, model.TimeZone, false,
                                                                     model.Webinar.Duration) + "<br>";

                string ceu = "";
                if (String.IsNullOrEmpty(model.Webinar.ceu) == false)
                {
                    string[] ceufull = model.Webinar.ceu.Split('|');
                    ceu = ceufull[0].ToString();
                }

                IEnumerable<int> featured = AppHelper.StringToIntList(model.UpcomingListing);
                StringBuilder upcomingDetail = new StringBuilder();

                if (Request.Form["TemplateVersion"] == "Daily")
                {
                    foreach (var ID in q)
                    {
                        if (i1 < 5)
                        {
                            upcoming.Append(ID.featuredItem.ToString());
                        }
                        i1++;
                    }

                    model.UpcomingListing = "<h3>Upcoming Webinars</h3>" + upcoming.ToString();

                    model.EventBody = model.Webinar.DescriptionLong + "<h2>" + model.Webinar.LearnCaption + "</h2>" + model.Webinar.LearnBody + "<h2>Who Should Attend</h2>" + model.Webinar.WhoAttend + "<h2>" + "About " + model.Webinar.Presenter.WebUser.FullName + "</h2>" + model.Webinar.Presenter.BiographyLong;

                    _orderManagementService.FireSendPerDayPromoEvent(model);
                    var genMsg = new AffPromoMsgViewModel(model.Affiliate.idUserAff, "string here");
                    //if (model.GeneratedMessages != null) model.GeneratedMessages.Add(genMsg);
                }
                //else
                //{
                //    List<int> featureWebinarIDs = new List<int>();

                //    foreach (int w in featured)
                //    {
                //        var webinar = _webinarManagementService.GetWebinar(w);
                //        featureWebinarIDs.Add(webinar.idWebinar);
                //        string eDate = "<b>" + DateTimeHelper.FormatDate(webinar.Date) + "</b><br />";
                //        eDate = eDate + DateTimeHelper.FormatTimeWithDuration(webinar.Date, model.TimeZone, false, webinar.Duration) + "<br />";

                //        upcomingDetail.Append("<p style='font-size:20px; font-weight:bold; font-family:trebuchet ms;'><a href='http://www.bankwebinars.com/Webinar/Details/" + webinar.idWebinar + "?idaff=" + model.Affiliate.idUserAff + "'>" + webinar.Title + "</a><br />");
                //        upcomingDetail.Append("<span style='font-size:12px'>" + eDate + "</span></p>");
                //        upcomingDetail.Append("<div style='font-family:trebuchet ms;'>" + webinar.Description + "</div>");
                //        upcomingDetail.Append("<p style='font-family:trebuchet ms;'><b>" + webinar.Presenter.WebUser.FullName + "</b></p>");
                //        upcomingDetail.Append("<p font-family:trebuchet ms;'><a href='http://www.bankwebinars.com/Webinar/Details/" + webinar.idWebinar + "?idaff=" + model.Affiliate.idUserAff + "'>Click here for more info!</a></p>");
                //        upcomingDetail.Append("<hr style='width:50%; height: 10px;' />");
                //    }

                //    q = model.Webinars.Where(a => a.idWebinar != model.Webinar.idWebinar && a.Date > model.SendDate).Where(
                //            a => !(featureWebinarIDs.Any(item2 => item2 == a.idWebinar))).OrderBy(a => a.Date).Take(12).Select(a =>
                //            new
                //            {
                //                featuredItem =
                //                "<p><a style=\"color: bisque; text-decoration: none; border-bottom: 1px dotted bisque;\" href=\"http://www.bankwebinars.com/Webinar/Details/" +
                //                a.idWebinar + "?idaff=" + model.Affiliate.idUserAff + "\">" + a.Title + "</a><br><font size='-3'> (" +
                //                a.Date.ToLongDateString() + ")</font></p>"
                //            });

                //    foreach (var ID in q)
                //    {
                //        if (i1 < 5)
                //        {
                //            upcoming.Append(ID.featuredItem.ToString());
                //        }
                //        i1++;
                //    }
                //    model.UpcomingListing = "<h3>Upcoming Webinars</h3>" + upcoming.ToString();

                //    model.EventBody = upcomingDetail.ToString();
                //    var genMsg = new AffPromoMsgViewModel(model.Affiliate.idUserAff, "mail msg");
                //    if (model.GeneratedMessages != null) model.GeneratedMessages.Add(genMsg);
                //}
            }
            return null;
        }
        [HttpGet]
        public ActionResult PerWeekPromo(int id)
        {
            ViewBag.NumberOfOrders = _orderManagementService.GetNumberOfOrdersPerWebinar(id);

            WebinarPromoViewModel model = new WebinarPromoViewModel()
            {

            };
            model.SendDate = TtsConfig.UtcNowAsCts;
            model.Webinars = _webinarManagementService.GetUpcomingWebinars().ToList();
            model.Webinar = _webinarManagementService.GetWebinar(id);
            model.TimeZone = USTimeZone.Eastern;
            model.Affiliates = new CUWebinars.Business.Repository.AffiliateRepository().GetAffiliatesByPromoType("Daily").ToList();

            return View(model);

        }

        [HttpGet]
        public ActionResult PerDayPromo(int id)
        {
            WebinarPromoViewModel model = new WebinarPromoViewModel()
            {

            };
            model.SendDate = TtsConfig.UtcNowAsCts;
            model.Webinars = _webinarManagementService.GetUpcomingWebinars().ToList();
            model.Webinar = _webinarManagementService.GetWebinar(id);
            model.TimeZone = USTimeZone.Eastern;
            model.Affiliates = new CUWebinars.Business.Repository.AffiliateRepository().GetAffiliatesByPromoType("Daily").ToList();
            //model.Affiliates = AffiliateFacade.Instance.FindNonDailyPromoSubscribers();

            return View(model);

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

        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogInAsUser(LogInAsOtherUserViewModel model)
        {
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

                _membershipService.AddClaim(impersonatedUserAccount,Business.Constants.ClaimTypes.BeingImpersonated, adminUserEmail);
                _membershipService.LogOutUser();

                //_stateService.SetValue(WebUiConstants.AdminUserEmail, adminUserEmail);
                //   TODO: Remove password challenge -- 
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

        [System.Web.Mvc.HttpGet]
        public ActionResult SynchWebinars()
        {

            _webinarManagementService.SynchToLegacy();

            return View();
        }


        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken(Order = 0)]
        [HandleAjaxException(Order = 1)]
        public ActionResult ManualPasswordReset(ManualPasswordResetViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _membershipService.CleanUser(_globalConfig.Tenant, model.Email, model.NewPassword);
                    return Json(new { Result = WebUiConstants.Success });
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

            return this.ModelStateJson(ModelState);
        }

        [System.Web.Mvc.AllowAnonymous]
        public PartialViewResult PasswordResetOperation()
        {
            var resetPasswordModel = new ResetPasswordModel
            {
                Email = string.Empty,
                EmailSent = false
            };

            return PartialView("~/Views/Admin/Home/_ResetPasswordPartial.cshtml", resetPasswordModel);
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        public JsonResult ResetPassword(string email)
        {
            var globals = GlobalConfig.GlobalConfigSingleton;

            try
            {
                _membershipService.ResetPassword(globals.Tenant, email);
                return Json(new { Result = WebUiConstants.Success });
            }
            catch (Exception exception)
            {
                _logger.Error("ResetPassword : {0}", exception.Message);
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

        [System.Web.Mvc.HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult EmailOrder(int? id, string emails)
        {
            if (id.HasValue)
            {
                try
                {
                    var order = _orderManagementService.GetOrderById(id.Value);

                    if (emails.Contains(","))
                        _orderManagementService.FireAdminEmailSendShippedOrderEvent(order, emails.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
                    else if (emails.Contains(";"))
                        _orderManagementService.FireAdminEmailSendShippedOrderEvent(order, emails.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
                    else
                        _orderManagementService.FireAdminEmailSendShippedOrderEvent(order, new string[] { emails }); // this is where a single email is specified


                    return Json(new
                    {
                        Result = WebUiConstants.Success,
                        Msg = string.Format("<p>Your Order ID is {0}. Please check your email for connection information for the webinar.</p>", id)
                    });
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, "There was a problem at the server. Please contact the administrator.");
                    _logger.ErrorException("ConfirmOrder|ConfirmOrder failed ", exception);
                    Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                }
                return this.ModelStateJson(ModelState);
            }

            return Json(new
            {
                Result = WebUiConstants.Fail,
                Msg = "You need to select an Order from the DropDownList"
            });
        }

        [System.Web.Mvc.HttpPost]
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
                            o => o.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).RegistrationType.ShowLiveNotifications.ToLower() == "yes"
                        );

                    if (emails.Contains(","))
                        _orderManagementService.FireAdminEmailConnectionInfoHandler(firstRetrievedOrderForWebinar, emails.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
                    else if (emails.Contains(";"))
                        _orderManagementService.FireAdminEmailConnectionInfoHandler(firstRetrievedOrderForWebinar, emails.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
                    else
                        _orderManagementService.FireAdminEmailConnectionInfoHandler(firstRetrievedOrderForWebinar, new string[] { emails }); // this is where a single email is specified

                    return Json(new
                    {
                        Result = WebUiConstants.Success,
                        Msg = string.Format("<p>Your Order ID is {0}. Please check your email for connection information for the webinar.</p>", id)
                    });
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, "There was a problem at the server. Please contact the administrator.");
                    _logger.ErrorException("ConfirmOrder|ConfirmOrder failed ", exception);
                    Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                }
                return this.ModelStateJson(ModelState);
            }

            return Json(new
            {
                Result = WebUiConstants.Fail,
                Msg = "You need to select an Order from the DropDownList"
            });
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult EmailRecordingPosted(int? id, string emails)
        {
            if (id.HasValue)
            {
                try
                {
                    //  id is a WebinarId
                    var firstRetrievedOrderForWebinar = _orderManagementService.GetOrdersForRecordedNotifications(id.Value).FirstOrDefault(
                        o => o.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).RegistrationType.ShowLiveNotifications.ToLower() == "yes"
                    );

                    if (emails.Contains(","))
                        _orderManagementService.FireAdminEmailConnectionInfoHandler(firstRetrievedOrderForWebinar, emails.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
                    else if (emails.Contains(";"))
                        _orderManagementService.FireAdminEmailConnectionInfoHandler(firstRetrievedOrderForWebinar, emails.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
                    else
                        _orderManagementService.FireAdminEmailConnectionInfoHandler(firstRetrievedOrderForWebinar, new string[] { emails }); // this is where a single email is specified

                    return Json(new
                    {
                        Result = WebUiConstants.Success,
                        Msg = string.Format("<p>Your Order ID is {0}. Please check your email for connection information for the webinar.</p>", id)
                    });
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, "There was a problem at the server. Please contact the administrator.");
                    _logger.ErrorException("ConfirmOrder|ConfirmOrder failed ", exception);
                    Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
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
        public ActionResult ExtendPostEventAccess(int? orderID, string newExpiryDate, string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(newExpiryDate)) throw new ValidationException("You need to enter a value.");

                var onDemandCode = RandomHelpers.GetUniqueCode(5);

                var orderIdProperty = new JProperty(JsonPropertyKeys.OrderId, orderID.Value);
                var expiryDateProperty = new JProperty(JsonPropertyKeys.ExpiryDate, newExpiryDate);
                var OnDemandCodeProperty = new JProperty(JsonPropertyKeys.OnDemandCode, onDemandCode);

                var claimValue = new JObject(
                    orderIdProperty,
                    expiryDateProperty,
                    OnDemandCodeProperty
                    );

                var order = _orderManagementService.GetOrderById(orderID.Value);

                _membershipService.UpdateDisplayPostEventMaterialsClaim(
                    _globalConfig.Tenant,
                    email,
                    DateTime.Parse(newExpiryDate),
                    order
                    );
                _logger.Info("Added expiryDate claim for: {0}. Date: {1}", orderID.Value, DateTime.Parse(newExpiryDate));
                return Json(new { Result = WebUiConstants.Success });
            }
            catch (Exception exception)
            {
                _logger.ErrorException(string.Format("ExtendPostEventAccess | Session {0}", _appHelper.GetUserAuditInfo()), exception);

                ModelState.AddModelError(string.Empty, exception.Message);
            }

            return this.ModelStateJson(ModelState);
        }

        [System.Web.Mvc.HttpPost]
        public JsonResult FirePasswordResetEvent(ChangePasswordFromResetKeyInputModel model, string verificationKey)
        {
            if (_membershipService.ChangePasswordFromResetKey(verificationKey, model.Password))
                model.ChangePasswordSucceeded = true;

            return Json(model);
        }


        public PartialViewResult GetJsonTextArea()
        {
            const string importOrderViaDashboardViewModel = @"{""AffiliateComments"": ""Affiliate comments"",
                                  ""BillingAddress.AddressType"": ""Billing"",
                                  ""BillingAddress.Name"": ""Alan Turing"",
                                  ""BillingAddress.Phone"": ""555-555-5555"",
                                  ""BillingAddress.StreetAddress"": ""968 Wildcat Dr"",
                                  ""BillingAddress.StreetAddress2"": """",
                                  ""BillingAddress.City"": ""Del Rio"",
                                  ""BillingAddress.Zip"": ""5000"",
                                  ""BillingAddress.State"": ""Tx"",
                                  ""BillingAddress.Country"": ""USA"",
                                  ""Email"": ""alanbturingy@turing.com"",
                                  ""FirstName"": ""Alan"",
                                  ""LastName"": ""Turing"",
                                  ""idAffiliate"": 19,
                                  ""idRegType"": 88,
                                  ""idWebinar"": 437,
                                  ""Institution"": ""Some Institution"",
                                  ""ShippingAddress.AddressType"": ""Shipping"",
                                  ""ShippingAddress.Name"": ""Alan Turing"",
                                  ""ShippingAddress.Phone"": ""555-555-5555"",
                                  ""ShippingAddress.StreetAddress"": ""968 Wildcat Dr"",
                                  ""ShippingAddress.StreetAddress2"": """",
                                  ""ShippingAddress.City"": ""Del Rio"",
                                  ""ShippingAddress.Zip"": ""5000"",
                                  ""ShippingAddress.State"": ""Tx"",
                                  ""ShippingAddress.Country"": ""USA"",
                                  ""SendNotification"": ""true"",
                                  ""Title"": ""Mr""}";

            ViewBag.Payload = importOrderViaDashboardViewModel;

            return PartialView("~/Views/Admin/Home/_ImportOrder.cshtml", importOrderViaDashboardViewModel);
        }

        public JsonResult ReadCsvAndReturnJson()
        {
            IList<IncomingOrderModel> incomingOrderModels = null;

            using (var fileStream =
                    System.IO.File.OpenRead(
                    Path.Combine(HttpRuntime.AppDomainAppPath, @"App_Data/Orders", "SampleData.csv"))
                //Path.Combine(HttpRuntime.AppDomainAppPath, @"App_Data/Orders", "OrderEntry.csv"))
                //Path.Combine(HttpRuntime.AppDomainAppPath, @"App_Data/Orders", "newCUOrders.csv"))
                    )
            {
                incomingOrderModels = CsvParseOps.ParseCsvForIncomingOrderModel(fileStream);
            }

            var returnPayload = JsonConvert.SerializeObject(incomingOrderModels);

            return Json(returnPayload, JsonRequestBehavior.AllowGet);
        }

        [ValidateJsonAntiForgeryToken(Order = 0)]
        [HandleAjaxException(Order = 1)]
        [HttpPost]
        public ActionResult UpdateAdditionalLocations(int orderRowId, IEnumerable<AdditionalLocation> additionalLocations)
        {
            var orderRow = _orderManagementService.GetOrderRowById(orderRowId);

            var manageOrderEditModel = new ManageOrderEditModel
            {
                AdditionalLocations = additionalLocations
            };

            SyncAdditionalLocations(manageOrderEditModel, orderRow);

            _orderManagementService.SaveChanges();

            return Json(new { Result = WebUiConstants.Success });
        }


        public ActionResult DisplayOrders()
        {
            return View("DisplayOrders", 1720); // hard coded value. It's a webinar id
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
        [HttpPost]

        [ClaimsAuthorize(IdentityConstants.Access, IdentityConstants.GetGridDataFeature)]
        public ActionResult GetGridData(int? webinarId)
        {
            if (ClaimsAuthorization.CheckAccess(IdentityConstants.Access, IdentityConstants.GetGridDataFeature))
            {
                int totalNumberOrders;

                return Json(new
                {
                    data = BuildDisplayOrdersViewModel(webinarId.Value, out totalNumberOrders)
                });
            }

            return PartialView("~/Views/Admin/Partials/_NotAuthorized.cshtml");
        }

        private IList<IDictionary<string, string>> BuildDisplayOrdersViewModel(int webinarId, out int totalNumberOrders)
        {
            var orders = _dataTablesService.GetOrdersByWebinar(webinarId, 19, out totalNumberOrders);

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
                    resendMsg = "Resend Connection Info";
                }

                responsePayloadInner = new Dictionary<string, string>();

                responsePayloadInner.Add("OrderId", orderColumn);
                responsePayloadInner.Add("OrderColumn", orderColumn);
                responsePayloadInner.Add("UserColumn", userColumn);
                responsePayloadInner.Add("InstitutionColumn", institutionColumn);
                responsePayloadInner.Add("BillingColumn", billingColumn);

                responsePayloadInner.Add("AffiliateColumn", order.Affiliate.ttsDomain);

                responsePayloadInner.Add("OrderDateColumn", order.OrderDate.ToShortDateString());
                responsePayloadInner.Add("StatusColumn", "<a href='/Admin/manageOrder/" + order.idOrderLegacy + "' target='_new' />" + order.OrderStatus.ToString()+"</a><br/>" + resendMsg);

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
                    resendMsg = "<button id='fireResendConfirmation' data-orderid='"+order.idOrder+"' type='button' class='btn btn-success btn-small' >Resend Connection Info</button>";
                }

                responsePayloadInner = new Dictionary<string, string>();

                responsePayloadInner.Add("OrderId", orderColumn);
                responsePayloadInner.Add("OrderColumn", orderColumn);
                responsePayloadInner.Add("UserColumn", userColumn);
                responsePayloadInner.Add("InstitutionColumn", institutionColumn);
                responsePayloadInner.Add("BillingColumn", billingColumn);

                responsePayloadInner.Add("AffiliateColumn", order.Affiliate.ttsDomain);

                responsePayloadInner.Add("OrderDateColumn", order.OrderDate.ToShortDateString());
                responsePayloadInner.Add("StatusColumn", "<a href='/Admin/manageOrder/" + order.idOrderLegacy + "' target='_new' />" + order.OrderStatus.ToString()+"</a><br/>" + resendMsg);

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

        public ActionResult GetDiscountForUser()
        {
            throw new NotImplementedException();
        }

        public ActionResult DiscountManagement()
        {

            var model = new DiscountPackageViewModel();
            {

            };

            return View(model);
        }
    }

    public class MembershipRebootConfigInert
    {
        private static readonly GlobalConfig _globalConfig = GlobalConfig.GlobalConfigSingleton;

        public static MembershipRebootConfiguration Create(string pathToRootDirectory, IStateService stateService)
        {
            var settings = SecuritySettings.FromConfiguration();
            var config = new MembershipRebootConfiguration(settings);

            var appinfo = new AspNetApplicationInformation(
                _globalConfig.Tenant,
                _globalConfig.EmailSignature,
                _globalConfig.RelativeLoginUrl,
                _globalConfig.RelativeConfirmChangeUrl,
                _globalConfig.RelativeCancelVerificationUrl,
                _globalConfig.RelativeConfirmPasswordResetUrl);

            IMessageDelivery delivery = new TtsSmtpMessageDeliveryInert();

            var emailFormatter = new TtsEmailFormatterInert(appinfo, stateService);

            config.AddEventHandler(new EmailAccountEventsHandler(emailFormatter, delivery));

            return config;
        }

    }
}