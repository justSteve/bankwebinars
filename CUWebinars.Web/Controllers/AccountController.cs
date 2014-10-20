using System.Globalization;
using CUWebinars.Web.Core.Orchestrators;
using CUWebinars.Web.Infrastructure.Attributes;
using CUWebinars.Web.Infrastructure.Extensions;
using BrockAllen.MembershipReboot;
using CUWebinars.Business.AccountService;
using CUWebinars.Business.Constants;
using CUWebinars.Business.Models;
using CUWebinars.Business.Services;
using CUWebinars.Web.Core;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Membership;
using CUWebinars.Web.Models;
using CUWebinars.Web.Notification.Templates;
using CUWebinars.Web.Services;
using CUWebinars.Web.ViewModel;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using Newtonsoft.Json.Linq;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Security;
using ClaimsExtensions = CUWebinars.Web.Helpers.ClaimsExtensions;
using ClaimTypes = CUWebinars.Business.Constants.ClaimTypes;


namespace CUWebinars.Web.Controllers
{
    [ElmahHandleError]
    [System.Web.Mvc.Authorize]
    public class AccountController : Controller
    {
        private const string ManageActionName = "Manage";
        private const string LoggedInResult = "LoggedIn";
        private const string ConfirmedResult = "Confirmed";
        private readonly GlobalConfig _globalConfig = GlobalConfig.GlobalConfigSingleton;

        private readonly IAccountControllerOrchestrator _accountControllerOrchestrator;
        private readonly ILogger _logger;
        private readonly IMembershipService _membershipService;
        private readonly IStateService _stateService;
        private readonly IOrderManagementService _orderManagementService;
        private bool _disposed;

        public AccountController(
            IAccountControllerOrchestrator accountControllerOrchestrator,
            ILogger logger,
            IMembershipService membershipService,
            IOrderManagementService orderManagementService,
            IStateService stateService)
        {
            _accountControllerOrchestrator = accountControllerOrchestrator;
            _logger = logger;
            _membershipService = membershipService;
            _orderManagementService = orderManagementService;
            _stateService = stateService;
        }

        public ActionResult COC(int idWebinar, string displayName)
        {
            //if (User.Identity.IsAuthenticated)
            //{

            //}
            //OrderRow row = orderManagementService.LoadOrderRow(id);
            ViewBag.Webinar = idWebinar;
            ViewBag.displayName = displayName;
            //Logger.Info("COC executed: " + id + " by " + membershipService.;

            return View("~/Views/home/coc.cshtml");
        }


        public ActionResult CertificateOfCompletionList(int id, string displayNames)
        {
            string[] listNames = displayNames.Split(Environment.NewLine.ToCharArray());

            string listing = listNames.Where(s => s != "").Aggregate("", (current, s) => current + ("<p>http://www.@Tenant.com/Home/COC/?idUser=" + id + "&displayName=" + s.Replace(' ', '+') + "</p>"));

            //foreach (string s in listNames)
            //{
            //    if (s != "")
            //    {
            //        listing += ("<p>http://www.@Tenant.com/Home/COC/?idUser=" + id + "&displayName=" +
            //                    s.Replace(' ', '+') + "</p>");
            //    }
            //}

            ViewData["listing"] = listing;
            //Logger.Instance.LogMessage("COC List executed: " + id + " by " +
            //                           UserFacade.Instance.GetCurrentUser().FullName);

            return View("~/Views/home/coc.cshtml");
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
                    Request.QueryString["City"],
                    Request.QueryString["State"],
                    "N",
                    "New",
                    Request.QueryString["Zip"]);

