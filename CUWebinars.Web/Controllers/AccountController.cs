using System;
using System.Diagnostics;
using CUWebinars.Business.Services;
using CUWebinars.Web.Core;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Membership;
using CUWebinars.Web.Models;
using CUWebinars.Web.Services;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using Ninject.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Security;
using CUWebinars.Business.AccountService;
using CUWebinars.Business.Models;
using CUWebinars.Business.Repository;
using CUWebinars.Web.ViewModel;
using System.Security.Claims;
using Thinktecture.IdentityModel.Authorization.Mvc;


namespace CUWebinars.Web.Controllers
{
    [System.Web.Mvc.Authorize]
    public class AccountController : Controller
    {
        private readonly GlobalConfig globalConfig = GlobalConfig.GlobalConfigSingleton;
        private TTSWebinarsContext db = new TTSWebinarsContext();
        private IMailService mail;
        public IMembershipService membershipService;
        private readonly IOrderRepository orderRepository;
        private readonly IRegTypeRepository RegTypeRepository;
        private readonly IStateService stateService;
        private readonly IOrderManagementService orderManagementService;

        public ILogger Logger { get; set; }
        //public IOrderService orderService;

        public AccountController(IMailService mail,
            ILogger logger,
            IMembershipService membershipService,
            IOrderRepository orderRepository,
            IOrderManagementService orderManagementService,
            IRegTypeRepository RegTypeRepository,
            IStateService stateService)
        {
            this.mail = mail;
            this.Logger = logger;
            this.membershipService = membershipService;
            this.orderRepository = orderRepository;
            this.orderManagementService = orderManagementService;
            this.RegTypeRepository = RegTypeRepository;
            this.stateService = stateService;
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

            string listing = "";

            foreach (string s in listNames)
            {
                if (s != "")
                {
                    listing += ("<p>http://www.BankWebinars.com/Home/COC/?idUser=" + id + "&displayName=" +
                                s.Replace(' ', '+') + "</p>");
                }
            }

            ViewData["listing"] = listing;
            //Logger.Instance.LogMessage("COC List executed: " + id + " by " +
            //                           UserFacade.Instance.GetCurrentUser().FullName);

            return View("~/Views/admin/Acc");
        }


        //[System.Web.Mvc.HttpGet]
        //public ActionResult CreateOrder()
        //{
        //    //var currentUser = GetWebUserFromIPrincipal();
        //    var refDataRepository = new RefDataRepository();
        //    //var bla = refDataRepository.FindRegTypesByWebinarId(435, false);

        //    var identity = ClaimsPrincipal.Current;

        //    var order = new Order
        //    {
        //        FirstName = "Birgit",
        //        LastName = "Roby",
        //        idOrder = 1114,
        //        OrderDate = DateTime.Parse("2014-02-05 21:32:53.000"),
        //        Affiliate = new Affiliate
        //        {
        //            ContactEmail = "affiliate@with.com",
        //            URL = "http://microsoft.com"
        //        }
        //    };

        //    orderManagementService.CreateOrderEvent(order, membershipService.GetUserAccountByUserId(identity.GetUserID()));


        //    orderManagementService.DispatchDummyOrder();

        //    return RedirectToLocal(null);
        //}

        //

