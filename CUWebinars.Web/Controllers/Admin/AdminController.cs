using System.Web.Http;
using BrockAllen.MembershipReboot;
using CUWebinars.Business.AccountService;
using CUWebinars.Business.Core;
using CUWebinars.Business.Models;
using CUWebinars.Business.Services;
using CUWebinars.Web.Core;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Infrastructure.Attributes;
using CUWebinars.Web.Infrastructure.Extensions;
using CUWebinars.Web.Models;
using CUWebinars.Web.Services;
using CUWebinars.Web.ViewModel;
using DotNetOpenAuth.Messaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using ClaimsExtensions = CUWebinars.Web.Helpers.ClaimsExtensions;
using DataOperations = CUWebinars.Web.Membership.DataOperations;

namespace CUWebinars.Web.Controllers.Admin
{

    [ElmahHandleError]
    [System.Web.Mvc.Authorize]

    public class AdminController : Controller
    {
        private readonly IOrderManagementService _orderManagementService;
        private readonly IAppHelper _appHelper;
        private readonly IMembershipService _membershipService;
        private readonly ILogger _logger;
        private readonly IWebinarManagementService _webinarManagementService;
        private readonly IStateService _stateService;
        private readonly GlobalConfig _globalConfig = GlobalConfig.GlobalConfigSingleton;
        private bool _disposed;

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
            IAppHelper appHelper)
        {
            _membershipService = membershipService;
            _logger = logger;
            _stateService = stateService;
            _webinarManagementService = webinarManagementService;
            _orderManagementService = orderManagementService;
            _appHelper = appHelper;
        }

        public ActionResult ManageOrder()
        {
            return View();
        }

        [System.Web.Mvc.HttpPost]
        [ValidateJsonAntiForgeryToken]
        public ActionResult ManageOrder(ManageOrderEditModel model)
        {
            if (ModelState.IsValid)
            {
                var order = ApplyModelChangesToOrder(model);
                _orderManagementService.UpdateOrderByAdmin(order);
            }

            return Json(new { Result = WebUiConstants.Success});
        }

        private Order ApplyModelChangesToOrder(ManageOrderEditModel model)
        {
            var order = _orderManagementService.GetOrderById(model.Id);
            var orderRow = order.OrderRows.Single(o => o.RowStatus == OrderRowStatus.Active);

            order.Total = model.DisplayRowPriceViewModel.PricesAndDiscounts.TotalOrderPrice;
            orderRow.RowPrice = order.Total;
            orderRow.UnitPrice = model.DisplayRowPriceViewModel.PricesAndDiscounts.UnitPrice;

            orderRow.AdditionalLocation.Clear();

            if (!ReferenceEquals(null, model.AdditionalLocations))
                orderRow.AdditionalLocation.AddRange(model.AdditionalLocations);

            return order;
        }