                try
                {
                    var billingAddress = new Address
                    {
                        AddressType = Enum.GetName(typeof(AddressType), 0),
                        City = Request.QueryString["City"],
                        Country = Request.QueryString["Country"],
                        Name = model.FirstName + ' ' + model.LastName,
                        Phone = Request.QueryString["Phone"],
                        State = Request.QueryString["State"],
                        StreetAddress = Request.QueryString["StreetAddress"],
                        StreetAddress2 = Request.QueryString["StreetAddress2"],
                        Zip = Request.QueryString["Zip"]
                    };

                    var shippingAddress = new Address
                    {
                        AddressType = Enum.GetName(typeof(AddressType), 1),
                        City = Request.QueryString["City"],
                        Country = Request.QueryString["Country"],
                        Name = model.FirstName + ' ' + model.LastName,
                        Phone = Request.QueryString["Phone"],
                        State = Request.QueryString["State"],
                        StreetAddress = Request.QueryString["StreetAddress"],
                        StreetAddress2 = Request.QueryString["StreetAddress2"],
                        Zip = Request.QueryString["Zip"]
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


                    if (!_stateService.HasValue(Constants.CurrentUser))
                        _stateService.SetValue(Constants.CurrentUser, webUser);

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


        public PartialViewResult GetLoginPartial()
        {
            return PartialView("_LoginPartial");
        }

        public ActionResult MyWebinars()
        {
            var currentUser = _accountControllerOrchestrator.GetWebUserFromIPrincipal();
            var model = new MyWebinarsDTO { WebUser = currentUser };

            ViewData["DiscountMsg"] = string.Empty;

            model.Scheduled = _orderManagementService.SelectOrdersWithScheduledWebinars(currentUser.idUser);
            model.Recorded = _orderManagementService.SelectOrdersWithRecordedWebinars(currentUser.idUser);
            model.Archived = _orderManagementService.SelectOrdersWithArchivedWebinars(currentUser.idUser);

            foreach (var order in model.Scheduled)
            {
                var row = order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active);
                if (row.Webinar.Status == WebinarStatus.Active)
                {
                    if (row.JoinURL == null && row.RegistrationType.ShowLiveNotifications == "Yes")
                    {
                        var regKeyResponse = _orderManagementService.CreateRegistrantKey(order.FirstName, order.LastName
                            , order.BillingEmail, row.Webinar.idWebinar, row.Webinar.WebinarKey);

                        if (ReferenceEquals(null, regKeyResponse))
                            //instead of throwing exception and halting execution
                            //throw new NullReferenceException("The Registration Key Response from the Citrix API resulted in a null response.");
                            _logger.ErrorException(
                                "Attempt to CreateRegistrantKey failed on Webinar: " + row.Webinar.idWebinar +
                                " email: " + order.BillingEmail,
                                new NullReferenceException("Attempt to CreateRegistrantKey failed on Webinar: " +
                                                           row.Webinar.idWebinar + " email: " + order.BillingEmail));

                            JObject parsedJsonObject = JObject.Parse(regKeyResponse);
                        if (regKeyResponse != null && parsedJsonObject[WebUiConstants.RegistrantKey] != null)
                            {
                                var registrantKey = parsedJsonObject[WebUiConstants.RegistrantKey].ToString();
                                var joinUrl = parsedJsonObject[WebUiConstants.JoinUrl].ToString();

                                row.RegistrantKey = registrantKey;
                                row.JoinURL = joinUrl;
                            }
                        //else
                        // devise better handling
                        //{
                        //    throw new NullReferenceException("Attempt to CreateRegistrantKey failed on Webinar: " +
                        //                                     row.Webinar.idWebinar + " email: " + order.BillingEmail);
                        //}
                    }
                }

            }


            return View("MyWebinars", model);
        }

        [System.Web.Mvc.AllowAnonymous]
        [System.Web.Mvc.HttpPost]
        public ActionResult MyCertificate(int orderID)
        {
            var currentUser = _accountControllerOrchestrator.GetWebUserFromIPrincipal();

            var currentOrder = _orderManagementService.GetOrderById(orderID);
            var model = new CertOfCompletionViewModel { CurrentUser = currentUser, Order = currentOrder };


            return View("MyCertificate", model);
        }

        [System.Web.Mvc.AllowAnonymous]
        public ActionResult Confirmed(string email, string surname)
        {
            try
            {
                var changeEmailFromKeyInputModel = _accountControllerOrchestrator.ConfirmUser(email, surname);

                return string.IsNullOrEmpty(changeEmailFromKeyInputModel.ScreenMessage) 
                    ? View(changeEmailFromKeyInputModel) 
                    : View("ThankYouConfirmed", changeEmailFromKeyInputModel);
            }
            catch (Exception exception)
            {
                _logger.Fatal(
                    "Account.Confirm Exception On GET: {0} Session={1}",
                    exception.Message,
                    AppHelper.GetUserAuditInfo()
                    );
                throw;
            }
        }

        [System.Web.Mvc.AllowAnonymous]
        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Confirmed(CreateUserConfirmedViewModel model)
        {
            try
            {
                if (_accountControllerOrchestrator.UserConfirmed(model))
                {
                    return RedirectToLocal(null);
                }
             
                _logger.Info("Your email address has already been successfully verified in our system. Session={0}",
                    AppHelper.GetUserAuditInfo());

                model.ScreenMessage = "Your email address has already been successfully verified in our system.";

                return View(model);
            }
            catch (Exception exception)
            {
                _logger.Fatal(
                    "Account.Confirm Exception On GET: {0} Session={1}",
                    exception.Message,
                    AppHelper.GetUserAuditInfo()
                    );
                throw;
            }
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
                throw;
            }

            ViewBag.ReturnUrl = Url.Action(ManageActionName);
            ViewBag.Title = WebUiConstants.ManageUser;

            return PartialView("Partials/_EditShippingAddress", model);
        }

