using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Security;
using BrockAllen.MembershipReboot;
using CUWebinars.Business.AccountService;
using CUWebinars.Business.Constants;
using CUWebinars.Business.Core;
using CUWebinars.Business.Core.Exceptions;
using CUWebinars.Business.Core.Helpers;
using CUWebinars.Business.Models;
using CUWebinars.Business.Repository;
using CUWebinars.Business.Services;
using CUWebinars.Web.Core;
using CUWebinars.Web.Core.Orchestrators;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Infrastructure;
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
using ClaimTypes = CUWebinars.Business.Constants.ClaimTypes;

namespace CUWebinars.Web.Controllers
{
    [ElmahHandleError]
    //[System.Web.Mvc.Authorize]
    public class AccountController : Controller
    {
        private const string ManageActionName = "Manage";
        private const string LoggedInResult = "LoggedIn";
        private const string ConfirmedResult = "Confirmed";

        private readonly GlobalConfig _globalConfig = GlobalConfig.GlobalConfigSingleton;

        private readonly IAffiliateRepository _affiliateRepository;

        private readonly IAccountControllerOrchestrator _accountControllerOrchestrator;
        private readonly ILogger _logger;
        private readonly IMembershipService _membershipService;
        private readonly IStateService _stateService;
        private readonly IAppHelper _appHelper;
        private readonly IUniversalMapper _universalMapper;
        private readonly IOrderManagementService _orderManagementService;
        private bool _disposed;

        public AccountController(
            IAccountControllerOrchestrator accountControllerOrchestrator,
            ILogger logger,
            IMembershipService membershipService,
            IOrderManagementService orderManagementService,
            IStateService stateService,
            IAppHelper appHelper,
            IUniversalMapper universalMapper,
            IAffiliateRepository affiliateRepository)
        {
            _accountControllerOrchestrator = accountControllerOrchestrator;
            _logger = logger;
            _membershipService = membershipService;
            _orderManagementService = orderManagementService;
            _stateService = stateService;
            _appHelper = appHelper;
            _universalMapper = universalMapper;
            _affiliateRepository = affiliateRepository;
        }



        [System.Web.Mvc.AllowAnonymous]
        public string Get([FromUri] CreateWebUserModel model)
        {

            if (ModelState.IsValid)
            {
                var RegisterModel = new RegisterModel
                {
                    //    BillingAddress = 
                };
            }
            return null;
        }

        public class CreateWebUserModel
        {
            public string UserType { get; set; }
            public string idWebUser { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Institution { get; set; }
            public string Email { get; set; }
            public string Title { get; set; }
            public string AccountDetailsTitle { get; set; }
            public string BillingAddress_Name { get; set; }
            public string BillingAddress_StreetAddress { get; set; }
            public string BillingAddress_StreetAddress2 { get; set; }
            public string BillingAddress_City { get; set; }
            public string BillingAddress_State { get; set; }
            public string BillingAddress_Zip { get; set; }
            public string BillingAddress_Country { get; set; }
            public string ShippingAddress_Name { get; set; }
            public string ShippingAddress_StreetAddress { get; set; }
            public string ShippingAddress_StreetAddress2 { get; set; }
            public string ShippingAddress_City { get; set; }
            public string ShippingAddress_State { get; set; }
            public string ShippingAddress_Zip { get; set; }
            public string ShippingAddress_Country { get; set; }

        }

        [System.Web.Mvc.AllowAnonymous]
        public string Get([FromUri] RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var email = model.Email.Trim();
                var firstName = model.FirstName.Trim();
                var lastName = model.LastName.Trim();

                //first check if email exists
                var checkIfUsed = _membershipService.GetUserByEmail(email);
                if (checkIfUsed != null)
                {
                    _logger.Error("dupe email attempt: " + email);
                    ModelState.AddModelError("Email", "That email already exists. Would you like to reset the password?");
                }

                var myInstitution = _membershipService.ProcessInstitutionForUser(model.Institution.Trim(),
                    email,
                    model.BillingAddress.City,
                    model.BillingAddress.State,
                    "N",
                    "New",
                    model.BillingAddress.Zip);

                try
                {
                    var billingAddress = new Address
                    {
                        AddressType = Enum.GetName(typeof(AddressType), 0),
                        City = model.BillingAddress.City,
                        Country = model.BillingAddress.Country,
                        Name = model.FirstName + ' ' + model.LastName,
                        Phone = model.BillingAddress.Phone,
                        State = model.BillingAddress.State,
                        StreetAddress = model.BillingAddress.StreetAddress,
                        StreetAddress2 = model.BillingAddress.StreetAddress2,
                        Zip = model.BillingAddress.Zip
                    };

                    var shippingAddress = new Address
                    {
                        AddressType = Enum.GetName(typeof(AddressType), 1),
                        City = model.ShippingAddress.City,
                        Country = model.ShippingAddress.Country,
                        Name = model.FirstName + ' ' + model.LastName,
                        Phone = model.ShippingAddress.Phone,
                        State = model.ShippingAddress.State,
                        StreetAddress = model.ShippingAddress.StreetAddress,
                        StreetAddress2 = model.ShippingAddress.StreetAddress2,
                        Zip = model.ShippingAddress.Zip
                    };

                    IList<Address> addresses = new List<Address> { billingAddress, shippingAddress };

                    var webUser = _membershipService.CreateWebUser(_globalConfig.Tenant,
                        firstName
                        , lastName
                        , model.Password
                        , email
                        , USTimeZone.Central
                        , UserType.Customer
                        , myInstitution.idInstitution
                        , addresses
                        , model.Title == null ? model.Title : model.Title.Trim()
                        , null
                        , DomainConstants.Active
                    );


                    if (!_stateService.HasValue(WebUiConstants.CurrentUser))
                        _stateService.SetValue(WebUiConstants.CurrentUser, webUser);

                    var userAccount = _membershipService.CreateUser(
                        _globalConfig.Tenant,
                        firstName,
                        lastName,
                        string.Empty,
                        //  Pass empty string for username because MembershipReboot assigns the email to the username field where emailIsUsername is true
                        model.Password,
                        email);

                    Debug.Assert(_stateService.HasValue(DomainConstants.VerificationKey),
                        "There's no reason session should not have a value for the VerificationKey at this point ");

                    var verificationKey = _stateService.GetValue<string>(DomainConstants.VerificationKey);
                    _stateService.ClearValue(DomainConstants.VerificationKey);

                    userAccount = _membershipService.VerifyEmailFromKey(
                        verificationKey,
                        model.Password
                    );

                    //membershipService.LogInUser(globalConfig.Tenant, model.Email, model.Password, true); // log the user in.
                    _logger.Info("/Account/Register UserAdded: " + model.Email);
                    return webUser.idUser.ToString(CultureInfo.InvariantCulture);
                    //return model.idWebUser.ToString();
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError(string.Empty, e.StatusCode.ToString());
                    _logger.Error("Importer Error: " + e.Message);
                }
            }
            else
            {
                ProcessModelStateErrors();
            }

            // If we got this far, something failed, redisplay form
            return "error";
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        [HandleAjaxException]
        public ActionResult GetInstitutionsByName(string institutionName)
        {
            if (!string.IsNullOrWhiteSpace(institutionName))
            {
                try
                {
                    var institutions = _accountControllerOrchestrator.GetInstitutionsByName(institutionName);

                    return Json(new { institutions = institutions.Select(i => i.InstitutionName) });
                }
                catch (Exception exception)
                {
                    _logger.Warn("Account.GetInstitutionsByName: " + exception.Message + " Session=" +
                                 _appHelper.GetUserAuditInfo());
                }
            }

            return Json(new { });
        }

        [HandleAjaxException]
        public PartialViewResult GetLoginPartial()
        {
            WebUser user = new WebUser();
            if (User.Identity.IsAuthenticated)
            {
                user = _accountControllerOrchestrator.GetWebUserFromIPrincipal();
                var userAccount = _membershipService.GetUserAccountByEmail(_globalConfig.Tenant, user.email);
                //var claimsViewModel = new ClaimsViewModel { UserClaims = userAccount.Claims };
            }
            return PartialView("_LoginPartial");
        }


        [System.Web.Mvc.AllowAnonymous]
        public ActionResult OrderDidNotComplete(int? id, string message)
        {
            // id is the Order's id.
            if (id.HasValue)
            {
                ViewBag.Message = message;
                var order = _accountControllerOrchestrator.GetOrderById(id.Value);
                return View(order);
            }
            return View();
        }

        [System.Web.Mvc.AllowAnonymous]
        public ActionResult OrderComplete(int? id)
        {
            // id is the Order's id.
            if (id.HasValue)
            {
                var order = _accountControllerOrchestrator.GetOrderById(id.Value);

                return View(order);
            }
            return View();
        }

        [System.Web.Mvc.AllowAnonymous]
        public ActionResult OrderCompleteAffiliate(int? id)
        {
            // id is the Order's id.
            if (id.HasValue)
            {
                var order = _accountControllerOrchestrator.GetOrderById(id.Value);
                return View(order);
            }
            return View();
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
                        _affiliateRepository.LoadByTTSDomain(claimTTSDomain));
                    return RedirectToAction("Index", "Admin");

                }
                CompliancePerspectivesModel compPersectivesModel =
                    _accountControllerOrchestrator.BuildCompPersectivesModel();
                var discountModel = _accountControllerOrchestrator.BuildDiscountModelForUser();
                var myWebinarsDTO = _accountControllerOrchestrator.BuildMyWebinarsDTO
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


        [System.Web.Mvc.AllowAnonymous]
        [System.Web.Mvc.HttpGet]
        public ActionResult MyCertificate(int orderID, string displayName, string displayInst = "")
        {
            //var currentUser = _accountControllerOrchestrator.GetWebUserFromIPrincipal();

            _logger.Info("MyCertificateDS orderId=" + orderID + ", displayName=" + displayName + ", displayInst" +
                         displayInst);

            var currentOrder = _orderManagementService.GetOrderById(orderID);
            if (ReferenceEquals(displayName, null))
            {
                displayName = currentOrder.FirstName + ' ' + currentOrder.LastName;
            }

            var model = new CertOfCompletionViewModel
            {
                DisplayName = displayName,
                Order = currentOrder,
                DisplayInst = displayInst
            };

            model.CeuShort = string.Empty;
            model.CeuStatement = string.Empty;

            if (!string.IsNullOrEmpty(model.Order.OrderRows.SingleOrDefault().Webinar.ceu))
            {
                string[] ceu = model.Order.OrderRows.SingleOrDefault().Webinar.ceu.Split('|');
                model.CeuShort = ceu[0];
                model.CeuStatement = ceu[1];
            }

            return View("MyCertificate", model);
        }


