using System.Linq;
using System.Security.Claims;
using System.Web.Helpers;
using BrockAllen.MembershipReboot;
using CUWebinars.Business.AccountService;
using CUWebinars.Business.Constants;
using CUWebinars.Business.Models;
using CUWebinars.Web.Core;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Membership;
using CUWebinars.Web.Models;
using CUWebinars.Web.Services;
using CUWebinars.Web.ViewModel;
using Newtonsoft.Json;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ClaimTypes = System.IdentityModel.Claims.ClaimTypes;

namespace CUWebinars.Web.Controllers
{
    public class MembershipNotificationOpsController : Controller
    {
        private readonly IMembershipService _membershipService;
        private readonly ILogger _logger;
        private readonly IStateService _stateService;
        private GlobalConfig globalConfig = GlobalConfig.GlobalConfigSingleton;
        private bool _disposed;

        public MembershipNotificationOpsController(IMembershipService membershipService, ILogger logger, IStateService stateService)
        {
            _membershipService = membershipService;
            _logger = logger;
            _stateService = stateService;
        }

        public ViewResult MembershipNotifications()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public PartialViewResult CreateAUser()
        {
            var registerViewModel = new RegisterViewModel
            {
                RegisterFields = new RegisterModel
                {
                    AccountDetailsTitle = WebUiConstants.Register,
                    BillingAddress = new AddressModel { TypeOfAddress = AddressType.Billing },
                    ShippingAddress = new AddressModel { TypeOfAddress = AddressType.Shipping }
                }
            };

            return PartialView("~/Views/MembershipNotificationOps/_RegisterPartial.cshtml", registerViewModel);
        }

        [HttpPost]
        public ActionResult CreateAUser(RegisterViewModel model)
        {
            var globals = GlobalConfig.GlobalConfigSingleton;
            var dataOperations = new DataOperations();

            try
            {
                UserAccount userAccount;
                if (model.RegisterFields != null)
                {
                    var myInstitution =
                        _membershipService.ProcessInstitutionForUser(model.RegisterFields.Institution.Trim(),
                            model.RegisterFields.Email.TrimEnd(),
                            model.RegisterFields.BillingAddress.City.Trim(),
                            model.RegisterFields.BillingAddress.State.Trim(),
                            "N",
                            "New",
                            model.RegisterFields.BillingAddress.Zip.Trim());

                    var addresses = ProcessAddresses(model);

                    _membershipService.CreateWebUser(globals.Tenant,
                        model.RegisterFields.FirstName,
                        model.RegisterFields.LastName,
                        string.Empty,
                        model.RegisterFields.Password,
                        USTimeZone.Central,
                        model.RegisterFields.UserType,
                        myInstitution.idInstitution,
                        addresses,
                        model.RegisterFields.Title,
                        null
                        );

                    userAccount = _membershipService.CreateUser(globals.Tenant, 
                        model.RegisterFields.FirstName,
                        model.RegisterFields.LastName, 
                        string.Empty, 
                        model.RegisterFields.Password,
                        model.RegisterFields.Email
                        );

                    Debug.Assert(_stateService.HasValue(DomainConstants.VerificationKey), "There's no reason session should not have a value for the VerificationKey at this point ");

                    var verificationKey = _stateService.GetValue<string>(DomainConstants.VerificationKey);
                    _stateService.ClearValue(DomainConstants.VerificationKey);


                    userAccount =_membershipService.VerifyEmailFromKey(
                            verificationKey,
                            model.RegisterFields.Password
                            );
                    
                }
                else
                {
                    var newPassword = System.Web.Security.Membership.GeneratePassword(8, 1);
                    var newEmail = string.Concat(Helpers.RandomString(10), "@", Helpers.RandomString(5), ".com");

                    Debug.WriteLine("Email: {0}{1}Password: {2}", newEmail, Environment.NewLine, newPassword);

                    _membershipService.CreateWebUser(globals.Tenant,
                        "John",
                        "Hancock",
                        newPassword,
                        newEmail,
                        USTimeZone.Central,
                        UserType.Customer,
                        25,
                        new Address[] {
                            new Address
                            {
                                AddressType = WebUiConstants.BillingAddress,
                                City = "Del Rio",
                                Country = "USA",
                                Name = "John Hancock",
                                Phone = "555-555-5555",
                                StreetAddress = string.Concat(new Random(100).Next(0, 1000).ToString(), " Wildcat Dr"),
                                StreetAddress2 = string.Empty,
                                State = "Tx",
                                Zip = "5000"
                            },
                            new Address
                            {
                                AddressType = WebUiConstants.ShippingAddress,
                                City = "Del Rio",
                                Country = "USA",
                                Name = "John Hancock",
                                Phone = "555-555-5555",
                                StreetAddress = string.Concat(new Random(100).Next(0, 1000).ToString(), " Wildcat Dr"),
                                StreetAddress2 = string.Empty,
                                State = "Tx",
                                Zip = "5000"
                            }},
                        "Dr",
                        null
                        );

                    userAccount = _membershipService.CreateUser(globals.Tenant, "John", "Hancock", string.Empty, newPassword, newEmail);

                    Debug.Assert(_stateService.HasValue(DomainConstants.VerificationKey), "There's no reason session should not have a value for the VerificationKey at this point ");

                    var verificationKey = _stateService.GetValue<string>(DomainConstants.VerificationKey);
                    _stateService.ClearValue(DomainConstants.VerificationKey);


                    userAccount = _membershipService.VerifyEmailFromKey(
                                        verificationKey,
                                        newPassword
                                    );

                    return Json(new { Result = WebUiConstants.Success, email = newEmail, password = newPassword });
                }
            }
            catch (Exception exception)
            {
                //  don't swallow exception over long term. 
                _logger.Error("CreateAUser: {0}", exception.Message);
            }

            return Json(new { Result = WebUiConstants.Fail });
        }