        [System.Web.Mvc.HttpGet]
        public ActionResult EditInstitution()
        {
            var model = new EditInstitutionModel();

            ViewBag.ReturnUrl = Url.Action(ManageActionName);
            ViewBag.Title = WebUiConstants.ManageUser;

            try
            {
                var user = _accountControllerOrchestrator.GetWebUserFromIPrincipal();
                model.Institution = user.Institution.InstitutionName;
            }
            catch (Exception exception)
            {
                _logger.ErrorException("In EditInstitution Action", exception);
                throw;
            }

            return PartialView("Partials/_EditInsitution", model);
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult EditNameTitle(EditNameTitleModel model)
        {
            ViewBag.ReturnUrl = Url.Action(ManageActionName);
            ViewBag.Title = WebUiConstants.ManageUser;

            try
            {
                var user = _accountControllerOrchestrator.GetWebUserFromIPrincipal();

                //  TODO: [dar] This looks like ti should be uncommented
                //model.FirstName = user.FirstName;
                //model.LastName = user.LastName;
                //model.Title = user.Title;

                _membershipService.UpdateNameTitle(model.FirstName, model.LastName, user.email, model.Title);
            }
            catch (Exception exception)
            {
                _logger.ErrorException("In EditNameTitle Action", exception);
                throw;
            }
            return PartialView("Partials/_EditNameTitle", model);
        }

        //[ClaimsAuthorize(Roles = "CUWebinarsAbsoluteAdmin")]
        public ActionResult Manage(ManageMessageId? message)
        {
            var manageModel = new ManageModel
            {
                StatusMessage = string.Empty
            };
            ViewBag.ReturnUrl = Url.Action(ManageActionName);
            ViewBag.Title = WebUiConstants.ManageUser;

            var user = _accountControllerOrchestrator.GetWebUserFromIPrincipal();

            var addresses = user.Addresses.ToArray();
            var billingAddress = addresses.First(a => a.AddressType == WebUiConstants.BillingAddress);
            var shippingAddress = addresses.First(a => a.AddressType == WebUiConstants.ShippingAddress);

            manageModel.RegisterFields = new RegisterModel
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
                    City = shippingAddress.City,
                    Country = shippingAddress.Country,
                    StreetAddress = shippingAddress.StreetAddress,
                    StreetAddress2 = shippingAddress.StreetAddress2,
                    State = shippingAddress.State,
                    Zip = shippingAddress.Zip,
                    Phone = shippingAddress.Phone,
                    Name = shippingAddress.Name,
                    TypeOfAddress = AddressType.Shipping
                },
                FirstName = user.FirstName,
                LastName = user.LastName,
                Institution = user.Institution.InstitutionName,
                Email = user.email,
                Title = user.Title,
                AccountDetailsTitle = WebUiConstants.ManageUser
            };

            return View(manageModel);
            //return null;
        }

        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(ManageModel model)
        {
            var updateFields = model.RegisterFields;

            var billingAddressFields = updateFields.BillingAddress;
            var shippingAddressFields = updateFields.ShippingAddress;

            var billingAddress = new Address
            {
                Name = model.RegisterFields.FirstName + ' ' + model.RegisterFields.LastName,
                StreetAddress = billingAddressFields.StreetAddress.Trim(),
                StreetAddress2 = billingAddressFields.StreetAddress2 == null ? billingAddressFields.StreetAddress2 : billingAddressFields.StreetAddress2.Trim(),
                State = billingAddressFields.State.Trim(),
                City = billingAddressFields.City.Trim(),
                Country = billingAddressFields.Country.Trim(),
                Zip = billingAddressFields.Zip.Trim(),
                Phone = billingAddressFields.Phone.Trim(),
                AddressType = Enum.GetName(typeof(AddressType), billingAddressFields.TypeOfAddress)
            };

            var shippingAddress = new Address
            {
                Name = shippingAddressFields.Name.Trim(),
                StreetAddress = shippingAddressFields.StreetAddress.Trim(),
                StreetAddress2 = shippingAddressFields.StreetAddress2 == null ? shippingAddressFields.StreetAddress2 : shippingAddressFields.StreetAddress2.Trim(),
                State = shippingAddressFields.State.Trim(),
                City = shippingAddressFields.City.Trim(),
                Country = shippingAddressFields.Country.Trim(),
                Zip = shippingAddressFields.Zip.Trim(),
                Phone = shippingAddressFields.Phone.Trim(),
                AddressType = Enum.GetName(typeof(AddressType), shippingAddressFields.TypeOfAddress)
            };

            _membershipService.UpdateUserDetails(_globalConfig.Tenant,
                updateFields.FirstName.Trim(),
                updateFields.LastName.Trim(),
                //updateFields.Password,
                updateFields.Email.Trim(),
                updateFields.Institution,
                billingAddress,
                shippingAddress,
                updateFields.Title == null ? "na" : updateFields.Title.Trim()
                );

            return View(model);
        }


        //
        // GET: /Account/Login

