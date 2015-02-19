using CUWebinars.Business.Constants;
using CUWebinars.Business.Core;
using CUWebinars.Business.CQS;
using CUWebinars.Business.CQS.Commands;
using CUWebinars.Business.CQS.Queries;
using CUWebinars.Business.Models;
using CUWebinars.Web.Membership;
using CUWebinars.Web.Models;
using CUWebinars.Web.Services;
using Ninject.Extensions.Logging;
using System;
using System.Diagnostics;

namespace CUWebinars.Web.Core.Orchestrators
{
    public class OrderControllerOrchestrator : IOrderControllerOrchestrator
    {
        private readonly GlobalConfig globalConfig = GlobalConfig.GlobalConfigSingleton;
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandProcessor _commandProcessor;
        private readonly IStateService _stateService;
        private readonly ILogger _logger;

        public OrderControllerOrchestrator(IQueryProcessor queryProcessor, ICommandProcessor commandProcessor, IStateService stateService, ILogger logger)
        {
            _queryProcessor = queryProcessor;
            _commandProcessor = commandProcessor;
            _stateService = stateService;
            _logger = logger;
        }

        public int MigrateOrder(MigrateOrderModel migrateOrderModel,
                    string email,
                    MigratorQueryResult migratorQueryResult,
                    string verificationKey,
                    string confirmChangeEmailUrl)
        {
            var migrateOrderRowCommand = new MigrateOrderRowCommand
            {
                AdditionalLocationsString = migrateOrderModel.AdditionalLocationsString,
                Email = email,
                Name = "c/o " + migrateOrderModel.FirstName + " " + migrateOrderModel.LastName,
                RegistrationType = migrateOrderModel.idRegType,
                Discount = migrateOrderModel.DiscountCode,
                OrderDate =  migrateOrderModel.OrderDate,
                Webinar = migratorQueryResult.Webinar
            };

            _commandProcessor.Execute(migrateOrderRowCommand);


            var migrateOrderCommand = new MigrateOrderCommand()
            {
                Affiliate = migratorQueryResult.Affiliate,
                AffiliateComments = migrateOrderModel.AffiliateComments,
                BillingAddress = migrateOrderModel.BillingAddress,
                ConfirmChangeEmailUrl = confirmChangeEmailUrl,
                Email = email,
                FirstName = migrateOrderModel.FirstName.Trim(),
                LastName = migrateOrderModel.LastName.Trim(),
                OrderRow = migrateOrderRowCommand.OrderRow, // out parameter of addOrderRowCommand command
                ShippingAddress = migrateOrderModel.ShippingAddress,
                VerificationKey = verificationKey,
                OrderDate = migrateOrderModel.OrderDate,
                Total = migrateOrderModel.Total,
                //idUserLegacy = migrateOrderModel.idUserLegacy,
                idOrderLegacy = migrateOrderModel.idOrderLegacy,
                Webinar = migratorQueryResult.Webinar,
                WebUser = migratorQueryResult.WebUser,
                AdditionalLocationsString = migrateOrderModel.AdditionalLocationsString
            };


            _commandProcessor.Execute(migrateOrderCommand);

            return migrateOrderCommand.OrderId;
        }

        public int ImportOrder(ImportOrderModel ImportOrderModel,
            string email,
            ImportQueryResult importQueryResult,
            string verificationKey,
            string confirmChangeEmailUrl,
            bool existingUser)
        {
            var ImportOrderRowCommand = new ImportOrderRowCommand
            {
                AdditionalLocationsString = ImportOrderModel.AdditionalLocationsString,
                Email = email,
                RegistrationType = Convert.ToInt32(ImportOrderModel.RegistrationType),
                Discount = ImportOrderModel.DiscountCode,
                Webinar = importQueryResult.Webinar
            };

            _commandProcessor.Execute(ImportOrderRowCommand);


            var ImportOrderCommand = new ImportOrderCommand()
            {
                Affiliate = importQueryResult.Affiliate,
                AffiliateComments = ImportOrderModel.AffiliateComments,
                BillingAddress = ImportOrderModel.BillingAddress,
                ConfirmChangeEmailUrl = confirmChangeEmailUrl,
                Email = email,
                FirstName = ImportOrderModel.FirstName.Trim(),
                LastName = ImportOrderModel.LastName.Trim(),
                OrderGenesis = existingUser ? OrderGenesis.ImportedForExistingUser : OrderGenesis.ImportedForNewUser,
                OrderRow = ImportOrderRowCommand.OrderRow, // out parameter of addOrderRowCommand command
                ShippingAddress = ImportOrderModel.ShippingAddress,
                VerificationKey = verificationKey,
                Webinar = importQueryResult.Webinar,
                WebUser = importQueryResult.WebUser,
                AdditionalLocationsString = ImportOrderModel.AdditionalLocationsString
            };
            
            _commandProcessor.Execute(ImportOrderCommand);

            return ImportOrderCommand.OrderId;
        }

        public int CreateNewOrder(IncomingOrderModel incomingOrderModel, string email, OrderManagementQueryResult orderManagementQueryResult, string verificationKey, string confirmChangeEmailUrl, bool userAlreadyExists)
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
                OrderGenesis = userAlreadyExists ? OrderGenesis.CreatedViaCartByExistingUser : OrderGenesis.CreatedViaCartByNewUser,
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

        public void FinalizeMigratedRegistation(MigrateOrderModel migrateOrder, string verificationKey)
        {
            var verifyAccountCommand = new VerifyAccountCommand
            {
                TempPassword = _stateService.GetValue<string>("tempPassword"),
                VerificationKey = verificationKey
            };

            _stateService.ClearValue("tempPassword");

            _commandProcessor.Execute(verifyAccountCommand);

            //  Now we clear the value, so TtsSmtpMessageDelivery can go back to business as usual.
            _stateService.ClearValue(DomainConstants.UserCreatedViaNewOrder);
        }

