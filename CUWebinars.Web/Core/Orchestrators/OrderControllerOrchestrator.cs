using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CUWebinars.Business.CQS;
using CUWebinars.Business.CQS.Commands;
using CUWebinars.Business.CQS.Queries;
using CUWebinars.Business.Models;
using CUWebinars.Web.Models;

namespace CUWebinars.Web.Core.Orchestrators
{
    public class OrderControllerOrchestrator : IOrderControllerOrchestrator
    {
        private readonly GlobalConfig globalConfig = GlobalConfig.GlobalConfigSingleton;
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandProcessor _commandProcessor;

        public OrderControllerOrchestrator(IQueryProcessor queryProcessor, ICommandProcessor commandProcessor)
        {
            _queryProcessor = queryProcessor;
            _commandProcessor = commandProcessor;
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
        }

        public OrderManagementQueryResult GetData(IncomingOrderModel incomingOrderModel, string email)
        {
            var orderManagementQuery = new OrderManagementQuery
            {
                AffiliateId = incomingOrderModel.idAffiliate,
                Email = email,
                WebinarId = incomingOrderModel.idWebinar
            };

            return _queryProcessor.Process(orderManagementQuery);
        }

        public WebUser ProcessNewUser(IncomingOrderModel incomingOrderModel, string email)
        {
            var firstName = incomingOrderModel.FirstName.Trim();
            var lastName = incomingOrderModel.LastName.Trim();
            var tempPassword = lastName.ToLower();

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
            return registerNewAccountCommand.WebUser; //  assign out parameter for later use
        }
    }
}