        [System.Web.Mvc.AllowAnonymous]
        public string Get([FromUri] RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var email = model.Email.Trim();
                var firstName = model.FirstName.Trim();
                var lastName = model.LastName.Trim();

                //first check if email exists
                var checkIfUsed = membershipService.GetUserByEmail(email);
                if (checkIfUsed != null)
                {
                    Logger.Error("dupe email attempt: " + email);
                    ModelState.AddModelError("Email", "That email already exists. Would you like to reset the password?");
                    //var scriptSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                    //var jsonString = scriptSerializer.Serialize(model);

                    //return Json(jsonString, JsonRequestBehavior.AllowGet);
                }

                var myInstitution = membershipService.ProcessInstitutionForUser(model.Institution.Trim(),
                    email,
                    Request.QueryString["City"],
                    Request.QueryString["State"],
                    "N",
                    "New",
                    Request.QueryString["Zip"]);

                try
                {
                    var billingAddress = new Address();
                    billingAddress.AddressType = Enum.GetName(typeof(AddressType), 0);
                    billingAddress.City = Request.QueryString["City"];
                    billingAddress.Country = Request.QueryString["Country"];
                    billingAddress.Name = model.FirstName + ' ' + model.LastName;
                    billingAddress.Phone = Request.QueryString["Phone"];
                    billingAddress.State = Request.QueryString["State"];
                    billingAddress.StreetAddress = Request.QueryString["StreetAddress"];
                    billingAddress.StreetAddress2 = Request.QueryString["StreetAddress2"];
                    billingAddress.Zip = Request.QueryString["Zip"];

                    var shippingAddress = new Address();
                    shippingAddress.AddressType = Enum.GetName(typeof(AddressType), 1);
                    shippingAddress.City = Request.QueryString["City"];
                    shippingAddress.Country = Request.QueryString["Country"];
                    shippingAddress.Name = model.FirstName + ' ' + model.LastName;
                    shippingAddress.Phone = Request.QueryString["Phone"];
                    shippingAddress.State = Request.QueryString["State"];
                    shippingAddress.StreetAddress = Request.QueryString["StreetAddress"];
                    shippingAddress.StreetAddress2 = Request.QueryString["StreetAddress2"];
                    shippingAddress.Zip = Request.QueryString["Zip"];

                    IList<Address> addresses = new List<Address> { billingAddress, shippingAddress };

                    var webUser = membershipService.CreateWebUser(globalConfig.Tenant,
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
                        , "A"
                        );

                    if (!stateService.HasValue(Constants.CurrentUser))
                        stateService.SetValue<WebUser>(Constants.CurrentUser, webUser);

                    var userAccount = membershipService.CreateUser(
                        globalConfig.Tenant,
                        firstName,
                        lastName,
                        string.Empty, //  Pass empty string for username because MembershipReboot assigns the email to the username field where emailIsUsername is true
                        model.Password,
                        email);

                    if (userAccount != null)
                    {
                        var dataOperations = new DataOperations();
                        dataOperations.SetNewAccountToVerified(userAccount.ID);
                    }

                    //membershipService.LogInUser(globalConfig.Tenant, model.Email, model.Password, true); // log the user in.
                    Logger.Info("/Account/Register UserAdded: " + model.Email);
                    return webUser.idUser.ToString();
                    //return model.idWebUser.ToString();
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError(string.Empty, e.StatusCode.ToString());
                    Logger.Error("Importer Error: " + e.Message);
                }
            }

            // If we got this far, something failed, redisplay form
            return "error";
        }

        //  TODO: fix 
        public ActionResult MyWebinars()
        {
            var currentUser = GetWebUserFromIPrincipal();
            var model = new MyWebinarsDTO { WebUser = currentUser };

            //IList<Discount> discountsList = DiscountFacade.Instance.LoadList();
            //IList<OrderRow> rowsWithDiscount = OrderFacade.Instance.SelectOrderRowsWithDiscountByUser(currentUser);
            //IList<DiscountDTO> discountDto = new DiscountDTOAssembler().Entities2DTOs(
            //    discountsList, rowsWithDiscount);

            ViewData["DiscountMsg"] = string.Empty;

            model.Scheduled = orderRepository.SelectOrdersWithScheduledWebinars(currentUser.idUser);
            model.Recorded = orderRepository.SelectOrdersWithRecordedWebinars(currentUser.idUser);
            model.Archived = orderRepository.SelectOrdersWithArchivedWebinars(currentUser.idUser);

            //if (model.Scheduled != null)
            //{
            //    var optionAndOrderdictionary = model.Scheduled;
            //    var optionAndOrderdictionarySortedByWebinarDate =
            //        optionAndOrderdictionary.OrderBy(f => f.Value.OrderRows.SingleOrDefault().Webinar.Date);

            //    model.Scheduled = optionAndOrderdictionarySortedByWebinarDate
            //        .ToDictionary<KeyValuePair<RegType, Order>, RegType, Order>(p => p.Key, p => p.Value);
            //}

            //if (model.Recorded != null)
            //{
            //    //var optionAndOrderdictionary = model.Recorded;
            //    //var optionAndOrderdictionarySortedByWebinarDate =
            //    //    optionAndOrderdictionary.OrderBy(f => f.Value.OrderRows.SingleOrDefault().Webinar.Date);

            //    //model.Recorded = optionAndOrderdictionarySortedByWebinarDate
            //    //    .ToDictionary<KeyValuePair<RegType, Order>, RegType, Order>(p => p.Key, p => p.Value);
            //    //model.Recorded = sortedEnum.ToList();
            //    }
            //if (model.Archived != null)
            //{
            //    var list = model.Archived;
            //    var sortedEnum = list.OrderBy(f => f.OrderRows.SingleOrDefault().Webinar.Date);
            //    model.Archived = sortedEnum.ToList();
            //}

            return View("MyWebinars", model);
        }

