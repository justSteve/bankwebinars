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
using CUWebinars.Business.CQS;
using CUWebinars.Business.CQS.Commands;
using CUWebinars.Business.CQS.Queries;
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
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandProcessor _commandProcessor;

        public OrderController(IMembershipService membershipService, IOrderManagementService orderManagementService,
            IStateService stateService, ILogger logger, IQueryProcessor queryProcessor, ICommandProcessor commandProcessor)
        {
            _membershipService = membershipService;
            _orderManagementService = orderManagementService;
            _stateService = stateService;
            _logger = logger;
            _queryProcessor = queryProcessor;
            _commandProcessor = commandProcessor;
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
                //Please use this general pattern when logging ModelState errors.
                var myErr = string.Empty;
                foreach (ModelState modelState in ViewData.ModelState.Values)
                {
                    foreach (ModelError error in modelState.Errors)
                    {
                        myErr += error.ErrorMessage + Environment.NewLine;
                    }
                }
                //a better implementation:
                //http://stackoverflow.com/questions/2845852/asp-net-mvc-how-to-convert-modelstate-errors-to-json
                //var errorList = ModelState.ToDictionary(
                //    kvp => kvp.Key,
                //    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                //);

                _logger.Error(myErr);
                return Json(new { Result = WebUiConstants.Fail });
            }

            try
            {
                var email = incomingOrderModel.Email.Trim();
                var firstName = incomingOrderModel.FirstName.Trim();
                var lastName = incomingOrderModel.LastName.Trim();
                var tempPassword = lastName.ToLower();

                _logger.Info("Begin import: " + email);

                var orderManagementQuery = new OrderManagementQuery
                {
                    AffiliateId = incomingOrderModel.idAffiliate,
                    Email = email,
                    WebinarId = incomingOrderModel.idWebinar
                };

                var orderManagementQueryResult = _queryProcessor.Process(orderManagementQuery);

                if ( ReferenceEquals(null, orderManagementQueryResult.WebUser))
                {
                    //  Here, we set a value which indicates to the TtsSmtpMessageDelivery object that the user was created while importing an order.
                    //  This will be checked in TtsSmtpMessageDelivery and the notification will not be sent if this value is present.
                    //  The idea being that the Order Submitted notification will contain the info nomrally in the User Registered email.
                    _stateService.SetValue(DomainConstants.UserCreatedViaNewOrder, true);

                    var registerNewAccountCommand = new RegisterNewAccountCommand
                    {
                        BillingAddress = incomingOrderModel.BillingAddress,
                        Email = email,
                        FirstName = firstName,
                        LastName =  lastName,
                        Institution = incomingOrderModel.Institution.Trim(),
                        ShippingAddress = incomingOrderModel.ShippingAddress,
                        TempPassword = tempPassword,
                        Tenant = globalConfig.Tenant,
                        Title = incomingOrderModel.Title == null ? null : incomingOrderModel.Title.Trim()
                    };

                    try
                    {
                        _commandProcessor.Execute(registerNewAccountCommand);
                        orderManagementQueryResult.WebUser = registerNewAccountCommand.WebUser; //  assign out parameter for later use

                        Debug.Assert(_stateService.HasValue(DomainConstants.VerificationKey), "There's no reason session should not have a value for the VerificationKey at this point ");

                        verificationKey = _stateService.GetValue<string>(DomainConstants.VerificationKey);
                        confirmChangeEmailUrl = _stateService.GetValue<string>(DomainConstants.ConfirmChangeEmailLink);
                        _stateService.ClearValue(DomainConstants.VerificationKey);
                        _stateService.ClearValue(DomainConstants.ConfirmChangeEmailLink);

                        var verifyAccountCommand = new VerifyAccountCommand
                        {
                            TempPassword = tempPassword,
                            VerificationKey = verificationKey
                        };

                        _commandProcessor.Execute(verifyAccountCommand);

                        //  Now we clear the value, so TtsSmtpMessageDelivery can go back to business as usual.
                        _stateService.ClearValue(DomainConstants.UserCreatedViaNewOrder);
                    }
                    catch (Exception exception)
                    {

                        _logger.ErrorException(string.Format("CreateOrder|CreateUser failed: {0}", exception.Message), exception);
                    }
                }

                var addOrderRowCommand = new AddOrderRowCommand
                {
                    AdditionalLocations = incomingOrderModel.AdditionalLocations,
                    Email = email,
                    RegistrationType = incomingOrderModel.idRegType,
                    Webinar = orderManagementQueryResult.Webinar,   
                };

                _commandProcessor.Execute(addOrderRowCommand);

                var addOrderCommand = new AddOrderCommand
                {
                    Affiliate = orderManagementQueryResult.Affiliate,
                    AffiliateComments = incomingOrderModel.AffiliateComments,
                    BillingAddress = incomingOrderModel.BillingAddress,
                    ConfirmChangeEmailUrl = confirmChangeEmailUrl,
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    OrderRow = addOrderRowCommand.OrderRow, // out parameter of addOrderRowCommand command
                    ShippingAddress = incomingOrderModel.ShippingAddress,
                    VerificationKey = verificationKey,
                    Webinar = orderManagementQueryResult.Webinar,
                    WebUser = orderManagementQueryResult.WebUser
                };

                _commandProcessor.Execute(addOrderCommand);

                idOfLastOrder = addOrderCommand.OrderId; // out parameter of AddOrderCommand command

                //idOfLastOrderOrderRow = importedOrder.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).idOrderRow;
                _logger.Info("Posted idOrder=" + idOfLastOrder);

                return Json(new { Result = idOfLastOrder.ToString() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                var errString = string.Format("Order creation failed on {0} - {1} with msg: {2}", incomingOrderModel.Email, incomingOrderModel.idWebinar, exception.Message);
                _logger.ErrorException(errString, exception);
            }

            return Json(new { Result = "0" }, JsonRequestBehavior.AllowGet);
            //return Json(new { Result = WebUiConstants.Fail });
        }
    }
}