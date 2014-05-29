using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using BrockAllen.MembershipReboot;
using CUWebinars.Business.AccountService;
using CUWebinars.Business.Constants;
using CUWebinars.Business.Models;
using CUWebinars.Business.Services;
using CUWebinars.Web.Core;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Models;
using CUWebinars.Web.Services;
using Newtonsoft.Json.Linq;
using Ninject.Extensions.Logging;

namespace CUWebinars.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly GlobalConfig globalConfig = GlobalConfig.GlobalConfigSingleton;
        private readonly IMembershipService _membershipService;
        private readonly IOrderManagementService _orderManagementService;
        private readonly IStateService _stateService;
        private readonly ILogger _logger;

        public OrderController(IMembershipService membershipService, IOrderManagementService orderManagementService,
            IStateService stateService, ILogger logger)
        {
            _membershipService = membershipService;
            _orderManagementService = orderManagementService;
            _stateService = stateService;
            _logger = logger;
        }

        /// <summary>
        /// For us to use a query string, this has to be a GET request.
        /// </summary>
        /// <param name="incomingOrderModel"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult CreateOrder(IncomingOrderModel incomingOrderModel)
        {
            int idOfLastOrder = default(int);
            string verificationKey = string.Empty;
            string confirmChangeEmailUrl = string.Empty;

            if (!ModelState.IsValid)
            {
                var myErr = "";
                foreach (ModelState modelState in ViewData.ModelState.Values)
                {
                    foreach (ModelError error in modelState.Errors)
                    {
                        myErr += modelState.Value.ToString();
                    }
                }
                //a better implementation:
                //http://stackoverflow.com/questions/2845852/asp-net-mvc-how-to-convert-modelstate-errors-to-json
                //var errorList = ModelState.ToDictionary(
                //    kvp => kvp.Key,
                //    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                //);

                _logger.Error(myErr);
                return Json(new { Result = "Fail" });
            }

            try
            {
                UserAccount userAccount;
                var email = incomingOrderModel.Email.Trim();

                _logger.Info("Begin import: " + email);

                var firstName = incomingOrderModel.FirstName.Trim();
                var lastName = incomingOrderModel.LastName.Trim();
                var tempPassword = lastName.ToLower();

                var webinar = _orderManagementService.GetWebinar(incomingOrderModel.idWebinar);
                var webUser = _membershipService.GetUserByEmail(email);
                var affiliate = _orderManagementService.GetAffiliateById(incomingOrderModel.idAffiliate);

                if (webUser == null)
                {
                    try
                    {


                    var institutionForUser =
                        _membershipService.ProcessInstitutionForUser(incomingOrderModel.Institution.Trim(),
                            email,
                            incomingOrderModel.BillingAddress.City,
                            incomingOrderModel.BillingAddress.State,
                            "N",
                            "New",
                            incomingOrderModel.BillingAddress.Zip
                            );

                    incomingOrderModel.BillingAddress.AddressType = WebUiConstants.BillingAddress;
                    incomingOrderModel.ShippingAddress.AddressType = WebUiConstants.ShippingAddress;

                    IList<Address> addresses = new List<Address>
                    {
                        incomingOrderModel.BillingAddress,
                        incomingOrderModel.ShippingAddress
                    };

                    USTimeZone userTimeZone = _membershipService.GetTimeZoneByZip();
                    webUser = _membershipService.CreateWebUser(globalConfig.Tenant
                        , firstName
                        , lastName
                        , tempPassword
                        , email
                        , userTimeZone
                        , UserType.Customer
                        , institutionForUser.idInstitution
                        , addresses
                        , incomingOrderModel.Title == null ? null : incomingOrderModel.Title.Trim()
                        , null
                        , DomainConstants.Active
                        );

                    webUser.Institution = institutionForUser;

                    //  Here, we set a value which indicates to the TtsSmtpMessageDelivery object that the user was created while importing an order.
                    //  This will be checked in TtsSmtpMessageDelivery and the notification will not be sent if this value is present.
                    //  The idea being that the Order Submitted notification will contain the info nomrally in the User Registered email.
                    _stateService.SetValue(DomainConstants.UserCreatedViaNewOrder, true);

                    userAccount = _membershipService.CreateUser(globalConfig.Tenant
                        , firstName
                        , lastName
                        , email
                        , tempPassword
                        , email
                        );

                    Debug.Assert(_stateService.HasValue(DomainConstants.VerificationKey), "There's no reason session should not have a value for the VerificationKey at this point ");

                    verificationKey = _stateService.GetValue<string>(DomainConstants.VerificationKey);
                    confirmChangeEmailUrl = _stateService.GetValue<string>(DomainConstants.ConfirmChangeEmailLink);
                    _stateService.ClearValue(DomainConstants.VerificationKey);
                    _stateService.ClearValue(DomainConstants.ConfirmChangeEmailLink);

                    userAccount = _membershipService.VerifyEmailFromKey(
                        verificationKey,
                        tempPassword
                        );

                    //  Now we clear the value, so TtsSmtpMessageDelivery can go back to business as usual.
                    _stateService.ClearValue(DomainConstants.UserCreatedViaNewOrder);
                    }
                    catch (Exception exception)
                    {

                        _logger.Error("CreateOrder|CreateUser failed: " + exception.Message); 
                    }
                }

                IList<AdditionalLocation> addLocation = new List<AdditionalLocation>();

                if (incomingOrderModel.AdditionalLocation != null && incomingOrderModel.AdditionalLocation.Any())
                {
                    var price = 150;
                    //string email,decimal price,string fullname
                    var additionalLocations = incomingOrderModel.AdditionalLocation;

                    foreach (var additionalLocation in additionalLocations)
                    {
                        var additionalLocationEmail = additionalLocation.Email;

                        addLocation.Add(_orderManagementService.CreateAdditionalLocation(
                            additionalLocationEmail,
                            price,
                            null) //field for FullName
                            //additionalLocationFirstName + ' ' + additionalLocationLastName)
                            );
                    }
                }

                var orderRow = _orderManagementService.CreateOrderRow(
                    webinar,
                    addLocation,
                    incomingOrderModel.idRegType
                    );

                orderRow.Discount = _orderManagementService.GetDiscount(incomingOrderModel.Email);

                var importedOrder = _orderManagementService.CreateNewOrder(affiliate, webUser, webinar, orderRow);

                importedOrder.AdminComments = "incomingOrderModel.AdminComments";
                importedOrder.AffiliateComments = incomingOrderModel.AffiliateComments;
                importedOrder.UserComments = "incomingOrderModel.UserComments";
                importedOrder.Origin = "incomingOrderModel.Origin";
                importedOrder.FirstName = webUser.FirstName;
                importedOrder.LastName = webUser.LastName;
                importedOrder.Institution = webUser.Institution.InstitutionName;
                importedOrder.BillingEmail = email;

                importedOrder.BillingAddress = incomingOrderModel.BillingAddress.StreetAddress;
                importedOrder.BillingAddress2 = incomingOrderModel.BillingAddress.StreetAddress2;
                importedOrder.BillingPhone = incomingOrderModel.BillingAddress.Phone;
                importedOrder.BillingCity = incomingOrderModel.BillingAddress.City;
                importedOrder.BillingState = incomingOrderModel.BillingAddress.State;
                importedOrder.BillingZip = incomingOrderModel.BillingAddress.Zip;

                importedOrder.ShippingAddress = incomingOrderModel.ShippingAddress.StreetAddress;
                importedOrder.ShippingAddress2 = incomingOrderModel.ShippingAddress.StreetAddress2;
                importedOrder.ShippingPhone = incomingOrderModel.ShippingAddress.Phone;
                importedOrder.ShippingCity = incomingOrderModel.ShippingAddress.City;
                importedOrder.ShippingState = incomingOrderModel.ShippingAddress.State;
                importedOrder.ShippingZip = incomingOrderModel.ShippingAddress.Zip;
                importedOrder.ShippingFirstName = firstName;
                importedOrder.ShippingLastName = lastName;

                if (orderRow.Webinar.WebinarKey != null
                    && orderRow.RegistrationType.ShowLiveNotifications == "Yes"
                    )
                {
                    var regKeyResponse = _orderManagementService.CreateRegistrantKey(importedOrder.FirstName,
                        importedOrder.LastName, importedOrder.BillingEmail, orderRow.idWebinar,
                        orderRow.Webinar.WebinarKey);

                    if (ReferenceEquals(null, regKeyResponse))
                        throw new NullReferenceException(
                            "The Registration Key Response from the Citrix API resulted in a null response.");

                    JObject parsedJsonObject = JObject.Parse(regKeyResponse);

                    if (parsedJsonObject["registrantKey"] != null)
                    {
                        var registrantKey = parsedJsonObject["registrantKey"].ToString();
                        var joinUrl = parsedJsonObject["joinUrl"].ToString();

                        orderRow.RegistrantKey = registrantKey;
                        orderRow.JoinURL = joinUrl;
                    }
                    else
                    {
                        /*  *************** 404 error condition *************** 
                             * json payload will look like:
                             *      {"description":"The webinar does not exist.","incident":3984078431536134144}
                             * which is not usable
                             */
                    }
                }


                _orderManagementService.SaveOrderChanges(importedOrder, verificationKey, confirmChangeEmailUrl);
                idOfLastOrder = importedOrder.idOrder;
                //idOfLastOrderOrderRow = importedOrder.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).idOrderRow;
                _logger.Info("Posted idOrder=" + idOfLastOrder);

                return Json(
                    new
                    {
                        Result = idOfLastOrder
                    },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                _logger.Error("Order creation failed: "+ exception.Message);
            }

            return Json(new { Result = WebUiConstants.Fail });
        }
    }
}