        [System.Web.Mvc.AllowAnonymous]
        public ActionResult Confirm(string id)
        {
            try
            {
                var account = membershipService.GetByVerificationKey(id);

                if (account.HasPassword())
                {
                    var vm = new ChangeEmailFromKeyInputModel { Key = id };
                    return View("Confirm", vm);
                }
            }
            catch (Exception exception)
            {
                //  TODO: Meaningful error handling/logging and feedback   
            }

            return null;
        }

        [System.Web.Mvc.AllowAnonymous]
        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Confirm(ChangeEmailFromKeyInputModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    BrockAllen.MembershipReboot.UserAccount account = membershipService.VerifyEmailFromKey(model.Key, model.Password);

                    // since we've changed the email, we need to re-issue the cookie that
                    // contains the claims.
                    membershipService.SignIn(account, true);
                    return RedirectToLocal(null);
                }
                catch (ValidationException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            return View("Confirm", model);
        }

        [ClaimsAuthorize(Roles = "Admin")]
        public ActionResult Manage(ManageMessageId? message)
        {
            ManageModel manageModel = new ManageModel
            {
                StatusMessage = string.Empty
            };



            ViewBag.ReturnUrl = Url.Action("Manage");
            ViewBag.Title = WebUiConstants.ManageUser;

            var user = GetWebUserFromIPrincipal();

            var addresses = user.Addresses.ToArray();
            var billingAddress = addresses.Where(a => a.AddressType == WebUiConstants.BillingAddress).First();
            var shippingAddress = addresses.Where(a => a.AddressType == WebUiConstants.ShippingAddress).First();

            manageModel.RegisterFields = new RegisterModel
                    {
                        BillingAddress = new AddressModel
                        {
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
                            TypeOfAddress = AddressType.Shipping
                        },
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Institution = user.Institution.InstitutionName,
                        Email = user.email,
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
                StreetAddress = shippingAddressFields.StreetAddress.Trim(),
                StreetAddress2 = shippingAddressFields.StreetAddress2 == null ? shippingAddressFields.StreetAddress2 : shippingAddressFields.StreetAddress2.Trim(),
                State = shippingAddressFields.State.Trim(),
                City = shippingAddressFields.City.Trim(),
                Country = shippingAddressFields.Country.Trim(),
                Zip = shippingAddressFields.Zip.Trim(),
                Phone = shippingAddressFields.Phone.Trim(),
                AddressType = Enum.GetName(typeof(AddressType), shippingAddressFields.TypeOfAddress)
            };

            membershipService.UpdateUserDetails(globalConfig.Tenant,
                updateFields.FirstName.Trim(),
                updateFields.LastName.Trim(),
                updateFields.Password,
                updateFields.Email.Trim(),
                updateFields.Institution,
                billingAddress,
                shippingAddress,
                updateFields.Title == null ? "na" : updateFields.Title.Trim()
                );


            // If we got this far, something failed, redisplay form
            return View(model);
        }


        //
        // GET: /Account/Login