        public PartialViewResult LogInAsUser()
        {
            var logInAsOtherUserViewModel = new LogInAsOtherUserViewModel
            {
                Email = string.Empty,
                Password = string.Empty
            };

            return PartialView("_LogInAsUser", logInAsOtherUserViewModel);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogInAsUser(LogInAsOtherUserViewModel model)
        {
            var adminUser = User.Identity as ClaimsIdentity;
            var adminUserEmail = adminUser.Claims.Single(c => c.Type == ClaimTypes.Email).Value;
            var impersonatedUserAccount = _membershipService.GetUserAccountByEmail(globalConfig.Tenant, model.Email);

            if (ReferenceEquals(null, impersonatedUserAccount))
            {
                var nullReferenceException =
                    new NullReferenceException(string.Format("There is no webuser with the email address {0}", model.Email));
                _logger.ErrorException("LogInAsUser | No User Found For Email", nullReferenceException);
                ModelState.AddModelError("Inavlid email", nullReferenceException);
                return View("","",""); //   TODO: Figure out how to show error messages
            }

            _membershipService.LogOutUser();

            _stateService.SetValue(WebUiConstants.AdminUserEmail, adminUserEmail);

            _membershipService.LogInAdminUserAsOtherUser(globalConfig.Tenant, 
                adminUserEmail.Trim(), model.Password.Trim(),
                impersonatedUserAccount
                );
            
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

            return PartialView("~/Views/MembershipNotificationOps/_ManualPasswordReset.cshtml", manualPasswordResetViewModel);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ManualPasswordReset(ManualPasswordResetViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userAccount = _membershipService.GetUserAccountByEmail(globalConfig.Tenant, model.Email);
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

            _logger.Error("ManualPasswordReset: {0}", AppHelper.GetUserAuditInfo());
            _logger.Error("ManualPasswordReset: {0}", myErr);

            // the idea of using a Constant in this case seems limited. ... but i'm not sure so check my thinking...
            // How can we best get the value of the modelState.Errors down to the view.
            // I know there's a convention of injecting an 'ErrorDiv' at either the top
            // of a form or on a field by field basis.
            // OTOH, i really like the idea of making the button label carry the error message.

            return Json(new {Result = WebUiConstants.Fail});
        }

        [AllowAnonymous]
        public PartialViewResult PasswordResetOperation()
        {
            var resetPasswordModel = new ResetPasswordModel
            {
                Email = string.Empty,
                EmailSent = false
            };

            return PartialView("~/Views/MembershipNotificationOps/_ResetPasswordPartial.cshtml", resetPasswordModel);
        }

        [HttpPost]
        [AllowAnonymous]
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

            return PartialView("~/Views/MembershipNotificationOps/_PasswordResetConfirm.cshtml", changePasswordFromResetKeyInputModel);
        }

        [HttpPost]
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

            return PartialView("~/Views/MembershipNotificationOps/_ImportOrder.cshtml", importOrderViaDashboardViewModel);
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

        internal static class Helpers
        {
            private static readonly Random Random = new Random((int)DateTime.Now.Ticks);

            /// <summary>
            /// Taken from StackOverflow answer http://stackoverflow.com/a/1122519/540156
            /// </summary>
            internal static string RandomString(int size)
            {
                var builder = new StringBuilder(size);
                for (var i = 0; i < size; i++)
                {
                    var ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * Random.NextDouble() + 65)));
                    builder.Append(ch);
                }

                return builder.ToString();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                var logger = _logger as IDisposable;
                if (logger != null)
                    logger.Dispose();

                _membershipService.Dispose();

                base.Dispose(true);
            }
            _disposed = true;
        }
    }
}