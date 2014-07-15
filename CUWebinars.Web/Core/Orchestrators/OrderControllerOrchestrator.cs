using System.Diagnostics;
using CUWebinars.Business.Constants;
using CUWebinars.Business.CQS;
using CUWebinars.Business.CQS.Commands;
using CUWebinars.Business.CQS.Queries;
using CUWebinars.Business.Models;
using CUWebinars.Web.Models;
using CUWebinars.Web.Services;

namespace CUWebinars.Web.Core.Orchestrators
{
    public class OrderControllerOrchestrator : IOrderControllerOrchestrator
    {
        private readonly GlobalConfig globalConfig = GlobalConfig.GlobalConfigSingleton;
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandProcessor _commandProcessor;
        private readonly IStateService _stateService;

        public OrderControllerOrchestrator(IQueryProcessor queryProcessor, ICommandProcessor commandProcessor, IStateService stateService)
        {
            _queryProcessor = queryProcessor;
            _commandProcessor = commandProcessor;
            _stateService = stateService;
        }

        public int CreateNewOrder(IncomingOrderModel incomingOrderModel,
            string email,
            OrderManagementQueryResult orderManagementQueryResult,
            string verificationKey,
            string confirmChangeEmailUrl)
        {
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
                FirstName = incomingOrderModel.FirstName.Trim(),
                LastName = incomingOrderModel.LastName.Trim(),
                OrderRow = addOrderRowCommand.OrderRow, // out parameter of addOrderRowCommand command
                ShippingAddress = incomingOrderModel.ShippingAddress,
                VerificationKey = verificationKey,
                Webinar = orderManagementQueryResult.Webinar,
                WebUser = orderManagementQueryResult.WebUser
            };

            _commandProcessor.Execute(addOrderCommand);

            return addOrderCommand.OrderId;
        }

        public void FinalizeNewRegistration(IncomingOrderModel incomingOrderModel, string verificationKey)
        {
            var verifyAccountCommand = new VerifyAccountCommand
            {
                TempPassword = incomingOrderModel.LastName.Trim().ToLower(),
                VerificationKey = verificationKey
            };

            _commandProcessor.Execute(verifyAccountCommand);

            //  Now we clear the value, so TtsSmtpMessageDelivery can go back to business as usual.
            _stateService.ClearValue(DomainConstants.UserCreatedViaNewOrder);
        }

        public OrderManagementQueryResult GetPreparatoryData(IncomingOrderModel incomingOrderModel, string email)
        {
            var orderManagementQuery = new OrderManagementQuery
            {
                AffiliateId = incomingOrderModel.idAffiliate,
                Email = email,
                WebinarId = incomingOrderModel.idWebinar
            };

            return _queryProcessor.Process(orderManagementQuery);
        }

        public string GetConfirmChangeEmailLinkForNewUserAccount()
        {
            var confirmChangeEmailUrl = _stateService.GetValue<string>(DomainConstants.ConfirmChangeEmailLink);
            _stateService.ClearValue(DomainConstants.ConfirmChangeEmailLink);

            return confirmChangeEmailUrl;
        }

        public string GetVerificationKeyForNewUserAccount()
        {
            var verificationKey = _stateService.GetValue<string>(DomainConstants.VerificationKey);
            _stateService.ClearValue(DomainConstants.VerificationKey);

            return verificationKey;
        }

        public WebUser ProcessNewUser(IncomingOrderModel incomingOrderModel, string email)
        {
            var firstName = incomingOrderModel.FirstName.Trim();
            var lastName = incomingOrderModel.LastName.Trim();
            var tempPassword = lastName.ToLower();

            //  Here, we set a value which indicates to the TtsSmtpMessageDelivery object that the user was created while importing an order.
            //  This will be checked in TtsSmtpMessageDelivery and the notification will not be sent if this value is present.
            //  The idea being that the Order Submitted notification will contain the info nomrally in the User Registered email.
            _stateService.SetValue(DomainConstants.UserCreatedViaNewOrder, true);

            var registerNewAccountCommand = new RegisterNewAccountCommand
            {
                BillingAddress = incomingOrderModel.BillingAddress,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                Institution = incomingOrderModel.Institution.Trim(),
                ShippingAddress = incomingOrderModel.ShippingAddress,
                TempPassword = tempPassword,
                Tenant = globalConfig.Tenant,
                Title = incomingOrderModel.Title == null ? null : incomingOrderModel.Title.Trim()
            };

            _commandProcessor.Execute(registerNewAccountCommand);

            Debug.Assert(_stateService.HasValue(DomainConstants.VerificationKey),
    "There's no reason session should not have a value for the VerificationKey at this point ");

            return registerNewAccountCommand.WebUser; //  assign out parameter for later use
        }
    }
}