        [System.Web.Mvc.AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            var loginModel = new LoginModel
                {
                    SignIn = new SignInModel(),
                    ResetPassword = new ResetPasswordModel(),
                    Register = new RegisterViewModel
                    {
                        RegisterFields = new RegisterModel
                        {
                            AccountDetailsTitle = WebUiConstants.Register,
                            BillingAddress = new AddressModel { TypeOfAddress = AddressType.Billing },
                            ShippingAddress = new AddressModel { TypeOfAddress = AddressType.Shipping }
                        }
                    }
                };

            //So that the user can be referred back to where they were when they click logon
            if (string.IsNullOrEmpty(returnUrl) && Request.UrlReferrer != null)
            {
                returnUrl = Server.UrlDecode(Request.UrlReferrer.PathAndQuery);
            }

            if (Url.IsLocalUrl(returnUrl) && !string.IsNullOrEmpty(returnUrl))
            {
                if (returnUrl.Contains("PasswordResetConfirm") || returnUrl.Equals("Account/Login"))
                    returnUrl = "/Home/Index";

                loginModel.ReturnUrl = returnUrl;
                loginModel.SignIn.ReturnUrl = returnUrl;
            }

            ViewBag.PageStyleType = "register";

            loginModel.ActiveTab = "login";

            return View(loginModel);
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult SignIn(SignInModel model)
        {
            if (ModelState.IsValid && membershipService.LogInUser(globalConfig.Tenant, model.Email, model.Password, model.RememberMe))
            {
                var retURL = model.ReturnUrl.Replace("http://localhost:5556", "");

                return RedirectToLocal(retURL);
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError(string.Empty, "The user name or password provided is incorrect.");
            return View("Login", new LoginModel
            {
                Register = new RegisterViewModel
                {
                    RegisterFields = new RegisterModel
                    {
                        AccountDetailsTitle = WebUiConstants.Register
                    }
                },
                ResetPassword = new ResetPasswordModel(),
                SignIn = model,
                ActiveTab = "login"
            });
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public JsonResult ResetPassword(string email)
        {
            var cResult = new Dictionary<string, string>(1);

            try
            {
                membershipService.ResetPassword(globalConfig.Tenant, email);
                cResult.Add(WebUiConstants.OpStatus, "Success");
            }
            catch (Exception)
            {
                cResult.Add(WebUiConstants.OpStatus, "Fail");
            }


            return Json(cResult, JsonRequestBehavior.AllowGet);
        }

        [System.Web.Mvc.AllowAnonymous]

        public ActionResult PasswordResetConfirm(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                ModelState.AddModelError("EmptyKey", "There appears to have been a problem with the link which you clicked to navigate to this page. Please try clicking the link from the email again.");
            }

            var vm = new ChangePasswordFromResetKeyInputModel()
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
                        ModelState.AddModelError("EmptyKey", "There appears to have been a problem with the link which you clicked to navigate to this page. Please try clicking the link from the email again.");
                    }
                    else
                    {
                        if (membershipService.ChangePasswordFromResetKey(model.Key, model.Password))
                            model.ChangePasswordSucceeded = true;
                    }

                    return View(model);
                }
            }
            catch (ValidationException validationException)
            {
                ModelState.AddModelError("InvalidPassword", "The new password must be different than the old password.");
            }