        public ActionResult ClaimsManagement()
        {
            var claimTypes = typeof(ClaimTypes).GetFields().Where(f => f.IsLiteral).ToList();
            var typesAsSelectList = new List<SelectListItem>(claimTypes.Count);
            string rawConstant;

            foreach (var claimType in claimTypes)
            {
                rawConstant = claimType.GetRawConstantValue().ToString();

                typesAsSelectList.Add(new SelectListItem
                {
                    Text = rawConstant.SubstringFromRight(rawConstant.Length - rawConstant.LastIndexOf(Path.AltDirectorySeparatorChar) - 1),
                    Value = claimType.GetRawConstantValue().ToString()
                });
            }
            
            var model = new AddClaimInputModel
            {
                ClaimTypes = typesAsSelectList,
            };

            return View(model);
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult ClaimsManagement(AddClaimInputModel model)
        {
            if (ModelState.IsValid)
            {
                var userAccount =
                    _membershipService.GetUserAccountByUserId(ClaimsExtensions.GetUserID(User));

                _membershipService.AddClaim(
                    userAccount,
                    model.NewClaimType,
                    model.NewClaimValue
                    );
                return Json(new { Result = "success" });
            }

            return this.ModelStateJson(ModelState);
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
                    Emails = additionalLocations.Select( al => al.Email).ToList()
                };

                return PartialView(
                    "~/Views/Webinar/Partials/_AdditionalLocationsModal.cshtml",
                    addAdditionalLocationViewModel
                    );
            }
            return null;// todo: return something.
        }

        public ActionResult GetOrderDetails(int? id = null)
        {
            if (id.HasValue)
            {
                try
                {
                    var order = _orderManagementService.GetOrderById(id.Value);
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
                        AdditionalLocationsRenderer = GetRenderer(additionalLocations.Select(al => al.Email).ToList()),
                        CostPerAdditionalLocation = additionalLocationsPricing.Item2,
                        DisplayOptionsInDropDownViewModel = new DisplayOptionsInDropDownViewModel
                        {
                            Options = _orderManagementService.GetOptionsByWebinarId(
                                order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).idWebinar,
                                false),
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
                        NumberOfAdditionalLocations = additionalLocationsCount,
                        UserId = order.idUser,
                        WebinarId = orderRow.idWebinar
                    };


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

        private TagBuilder GetRenderer(IList<string> emailAddresses)
        {
            const string locationsSpanPrefix = "LocationSpan-";
            string breakSuffix = "-break";
            const string additionalLocationDeleteSuffix = "-AdditionLocationEmail-delete";
            const string additionalLocationEmailPrefix = "AdditionalLocationEmail-";
            const string nonBreakingSpace = "&nbsp;";
            var stringBuilder = new StringBuilder();

    
            var spanBuilder = new TagBuilder("span");
            var inputBuilder = new TagBuilder("input");
            var iconBuilder = new TagBuilder("i");

            /* The generataed element will look like this:
            
                <span id="LocationSpan-0">
                   <input aria-describedby="AdditionalLocationEmail_0-error" aria-invalid="false" class="valid" id="AdditionalLocationEmail-0" name="AdditionalLocations[0].Email" placeholder="Enter email address" type="email" value="test@yahoo.com">&nbsp;
                   <i class="icon-white icon-trash" id="0-AdditionLocationEmail-delete" style="cursor: pointer"></i>
                   <br id="0-break">
                </span>             
             */

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


            var div = new TagBuilder("div");
            div.GenerateId("collectAdditionalLocations");
            div.AddCssClass("addLocsBox");
            div.InnerHtml = stringBuilder.ToString();

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

            _orderManagementService.FireOrderSubmittedEvent(order);
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

            _orderManagementService.FireSendConnectionInfoNotificationEvent(new Order[] { order });
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
            var orders = _orderManagementService.GetOrdersForLiveNotifications(webinarId);

            foreach (var order in orders)
            {
                AdditionalLocation nuller = new AdditionalLocation();
               if (string.IsNullOrEmpty(order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).JoinURL)) GenerateRegistrantKey(order, nuller);

                if (order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).AdditionalLocation.Count > 0)
                {
                    foreach (var additionalLocation in order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).AdditionalLocation)
                    {
                        if (string.IsNullOrEmpty(order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).JoinURL)) GenerateRegistrantKey(order, additionalLocation);
                    }
                }
            }
            _orderManagementService.FireSendConnectionInfoNotificationEvent(orders);

            return Json(new { Result = WebUiConstants.Success });
        }

        private void GenerateRegistrantKey(Order order, AdditionalLocation additionalLocation)
        {
            var row = order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active);
            var regKeyResponse = "";
            if (additionalLocation.Email == null)
            {
                if (row.JoinURL == null && row.RegistrationType.ShowLiveNotifications == "Yes")
                {
                    regKeyResponse = _orderManagementService.CreateRegistrantKey(order.FirstName, order.LastName
                        , order.BillingEmail, row.Webinar.idWebinar, row.Webinar.WebinarKey);
                }
            }
            else
            {
                var contents = additionalLocation.FullName.Split(' ').ToString();
                var lastName = contents.Skip(1).ToString();

                regKeyResponse = _orderManagementService.CreateRegistrantKey(additionalLocation.FullName.Split(' ')[0],
                    lastName, additionalLocation.Email, row.Webinar.idWebinar, row.Webinar.WebinarKey);
            }

            JObject parsedJsonObject;

            if (ReferenceEquals(null, regKeyResponse))
            {
                //throw new NullReferenceException(
                //    "The Registration Key Response from the Citrix API resulted in a null response.");
                _logger.FatalException("The Registration Key creation failed.", new NullReferenceException("The Registration Key Response from the Citrix API resulted in a null response."));

            }
            else
            {
                parsedJsonObject = JObject.Parse(regKeyResponse);

                if (parsedJsonObject[WebUiConstants.RegistrantKey] != null)
                {
                    _logger.Info("RegKey for ." + order.idOrder+ " = " + regKeyResponse);

                    var registrantKey = parsedJsonObject[WebUiConstants.RegistrantKey].ToString();
                    var joinUrl = parsedJsonObject[WebUiConstants.JoinUrl].ToString();
                    if (additionalLocation.Email == null)
                    {
                        row.RegistrantKey = registrantKey;
                        row.JoinURL = joinUrl;
                    }
                    else
                    {
                        additionalLocation.JoinURL = joinUrl;
                        additionalLocation.RegistrantKey = registrantKey;
                    }

                    var pricesAndDiscounts = default(PricesAndDiscounts); // not needed here. Just used b/c ref parameter required below.

                    var resultOfUpdate = _orderManagementService.UpdateOrderChanges(order, ref pricesAndDiscounts);
                }
            }
        }

        public PartialViewResult SendRecordingPosted()
        {
            var model = new AdhocNotificationViewModel
            {
                Webinars = EventInvokerHelpers.GetRecordedWebinarsAsSelectListItems(_webinarManagementService)
            };

            return PartialView("~/Views/Admin/Home/_SendRecordingPosted.cshtml", model);
        }

        [System.Web.Mvc.HttpPost]
        public JsonResult SendRecordingPosted(int webinarId)
        {
            var orders = _orderManagementService.GetOrdersForRecordedNotifications(webinarId);

            if (orders.Any())
            {
                _orderManagementService.FireSendRecordingIsPostedEvent(orders);

                return Json(new { Result = WebUiConstants.Success });
            }

            return Json(new { Result = WebUiConstants.NoOrdersForWebinar });
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


        public PartialViewResult LogInAsUser()
        {
            var logInAsOtherUserViewModel = new LogInAsOtherUserViewModel
            {
                Email = string.Empty,
                Password = string.Empty
            };

            return PartialView("~/Views/Admin/Home/_LogInAsUser.cshtml", logInAsOtherUserViewModel);
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
                    return View("", "", ""); //   TODO: Figure out how to show error messages
                }

                _membershipService.LogOutUser();

                _stateService.SetValue(WebUiConstants.AdminUserEmail, adminUserEmail);

                _membershipService.LogInAdminUserAsOtherUser(_globalConfig.Tenant,
                    adminUserEmail.Trim(), model.Password.Trim(),
                    impersonatedUserAccount
                    );

                return RedirectToAction("Index", "Home");
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

        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ManualPasswordReset(ManualPasswordResetViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userAccount = _membershipService.GetUserAccountByEmail(_globalConfig.Tenant, model.Email);
                if (userAccount == null)

                    return Json(new { Result = WebUiConstants.InvalidEmail });
                var newPassword = Crypto.HashPassword(model.NewPassword);

                var dataOperations = new DataOperations();
                dataOperations.ManualPasswordReset(userAccount.ID, newPassword);

                return Json(new { Result = WebUiConstants.Success });
            }

            //Please use this general pattern when logging ModelState errors.
            var myErr = "";
            foreach (ModelState modelState in ViewData.ModelState.Values)
            {
                foreach (ModelError error in modelState.Errors)
                {
                    myErr += error.ErrorMessage + System.Environment.NewLine;
                }
            }

            _logger.Error("ManualPasswordReset: {0}", _appHelper.GetUserAuditInfo());
            _logger.Error("ManualPasswordReset: {0}", myErr);

            return Json(new { Result = WebUiConstants.Fail + myErr });
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
    }
}