        public void FinalizeImportedRegistation(string verificationKey)
        {
            var verifyAccountCommand = new VerifyAccountCommand
            {
                TempPassword = _stateService.GetValue<string>("tempPassword"),
                VerificationKey = verificationKey
            };

            _stateService.ClearValue("tempPassword");

            _commandProcessor.Execute(verifyAccountCommand);

            //  Now we clear the value, so TtsSmtpMessageDelivery can go back to business as usual.
            _stateService.ClearValue(DomainConstants.UserCreatedViaNewOrder);
        }

        public MigratorQueryResult GetPreparatoryDataForMigrator(MigrateOrderModel migrateOrderModel, string email)
        {

            int _discountCode;
            string discountCode = null;

            bool result = Int32.TryParse(migrateOrderModel.DiscountCode, out _discountCode);
            if (result)
            {

                var migratorQuery = new MigratorQuery
                {
                    AffiliateId = migrateOrderModel.idAffiliate,
                    Email = email,
                    LegacyOrderId = migrateOrderModel.idOrderLegacy,
                    DiscountId = _discountCode,
                    WebinarId = migrateOrderModel.idWebinar
                };


                return _queryProcessor.Process(migratorQuery);

            }
            else
            {
                var migratorQuery = new MigratorQuery
                {
                    AffiliateId = migrateOrderModel.idAffiliate,
                    Email = email,
                    LegacyOrderId = migrateOrderModel.idOrderLegacy,
                    WebinarId = migrateOrderModel.idWebinar
                };


                return _queryProcessor.Process(migratorQuery);
            }
        }

        public ImportQueryResult GetPreparatoryDataForImporter(ImportOrderModel importOrderModel, string email)
        {
            var importQuery = new ImportQuery
            {
                AffiliateId = importOrderModel.idAffiliate,
                Email = email,
                WebinarId = importOrderModel.idWebinar
            };

            return _queryProcessor.Process(importQuery);
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
            // this value was inserted into Session in the TtsTokenizer when the UserAccount was created by MR.
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

            Debug.Assert(_stateService.HasValue(DomainConstants.VerificationKey), "There's no reason session should not have a value for the VerificationKey at this point ");

            return registerNewAccountCommand.WebUser; //  assign out parameter for later use
        }

        public WebUser MigrateUser(MigrateOrderModel migrateOrder, string email)
        {
            var firstName = migrateOrder.FirstName.Trim();
            var lastName = migrateOrder.LastName.Trim();

            // HACK: We are logging the temppassword as a means of persisting it to enable Admins to email it to the imported user. 
            // It is logged at Error level b/c that level will always be logged (but Info-level may be turned off). 
            var tempPassword = PasswordGenerator.GenerateRandomString(6);
            _logger.Error("TempPassword for migrated user | {0} : {1}", email, tempPassword);

            _stateService.SetValue("tempPassword", tempPassword);

            //  Here, we set a value which indicates to the TtsSmtpMessageDelivery object that the user was created while importing an order.
            //  This will be checked in TtsSmtpMessageDelivery and the notification will not be sent if this value is present.
            _stateService.SetValue(DomainConstants.UserCreatedViaNewOrder, true);

            var registerNewAccountCommand = new RegisterNewAccountCommand
            {
                BillingAddress = migrateOrder.BillingAddress,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                Institution = migrateOrder.Institution.Trim(),
                ShippingAddress = migrateOrder.ShippingAddress,
                TempPassword = tempPassword,
                Tenant = globalConfig.Tenant,
                Title = migrateOrder.Title == null ? null : migrateOrder.Title.Trim()
            };

            _commandProcessor.Execute(registerNewAccountCommand);

            Debug.Assert(_stateService.HasValue(DomainConstants.VerificationKey),
    "There's no reason session should not have a value for the VerificationKey at this point ");

            return registerNewAccountCommand.WebUser; //  assign out parameter for later use
        }

        public WebUser ImportUser(ImportOrderModel importOrder, string email)
        {
            var firstName = importOrder.FirstName.Trim();
            var lastName = importOrder.LastName.Trim();

            // HACK: We are logging the temppassword as a means of persisting it to enable Admins to email it to the imported user. 
            // It is logged at Error level b/c that level will always be logged (but Info-level may be turned off). 
            var tempPassword = PasswordGenerator.GenerateRandomString(6);
            _logger.Error("TempPassword for imported user | {0} : {1}", email, tempPassword);

            _stateService.SetValue("tempPassword", tempPassword);


            //  Here, we set a value which indicates to the TtsSmtpMessageDelivery object that the user was created while importing an order.
            //  This will be checked in TtsSmtpMessageDelivery and the notification will not be sent if this value is present.
            //  The idea being that the Order Submitted notification will contain the info nomrally in the User Registered email.
            _stateService.SetValue(DomainConstants.UserCreatedViaNewOrder, true);

            var registerNewAccountCommand = new RegisterNewAccountCommand
            {
                BillingAddress = importOrder.BillingAddress,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                Institution = importOrder.Institution.Trim(),
                ShippingAddress = importOrder.ShippingAddress,
                TempPassword = tempPassword,
                Tenant = globalConfig.Tenant,
                Title = importOrder.Title == null ? null : importOrder.Title.Trim()
            };

            _commandProcessor.Execute(registerNewAccountCommand);

            Debug.Assert(_stateService.HasValue(DomainConstants.VerificationKey),
    "There's no reason session should not have a value for the VerificationKey at this point ");

            return registerNewAccountCommand.WebUser; //  assign out parameter for later use
        }

    }
}