            return View(model);
        }

        //
        // POST: /Account/LogOff

        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff(string bla)
        {
            if (User.Identity.IsAuthenticated)
            {
                membershipService.LogOutUser();

                return RedirectToAction("Index", "Home");
            }

            return View();
        }


        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.AllowAnonymous]
        public ActionResult test()
        {
            return View();
        }

        // POST: /Account/Register
        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public JsonResult CheckInstitution(string institution)
        {
            Dictionary<string, string> cResult = new Dictionary<string, string>();
            cResult.Add("success", "steve@juststeve.com");

            return Json(cResult, JsonRequestBehavior.AllowGet);

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

            if (string.IsNullOrWhiteSpace(zipAddress))
            {
                resultObject.Add("success", "false");
                return Json(resultObject, JsonRequestBehavior.AllowGet);
            }

            var myCity = new string(AppHelper.CharsToTitleCase(zipAddress.Split(fieldDelimiter).First()).ToArray());

            resultObject.Add("success", "true");
            resultObject.Add("City", myCity);
            resultObject.Add("State", zipAddress.Split(fieldDelimiter)[1]);
            resultObject.Add("TimeZone", ((int)Enum.Parse(typeof(USTimeZone), zipAddress.Split(fieldDelimiter)[2])).ToString());
            //resultObject.Add("TimeZone", 3.ToString());

            return Json(resultObject, JsonRequestBehavior.AllowGet);
        }

        //[System.Web.Mvc.ActionName("autocomplete")]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Get)]
        [System.Web.Mvc.AllowAnonymous]
        public JsonResult AutocompleteInstitution()
        {
            List<string> ac = AppHelper.InstitutionAutoComplete(Request.QueryString["term"], Request.QueryString["zip"]);
            string name, msa;
            var retValue = new { values = ac };
            return Json(ac, JsonRequestBehavior.AllowGet);

        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public JsonResult CheckEmail(string email, bool disregardIntitutionDomain)
        {
            Logger.Info("CheckEmail called: " + email);
            var resultObject = new Dictionary<string, string>();

            var user = membershipService.GetUserByEmail(email);

            if (user != null)
            {//email exists and view is notified.
                resultObject.Add("success", "foundExisting");
                return Json(resultObject, JsonRequestBehavior.AllowGet);
            }

            resultObject.Add("email", "wasNotFound");

            if (disregardIntitutionDomain)
                return Json(resultObject, JsonRequestBehavior.AllowGet);

            var domain = email.Split('@')[1];
            var institution = membershipService.GetInstitutionByDomain(domain);

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
            Logger.Info("CheckEmailResult: " + resultObject);
            return Json(resultObject, JsonRequestBehavior.AllowGet);
        }

        // POST: /Account/Register
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        [ValidateAntiForgeryToken]
        public JsonResult Register(RegisterViewModel model)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);


            foreach (var error in errors)
            {
                Debug.WriteLine(error.ErrorMessage);
                Logger.Error("Account/Register: " + error.ErrorMessage);
            }

            if (ModelState.IsValid)
            {
                var email = model.RegisterFields.Email.Trim();
                var firstName = model.RegisterFields.FirstName.Trim();
                var lastName = model.RegisterFields.LastName.Trim();

                //first check if email exists
                var checkIfUsed = membershipService.GetUserByEmail(email);
                if (checkIfUsed != null)
                {
                    Logger.Error("dupe email attempt: " + email);
                    ModelState.AddModelError("Email", "That email already exists. Would you like to reset the password?");
                    //var scriptSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                    //var jsonString = scriptSerializer.Serialize(model);

                    //return Json(jsonString, JsonRequestBehavior.AllowGet);
                }

                var myInstitution = membershipService.ProcessInstitutionForUser(model.RegisterFields.Institution.Trim(),
                    email,
                    model.RegisterFields.BillingAddress.City.Trim(),
                    model.RegisterFields.BillingAddress.State.Trim(),
                    "N",
                    "New",
                    model.RegisterFields.BillingAddress.Zip.Trim());

                try
                {
                    IList<Address> addresses = new List<Address> { new Address
                        {
                            AddressType = Enum.GetName(typeof (AddressType), model.RegisterFields.BillingAddress.TypeOfAddress),
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
                            AddressType = Enum.GetName(typeof (AddressType), model.RegisterFields.ShippingAddress.TypeOfAddress),
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

                    var webUser = membershipService.CreateWebUser(globalConfig.Tenant,
                        firstName
                        , lastName
                        , model.RegisterFields.Password
                        , email
                        , USTimeZone.Central
                        , UserType.Customer
                        , myInstitution.idInstitution
                        , addresses
                        , model.RegisterFields.Title == null ? model.RegisterFields.Title : model.RegisterFields.Title.Trim()
                        , null
                        , "A"
                        );

                    if (!stateService.HasValue(Constants.CurrentUser))
                        stateService.SetValue<WebUser>(Constants.CurrentUser, webUser);

                    var userAccount = membershipService.CreateUser(
                        globalConfig.Tenant,
                        firstName,
                        lastName,
                        string.Empty, //  Pass empty string for username because MembershipReboot assigns the email to the username field where emailIsUsername is true
                        model.RegisterFields.Password,
                        email);

                    if (userAccount != null)
                    {
                        var dataOperations = new DataOperations();
                        dataOperations.SetNewAccountToVerified(userAccount.ID);
                    }

                    membershipService.LogInUser(globalConfig.Tenant, model.RegisterFields.Email, model.RegisterFields.Password, true); // log the user in.
                    Logger.Info("/Account/Register UserAdded: " + model.RegisterFields.Email);
                    return Json(new { Status = "Success" });

                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError(string.Empty, ErrorCodeToString(e.StatusCode)); //TODO: add error message here and handle in razor
                    Logger.Error("/Account/Register: " + e.Message);
                }
            }

            // If we got this far, something failed, redisplay form
            return Json(new { Status = "Fail" });

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

        //public ActionResult MyWebinars(ManageMessageId? message)
        //{
        //    var Id = 0;
        //    var webinars = _repos.GetOrdersByUser(Id);
        //    //var dtos = new WebinarDTOAssembler().Entities2DTOs(webinars);
        //    //return View(dtos);
        //    return View(webinars);
        //}

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
                return RedirectToAction("Manage");
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


        private WebUser GetWebUserFromIPrincipal()
        {
            var identity = ClaimsPrincipal.Current;

            var userAccount = membershipService.GetUserAccountByUserId(identity.GetUserID());

            var user = membershipService.GetUserByEmail(userAccount.Email);
            return user;
        }

        #endregion
    }
}