        [System.Web.Mvc.AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.PageStyleType = "register";
            return View(_accountControllerOrchestrator.LogUserIn(returnUrl));
        }


        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        public ActionResult SignInForExcelFile(SignInModel model)
        {
            if (ModelState.IsValid && _membershipService.LogInUser(_globalConfig.Tenant, model.Email, model.Password, model.RememberMe))
            {
                return Content("true");
            }
                ProcessModelStateErrors();
                return Content("false");
            }


        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult SignIn(SignInModel model)
        {
            if (!ModelState.IsValid)
            {
                return this.ModelStateJson(ModelState);
            }

            string userMustVerify;

            if (_accountControllerOrchestrator.SignUserIn(model, out userMustVerify))
            {
                return Json(new {result = LoggedInResult} );
            }

            if (!string.IsNullOrEmpty(userMustVerify))
            {
                _logger.Info("Account.SignIn UserMustVerify. Session={0}, Email: {1} surname: {2}",
                    AppHelper.GetUserAuditInfo(),
                    model.Email,
                    model.Password
                    );

                return Json(new {result = ConfirmedResult, email = model.Email, surname = model.Password });
            }

            // If we got this far, something failed, redisplay form
            _logger.Warn("Account.SignIn Failed. {0} | {1} Session= {2}",
                model.Email,
                model.Password,
                AppHelper.GetUserAuditInfo()
                );

            ModelState.AddModelError(
                string.Empty, // Needs to be an empty string to show up in ValidationSummary as not model-level error.
                "The user name or password provided is incorrect."
                );

            return this.ModelStateJson(ModelState);
        }
        
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        [ValidateJsonAntiForgeryToken]
        public ActionResult SignInAfterCheckout(int? userId)
        {
            string userMustVerify;

            var email = _membershipService.GetWebUserById(userId.Value).email;

            var tempPassword = _stateService.GetValue<string>(DomainConstants.TempPassword);

            _stateService.ClearValue(DomainConstants.TempPassword);

            if (_accountControllerOrchestrator.SignUserIn(new SignInModel {Email = email, Password = tempPassword, ReturnUrl = "/"}, out userMustVerify))
            {
                return Json(new {result = LoggedInResult} );
            }

            if (!string.IsNullOrEmpty(userMustVerify))
            {
                _logger.Info("Account.SignIn UserMustVerify. Session={0}, Email: {1} surname: {2}",
                    AppHelper.GetUserAuditInfo(),
                    email,
                    tempPassword
                    );

                return Json(new {result = ConfirmedResult, email = email, surname = tempPassword });
            }

            // If we got this far, something failed, redisplay form
            _logger.Warn("Account.SignIn Failed. {0} | {1} Session= {2}",
                email,
                tempPassword,
                AppHelper.GetUserAuditInfo()
                );

            ModelState.AddModelError(
                string.Empty, // Needs to be an empty string to show up in ValidationSummary as not model-level error.
                "The user name or password provided is incorrect."
                );

            return this.ModelStateJson(ModelState);
        }
        
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        public ActionResult SignInFromCart(SignInModel model)
        {
            if (!ModelState.IsValid)
            {
                return this.ModelStateJson(ModelState);
            }

            try
            {
                string userMustVerify; // not relevant in this user flow. So gets discarded.

                if (_accountControllerOrchestrator.SignUserIn(model, out userMustVerify))
                {
                    _logger.Info("Account.SignIn Post Success in cart. Session={0}", AppHelper.GetUserAuditInfo());
                    WebUser webUser = _accountControllerOrchestrator.GetWebUserByEmail(model.Email);

                    return Json(new { result = LoggedInResult, UserId = webUser.idUser });
                }
            }
            catch (Exception exception)
            {
                _logger.ErrorException("In SignInFromCart Action", exception);
            }

            // If we got this far, something failed, redisplay form
            _logger.Warn("Account.SignIn Failed. {0} | {1} Session= {2}",
                model.Email,
                model.Password,
                AppHelper.GetUserAuditInfo()
                );

            ModelState.AddModelError(
                string.Empty, // Needs to be an empty string to show up in ValidationSummary as not model-level error.
                "The user name or password provided is incorrect."
                );

            return this.ModelStateJson(ModelState);
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public JsonResult ResetPassword(string email)
        {
            _logger.Info("Account.ResetPassword GET. Email = {1} Session={0}", AppHelper.GetUserAuditInfo(), email);

            try
            {
                _accountControllerOrchestrator.ResetPassword(_globalConfig.Tenant, email);
                return Json(new { Result = WebUiConstants.Success });
            }
            catch (Exception ex)
            {
                _logger.Fatal("Account.ResetPassword GET Failed. {0}, Session = {1} on email: {2}", ex.Message, AppHelper.GetUserAuditInfo(), email);
            }


            return Json(new { Result = WebUiConstants.Fail });
        }


        //[System.Web.Mvc.AllowAnonymous]
        //public ViewResult ResetPasswordWhereNotVerified(string tempPassword, string email)
        //{
        //    _logger.Info("Account.ResetPasswordWhereNotVerified GET. Session=" + AppHelper.GetUserAuditInfo());

        //    var localPasswordModel = new LocalPasswordModel
        //    {
        //        Email = email,
        //        ConfirmPassword = string.Empty,
        //        OldPassword = tempPassword,
        //        NewPassword = string.Empty
        //    };

        //    return View(localPasswordModel);
        //}
        //[System.Web.Mvc.HttpPost]
        ////[ValidateAntiForgeryToken]
        //[System.Web.Mvc.AllowAnonymous]
        //public ViewResult ResetPasswordWhereNotVerified(LocalPasswordModel localPasswordModel)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _logger.Info("Account.ResetPasswordWhereNotVerified POST. Session=" + AppHelper.GetUserAuditInfo());

        //        _stateService.SetValue(DomainConstants.UserCreatedViaNewOrder, true);
        //        _membershipService.ResetPassword(_globalConfig.Tenant, localPasswordModel.Email);

        //        var key = _stateService.GetValue<string>(DomainConstants.VerificationKey);

        //        _stateService.ClearValue(DomainConstants.UserCreatedViaNewOrder);

        //        _membershipService.ChangePasswordFromResetKey(key, localPasswordModel.NewPassword);

        //        _stateService.ClearValue(DomainConstants.VerificationKey);

        //        return View(localPasswordModel);
        //    }

        //    return View();
        //}


        [System.Web.Mvc.AllowAnonymous]

        public ActionResult PasswordResetConfirm(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.Warn("Account.PasswordResetConfirm.GET was passed empty or null ID. Session=PasswordResetConfirm. " + AppHelper.GetUserAuditInfo());
                ModelState.AddModelError(string.Empty, "There appears to have been a problem with the link which you clicked to navigate to this page. Please try clicking the link from the email again.");
            }

            var vm = new ChangePasswordFromResetKeyInputModel
            {
                Key = id
            };


            return View(vm);
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult PasswordResetConfirm(ChangePasswordFromResetKeyInputModel model, string returnUrl)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (string.IsNullOrWhiteSpace(model.Key))
                    {
                        _logger.Warn(
                            "Account.PasswordResetConfirm.POST was passed empty or null ID. Session=PasswordResetConfirm. " +
                            AppHelper.GetUserAuditInfo());
                        ModelState.AddModelError(string.Empty,
                            "There appears to have been a problem with the link which you clicked to navigate to this page. Please try clicking the link from the email again.");
                    }
                    else
                    {
                        if (_accountControllerOrchestrator.ChangePasswordFromResetKey(model.Key, model.Password))
                            model.ChangePasswordSucceeded = true;
                        else
                        {
                            //  TODO: I added "please contact your system administrator" for now. Do we have a number which they can call?
                            ModelState.AddModelError(string.Empty,
                                "Unable to change password from Reset key. Please please contact your system administrator");
                        }
                    }
                    _logger.Info("Account.PasswordResetConfirm Post. Session=" + AppHelper.GetUserAuditInfo());

                    return View(model);
                }
                    _logger.Error("Invalid ModelState in PasswordResetConfirm");
                    ProcessModelStateErrors();
                }
            catch (ValidationException validationException)
            {

                _logger.Fatal("Account.ResetPassword. " + validationException.Message + " Session=" + AppHelper.GetUserAuditInfo());
                ModelState.AddModelError(string.Empty, "The new password must be different than the old password.");
            }

            return View(model);
        }

        //
        // POST: /Account/LogOff

        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            if (User.Identity.IsAuthenticated)
            {
                _logger.Info("Account.LogOff. Session=" + AppHelper.GetUserAuditInfo());

                try
                {
                    _accountControllerOrchestrator.LogUserOut();
                }
                catch (Exception exception)
                {
                    _logger.ErrorException("In LogOff Action", exception);
                    throw;
                }
            }

            _logger.Fatal("Account.LogOff at Unauthenticated User. Session=" + AppHelper.GetUserAuditInfo());
            return RedirectToAction("Index", "Home");
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
            const char fieldDelimiter = ',';
            int numVal;
            var resultObject = new Dictionary<string, string>();

            if (!int.TryParse(zip, out numVal))
            {
                var zipToParse = zip.Split('-').FirstOrDefault();

                if (zipToParse == null || !int.TryParse(zipToParse, out numVal))
                {
                    resultObject.Add("success", "invalid format");
                    return Json(resultObject, JsonRequestBehavior.AllowGet);
                }
            }

            var zipAddress = AppHelper.GetCityStateFromZip(numVal);

            var zipCentricFields = zipAddress.Split(fieldDelimiter);

            if (string.IsNullOrWhiteSpace(zipAddress))
            {
                resultObject.Add("success", "false");
                return Json(resultObject, JsonRequestBehavior.AllowGet);
            }

            Debug.Assert(zipCentricFields.Length == 3, "There must be 3 fields in zipCentricFields, otherwise we have bad data.");

            if (zipCentricFields.Length < 3)
                zipCentricFields = AppHelper.AddNonvalidToArray(zipCentricFields);

            var myCity = new string(AppHelper.CharsToTitleCase(zipCentricFields[0]).ToArray());

            resultObject.Add("success", "true");
            resultObject.Add("City", myCity);
            resultObject.Add("State", zipCentricFields[1]);
            resultObject.Add("TimeZone", ((int)Enum.Parse(typeof(USTimeZone), zipCentricFields[2])).ToString(CultureInfo.InvariantCulture));
            //resultObject.Add("TimeZone", 3.ToString());

            return Json(resultObject, JsonRequestBehavior.AllowGet);
        }

        //[System.Web.Mvc.ActionName("autocomplete")]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Get)]
        [System.Web.Mvc.AllowAnonymous]
        public JsonResult AutocompleteInstitution()
        {
            List<string> ac = AppHelper.InstitutionAutoComplete(Request.QueryString["term"], Request.QueryString["zip"]);
            //string name, msa;
            //var retValue = new { values = ac };
            return Json(ac, JsonRequestBehavior.AllowGet);

        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public JsonResult CheckEmail(string email, bool disregardInstitutionDomain)
        {
            _logger.Info("CheckEmail called: " + email + "| Session=" + AppHelper.GetUserAuditInfo());
            var resultObject = new Dictionary<string, string>();

            var user = _membershipService.GetUserByEmail(email);

            if (user != null)
            {//email exists and view is notified.
                resultObject.Add("success", "foundExisting");
                return Json(resultObject, JsonRequestBehavior.AllowGet);
            }

            resultObject.Add("email", "wasNotFound");

            if (disregardInstitutionDomain)
                return Json(resultObject, JsonRequestBehavior.AllowGet);

            var domain = email.Split('@')[1];
            var institution = _membershipService.GetInstitutionByDomain(domain);

            if (institution == null)
                return Json(resultObject, JsonRequestBehavior.AllowGet);

            //  if we have a match between user's email (domain)
            //  and the domainName stored in existing Institution record
            //  we can offer to populate the user's billing info
            resultObject.Add("success", "foundInstitution");
            resultObject.Add("Institution", institution.InstitutionName);
            resultObject.Add("Address", institution.Address);
            resultObject.Add("City", institution.City);
            resultObject.Add("State", institution.State);
            resultObject.Add("Zip", institution.Zip);
            _logger.Info("CheckEmailResult: " + resultObject);
            return Json(resultObject, JsonRequestBehavior.AllowGet);
        }

        // POST: /Account/Register
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        [ValidateJsonAntiForgeryToken]
        public ActionResult RegisterFromCart(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                _logger.Info("Account.Register: " + model.RegisterFields.Email.Trim() + "| Session=" +
                             AppHelper.GetUserAuditInfo());

                var email = model.RegisterFields.Email.Trim();
                var firstName = model.RegisterFields.FirstName.Trim();
                var lastName = model.RegisterFields.LastName.Trim();

                //first check if email exists
                var checkIfUsed = _membershipService.GetUserByEmail(email);
                if (checkIfUsed != null)
                {
                    _logger.Error("dupe email attempt: " + email);
                    ModelState.AddModelError("", "That email already exists. Would you like to reset the password?");
                }

                var myInstitution = _membershipService.ProcessInstitutionForUser(
                    model.RegisterFields.Institution.Trim(),
                    email,
                    model.RegisterFields.BillingAddress.City.Trim(),
                    model.RegisterFields.BillingAddress.State.Trim(),
                    "N",
                    "New",
                    model.RegisterFields.BillingAddress.Zip.Trim());

                try
                {
                    IList<Address> addresses = new List<Address>
                    {
                        new Address
                        {
                            AddressType =
                                Enum.GetName(typeof (AddressType), model.RegisterFields.BillingAddress.TypeOfAddress),
                            City = model.RegisterFields.BillingAddress.City.Trim(),
                            Country = model.RegisterFields.BillingAddress.Country.Trim(),
                            Name = firstName + ' ' + lastName,
                            Phone = model.RegisterFields.BillingAddress.Phone.Trim(),
                            State = model.RegisterFields.BillingAddress.State.Trim(),
                            StreetAddress = model.RegisterFields.BillingAddress.StreetAddress.Trim(),
                            StreetAddress2 =
                                model.RegisterFields.BillingAddress.StreetAddress2 == null
                                    ? model.RegisterFields.BillingAddress.StreetAddress2
                                    : model.RegisterFields.BillingAddress.StreetAddress2.Trim(),
                            Zip = model.RegisterFields.BillingAddress.Zip.Trim()
                        },
                        new Address
                        {
                            AddressType =
                                Enum.GetName(typeof (AddressType), model.RegisterFields.ShippingAddress.TypeOfAddress),
                            City = model.RegisterFields.ShippingAddress.City.Trim(),
                            Country = model.RegisterFields.ShippingAddress.Country.Trim(),
                            Name = firstName + ' ' + lastName,
                            Phone = model.RegisterFields.ShippingAddress.Phone.Trim(),
                            State = model.RegisterFields.ShippingAddress.State.Trim(),
                            StreetAddress = model.RegisterFields.ShippingAddress.StreetAddress.Trim(),
                            StreetAddress2 =
                                model.RegisterFields.ShippingAddress.StreetAddress2 == null
                                    ? model.RegisterFields.ShippingAddress.StreetAddress2
                                    : model.RegisterFields.ShippingAddress.StreetAddress2.Trim(),
                            Zip = model.RegisterFields.ShippingAddress.Zip.Trim()
                        }
                    };

                    var webUser = _membershipService.CreateWebUser(_globalConfig.Tenant
                        , firstName
                        , lastName
                        , string.Empty
                        , email
                        , USTimeZone.Central
                        , UserType.Customer
                        , myInstitution.idInstitution
                        , addresses
                        ,
                        model.RegisterFields.Title == null
                            ? model.RegisterFields.Title
                            : model.RegisterFields.Title.Trim()
                        , null
                        , DomainConstants.Active
                        );

                    if (!_stateService.HasValue(Constants.CurrentUser))
                        _stateService.SetValue(Constants.CurrentUser, webUser);

                    //_membershipService.AddAccountTypeNotVerifiedClaim(userAccount, ClaimValues.ManualRegistration);

                    _logger.Info("Account.Register UserAdded: " + model.RegisterFields.Email);

                    return Json(new {Result = WebUiConstants.Success, UserId = webUser.idUser });

                }
                catch (Exception exception)
                {
                    ModelState.AddModelError("", exception.Message);
                    //TODO: add error message here and handle in razor
                    _logger.Error("Account.Register Catch block: " + exception.Message + "| Session=" +
                                  AppHelper.GetUserAuditInfo());
                }
            }
            _logger.Fatal("Account.Register failed! Session=" + AppHelper.GetUserAuditInfo());

            // If we got this far, something failed, redisplay form
            return this.ModelStateJson(ModelState);

        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                _logger.Info("Account.Register: " + model.RegisterFields.Email.Trim() + "| Session=" +
                             AppHelper.GetUserAuditInfo());

                var email = model.RegisterFields.Email.Trim();
                var firstName = model.RegisterFields.FirstName.Trim();
                var lastName = model.RegisterFields.LastName.Trim();

                //first check if email exists
                var checkIfUsed = _membershipService.GetUserByEmail(email);
                if (checkIfUsed != null)
                {
                    _logger.Error("dupe email attempt: " + email);
                    ModelState.AddModelError("Email", "That email already exists. Would you like to reset the password?");
                }

                var myInstitution = _membershipService.ProcessInstitutionForUser(
                    model.RegisterFields.Institution.Trim(),
                    email,
                    model.RegisterFields.BillingAddress.City.Trim(),
                    model.RegisterFields.BillingAddress.State.Trim(),
                    "N",
                    "New",
                    model.RegisterFields.BillingAddress.Zip.Trim());

                try
                {
                    IList<Address> addresses = new List<Address>
                    {
                        new Address
                        {
                            AddressType =
                                Enum.GetName(typeof (AddressType), model.RegisterFields.BillingAddress.TypeOfAddress),
                            City = model.RegisterFields.BillingAddress.City.Trim(),
                            Country = model.RegisterFields.BillingAddress.Country.Trim(),
                            Name = firstName + ' ' + lastName,
                            Phone = model.RegisterFields.BillingAddress.Phone.Trim(),
                            State = model.RegisterFields.BillingAddress.State.Trim(),
                            StreetAddress = model.RegisterFields.BillingAddress.StreetAddress.Trim(),
                            StreetAddress2 =
                                model.RegisterFields.BillingAddress.StreetAddress2 == null
                                    ? model.RegisterFields.BillingAddress.StreetAddress2
                                    : model.RegisterFields.BillingAddress.StreetAddress2.Trim(),
                            Zip = model.RegisterFields.BillingAddress.Zip.Trim()
                        },
                        new Address
                        {
                            AddressType =
                                Enum.GetName(typeof (AddressType), model.RegisterFields.ShippingAddress.TypeOfAddress),
                            City = model.RegisterFields.ShippingAddress.City.Trim(),
                            Country = model.RegisterFields.ShippingAddress.Country.Trim(),
                            Name = firstName + ' ' + lastName,
                            Phone = model.RegisterFields.ShippingAddress.Phone.Trim(),
                            State = model.RegisterFields.ShippingAddress.State.Trim(),
                            StreetAddress = model.RegisterFields.ShippingAddress.StreetAddress.Trim(),
                            StreetAddress2 =
                                model.RegisterFields.ShippingAddress.StreetAddress2 == null
                                    ? model.RegisterFields.ShippingAddress.StreetAddress2
                                    : model.RegisterFields.ShippingAddress.StreetAddress2.Trim(),
                            Zip = model.RegisterFields.ShippingAddress.Zip.Trim()
                        }
                    };

                    var webUser = _membershipService.CreateWebUser(_globalConfig.Tenant,
                        firstName
                        , lastName
                        , model.RegisterFields.Password
                        , email
                        , USTimeZone.Central
                        , UserType.Customer
                        , myInstitution.idInstitution
                        , addresses
                        ,
                        model.RegisterFields.Title == null
                            ? model.RegisterFields.Title
                            : model.RegisterFields.Title.Trim()
                        , null
                        , DomainConstants.Active
                        );

                    if (!_stateService.HasValue(Constants.CurrentUser))
                        _stateService.SetValue(Constants.CurrentUser, webUser);

                    var userAccount = _membershipService.CreateUser(
                        _globalConfig.Tenant,
                        firstName,
                        lastName,
                        string.Empty,
                        //  Pass empty string for username because MembershipReboot assigns the email to the username field where emailIsUsername is true
                        model.RegisterFields.Password,
                        email);

                    _membershipService.AddAccountTypeNotVerifiedClaim(userAccount, ClaimValues.ManualRegistration);

                    Debug.Assert(_stateService.HasValue(DomainConstants.VerificationKey),
                        "There's no reason session should not have a value for the VerificationKey at this point ");

                    var verificationKey = _stateService.GetValue<string>(DomainConstants.VerificationKey);
                    _stateService.ClearValue(DomainConstants.VerificationKey);

                    userAccount = _membershipService.VerifyEmailFromKey(
                        verificationKey,
                        model.RegisterFields.Password
                        );

                    _membershipService.LogInUser(_globalConfig.Tenant, model.RegisterFields.Email,
                        model.RegisterFields.Password, true); // log the user in.
                    _logger.Info("Account.Register UserAdded: " + model.RegisterFields.Email);

                    return Json(new { Result = WebUiConstants.Success });

                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("MembershipCreateUserException", ErrorCodeToString(e.StatusCode));
                    //TODO: add error message here and handle in razor
                    _logger.Error("Account.Register Catch block: " + e.Message + "| Session=" +
                                  AppHelper.GetUserAuditInfo());
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("Exception", e.Message);
                    //TODO: add error message here and handle in razor
                    _logger.Error("Account.Register Catch block: " + e.Message + "| Session=" +
                                  AppHelper.GetUserAuditInfo());
                }
            }
            else
            {
                return this.ModelStateJson(ModelState);
            }
            _logger.Fatal("Account.Register failed! Session=" + AppHelper.GetUserAuditInfo());

            // If we got this far, something failed, redisplay form
            return Json(new { Result = WebUiConstants.Fail });

        }

        // POST: /Account/Register
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        [ValidateJsonAntiForgeryToken]
        public ActionResult CreateUserAccountFromCart(RegisterViewModel model)
        {
            //  Not adding any ModelState errors in this method. This method is not to return any GUI feedback.
            //  It is effective invoked as a fire and forget, even thought it sends a http response (which gets ignored at client.)

            if (ModelState.IsValid)
            {
                var email = model.RegisterFields.Email.Trim();
                var firstName = model.RegisterFields.FirstName.Trim();
                var lastName = model.RegisterFields.LastName.Trim();

                //first check if email exists
                var checkIfUsed = _membershipService.GetUserByEmail(email);
                if (checkIfUsed != null)
                {
                    _logger.Error("dupe email attempt: " + email);
                }
                try
                {
                    //  model.Passwor should be null or empty. Generate own temp password here
                    model.RegisterFields.Password = PasswordGenerator.GenerateRandomString(8);
                    if (_stateService.HasValue(DomainConstants.TempPassword))
                        _stateService.ClearValue(DomainConstants.TempPassword);
                    
                    _stateService.SetValue(DomainConstants.TempPassword, model.RegisterFields.Password); // will use in TtsTokenizer
#if DEBUG
                    _logger.Info("Password {0} created for user {1}", model.RegisterFields.Password, model.RegisterFields.Email);
#endif

                    var userAccount = _membershipService.CreateUser(
                        _globalConfig.Tenant,
                        firstName,
                        lastName,
                        string.Empty,
                        //  Pass empty string for username because MembershipReboot assigns the email to the username field where emailIsUsername is true
                        model.RegisterFields.Password,
                        email);

                    _membershipService.AddAccountTypeNotVerifiedClaim(userAccount, ClaimValues.CartRegistration);

                    Debug.Assert(_stateService.HasValue(DomainConstants.VerificationKey),
                        "There's no reason session should not have a value for the VerificationKey at this point ");

                    var verificationKey = _stateService.GetValue<string>(DomainConstants.VerificationKey);
                    _stateService.ClearValue(DomainConstants.VerificationKey);

                    userAccount = _membershipService.VerifyEmailFromKey(
                        verificationKey,
                        model.RegisterFields.Password
                        );

                    return Json(new { Result = WebUiConstants.Success });

                }
                catch (MembershipCreateUserException e)
                {
                    //TODO: add error message here and handle in razor
                    _logger.Error("CreateUserAccountFromCart Catch block: " + e.Message + "| Session=" +
                                  AppHelper.GetUserAuditInfo());
                }
                catch (Exception e)
                {
                    //TODO: add error message here and handle in razor
                    _logger.Error("CreateUserAccountFromCart Catch block: " + e.Message + "| Session=" +
                                  AppHelper.GetUserAuditInfo());
                }
            }

            _logger.Fatal("CreateUserAccountFromCart failed! Session=" + AppHelper.GetUserAuditInfo());

            // If we got this far, something failed, redisplay form
            return Json(new { Result = WebUiConstants.Fail });            
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

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback

        [System.Web.Mvc.AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
            if (!result.IsSuccessful)
            {
                return RedirectToAction("ExternalLoginFailure");
            }

            if (OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: false))
            {
                return RedirectToLocal(returnUrl);
            }

            if (User.Identity.IsAuthenticated)
            {
                // If the current user is logged in add the new account
                OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // User is new, ask for their desired membership name
                string loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
                ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
                ViewBag.ReturnUrl = returnUrl;
                return View("ExternalLoginConfirmation", new RegisterExternalLoginModel { UserName = result.UserName, ExternalLoginData = loginData });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLoginConfirmation(RegisterExternalLoginModel model, string returnUrl)
        {
            string provider = null;
            string providerUserId = null;

            if (User.Identity.IsAuthenticated || !OAuthWebSecurity.TryDeserializeProviderUserId(model.ExternalLoginData, out provider, out providerUserId))
            {
                return RedirectToAction(ManageActionName);
            }

            if (ModelState.IsValid)
            {
                // Insert a new user into the database
                using (UsersContext db = new UsersContext())
                {
                    UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());
                    // Check if user already exists
                    if (user == null)
                    {
                        // Insert name into the profile table
                        db.UserProfiles.Add(new UserProfile { UserName = model.UserName });
                        db.SaveChanges();

                        OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, model.UserName);
                        OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie: false);

                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("UserName", "User name already exists. Please enter a different user name.");
                    }
                }
            }

            ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(provider).DisplayName;
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // GET: /Account/ExternalLoginFailure

        [System.Web.Mvc.AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [System.Web.Mvc.AllowAnonymous]
        [ChildActionOnly]
        public ActionResult ExternalLoginsList(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
        }

        [ChildActionOnly]
        public ActionResult RemoveExternalLogins()
        {
            //ICollection<OAuthAccount> accounts = OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name);
            //List<ExternalLogin> externalLogins = new List<ExternalLogin>();
            //foreach (OAuthAccount account in accounts)
            //{
            //    AuthenticationClientData clientData = OAuthWebSecurity.GetOAuthClientData(account.Provider);

            //    externalLogins.Add(new ExternalLogin
            //    {
            //        Provider = account.Provider,
            //        ProviderDisplayName = clientData.DisplayName,
            //        ProviderUserId = account.ProviderUserId,
            //    });
            //}

            //ViewBag.ShowRemoveButton = externalLogins.Count > 1 || OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            //return PartialView("_RemoveExternalLoginsPartial", externalLogins);
            return null;
        }

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

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
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
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

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
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                var logger = _logger as IDisposable;
                if (logger != null)
                    logger.Dispose();

                _membershipService.Dispose();
                _orderManagementService.Dispose();

                base.Dispose(true);
            }
            _disposed = true;
        }

        private void ProcessModelStateErrors()
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors); // Get all errors flattened

            _logger.Error("Invalid ModelState Detected in Register");

            foreach (var error in errors)
            {
                Debug.WriteLine(error.ErrorMessage);

                _logger.Error("Account.Register ModelError: {0} | Session={1}", 
                    error.ErrorMessage,
                    AppHelper.GetUserAuditInfo()
                    );
            }
        }
    }
}
