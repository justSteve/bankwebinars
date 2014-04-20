using System.IO;
using System.Web;
using System.Web.UI.WebControls.WebParts;
using BrockAllen.MembershipReboot;
using CUWebinars.Business.AccountService;
using CUWebinars.Business.Models;
using CUWebinars.Web.Core;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Membership;
using CUWebinars.Web.Models;
using CUWebinars.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Web.Mvc;
using Newtonsoft.Json;
using Ninject.Extensions.Logging;

namespace CUWebinars.Web.Controllers
{
    public class MembershipNotificationOpsController : Controller
    {
        private readonly IMembershipService _membershipService;
        private readonly ILogger _logger;

        public MembershipNotificationOpsController(IMembershipService membershipService, ILogger logger)
        {
            _membershipService = membershipService;
            _logger = logger;
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
                    BillingAddress = new AddressModel {TypeOfAddress = AddressType.Billing},
                    ShippingAddress = new AddressModel {TypeOfAddress = AddressType.Shipping}
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

                    userAccount = _membershipService.CreateUser(globals.Tenant, model.RegisterFields.FirstName,
                        model.RegisterFields.LastName, string.Empty, model.RegisterFields.Password,
                        model.RegisterFields.Email);
                    
                    dataOperations.SetNewAccountToVerified(userAccount.ID);
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

                    //_membershipService.AddAddressesForWebUser(addresses);

                    userAccount =_membershipService.CreateUser(globals.Tenant, "John", "Hancock", string.Empty, newPassword, newEmail);
                    dataOperations.SetNewAccountToVerified(userAccount.ID);

                    return Json(new { result = "success", email = newEmail, password = newPassword });
                }
            }
            catch (Exception exception)
            {
                //  don't swallow exception over long term. 
                return Json(new { result = "failed" });
            }

            return View("MembershipNotifications");
        }

        [AllowAnonymous]
        public PartialViewResult PasswordResetOperation()
        {
            var resetPasswordModel = new ResetPasswordModel
            {
                Email = "",
                EmailSent = false
            };

            return PartialView("~/Views/MembershipNotificationOps/_ResetPasswordPartial.cshtml", resetPasswordModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult ResetPassword(string email)
        {
            var globals = GlobalConfig.GlobalConfigSingleton;
            var cResult = new Dictionary<string, string>(1);

            try
            {
                _membershipService.ResetPassword(globals.Tenant, email);
                cResult.Add(WebUiConstants.OpStatus, "Success");
            }
            catch (Exception exception)
            {
                cResult.Add(WebUiConstants.OpStatus, "Fail");
            }


            return Json(cResult, JsonRequestBehavior.AllowGet);
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
                                  ""BillingAddress"": {
                                    ""AddressType"": ""Billing"",
                                    ""Name"": ""Alan Turing"",
                                    ""Phone"": ""555-555-5555"",
                                    ""StreetAddress"": ""968 Wildcat Dr"",
                                    ""StreetAddress2"": """",
                                    ""City"": ""Del Rio"",
                                    ""Zip"": ""5000"",
                                    ""State"": ""Tx"",
                                    ""Country"": ""USA""
                                  },
                                  ""Email"": ""alanbturingy@turing.com"",
                                  ""FirstName"": ""Alan"",
                                  ""LastName"": ""Turing"",
                                  ""idAffiliate"": 19,
                                  ""idRegType"": 88,
                                  ""idWebinar"": 437,
                                  ""Institution"": ""Some Institution"",
                                  ""ShippingAddress"": {
                                    ""AddressType"": ""Shipping"",
                                    ""Name"": ""Alan Turing"",
                                    ""Phone"": ""555-555-5555"",
                                    ""StreetAddress"": ""968 Wildcat Dr"",
                                    ""StreetAddress2"": """",
                                    ""City"": ""Del Rio"",
                                    ""Zip"": ""5000"",
                                    ""State"": ""Tx"",
                                    ""Country"": ""USA""
                                  },
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
                    Path.Combine(HttpRuntime.AppDomainAppPath, @"App_Data/Orders", "sampleorderFromExcel.csv"))
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
                    Enum.GetName(typeof (AddressType), registerViewModel.RegisterFields.BillingAddress.TypeOfAddress),
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
                    Enum.GetName(typeof (AddressType), registerViewModel.RegisterFields.ShippingAddress.TypeOfAddress),
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
    }
}