        [System.Web.Mvc.AllowAnonymous]
        [System.Web.Mvc.HttpGet]
        public ActionResult MyCertificateDS(int webinarId, string displayName, string displayInst)
        {
            var currentWebinar = _orderManagementService.GetWebinarById(webinarId);

            _logger.Info("MyCertificateDS webinarId=" + webinarId + ", displayName=" + displayName + ", displayInst" +
                         displayName);
            var model = new CertOfCompletionDSViewModel
            {
                DisplayName = displayName,
                Webinar = currentWebinar,
                DisplayInst = displayInst
            };

            model.CeuShort = string.Empty;
            model.CeuStatement = string.Empty;

            return View("MyCertificateDS", model);
        }

        [System.Web.Mvc.AllowAnonymous]
        public ActionResult Confirmed(string email, string password)
        {
            _logger.Info("Account.Confirmed: " + email);
            try
            {
                var changeEmailFromKeyInputModel = _accountControllerOrchestrator.ConfirmUser(email, password);

                return string.IsNullOrEmpty(changeEmailFromKeyInputModel.ScreenMessage)
                    ? View(changeEmailFromKeyInputModel)
                    : View("ThankYouConfirmed", changeEmailFromKeyInputModel);
            }
            catch (Exception exception)
            {
                _logger.Fatal(
                    "Account.Confirm Exception On GET: {0} Session={1}",
                    exception.Message,
                    _appHelper.GetUserAuditInfo()
                );
                ErrorSignal.FromCurrentContext().Raise(exception);
                throw;
            }
        }

        [System.Web.Mvc.AllowAnonymous]
        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken(Order = 0)]
        [HandleAjaxException(Order = 1)]
        public ActionResult WPSConfirmation()
        {
            return View();

        }

        [System.Web.Mvc.AllowAnonymous]
        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken(Order = 0)]
        [HandleAjaxException(Order = 1)]
        public ActionResult Confirmed(CreateUserConfirmedViewModel model)
        {
            try
            {
                if (_accountControllerOrchestrator.UserConfirmed(model))
                {
                    return RedirectToLocal(null);
                }

                _logger.Info("Email is successfully verified in our system: {1} Session={0}",
                    _appHelper.GetUserAuditInfo(), model.Email);

                model.ScreenMessage = "Your email (" + model.Email + " ) is confirmed.";

                return View(model);
            }
            catch (Exception exception)
            {
                _logger.Fatal(
                    "Account.Confirm Exception On Post: {0} Session={1}",
                    exception.Message,
                    _appHelper.GetUserAuditInfo()
                );
                ModelState.AddModelError(string.Empty, exception.Message);
                ErrorSignal.FromCurrentContext().Raise(exception);
            }

            return this.ModelStateJson(ModelState);
        }

        [System.Web.Mvc.HttpGet]
        public ActionResult EditBillingAddress()
        {
            _logger.Info("Editing Billing Address - GET");

            EditBillingAddressModel model;

            try
            {
                model = _accountControllerOrchestrator.BuildBillingAddressModel();
            }
            catch (Exception exception)
            {
                _logger.ErrorException("In EditBillingAddress Action", exception);
                ErrorSignal.FromCurrentContext().Raise(exception);
                throw;
            }

            ViewBag.ReturnUrl = Url.Action(ManageActionName);
            ViewBag.Title = WebUiConstants.ManageUser;

            return PartialView("Partials/_EditBillingAddress", model);
        }

        [System.Web.Mvc.HttpGet]
        public ActionResult EditShippingAddress()
        {
            _logger.Info("Editing Billing Address - GET");


            EditShippingAddressModel model;

            try
            {
                model = _accountControllerOrchestrator.BuildShippingAddressModel();
            }
            catch (Exception exception)
            {
                _logger.ErrorException("In EditShippingAddress Action", exception);
                ErrorSignal.FromCurrentContext().Raise(exception);
                throw;
            }

            ViewBag.ReturnUrl = Url.Action(ManageActionName);
            ViewBag.Title = WebUiConstants.ManageUser;

            return PartialView("Partials/_EditShippingAddress", model);
        }


        [System.Web.Mvc.HttpPost]
        public ActionResult EditNameTitle(EditNameTitleModel model)
        {
            ViewBag.ReturnUrl = Url.Action(ManageActionName);
            ViewBag.Title = WebUiConstants.ManageUser;

            try
            {
                var user = _accountControllerOrchestrator.GetWebUserFromIPrincipal();

                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
                model.Title = user.Title;

                _accountControllerOrchestrator.UpdateNameTitle(model.FirstName, model.LastName, user.email, model.Title);
            }
            catch (Exception exception)
            {
                _logger.ErrorException("In EditNameTitle Action", exception);
                ErrorSignal.FromCurrentContext().Raise(exception);
                throw;
            }
            return PartialView("Partials/_EditNameTitle", model);
        }

        //[System.Web.Mvc.HttpPost]
        //[HandleAjaxException]
        public JsonResult ShareNotifications(ShareNotisViewModel shareNotis)
        {
            var order = _orderManagementService.GetOrderById(shareNotis.idOrder);

            try
            {
                //following copies pattern found at WebinarController | Identify
                var newJson = new JProperty(JsonPropertyKeys.CarbonCopy, shareNotis.addresses);
                order.UserComments = JsonHelpers.ReplaceJsonWithStoredField(order.UserComments, newJson, JsonPropertyKeys.CarbonCopy);

                _orderManagementService.SaveChanges();
            }
            catch (Exception)
            {
                return Json(new { Result = WebUiConstants.Fail, UpdateCaption = "CC List Failed!" });
            }

            return Json(new { Result = WebUiConstants.Success, UpdateCaption = "CC List is Updated." });
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult EditDiscount(int? id, string returnUrl = null)
        {
            return null;
        }

        [System.Web.Mvc.HttpGet]
        public ActionResult EditUserFromOrder(int? id, string email, string returnUrl = null)
        {
            Debug.Assert(id != null, "id != null");
            _logger.Info("EditUserFromOrder:" + id);
            if (id.Value > 0)
            {
                try
                {
                    var user = _accountControllerOrchestrator.GetWebUserById(id.Value);

                    WebUser editingUser = null;

                    if (User.Identity.IsAuthenticated)
                    {
                        editingUser = _accountControllerOrchestrator.GetWebUserFromIPrincipal();
                    }

                    _logger.Info(string.Format("{0} is editing {1}", editingUser.email, user.email));

                    if (email != user.email)
                    {
                        _logger.Info("EditUserFromOrder searching for user by email: " + email);
                        user = _accountControllerOrchestrator.GetWebUserByEmail(email);
                    }

                    var editModel = BuildEditUserInfoModel(user, returnUrl);
                    return View("EditUser", editModel);
                }
                catch (Exception ex)
                {
                    _logger.Fatal("EditUserFromOrder error on " + id.Value + " " + ex);
                }
            }
            return View("EditUser", null);
        }



        public JsonResult GetResendInfoForm(int orderId)
        {
            string html = "";

            try
            {
                var order = _orderManagementService.GetOrderById(orderId);

                WebUser editingUser = null;

                if (User.Identity.IsAuthenticated)
                {
                    editingUser = _accountControllerOrchestrator.GetWebUserFromIPrincipal();
                }

                var editModel = BuildOrderInfoModel(order, "");

                html = ViewHelpers.RenderViewToString(ControllerContext,
                    "~/Views/Shared/EditorTemplates/DataTablesEditorTemplates/ResendInfo_Compact.cshtml",
                    editModel, true);
            }
            catch (Exception ex)
            {
                _logger.FatalException("GetResendInfoForm error on " + orderId, ex);
            }

            return Json(new { html = html });

        }

        public JsonResult GetEditBillingForm_Compact(int orderId)
        {
            string html = "";
            var order = _orderManagementService.GetOrderById(orderId);

            try
            {
                WebUser editingUser = null;

                if (User.Identity.IsAuthenticated)
                {
                    editingUser = _accountControllerOrchestrator.GetWebUserFromIPrincipal();
                }

                _logger.Info(string.Format("GetEditBillingForm_Compact: {0} opened {1}", editingUser.email, orderId));

                var editModel = BuildOrderInfoModel(order, "");

                html = ViewHelpers.RenderViewToString(ControllerContext,
                    "~/Views/Shared/EditorTemplates/DataTablesEditorTemplates/EditOrder_Compact.cshtml",
                    editModel, true);
            }
            catch (Exception ex)
            {
                _logger.FatalException("GetOrderInfoForm error on " + orderId, ex);
            }

            return Json(new { html = html, adminComments = order.AdminComments });

        }

        public JsonResult GetEditDiscountForm(int orderId)
        {
            string html = "";

            try
            {
                WebUser editingUser = null;

                if (User.Identity.IsAuthenticated)
                {
                    editingUser = _accountControllerOrchestrator.GetWebUserFromIPrincipal();
                }

                _logger.Info(string.Format("{0} is editing {1}", editingUser.email, orderId));
                var order = _orderManagementService.GetOrderById(orderId);

                var editModel = BuildOrderInfoModel(order, "");

                html = ViewHelpers.RenderViewToString(ControllerContext,
                    "~/Views/Shared/EditorTemplates/DataTablesEditorTemplates/EditCompact_Discount.cshtml",
                    editModel, true);
            }
            catch (Exception ex)
            {
                _logger.FatalException("GetOrderInfoForm error on " + orderId, ex);
            }

            return Json(new { html = html });

        }



        public JsonResult GetEditUserCompactForm(int? id)
        {
            // similar to the existing EditUserFromOrder routine

            //if (id.Value <= 0)
            //    throw new Exception("Id of user to edit must be >= 0");

            string html = "";

            try
            {
                var user = _accountControllerOrchestrator.GetWebUserById(id.Value);

                WebUser editingUser = null;

                if (User.Identity.IsAuthenticated)
                {
                    editingUser = _accountControllerOrchestrator.GetWebUserFromIPrincipal();
                }

                _logger.Info(string.Format("{0} is editing {1}", editingUser.email, user.email));

                var editModel = BuildEditUserInfoModel(user, "");

                html = ViewHelpers.RenderViewToString(ControllerContext,
                    "~/Views/Shared/EditorTemplates/DataTablesEditorTemplates/EditUser_Compact.cshtml",
                    editModel, true);
            }
            catch (Exception ex)
            {
                _logger.FatalException("GetEditUserCompactForm error on " + id.Value, ex);
            }

            return Json(new { html = html });

        }


        public JsonResult GetEditInstitutionForm(int? id)
        {
            // similar to the existing EditUserFromOrder routine

            //if (id.Value <= 0)
            //    throw new Exception("Id of user to edit must be >= 0");

            string html = "";

            try
            {
                var user = _accountControllerOrchestrator.GetWebUserById(id.Value);

                WebUser editingUser = null;

                if (User.Identity.IsAuthenticated)
                {
                    editingUser = _accountControllerOrchestrator.GetWebUserFromIPrincipal();
                }

                _logger.Info(string.Format("{0} is editing {1}", editingUser.email, user.email));

                var editModel = BuildEditInstitutionInfoModel(user, "");

                html = ViewHelpers.RenderViewToString(ControllerContext,
                    "~/Views/Shared/EditorTemplates/DataTablesEditorTemplates/EditInstitution.cshtml",
                    editModel, true);
            }
            catch (Exception ex)
            {
                _logger.FatalException("EditInstitution error on " + id.Value, ex);
            }

            return Json(new { html = html });

        }





        [System.Web.Mvc.HttpGet]
        public ActionResult EditPresenter(int? id, string returnUrl = null)
        {
            if (id.HasValue && id > 0) // if not, assume it is logged in user
            {
                var user = _accountControllerOrchestrator.GetWebUserById(id.Value);

                WebUser editingUser = null;

                if (User.Identity.IsAuthenticated)
                {
                    editingUser = _accountControllerOrchestrator.GetWebUserFromIPrincipal();
                }

                _logger.Info(string.Format("{0} is editing {1}", editingUser.email, user.email));

                var editModel = BuildEditPresenterInfoModel(user, returnUrl);

                return View(editModel);
            }
            return null;
        }



        [System.Web.Mvc.HttpGet]
        public ActionResult EditUser(int? id, string returnUrl = null)
        {
            if (id.HasValue && id > 0) // if not, assume it is logged in user
            {
                var user = _accountControllerOrchestrator.GetWebUserById(id.Value);

                WebUser editingUser = null;

                if (User.Identity.IsAuthenticated)
                {
                    editingUser = _accountControllerOrchestrator.GetWebUserFromIPrincipal();

                    _logger.Info(string.Format("{0} is editing {1}", editingUser.email, user.email));

                    var editModel = BuildEditUserInfoModel(user, returnUrl);

                    return View(editModel);
                }

                return RedirectToAction("Login", "Account", new { ReturnURL = "/Account/EditUser/" + id });
            }
            else
            {
                var editingUser = _accountControllerOrchestrator.GetWebUserFromIPrincipal();

                _logger.Info(string.Format("{0} is editing their own details", editingUser.email));

                var editModel = BuildEditUserInfoModel(editingUser, returnUrl);

                return View(editModel);
            }
        }

        private EditUserInfoModel BuildEditUserInfoModel(WebUser user, string returnUrl)
        {
            if (user == null)
            {
                throw new NullReferenceException();
            }
            var addresses = user.Addresses.ToArray();
            Address billingAddress = new Address();
            Address shippingAddress = new Address();
            if (user.Addresses != null)
            {
                billingAddress = addresses.FirstOrDefault(a => a.AddressType == WebUiConstants.BillingAddress);
                shippingAddress = addresses.FirstOrDefault(a => a.AddressType == WebUiConstants.ShippingAddress);
            }
            else
            {
                user.Addresses = new List<Address>();
            }
            if (billingAddress == null)
            {
                billingAddress = _accountControllerOrchestrator.BuildPlaceHolderAddressBilling(user.email);
                user.Addresses.Add(billingAddress);
                _membershipService.UpdateUserDetails(user);
            }
            if (shippingAddress == null)
            {
                shippingAddress = _accountControllerOrchestrator.BuildPlaceHolderAddressShipping(user.email);
                user.Addresses.Add(shippingAddress);
                shippingAddress.Name = user.FirstName + ' ' + user.LastName;
                _membershipService.UpdateUserDetails(user);
            }
            var editModel = new EditUserInfoModel
            {
                EditFields = new EditUserModel
                {
                    BillingAddress = new AddressModel
                    {
                        Name = user.FirstName + ' ' + user.LastName,
                        City = billingAddress.City,
                        Country = billingAddress.Country,
                        StreetAddress = billingAddress.StreetAddress,
                        StreetAddress2 = billingAddress.StreetAddress2,
                        State = billingAddress.State,
                        Zip = billingAddress.Zip,
                        Phone = billingAddress.Phone,
                        TypeOfAddress = AddressType.Billing
                    },
                    ShippingAddress = new AddressModel
                    {
                        City =
                            string.IsNullOrWhiteSpace(shippingAddress.City) ? billingAddress.City : shippingAddress.City,
                        Country =
                            string.IsNullOrWhiteSpace(shippingAddress.Country)
                                ? billingAddress.Country
                                : shippingAddress.Country,
                        StreetAddress =
                            string.IsNullOrWhiteSpace(shippingAddress.StreetAddress)
                                ? billingAddress.StreetAddress
                                : shippingAddress.StreetAddress,
                        StreetAddress2 =
                            string.IsNullOrWhiteSpace(shippingAddress.StreetAddress2)
                                ? billingAddress.StreetAddress2
                                : shippingAddress.StreetAddress2,
                        State =
                            string.IsNullOrWhiteSpace(shippingAddress.State)
                                ? billingAddress.State
                                : shippingAddress.State,
                        Zip = string.IsNullOrWhiteSpace(shippingAddress.Zip) ? billingAddress.Zip : shippingAddress.Zip,
                        Phone =
                            string.IsNullOrWhiteSpace(shippingAddress.Phone)
                                ? billingAddress.Phone
                                : shippingAddress.Phone,
                        Name =
                            string.IsNullOrWhiteSpace(shippingAddress.Name) ? billingAddress.Name : shippingAddress.Name,
                        TypeOfAddress = AddressType.Shipping
                    },
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Institution = user.Institution.InstitutionName,
                    Email = user.email,
                    Title = user.Title,
                    SageAccountId = user.SageAccountId,
                    TimeZone = user.timeZone,
                    AccountDetailsTitle = WebUiConstants.ManageUser
                },
                LoggedInUser = (ClaimsIdentity)User.Identity,
                ReturnUrl = returnUrl,
                StatusMessage = string.Empty
            };
            return editModel;
        }

        private EditPresenterInfoModel BuildEditPresenterInfoModel(WebUser user, string returnUrl)
        {
            if (user == null)
            {
                throw new NullReferenceException();
            }
            Presenter presenter = _membershipService.GetPresenter(user.idUser);
            var editModel = new EditPresenterInfoModel
            {
                EditFields = new EditPresenterModel
                {
                    PhotoThumb = presenter.PhotoThumb,
                    PhotoFull = presenter.PhotoFull,
                    BioLong = presenter.BiographyLong,
                    BioShort = presenter.Biography,
                    WebUser = _orderManagementService.GetWebUser(user.idUser),
                    AccountDetailsTitle = WebUiConstants.ManageUser
                },
                LoggedInUser = (ClaimsIdentity)User.Identity,
                ReturnUrl = returnUrl,
                StatusMessage = string.Empty
            };
            return editModel;
        }

        private EditOrderInfoModel BuildOrderInfoModel(Order order, string empty)
        {
            //Used by EditOrder_Compact grid order editor
            if (order == null) throw new ArgumentNullException("order");
            var orderRow = order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active);
            var userAccount = _membershipService.GetUserAccountByEmail(_globalConfig.Tenant, order.BillingEmail);

            var additionalLocationsPricing = orderRow.Webinar.AdditionalLocationPrice;
            //_orderManagementService.GetCostOfAdditionalLocations(orderRow.AdditionalLocation, orderRow.idWebinar);
            var additionalLocations = orderRow.AdditionalLocation;
            var additionalLocationsCount = 0;
            if (orderRow.AdditionalLocation != null && orderRow.AdditionalLocation.Count > 0)
            {
                additionalLocations = _appHelper.CheckAdditionalLocationsForValidEmail(additionalLocations.ToList());
                additionalLocationsCount = additionalLocations.Count;
            }

            PostEventClaim postEvent = new PostEventClaim();
            var claimsViewModel = new ClaimsViewModel();
            if (userAccount != null)
            {
                claimsViewModel = new ClaimsViewModel { UserClaims = userAccount.Claims };
            }
            DisplayOptionsInDropDownViewModel regTypeDD = new DisplayOptionsInDropDownViewModel
            {
                Options = _orderManagementService.GetAllPossibleOptionsByWebinarId(orderRow.idWebinar, false),
                OrderRowId = orderRow.idOrderRow,
                OrderRowRegistrationType = orderRow.RegistrationType
            };

            ViewBag.RegTypeDropDownHtml = ViewHelpers.RenderViewToString(ControllerContext,
                "~/Views/Shared/EditorTemplates/DataTablesEditorTemplates/EditRegType_DropDown.cshtml",
                regTypeDD, true);

            DiscountModel discount = null;

            if (!orderRow.Webinar.Title.StartsWith("Compliance Perspectives"))
                _accountControllerOrchestrator.BuildDiscountModelForUser(orderRow.Discount, order.idUser);

            if (orderRow.Discount != null)
            {
                discount = _accountControllerOrchestrator.BuildDiscountModelForOrder(orderRow, order.idUser);
            }

            if (orderRow.Webinar.Title.StartsWith("Compliance Perspectives"))
            {
                discount = _accountControllerOrchestrator.BuildCPSubscriptionModel(orderRow.Discount, order.idUser);
            }
            var editModel = new EditOrderInfoModel
            {
                EditFields = new EditOrderModel
                {
                    AuditInfo = GetFirstPageOrigin(order),
                    AdditionalLocationsAvailableOnLoad = false, // see below for where this is properly decided.
                    AdditionalLocationsRenderer =
                        ViewHelpers.GetRendererOfAdditionalLocations(additionalLocations.Select(al => al.Email).ToList()),

                    AdditionalLocations = additionalLocations,
                    CostPerAdditionalLocation = orderRow.Webinar.AdditionalLocationPrice,
                    // additionalLocationsPricing.Item2,
                    ClaimsViewModel = claimsViewModel, // new ClaimsViewModel { UserClaims = userAccount.Claims },

                    DisplayRowPriceViewModel = new DisplayRowPriceViewModel
                    {
                        NumberOfAdditionalLocations = additionalLocationsCount,
                        //OrderStatus = order.OrderStatus,
                        //Price = Convert.ToDecimal(orderRow.RegistrationType.Price),
                        PricesAndDiscounts =
                            _orderManagementService.CalculateOrderCost(order, orderRow.Webinar.AdditionalLocationPrice),
                        RegistrationType = orderRow.RegistrationType,
                        //RowPrice = orderRow.RowPrice
                    },
                    Discount = discount,
                    Order = order,
                    WebUser = order.WebUser,
                    WebinarId = orderRow.Webinar.idWebinar,
                    idOrderRow = orderRow.idOrderRow,
                    NumberOfAdditionalLocations = additionalLocationsCount
                }
            };
            var onDemandClaim = new PostEventClaim
            {
                OnDemandCode = "notfound",
                OrderId = 0,
                ExpiryDate = DateTime.Now.AddYears(-10)
            };
            try
            {
                var odClaimIsFound = false;
                editModel.EditFields.PostEventClaim = onDemandClaim;
                foreach (
                    var claim in
                    claimsViewModel.UserClaims.Where(
                        c => c.Type == "http://ttstrain.com/ws/2014/01/identity/claims/DisplayPostEventMaterials"
                             ||
                             c.Type ==
                             "http://ttstrain.com/ws/2014/01/identity/claims/DisplayPostEventMaterialsExtended")
                )
                {

                    var row =
                        editModel.EditFields.Order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active);


                    if (row.OnDemandCode != null && claim.Value.Contains(row.OnDemandCode))
                    {
                        odClaimIsFound = true;
                        var thisClaim = JsonConvert.DeserializeObject<PostEventClaim>(claim.Value);

                        onDemandClaim.OrderId = editModel.EditFields.Order.idOrder;
                        onDemandClaim.OnDemandCode = thisClaim.OnDemandCode;
                        onDemandClaim.ExpiryDate = thisClaim.ExpiryDate;

                        editModel.EditFields.PostEventClaim = onDemandClaim;
                    }



                }
                if (!odClaimIsFound)
                {

                    var row = order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active);
                    Debug.Assert(row != null, "row != null @ BuildOrderInfoModel");
                    if (string.IsNullOrEmpty(row.OnDemandCode))
                    {
                        row.OnDemandCode = RandomHelpers.GetUniqueCode(5);
                        _orderManagementService.SaveOrderChanges(editModel.EditFields.Order, null, null);
                    }

                    _membershipService.AddClaimForPostEventMaterials(order.BillingEmail, row, _orderManagementService.CalculatePostEventMaterialsAccessExpiry(row), _globalConfig.Tenant);

                }
            }
            catch (Exception ex)
            {
                _logger.FatalException("BuildOrderInforModel was CheckingOnDemandClaims: " + editModel.EditFields.Order.idOrder, ex);
                //   throw;
            }

            var regType = orderRow.RegistrationType;

            var option = CheckIfAddLocAvailable(regType.idRegType);
            //governs if addLoc editor is displayed.
            if (option.HasValue)
            {
                editModel.EditFields.AdditionalLocationsAvailableOnLoad = option.Value;
            }

            return editModel;
        }

        private string GetFirstPageOrigin(Order order)
        {
            try
            {
                AuditInfoModel parseOutAudit = JsonConvert.DeserializeObject<AuditInfoModel>(order.AuditInfo.Replace("{\"AuditInfo\":", "").Replace("}}", "}"));
                dynamic jsonObject = new JObject();
                jsonObject.FirstPage = parseOutAudit.FirstPage;
                jsonObject.RemoteUser = parseOutAudit.RemoteUser;
                jsonObject.Origin = order.Origin;

                return JsonConvert.SerializeObject(jsonObject);
            }
            catch (Exception ex)
            {
                dynamic jsonObject = new JObject();
                jsonObject.FirstPage = "not found: " + order.idOrder;
                jsonObject.RemoteUser = "not found ex:" + ex.Message;
                jsonObject.Origin = order.Origin;

                return JsonConvert.SerializeObject(jsonObject);
            }
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


        private EditInstitutionInfoModel BuildEditInstitutionInfoModel(WebUser user, string returnUrl)
        {
            if (user == null)
            {
                throw new NullReferenceException();
            }
            var institution = user.Institution;

            var editModel = new EditInstitutionInfoModel
            {
                EditFields = new EditInstitutionModel
                {
                    InstitutionName = user.Institution.InstitutionName,
                    Address = user.Institution.Address,
                    City = user.Institution.City,
                    Zip = user.Institution.Zip,
                    State = user.Institution.State,
                    Country = user.Institution.Country,
                    idInstitution = user.idUserInstitution,
                    idOfCurrentUser = user.idUser
                },
                LoggedInUser = (ClaimsIdentity)User.Identity,
                ReturnUrl = returnUrl,
                StatusMessage = string.Empty
            };
            return editModel;
        }

        private EditInstitutionInfoModel BuildEditInstitutionModel(WebUser user, string returnUrl)
        {
            if (user == null)
            {
                throw new NullReferenceException();
            }
            var institution = user.Institution;
            //
            var editModel = new EditInstitutionInfoModel
            {
                EditFields = new EditInstitutionModel
                {
                    idOfCurrentUser = user.idUser,
                    idInstitution = institution.idInstitution,
                    InstitutionName = institution.InstitutionName,
                    Address = institution.Address,
                    City = institution.City,
                    State = institution.State,
                    Zip = institution.Zip

                },
                LoggedInUser = (ClaimsIdentity)User.Identity,
                ReturnUrl = returnUrl,
                StatusMessage = string.Empty
            };
            return editModel;
        }


        [System.Web.Mvc.HttpPost, System.Web.Mvc.AllowAnonymous]
        [ValidateAntiForgeryToken(Order = 0)]
        [ValidateInput(false)]
        [HandleAjaxException(Order = 1)]
        public ActionResult UpdateUser(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _accountControllerOrchestrator.EditUser(model);
                    return Json(new { Result = WebUiConstants.Success });
                }
                catch (Exception exception)
                {
                    _logger.ErrorException("In EditUser Action: ", exception);
                    ErrorSignal.FromCurrentContext().Raise(exception);
                    throw;
                }
            }
            return this.ModelStateJson(ModelState);
        }

        [System.Web.Mvc.HttpPost, System.Web.Mvc.AllowAnonymous]
        [ValidateAntiForgeryToken(Order = 0)]
        [ValidateInput(false)]
        [HandleAjaxException(Order = 1)]
        public ActionResult UpdatePresenter(EditPresenterInfoModel model)
        {
            if (ModelState.IsValid)
            {

                try
                {
                    var presenter = new Presenter
                    {
                        idUser = model.EditFields.WebUser.idUser,
                        BiographyLong = model.EditFields.BioLong,
                        Biography = model.EditFields.BioShort,
                        PhotoFull = model.EditFields.PhotoFull,
                        PhotoThumb = model.EditFields.PhotoThumb
                    };

                    var dataOperations = new DataOperations(TtsConfig.DefaultConnectionString);
                    var UpdatePresenterBySproc = dataOperations.UpdatePresenter(presenter);

                    var results = Json(new { Result = WebUiConstants.Success });
                    if (UpdatePresenterBySproc == 0)
                        results = Json(new { Result = WebUiConstants.Fail });
                    var user = _membershipService.GetWebUserById(model.EditFields.WebUser.idUser);

                    try
                    {
                        if (user.email != model.WebUser.email || user.FirstName != model.WebUser.FirstName ||
                            user.Initial != model.WebUser.Initial || user.LastName != model.WebUser.LastName ||
                            user.Institution.InstitutionName != model.WebUser.Institution.InstitutionName)
                            _membershipService.UpdateUserDetails(_globalConfig.Tenant,
                                model.EditFields.WebUser.FirstName, model.EditFields.WebUser.LastName, user.email,
                                model.EditFields.WebUser.Institution.InstitutionName,
                                user.Addresses.SingleOrDefault(a => a.AddressType == "Billing"),
                                user.Addresses.SingleOrDefault(a => a.AddressType == "Shipping"), model.WebUser.Title,
                                user.SageAccountId, (int)model.WebUser.timeZone);
                    }
                    catch (Exception ex)
                    {
                        _logger.ErrorException("In EditPresenter DETAILS: ", ex);

                        results = Json(new { Result = WebUiConstants.Fail });
                    }

                    try
                    {
                        if (user.email != model.EditFields.WebUser.email)
                            _membershipService.UpdateUserEmail(user.email, model.WebUser.email, _globalConfig.Tenant);

                        results = Json(new { Result = WebUiConstants.Fail });
                    }
                    catch (Exception ex)
                    {

                        _logger.ErrorException("In EditPresenter EMAIL: ", ex);
                    }

                    return results;
                }
                catch (Exception exception)
                {
                    _logger.ErrorException("In EditPresenter Action: ", exception);
                    ErrorSignal.FromCurrentContext().Raise(exception);
                    throw;
                }
            }
            return this.ModelStateJson(ModelState);
        }

        [System.Web.Mvc.HttpPost, System.Web.Mvc.AllowAnonymous]
        [ValidateAntiForgeryToken(Order = 0)]
        [ValidateInput(false)]
        [HandleAjaxException(Order = 1)]
        public ActionResult EditEmail(string oldEmail, string newEmail)
        {
            var user = _accountControllerOrchestrator.GetWebUserByEmail(newEmail);

            if (!ReferenceEquals(null, user))
                return Json(new { Result = WebUiConstants.Fail, EmailExists = "Yes" });

            _logger.Info("Updating Email from " + oldEmail + " to: " + newEmail + " by: " +
                         _appHelper.GetUserAuditInfo());
            if (ModelState.IsValid)
            {
                try
                {
                    _accountControllerOrchestrator.EditEmail(oldEmail, newEmail, _globalConfig.Tenant);

                    return Json(new
                    {
                        Result = WebUiConstants.Success,
                        Msg = "UpdatedEmailTo: " + newEmail + " from: " + oldEmail
                    });


                }
                catch (Exception exception)
                {
                    _logger.ErrorException("In EditEmail Action: ", exception);
                    ErrorSignal.FromCurrentContext().Raise(exception);
                    return Json(new { Result = WebUiConstants.Fail });

                }
            }
            return this.ModelStateJson(ModelState);
        }



        [System.Web.Mvc.HttpPost, System.Web.Mvc.AllowAnonymous]
        //[ValidateAntiForgeryToken(Order = 0)]
        [ValidateInput(false)]
        [HandleAjaxException(Order = 0)]
        public ActionResult UpdateUserByAffiliate(FormCollection _model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var model = new EditUserViewModel
                    {
                        //EditFields = _model.EditFields
                    };

                    _accountControllerOrchestrator.EditUser(model);
                    return Json(new { Result = WebUiConstants.Success });
                }
                catch (Exception exception)
                {
                    _logger.ErrorException("In EditUser Action: ", exception);
                    ErrorSignal.FromCurrentContext().Raise(exception);
                    throw;
                }
            }
            return this.ModelStateJson(ModelState);
        }


        [System.Web.Mvc.HttpPost, System.Web.Mvc.AllowAnonymous]
        [ValidateAntiForgeryToken(Order = 0)]
        [ValidateInput(false)]
        [HandleAjaxException(Order = 1)]
        public ActionResult UpdateInstitution(EditInstitutionInfoModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _accountControllerOrchestrator.EditInstitution(model);
                    return Json(new { Result = WebUiConstants.Success });
                }
                catch (Exception exception)
                {
                    _logger.ErrorException("In EditInstitution Action: ", exception);
                    ErrorSignal.FromCurrentContext().Raise(exception);
                    throw;
                }
            }
            return this.ModelStateJson(ModelState);
        }

        //[ClaimsAuthorize(Roles = "CUWebinarsAbsoluteAdmin")]
        public ActionResult Manage(ManageMessageId? message)
        {
            try
            {
                var manageModel = _accountControllerOrchestrator.BuildManageModel(message);

                ViewBag.ReturnUrl = Url.Action(ManageActionName);

                return View(manageModel);
            }
            catch (Exception exception)
            {
                _logger.ErrorException("In Manage Action", exception);
                throw;
            }
        }

        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        [System.Web.Mvc.AllowAnonymous]
        public ActionResult EditContactInfo(EditContactInfoModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _accountControllerOrchestrator.EditContactInfo(model);
                    return Json(new { Result = WebUiConstants.Success });
                }
                catch (Exception exception)
                {
                    _logger.ErrorException("In EditContactInfo Action", exception);
                    ErrorSignal.FromCurrentContext().Raise(exception);
                    throw;
                }
            }
            return this.ModelStateJson(ModelState);
        }


        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        public ActionResult Uploader(HttpPostedFileBase file, string target)
        {
            string formFields = Request.Form.ToString().Replace("parmList=", "");
            formFields = HttpUtility.UrlDecode(formFields);
            _logger.Info("UploadPhotoPostBack: " + formFields);
            try
            {
                HttpFileCollectionBase uploadFiles = Request.Files;

                // Build HTML listing the files received.
                string summary = "<p>Files Uploaded:</p><ol>";

                // Loop over the uploaded files and save to disk.
                // Access the uploaded file's content in-memory:
                System.IO.Stream inStream = file.InputStream;
                byte[] fileData = new byte[file.ContentLength];
                inStream.Read(fileData, 0, file.ContentLength);

                // Save the posted file in our "data" virtual directory.
                file.SaveAs(Server.MapPath("") + "\\App_Data\\" + file.FileName);


            }
            catch (Exception ex)
            {

                throw;
            }

            return null;

        }

        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        [System.Web.Mvc.AllowAnonymous]
        public ActionResult Manage(ManageModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _accountControllerOrchestrator.UpdateUserDetails(model);
                    return Json(new { Result = WebUiConstants.Success });
                }
                catch (Exception exception)
                {
                    _logger.ErrorException("In Manage Action", exception);
                    ErrorSignal.FromCurrentContext().Raise(exception);
                    throw;
                }
            }
            return this.ModelStateJson(ModelState);
        }


        //
        // GET: /Account/Login

        [System.Web.Mvc.AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            try
            {
                ViewBag.PageStyleType = "register";
                return View(_accountControllerOrchestrator.BuildLoginModel(returnUrl));
            }
            catch (Exception exception)
            {
                _logger.ErrorException("In Login Action", exception);
                throw;
            }
        }



        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        public ActionResult SignInFromReset(SignInModel model)
        {

            if (ModelState.IsValid && _accountControllerOrchestrator.LogUserIn(model))
            {
                var userPendingOrder =
                    _orderManagementService.GetOrdersByEmail(model.Email, 19)
                        .Where(o => o.OrderStatus == OrderStatus.InProcess)
                        .ToList();

                if (userPendingOrder.Any())
                {
                    return RedirectToAction("Checkout", "Cart");
                }
            }
            ProcessModelStateErrors();
            return RedirectToAction("MyWebinars", "Account");
        }


        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        [ValidateAntiForgeryToken(Order = 0)]
        [HandleAjaxException(Order = 1)]
        public ActionResult SignIn(SignInModel model)
        {
            if (ModelState.IsValid)
            {
                if (Request.IsAuthenticated)
                {
                    _logger.Warn("Authenticated user was served SignIn page. SessionInfo: " +
                                 _appHelper.GetSessionStartInfo());
                    return RedirectToAction("MyWebinars");
                }

                try
                {
                    string userMustVerify;

                    if (_accountControllerOrchestrator.SignUserIn(model, out userMustVerify))
                    {
                        var webUser = _orderManagementService.GetWebUser(model.Email);

                        var userPendingOrder =
                            _orderManagementService.GetOrdersByEmail(model.Email, 19)
                                .Where(o => o.OrderStatus == OrderStatus.InProcess)
                                .ToList();
                        var MsgForUser = "";
                        if (userPendingOrder.Count() > 1)
                        {
                            MsgForUser =
                                "We found these 'In Process' orders and are forwarding you to a screen where you can place the order 'OnHold', 'Cancel', or 'Submit' it.";

                            return Json(new { msgForUser = MsgForUser, result = LoggedInResult, returnUrl = "/cart/checkout" });
                        }

                        if (userPendingOrder.Count() == 1)
                        {

                            MsgForUser =
                                "We found an 'In Process' order and are forwarding you to a screen where you can place the order 'OnHold', 'Cancel', or 'Submit' it.";

                            return
                                Json(
                                    new
                                    {
                                        msgForUser = MsgForUser,
                                        result = LoggedInResult,
                                        returnUrl = "/resume/" + userPendingOrder.SingleOrDefault().idOrder
                                    });
                        }

                        // Handles an edge case where a user has been created anonymously in the cart and has just set their password.
                        // In such a case, we don't want to redirect back to the page where they just set their password. So send to base instead.
                        var returnUrl = string.IsNullOrWhiteSpace(model.ReturnUrl)
                            ? @"/"
                            : model.ReturnUrl.Contains(@"ACC/APWD") ? @"/" : Server.HtmlDecode(model.ReturnUrl);
                        if (returnUrl == "MyWebinars") returnUrl = "/MyWebinars";
                        return Json(new { result = LoggedInResult, returnUrl = returnUrl });
                    }


                    if (!string.IsNullOrEmpty(userMustVerify))
                    {
                        _logger.Info("Account.SignIn UserMustVerify. Session: {0}, Email: {1}",
                            _appHelper.GetUserAuditInfo(),
                            model.Email
                        );

                        return Json(new { result = ConfirmedResult, email = model.Email, password = model.Password });
                    }

                    // If we got this far, something failed, redisplay form
                    _logger.Warn("Account.SignIn Failed. Email: {0},  Session: {1}",
                        model.Email,
                        _appHelper.GetUserAuditInfo()
                    );

                    ModelState.AddModelError(
                        string.Empty,
                        // Needs to be an empty string to show up in ValidationSummary as not model-level error.
                        "The user name or password provided is incorrect."
                    );
                }
                catch (Exception exception)
                {
                    _logger.ErrorException(
                        string.Format("Account.SignIn Failed. Email: {0}, Session: {1}"
                            , model.Email, _appHelper.GetUserAuditInfo()), exception);
                    ErrorSignal.FromCurrentContext().Raise(exception);

                    ModelState.AddModelError(
                        string.Empty,
                        // Needs to be an empty string to show up in ValidationSummary as not model-level error.
                        "Server Error #536: For customer service contact us by using the Online Chat button below or emailing " +
                        _globalConfig.TenantEmail + "."
                    );
                }
            }

            return this.ModelStateJson(ModelState);
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        [ValidateAntiForgeryToken(Order = 0)]
        [HandleAjaxException(Order = 1)]

        public ActionResult SignInFromCart(SignInModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string userMustVerify; // not relevant in this user flow. So gets discarded.

                    if (_accountControllerOrchestrator.SignUserIn(model, out userMustVerify))
                    {
                        _logger.Info("Account.SignIn Post Success in cart.{1} Session={0}",
                            _appHelper.GetUserAuditInfo(), model.Email);
                        WebUser webUser = _accountControllerOrchestrator.GetWebUserByEmail(model.Email);
                        if (_stateService.GetValue<Affiliate>(WebUiConstants.CurrentAffiliate) != null)
                        {
                            var cAffilliate = _stateService.GetValue<Affiliate>(WebUiConstants.CurrentAffiliate);

                            if (cAffilliate.idUserAff == 19)
                            {
                                _logger.Info(
                                    "SigninFromCart calls DetermineAffiliateByAlternativeMeans where aff=19: " +
                                    webUser.idUser);

                                var affiliate =
                                    _orderManagementService.DetermineAffiliateByAlternativeMeans(webUser.idUser);
                                // if null returned, just use whatever is stored in Session for CurrentAffiliate.
                                //      O/w, set that value.
                                if (!ReferenceEquals(null, affiliate))
                                {
                                    _stateService.SetValue(WebUiConstants.CurrentAffiliate, affiliate);
                                    _logger.Info("Account.SignInFromCart. Email: {1}, Affiliate: {2},  Session: {0}",
                                        _appHelper.GetUserAuditInfo(),
                                        model.Email, affiliate.ttsDomain
                                    );
                                }
                                else
                                {
                                    _logger.Warn(
                                        "Account.SignInFromCartDidNotFindExistingAffiliate. Email: {1}, Affiliate: null, Session: {0}",
                                        _appHelper.GetUserAuditInfo(),
                                        model.Email
                                    );
                                }
                            }
                        }
                        return Json(new { result = LoggedInResult, UserId = webUser.idUser });
                    }
                }
                catch (Exception exception)
                {
                    _logger.ErrorException("In SignInFromCart Action", exception);
                    ErrorSignal.FromCurrentContext().Raise(exception);
                }

                // If we got this far, something failed, redisplay form
                _logger.Warn("Account.SignInFromCart Failed. {0} |  Session= {1}",
                    model.Email,
                    _appHelper.GetUserAuditInfo()
                );

                ModelState.AddModelError(
                    string.Empty,
                    // Needs to be an empty string to show up in ValidationSummary as not model-level error.
                    "The user name or password provided is incorrect."
                );
            }

            return this.ModelStateJson(ModelState);
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        //[ValidateJsonAntiForgeryToken]
        public JsonResult ResetPassword(string email)
        {
            _logger.Info("Account.ResetPassword GET. Email = {1} Session={0}", _appHelper.GetUserAuditInfo(), email);
            var user = new WebUser();
            var errorsDictionary = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                {"Result", WebUiConstants.Fail}
            };
            try
            {
                user = _accountControllerOrchestrator.GetWebUserByEmail(email);
            }
            catch (Exception ex)
            {
                _logger.FatalException("ResetPassword | Attempt to get WebUserByEmail tossed: ", ex);

                throw;
            }
            try
            {
                if (user == null)
                    return Json(new { Result = WebUiConstants.Fail });

                _accountControllerOrchestrator.ResetPassword(_globalConfig.Tenant, email);
                //invalid Logins return here

                return Json(new { Result = WebUiConstants.Success });
            }
            catch (ValidationException validationException)
            {
                if (validationException.Message.StartsWith("Invalid email", StringComparison.OrdinalIgnoreCase))
                {
                    errorsDictionary.Add("Invalid", "UnkownEmail");
                }

                _logger.FatalException(
                    string.Format("Account.ResetPassword GET Failed. {0}, Session = {1} on email: {2}",
                        validationException.Message,
                        _appHelper.GetUserAuditInfo(),
                        email), validationException
                );
            }
            catch (UserCreatedMembershipException userCreatedMembershipException)
            {
                _logger.FatalException(
                    string.Format(
                        "Account.ResetPassword Failed. {0}, Session = {1} on email: {2}",
                        userCreatedMembershipException.Message,
                        _appHelper.GetUserAuditInfo(), email),
                    userCreatedMembershipException
                );

                try
                {
                    var removeNotVerified = _accountControllerOrchestrator.RemoveClaim(email);
                }
                catch (Exception ex)
                {
                    _logger.FatalException(
                        string.Format(
                            "Account.ResetPassword Failed AGAIN. {0}, Session = {1} on email: {2}",
                            userCreatedMembershipException.Message,
                            _appHelper.GetUserAuditInfo(), email),
                        ex
                    );

                    errorsDictionary.Add("Invalid", "UserNotVerified");
                }

            }
            catch (Exception exception)
            {
                _logger.FatalException(
                    string.Format("Account.ResetPassword GET Failed. {0}, Session = {1} on email: {2}",
                        exception.Message, _appHelper.GetUserAuditInfo(), email),
                    exception
                    );
            }

            return Json(errorsDictionary);
        }



        [System.Web.Mvc.AllowAnonymous]

        public ActionResult PasswordResetConfirm(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.Warn(
                    "Account.PasswordResetConfirm.GET was passed empty or null ID. Session=PasswordResetConfirm. " +
                    _appHelper.GetUserAuditInfo());
                ModelState.AddModelError(string.Empty,
                    "There appears to have been a problem with the link which you clicked to navigate to this page. Please try clicking the link from the email again.");
            }

            var vm = new ChangePasswordFromResetKeyInputModel
            {
                Key = id
            };

            UserAccount userAccount = _membershipService.GetUserAccountByVerificationKey(id);
            if (ReferenceEquals(null, userAccount))
            {
                _logger.Error("PasswordResetConfirm | Expired Key ");
                vm.Email = "Expired Key";
            }
            else
            {
                vm.Email = userAccount.Email;
            }
            return View(vm);
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult PasswordResetConfirm(ChangePasswordFromResetKeyInputModel model, string returnUrl)
        {

            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(model.Key))
                {
                    _logger.Error(
                        "Account.PasswordResetConfirm.POST was passed empty or null ID. Session=PasswordResetConfirm. " +
                        _appHelper.GetUserAuditInfo());
                    ModelState.AddModelError(string.Empty,
                        "There was a problem with the link which you clicked to navigate to this page. Please try clicking the link from the email again. For customer service contact us by using the Online Chat button below or emailing " + _globalConfig.TenantEmail + ".");
                }
                else
                {
                    UserAccount userAccount = _membershipService.GetUserAccountByVerificationKey(model.Key);
                    if (ReferenceEquals(null, userAccount))
                    {
                        _logger.Error("PasswordResetConfirm | User not found " + model.Email + " for key:" + model.Key);
                        return Json(new { Result = "Not Found: " + model.Email });
                    }

                    try
                    {
                        if (_accountControllerOrchestrator.ChangePasswordFromResetKey(model.Key, model.Password))
                        {
                            var ordersInProc = _orderManagementService
                                 .GetOrdersByEmail(model.Email, 19).FirstOrDefault(o => o.OrderStatus == OrderStatus.InProcess);

                            bool checkoutRedirect = ordersInProc != null;

                            model.ChangePasswordSucceeded = true;

                            _logger.Info("Account.PasswordResetConfirm Success. Session=" +
                                         _appHelper.GetUserAuditInfo());
                            return Json(new { Result = "Success", email = model.Email, password = model.Password });
                        }
                        else
                        {
                            model.ChangePasswordSucceeded = false;
                            _logger.Info("Account.PasswordResetConfirm Failed on " + model.Email + ". Session=" +
                                         _appHelper.GetUserAuditInfo());

                            return Json(new { Result = "Invalid Key" });
                        }
                    }
                    catch (ValidationException validationException)
                    {
                        _logger.Fatal("Account.ResetPassword. " + validationException.Message + " Session=" +
                                      _appHelper.GetUserAuditInfo());
                        ModelState.AddModelError(string.Empty, "The new password must be different than the old password.");
                    }
                    catch (Exception exception)
                    {
                        _logger.Error("_accountControllerOrchestrator.ChangePasswordFromResetKey {0} tossed error to: {1}", model.Key, model.Email);
                        ModelState.AddModelError(string.Empty,
                            "We've logged an error. Please attempt the password reset procedure again. In case of persistent failures contact us at " + _globalConfig.TenantEmail + " - or, for immediate assistance contact us with our Help & Feedback button in your lower right screen.");
                        ErrorSignal.FromCurrentContext().Raise(exception);
                    }
                }
            }

            _logger.Error("Invalid ModelState in PasswordResetConfirm" + model.Key);

            return this.ModelStateJson(ModelState);
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public ActionResult PasswordResetConfirmInCheckout(ChangePasswordFromResetKeyInputModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(model.Key))
                {
                    _logger.Error(
                        "Account.PasswordResetConfirm.POST was passed empty or null ID. Session=PasswordResetConfirm. " +
                        _appHelper.GetUserAuditInfo());

                    return Json(new { Result = "Fail", Msg = "Invalid Key: Please ensure you are using the most recent Reset Request email." });

                }
                else
                {
                    var _key = model.Key.Split('/').Last();
                    UserAccount userAccount = _membershipService.GetUserAccountByVerificationKey(_key);
                    if (ReferenceEquals(null, userAccount))
                    {
                        _logger.Error("PasswordResetConfirmICO | User not found " + model.Email + " for key:" + model.Key);
                        return Json(new { Result = "Failed", Msg = "Not Found: " + model.Email });
                    }

                    try
                    {
                        if (_accountControllerOrchestrator.ChangePasswordFromResetKey(_key, model.Password))
                        {
                            model.ChangePasswordSucceeded = true;
                            _logger.Info("Account.PasswordResetConfirmICO.Session=" + _appHelper.GetUserAuditInfo());
                            return Json(new { Result = "Success" });
                        }
                        else
                        {
                            model.ChangePasswordSucceeded = false;
                            _logger.Info("Account.PasswordResetConfirmICO.Failed on " + model.Email + ". Session=" +
                                         _appHelper.GetUserAuditInfo());

                            return Json(new { Result = "Fail", Msg = "The new password must be different than the old password." });
                        }
                    }
                    catch (ValidationException validationException)
                    {

                        _logger.Fatal("Account.ResetPassword. " + validationException.Message + " Session=" +
                                      _appHelper.GetUserAuditInfo());

                        return Json(new { Result = "Fail", Msg = "The new password must be different than the old password." });

                    }
                    catch (Exception exception)
                    {
                        _logger.Error("_accountControllerICO {0} tossed error to: {1}", model.Key, model.Email);
                        return Json(new { Result = "Fail", Msg = "We've logged an error. Please attempt the password reset procedure again. In case of persistent failures contact us at " + _globalConfig.TenantEmail + " - or, for immediate assistance contact us with our Help & Feedback button in your lower right screen." });
                        ErrorSignal.FromCurrentContext().Raise(exception);

                    }
                }
            }

            _logger.Error("Invalid ModelState in PasswordResetConfirmICO" + model.Key);

            return this.ModelStateJson(ModelState);
        }

        //
        // POST: /Account/LogOff

        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {

            string[] myCookies = Request.Cookies.AllKeys;
            foreach (string cookie in myCookies)
            {
                Response.Cookies[cookie].Expires = DateTime.Now.AddDays(-1);
            }
            if (User.Identity.IsAuthenticated)
            {
                _logger.Info("Account.LogOff. Session=" + _appHelper.GetUserAuditInfo());

                try
                {

                    _accountControllerOrchestrator.LogUserOut((ClaimsPrincipal)User);

                }
                catch (Exception exception)
                {
                    _logger.FatalException(
                        string.Format("Account.LogOff at Unauthenticated User. Session={0}",
                            _appHelper.GetUserAuditInfo()), exception);

                    ErrorSignal.FromCurrentContext().Raise(exception);
                }
            }

            return Redirect(_globalConfig.TenantURL);
        }


        // POST: /Account/Register
        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public JsonResult CheckInstitution(string institution)
        {
            return Json(new { Result = WebUiConstants.Success }, JsonRequestBehavior.AllowGet);
        }

        // POST: /Account/Register
        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public JsonResult CheckZip(string zip)
        {
            var resultObject = new Dictionary<string, string>();
            const string success = "success";
            string zipAddress;

            int? numVal = _accountControllerOrchestrator.ParseZip(zip);

            //  If zip can't be parsed, send error to client
            if (!numVal.HasValue)
            {
                resultObject.Add(success, "invalid format");
                return Json(resultObject, JsonRequestBehavior.AllowGet);
            }

            try
            {
                zipAddress = _appHelper.GetCityStateFromZip(numVal.Value);
            }
            catch (Exception exception)
            {
                _logger.ErrorException("In CheckZip Action", exception);
                ErrorSignal.FromCurrentContext().Raise(exception);
                throw;
            }

            //  If no zip address fields found, send error to client
            if (string.IsNullOrWhiteSpace(zipAddress))
            {
                resultObject.Add(success, "false");
                return Json(resultObject, JsonRequestBehavior.AllowGet);
            }

            //  Build and return the good fields
            _accountControllerOrchestrator.BuildCityStateTimeZoneData(resultObject, zipAddress);

            //resultObject.Add("TimeZone", 3.ToString());

            return Json(resultObject, JsonRequestBehavior.AllowGet);
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.AllowAnonymous]
        public ActionResult AddPasswordForCartCreatedUser(string email)
        {


            int idOrder = 0;
            string s = email;
            bool result = int.TryParse(s, out idOrder);

            if (result)
                email = _orderManagementService.GetOrderById(idOrder).BillingEmail;

            if (string.IsNullOrWhiteSpace(email))
                return View();

            var createUserConfirmedViewModel =
                _accountControllerOrchestrator.PrepareViewForCartUserAddingPassword(email);

            return View(createUserConfirmedViewModel);
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        [ValidateAntiForgeryToken(Order = 0)]
        [HandleAjaxException(Order = 1)]
        public ActionResult AddPasswordForCartCreatedUser(CreateUserConfirmedViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (_accountControllerOrchestrator.AddPasswordForCartCreatedUser(model))
                        return Json(new { Result = WebUiConstants.Success });


                    return Json(new { Result = WebUiConstants.TimedOut });
                }
                catch (Exception exception)
                {
                    _logger.ErrorException(string.Format("AddPasswordForCartCreatedUser | Session: {0}", _appHelper.GetSessionStartInfo()), exception);
                    ModelState.AddModelError(string.Empty, "We have logged an error. For customer service contact us by using the Online Chat button below or emailing " + _globalConfig.TenantEmail + ".");
                }
            }

            return this.ModelStateJson(ModelState);
        }

        //[System.Web.Mvc.ActionName("autocomplete")]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Get)]
        [System.Web.Mvc.AllowAnonymous]
        public JsonResult AutocompleteInstitution()
        {
            List<string> ac = _appHelper.InstitutionAutoComplete(Request.QueryString["term"], Request.QueryString["zip"]);
            //string name, msa;
            //var retValue = new { values = ac };
            return Json(ac, JsonRequestBehavior.AllowGet);

        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        [ValidateAntiForgeryToken(Order = 0)]
        [HandleAjaxException(Order = 1)]
        public JsonResult CheckEmail(string email, bool disregardInstitutionDomain, int? orderId = null)
        {
            try
            {
                _logger.Info("CheckEmail called: " + email + "| Session=" + _appHelper.GetUserAuditInfo());
                var resultObject = new Dictionary<string, string>();

                if (orderId.HasValue)
                {
                    _accountControllerOrchestrator.UpdateBillingEmailOfOrder(orderId.Value, email);
                }

                var webUserId = _accountControllerOrchestrator.GetWebUserIdByEmail(email);

                if (webUserId.HasValue && webUserId > 0)
                {
                    var userAcct = _membershipService.GetUserAccountByWebUserId(_globalConfig.Tenant, webUserId.Value);

                    if (orderId != null)
                    {
                        var foundExistingOrder = _orderManagementService.UserHasPrexistingOrder(
                                _orderManagementService.GetOrderById(orderId.Value));
                        if (foundExistingOrder != null)
                        {
                            resultObject.Add("orderId", foundExistingOrder.idOrder.ToString());
                            resultObject.Add("orderRowId", foundExistingOrder.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active).idOrderRow.ToString());
                            _logger.Info("CheckEmail found existing: " + foundExistingOrder.idOrder + " when checking: " + orderId.Value);
                        }

                    }


                    if (userAcct != null && !userAcct.HasClaim(ClaimTypes.FullName))
                    {

                        resultObject.Add("isConfirmed", "false");
                    }
                    else
                    {
                        resultObject.Add("isConfirmed", "true");
                    }
                    //  Email exists and view is notified.
                    resultObject.Add("success", "foundExisting");
                    return Json(resultObject);
                }

                resultObject.Add("email", "wasNotFound");
                resultObject.Add("createUser", "true");

                if (disregardInstitutionDomain)
                {
                    resultObject["createUser"] = "false";
                    return Json(resultObject);
                }

                var institution = _accountControllerOrchestrator.GetInstitutionFromEmail(email);

                if (institution == null)
                    return Json(resultObject);

                //  if we have a match between user's email (domain)
                //  and the domainName stored in existing Institution record
                //  we can offer to populate the user's billing info
                resultObject.Add("success", "foundInstitution");
                resultObject.Add("Institution", institution.InstitutionName);
                resultObject.Add("Address", institution.Address);
                resultObject.Add("City", institution.City);
                resultObject.Add("State", institution.State);
                resultObject.Add("Zip", institution.Zip);

                _logger.Info("CheckEmail: institution was found. idInstitution={0}", institution.idInstitution);

                return Json(resultObject);
            }
            catch (Exception exception)
            {
                _logger.ErrorException("In CheckEmail method", exception);
                //Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                return Json(new { error = WebUiConstants.Fail });
            }
        }

        // POST: /Account/Register
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        [ValidateJsonAntiForgeryToken(Order = 0)]
        [HandleAjaxException(Order = 1)]
        public ActionResult RegisterFromCart(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                _logger.Info("Account.Register: {0} | Session= {1}", model.RegisterFields.Email.Trim(),
                    _appHelper.GetUserAuditInfo());

                try
                {
                    var webUser = _accountControllerOrchestrator.GetWebUserByEmail(model.RegisterFields.Email);

                    if (webUser != null)
                    {
                        _logger.Info("Existing email is resubmitted for registration {0} | Sesssion = {1}", model.RegisterFields.Email.Trim(), _appHelper.GetUserAuditInfo());

                        return Json(new { Result = WebUiConstants.Success, UserId = webUser.idUser, Email = webUser.email });
                    }

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);

                    _logger.FatalException("Account.Register Catch block: " +
                        ex.Message + "| Session=" +
                        _appHelper.GetUserAuditInfo(),
                        ex);
                }

                try
                {
                    var webUser = _accountControllerOrchestrator.CreateWebUserFromCart(model);

                    _logger.Info("New email is registered {0} | Sesssion = {1}", model.RegisterFields.Email.Trim(), _appHelper.GetUserAuditInfo());

                    return Json(new { Result = WebUiConstants.Success, UserId = webUser.idUser, Email = webUser.email });
                }
                catch (MembershipCreateUserException membershipCreateUserException)
                {
                    ModelState.AddModelError(string.Empty, ErrorCodeToString(membershipCreateUserException.StatusCode));

                    _logger.FatalException("Account.Register Catch block: " +
                        membershipCreateUserException.Message +
                        "| Session=" +
                        _appHelper.GetUserAuditInfo(),
                        membershipCreateUserException
                        );
                }
                catch (ValidationException validationException)
                {
                    ModelState.AddModelError(string.Empty, validationException.Message);

                    _logger.FatalException("Account.Register Catch block: " +
                        validationException.Message + "| Session=" +
                        _appHelper.GetUserAuditInfo(),
                        validationException);
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError(string.Empty, "Server Error at Account.Register. For customer service contact us by using the Online Chat button below or emailing " + _globalConfig.TenantEmail + ".");
                    ErrorSignal.FromCurrentContext().Raise(exception);
                    _logger.FatalException(string.Format("Account.Register Catch block | Session= {0}", _appHelper.GetUserAuditInfo()), exception);
                }

                // If we got this far, something failed, redisplay form
                return this.ModelStateJson(ModelState);
            }

            _logger.Fatal("Account.Register failed! Session={0}", _appHelper.GetUserAuditInfo());

            return this.ModelStateJson(ModelState);
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        [ValidateAntiForgeryToken(Order = 0)]
        [HandleAjaxException(Order = 1)]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                _logger.Info("Account.Register: " + model.RegisterFields.Email.Trim() +
                             "| Session=" + _appHelper.GetUserAuditInfo());

                try
                {
                    _accountControllerOrchestrator.RegisterAndLogInUser(model);
                    _logger.Info("Account.Register UserAdded: " + model.RegisterFields.Email);

                    return Json(new { Result = WebUiConstants.Success });
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError(string.Empty, ErrorCodeToString(e.StatusCode));

                    _logger.Error("Account.Register Catch block: " + e.Message + "| Session=" +
                                  _appHelper.GetUserAuditInfo());
                }
                catch (ValidationException validationException)
                {
                    ModelState.AddModelError(string.Empty, validationException.Message);

                    _logger.Error("Account.Register Catch block: " + validationException.Message + "| Session=" +
                                  _appHelper.GetUserAuditInfo());
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty,
                        "An error has occurred at the server and it has been logged. Please try again, or contact us so we can resolve the problem.");

                    _logger.Error("Account.Register Catch block: " + e.Message + "| Session=" +
                                  _appHelper.GetUserAuditInfo());
                    _logger.Error("Account.Register Catch block InnerException: " +
                                  (e.InnerException == null ? string.Empty : e.InnerException.Message));
                }
            }
            else
            {
                return this.ModelStateJson(ModelState);
            }
            _logger.Fatal("Account.Register failed! Session=" + _appHelper.GetUserAuditInfo());

            // If we got this far, something failed, redisplay form
            return this.ModelStateJson(ModelState);

        }

        // POST: /Account/Register
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        [ValidateJsonAntiForgeryToken(Order = 0)]
        [HandleAjaxException(Order = 1)]
        public ActionResult CreateUserAccountFromCart(string email)
        {
            //  Not adding any ModelState errors in this method. This method is not to return any GUI feedback.
            //  It is effective invoked as a fire and forget, even though it sends an http response (which gets ignored at client.)

            if (!string.IsNullOrWhiteSpace(email))
            {
                try
                {
                    _logger.Info("CreateUserAccountFromCart for: " + email);
                    var tempPassword = _accountControllerOrchestrator.CreateUserAccountFromCart(email);

                    return Json(new { Result = WebUiConstants.Success, KeyForUser = tempPassword });
                }
                catch (MembershipCreateUserException e)
                {
                    _logger.Error("CreateUserAccountFromCart Catch block: {0} | Session= {1}",
                        e.Message, _appHelper.GetUserAuditInfo());
                }
                catch (Exception e)
                {
                    _logger.Error("CreateUserAccountFromCart Catch block: {0} | Session= {1}",
                        e.Message, _appHelper.GetUserAuditInfo());
                }
            }

            _logger.Fatal("CreateUserAccountFromCart failed! Session=" + _appHelper.GetUserAuditInfo());

            return Json(new { Result = WebUiConstants.Fail }); // This actually gets discarded by view.           
        }

        public PartialViewResult GetAddUserFieldsForModal()
        {
            var editUserModel = new EditUserModel
            {
                BillingAddress = new AddressModel { TypeOfAddress = AddressType.Billing },
                UserType = UserType.Customer,
            };

            return PartialView("~/Views/Webinar/Partials/_AddNewUser.cshtml", editUserModel);
        }

        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken(Order = 0)]
        [HandleAjaxException(Order = 1)]

        public ActionResult AdminAddNewUser(EditUserModel editUserModel)
        {
            if (ModelState.IsValid)
            {
                var idUser = 0;
                try
                {
                    idUser = _accountControllerOrchestrator.CreateUserForAdmin(editUserModel);

                    return Json(new { Result = WebUiConstants.Success, UserId = idUser });
                }
                catch (Exception exception)
                {
                    if (exception.Message.Contains("already"))
                    {
                        _stateService.SetValue("editUserModel", editUserModel);

                        idUser = Convert.ToInt32(_accountControllerOrchestrator.GetWebUserIdByEmail(editUserModel.Email));
                        return Json(new { Result = WebUiConstants.Success, UserId = idUser });
                    }
                    _logger.ErrorException("In AdminAddNewUser Action", exception);
                    return Json(new { Result = WebUiConstants.Fail });
                }
            }
            return this.ModelStateJson(ModelState);
        }

        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken(Order = 0)]
        [HandleAjaxException(Order = 1)]
        public ActionResult UpdateShippingDetails(ShippingDetailsModel shippingDetailsModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var usershipping =
                        _accountControllerOrchestrator.GetWebUserById(shippingDetailsModel.UserId)
                            .Addresses.Where(a => a.AddressType == "Shipping");
                    //if (shippingDetailsModel.ShippingAddress != usershipping)
                    //{
                    //    _logger.Info("Shipping details were updated by: " + _appHelper.GetUserAuditInfo());
                    //    _accountControllerOrchestrator.UpdateShippingAddressDetails(
                    //        shippingDetailsModel.ShippingAddress, shippingDetailsModel.UserId);
                    //}
                    _accountControllerOrchestrator.AddShippingAddressVerifiedClaim(shippingDetailsModel.UserId);

                    return Json(new { Result = WebUiConstants.Success });
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
                    _logger.Error("UpdateShippingDetails dbEntityValidationException errors | {0}",
                        stringBuilder.ToString());
                }

                catch (Exception e)
                {
                    _logger.Error("UpdateShippingDetails Catch block: {0} | Session = {1} | UserId: {2}", e.Message,
                        _appHelper.GetUserAuditInfo(), shippingDetailsModel.UserId);
                }
                return Json(new { Result = WebUiConstants.Fail });
            }
            return this.ModelStateJson(ModelState);

        }

        //
        // POST: /Account/Disassociate

        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disassociate(string provider, string providerUserId)
        {
            //string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
            //ManageMessageId? message = null;

            //// Only disassociate the account if the currently logged in user is the owner
            //if (ownerAccount == User.Identity.Name)
            //{
            //    // Use a transaction to prevent the user from deleting their last login credential
            //    using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
            //    {
            //        bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            //        if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
            //        {
            //            OAuthWebSecurity.DeleteAccount(provider, providerUserId);
            //            scope.Complete();
            //            message = ManageMessageId.RemoveLoginSuccess;
            //        }
            //    }
            //}

            //return RedirectToAction("Manage", new { Message = message });
            return null;
        }

        //
        // POST: /Account/ExternalLogin



        #region Helpers

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (!ReferenceEquals(null, returnUrl))
            {
                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "Home");
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return
                        "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return
                        "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return
                        "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return
                        "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }

        #endregion
        private string ProcessModelStateErrors()
        {
            //Please use this general pattern when logging ModelState errors.
            var myErr = "ProcessModelStateErrors found errors.";

            foreach (ModelState modelState in ViewData.ModelState.Values)
            {
                foreach (ModelError error in modelState.Errors)
                {
                    myErr += error.ErrorMessage + Environment.NewLine;
                }
            }
            myErr += "Session Info: " + Environment.NewLine;
            myErr += _appHelper.GetUserAuditInfo();

            //a better implementation:
            //http://stackoverflow.com/questions/2845852/asp-net-mvc-how-to-convert-modelstate-errors-to-json
            var errorList = ModelState.Where(kvp => kvp.Value.Errors.Count > 0)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Errors.Select(e => e.ErrorMessage)
                    .ToArray()
            );

            _logger.Error(errorList.ToString());
            return myErr;
        }


        public ActionResult GetUserMessages()
        {
            return PartialView("_MessagesPartial");
        }



        //[ValidateAntiForgeryToken(Order = 0)]
        //[HandleAjaxException(Order = 1)]
        //public ActionResult UpdateSubscriptionDetails(DiscountDetailsModel discountDetailsModel)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _accountControllerOrchestrator.UpdateDiscountDetails(discountDetailsModel.Discount,
        //                discountDetailsModel.UserId);
        //            return Json(new { Result = WebUiConstants.Success });
        //        }
        //        catch (DbEntityValidationException dbEntityValidationException)
        //        {
        //            var stringBuilder = new StringBuilder();

        //            foreach (var validationErrors in dbEntityValidationException.EntityValidationErrors)
        //            {
        //                foreach (var validationError in validationErrors.ValidationErrors)
        //                {
        //                    //Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName,validationError.ErrorMessage);
        //                    stringBuilder.AppendFormat("Property: {0} Error: {1} ", validationError.PropertyName,
        //                        validationError.ErrorMessage);
        //                }
        //            }
        //            _logger.Error("UpdateDiscountDetails dbEntityValidationException errors | {0}",
        //                stringBuilder.ToString());
        //        }

        //        catch (Exception e)
        //        {
        //            _logger.Error("UpdateDiscountDetails Catch block: {0} | Session = {1} | UserId: {2}", e.Message,
        //                _appHelper.GetUserAuditInfo(), discountDetailsModel.UserId);
        //        }
        //        return Json(new { Result = WebUiConstants.Fail });
        //    }
        //    return this.ModelStateJson(ModelState);
        //}

        protected override void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                var logger = _logger as IDisposable;
                if (logger != null)
                    logger.Dispose();

                _accountControllerOrchestrator.Dispose();
                _membershipService.Dispose();
                _orderManagementService.Dispose();

                base.Dispose(true);
            }
            _disposed = true;
        }



        [System.Web.Mvc.HttpPost]
        [HandleAjaxException]
        public ActionResult UpdateCpSubscription(string notes, int idOrder, DateTime newCPExpiryDate)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var thisSubscription = _orderManagementService.GetDiscountByCode("CP_" + idOrder);
                    var priorNotes = thisSubscription.Notes ?? "";

                    var priorExpiry = thisSubscription.DateValidTo;

                    notes = "Subscription was invoiced on " + DateTime.Now.ToShortDateString() + " " +
                            "from: " + priorExpiry.ToShortDateString()
                        + " to: " + newCPExpiryDate + " by: " + User.Identity.Name + " USER NOTES: " + priorNotes;


                    thisSubscription.Notes = notes;
                    thisSubscription.DateValidTo = newCPExpiryDate;
                    thisSubscription.DateBilled = DateTime.Now;

                    _accountControllerOrchestrator.UpdateDiscountDetails(thisSubscription);
                    return Json(new { Result = WebUiConstants.Success, Notes = "Notes:" + notes });
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
                    _logger.Error("UpdateDiscountDetails dbEntityValidationException errors | {0}",
                        stringBuilder.ToString());
                }

                catch (Exception e)
                {
                    _logger.Error("UpdateDiscountDetails Catch block: {0} | Session = {1} | idOrder: {2}", e.Message,
                        _appHelper.GetUserAuditInfo(), idOrder);
                }
                return Json(new { Result = WebUiConstants.Fail });
            }
            return this.ModelStateJson(ModelState);

        }
        [System.Web.Mvc.HttpPost]
        [HandleAjaxException]
        public ActionResult UpdateDiscountNotes(string notes, int idOrder)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var thisSubscription = _orderManagementService.GetDiscountByOrderId(idOrder);
                    var priorNotes = thisSubscription.Notes ?? "";

                    notes += priorNotes;

                    thisSubscription.Notes = notes;

                    _accountControllerOrchestrator.UpdateDiscountDetails(thisSubscription);
                    return Json(new { Result = WebUiConstants.Success, Notes = "Notes:" + notes });
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
                    _logger.Error("UpdateDiscountDetails dbEntityValidationException errors | {0}",
                        stringBuilder.ToString());
                }

                catch (Exception e)
                {
                    _logger.Error("UpdateDiscountDetails Catch block: {0} | Session = {1} | idOrder: {2}", e.Message,
                        _appHelper.GetUserAuditInfo(), idOrder);
                }
                return Json(new { Result = WebUiConstants.Fail });
            }
            return this.ModelStateJson(ModelState);

        }

        //public ActionResult GetUserDiscount()
        //{
        //    var discountModel = _accountControllerOrchestrator.BuildDiscountModelForUser();
        //    return discountModel;

        //}
    }
}
