using BrockAllen.MembershipReboot.Ef;
using CUWebinars.Business.AccountService;
using CUWebinars.Business.CQS;
using CUWebinars.Business.CQS.CommandHandlers;
using CUWebinars.Business.CQS.Commands;
using CUWebinars.Business.CQS.QueryHandlers;
using CUWebinars.Business.Models;
using CUWebinars.Business.Services;
using CUWebinars.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace CUWebinars.Business.Tests.UnitTests
{
    [TestClass]
    public class OrderManagementCommandHandlersTests
    {
        Mock<IMembershipService> _membershipServiceMock;
        Mock<IOrderManagementService> _orderManagementServiceMock;
        Webinar _webinar;
        int _idRegType;
        IList<IncomingAdditionalLocation> _incomingAdditionalLocation;
        IList<AdditionalLocation> _additionalLocations;
        const string cuwebinars = "CUWebinars";
        const string acmeInc = "ACME Inc";
        private const string email = "somevalid@emailaddress.com";
        private const string _firstName = "John";
        private const string _lastName = "Hancock";
        private readonly string fullName = string.Concat(_firstName, " ", _lastName);
        AddOrderRowCommand _addOrderRowCommand;
        AddOrderCommand _addOrderCommand;

        [TestInitialize]
        public void TestSetup()
        {
            _membershipServiceMock = new Mock<IMembershipService>();
            _orderManagementServiceMock = new Mock<IOrderManagementService>();
            new OrderManagementQueryHandlers(_membershipServiceMock.Object, _orderManagementServiceMock.Object);
        }

        [TestMethod]
        [TestCategory(TestCategories.OrderManagementCommandHandlers)]
        public void AddOrderRowCommandHandlerCallsCreateOrderRow()
        {
            //  Arrange
            PopulateFields();

            _orderManagementServiceMock.Setup(
                o => o.CreateOrderRow(_webinar, _additionalLocations, _idRegType))
                .Returns(new OrderRow()).Verifiable();


            var orderManagementCommandHandler = new OrderManagementCommandHandlers(_orderManagementServiceMock.Object,_membershipServiceMock.Object, new PostCommitRegistrator());

            //  Act
            orderManagementCommandHandler.Handle(_addOrderRowCommand);

            //  Assert
            _orderManagementServiceMock.Verify();
        }
        
        [TestMethod]
        [TestCategory(TestCategories.OrderManagementCommandHandlers)]
        public void AddOrderRowCommandHandlerCallsGetDiscount()
        {
            //  Arrange
            PopulateFields();

            _orderManagementServiceMock.Setup(
                o => o.CreateOrderRow(_webinar, _additionalLocations, _idRegType))
                .Returns(new OrderRow());
            _orderManagementServiceMock.Setup(o => o.GetDiscount(email)).Verifiable();


            var orderManagementCommandHandler = new OrderManagementCommandHandlers(_orderManagementServiceMock.Object,_membershipServiceMock.Object, new PostCommitRegistrator());

            //  Act
            orderManagementCommandHandler.Handle(_addOrderRowCommand);

            //  Assert
            _orderManagementServiceMock.Verify();
        }

        [TestMethod]
        [TestCategory(TestCategories.OrderManagementCommandHandlers)]
        public void AddOrderRowCommandHandlerCreateOrderRowOutParameter()
        {
            //  Arrange
            PopulateFields();

            _orderManagementServiceMock.Setup(o => o.CreateOrderRow(_webinar, _additionalLocations, _idRegType))
                .Returns(new OrderRow());

            var orderManagementCommandHandler = new OrderManagementCommandHandlers(_orderManagementServiceMock.Object,_membershipServiceMock.Object, new PostCommitRegistrator());

            //  Act
            orderManagementCommandHandler.Handle(_addOrderRowCommand);

            //  Assert
            Assert.IsNotNull(_addOrderRowCommand.OrderRow);
        }

        [TestMethod]
        [TestCategory(TestCategories.OrderManagementCommandHandlers)]
        public void AddOrderRowCommandHandlerThrowsExceptionWhenPassedNullReference()
        {
            //  Arrange
            AddOrderRowCommand addOrderCommand = null;
            var orderManagementCommandHandler = new OrderManagementCommandHandlers(_orderManagementServiceMock.Object,_membershipServiceMock.Object, new PostCommitRegistrator());

            //  Act
            //  Assert                        
            ExceptionAssert.Throws<ArgumentNullException>(
                () => orderManagementCommandHandler.Handle(addOrderCommand)
                );
        }

        [TestMethod]
        [TestCategory(TestCategories.OrderManagementCommandHandlers)]
        public void RegisterNewAccountCommandInvokesProcessInstitutionForUser()
        {
            //  Arrange
            string password = BusinessTestHelper.GetRandomString(8);
            var addresses = BusinessTestHelper.GetAddresses(fullName);

            var institution = new Institution {idInstitution = 25};
            var registerNewAccountCommand = new RegisterNewAccountCommand
            {
                BillingAddress = addresses[0],
                Email = email,
                FirstName = _firstName,
                LastName = _lastName,
                Institution = acmeInc,
                ShippingAddress = addresses[1],
                TempPassword = password,
                Tenant = cuwebinars,
                Title = null
            };

            var userAccountService =
                new UserAccountServiceHappyPathFake(new DefaultUserAccountRepository());

            var userAccount = userAccountService.CreateAccount(cuwebinars, password, email);

            _membershipServiceMock.Setup(
                m =>
                    m.ProcessInstitutionForUser(acmeInc, email, registerNewAccountCommand.BillingAddress.City,
                        registerNewAccountCommand.BillingAddress.State, "N", "New",
                        registerNewAccountCommand.BillingAddress.Zip))
                        .Returns(institution)
                        .Verifiable();
            
            _membershipServiceMock.Setup(m => m.GetTimeZoneByZip()).Returns(USTimeZone.Central);


            _membershipServiceMock.Setup(m => m.CreateUser(cuwebinars, _firstName, _lastName, email, password, email))
                .Returns(userAccount);

            var orderManagementCommandHandler = new OrderManagementCommandHandlers(
                _orderManagementServiceMock.Object, 
                _membershipServiceMock.Object, 
                new PostCommitRegistrator()
                );

            //  Act
            orderManagementCommandHandler.Handle(registerNewAccountCommand);

            //  Assert                        
            _membershipServiceMock.VerifyAll();
        }

        [TestMethod]
        [TestCategory(TestCategories.OrderManagementCommandHandlers)]
        public void RegisterNewAccountCommandInvokesGetTimeZoneByZip()
        {
            //  Arrange
            
            string password = BusinessTestHelper.GetRandomString(8);
            var addresses = BusinessTestHelper.GetAddresses(fullName);

            var institution = new Institution { idInstitution = 25 };
            var registerNewAccountCommand = new RegisterNewAccountCommand
            {
                BillingAddress = addresses[0],
                Email = email,
                FirstName = _firstName,
                LastName = _lastName,
                Institution = acmeInc,
                ShippingAddress = addresses[1],
                TempPassword = password,
                Tenant = cuwebinars,
                Title = null
            };

            var userAccountService =
                new UserAccountServiceHappyPathFake(new DefaultUserAccountRepository());

            var userAccount = userAccountService.CreateAccount(cuwebinars, password, email);

            _membershipServiceMock.Setup(
                m =>
                    m.ProcessInstitutionForUser(acmeInc, email, registerNewAccountCommand.BillingAddress.City,
                        registerNewAccountCommand.BillingAddress.State, "N", "New",
                        registerNewAccountCommand.BillingAddress.Zip))
                        .Returns(institution);

            _membershipServiceMock.Setup(m => m.GetTimeZoneByZip()).Returns(USTimeZone.Central)
                .Verifiable();


            _membershipServiceMock.Setup(m => m.CreateUser(cuwebinars, _firstName, _lastName, email, password, email))
                .Returns(userAccount);

            var orderManagementCommandHandler = new OrderManagementCommandHandlers(
                _orderManagementServiceMock.Object,
                _membershipServiceMock.Object,
                new PostCommitRegistrator()
                );

            //  Act
            orderManagementCommandHandler.Handle(registerNewAccountCommand);

            //  Assert                        
            _membershipServiceMock.Verify();
        }
        
        [TestMethod]
        [TestCategory(TestCategories.OrderManagementCommandHandlers)]
        public void RegisterNewAccountCommandInvokesCreateWebUser()
        {
            //  Arrange
            
            string password = BusinessTestHelper.GetRandomString(8);
            var addresses = BusinessTestHelper.GetAddresses(fullName);
            var timeZone = USTimeZone.Central;

            var institution = new Institution { idInstitution = 25 };
            var registerNewAccountCommand = new RegisterNewAccountCommand
            {
                BillingAddress = addresses[0],
                Email = email,
                FirstName = _firstName,
                LastName = _lastName,
                Institution = acmeInc,
                ShippingAddress = addresses[1],
                TempPassword = password,
                Tenant = cuwebinars,
                Title = null
            };

            var userAccountService =
                new UserAccountServiceHappyPathFake(new DefaultUserAccountRepository());

            var userAccount = userAccountService.CreateAccount(cuwebinars, password, email);

            _membershipServiceMock.Setup(
                m =>
                    m.ProcessInstitutionForUser(acmeInc, email, registerNewAccountCommand.BillingAddress.City,
                        registerNewAccountCommand.BillingAddress.State, "N", "New",
                        registerNewAccountCommand.BillingAddress.Zip))
                        .Returns(institution);

            _membershipServiceMock.Setup(m => m.GetTimeZoneByZip()).Returns(timeZone);

            _membershipServiceMock.Setup(m => m.CreateWebUser(cuwebinars, _firstName, _lastName, password, email, timeZone, UserType.Customer, institution.idInstitution, addresses,null,null, "A")).Returns(new WebUser()).Verifiable();


            _membershipServiceMock.Setup(m => m.CreateUser(cuwebinars, _firstName, _lastName, email, password, email))
                .Returns(userAccount);

            var orderManagementCommandHandler = new OrderManagementCommandHandlers(
                _orderManagementServiceMock.Object,
                _membershipServiceMock.Object,
                new PostCommitRegistrator()
                );

            //  Act
            orderManagementCommandHandler.Handle(registerNewAccountCommand);

            //  Assert                        
            _membershipServiceMock.Verify();
        }
        
        [TestMethod]
        [TestCategory(TestCategories.OrderManagementCommandHandlers)]
        public void RegisterNewAccountCommandInvokesCreateUser()
        {
            //  Arrange
            
            string password = BusinessTestHelper.GetRandomString(8);
            var addresses = BusinessTestHelper.GetAddresses(fullName);
            var timeZone = USTimeZone.Central;

            var institution = new Institution { idInstitution = 25 };
            var registerNewAccountCommand = new RegisterNewAccountCommand
            {
                BillingAddress = addresses[0],
                Email = email,
                FirstName = _firstName,
                LastName = _lastName,
                Institution = acmeInc,
                ShippingAddress = addresses[1],
                TempPassword = password,
                Tenant = cuwebinars,
                Title = null
            };

            var userAccountService =
                new UserAccountServiceHappyPathFake(new DefaultUserAccountRepository());

            var userAccount = userAccountService.CreateAccount(cuwebinars, password, email);

            _membershipServiceMock.Setup(
                m =>
                    m.ProcessInstitutionForUser(acmeInc, email, registerNewAccountCommand.BillingAddress.City,
                        registerNewAccountCommand.BillingAddress.State, "N", "New",
                        registerNewAccountCommand.BillingAddress.Zip))
                        .Returns(institution);

            _membershipServiceMock.Setup(m => m.GetTimeZoneByZip()).Returns(timeZone);

            _membershipServiceMock.Setup(m => m.CreateUser(cuwebinars, _firstName, _lastName, email, password, email))
                .Returns(userAccount)
                .Verifiable();

            var orderManagementCommandHandler = new OrderManagementCommandHandlers(
                _orderManagementServiceMock.Object,
                _membershipServiceMock.Object,
                new PostCommitRegistrator()
                );

            //  Act
            orderManagementCommandHandler.Handle(registerNewAccountCommand);

            //  Assert                        
            _membershipServiceMock.Verify();
        }


        [TestMethod]
        [TestCategory(TestCategories.OrderManagementCommandHandlers)]
        public void RegisterNewAccountCommandThrowsExceptionWhenPassedNullReference()
        {
            RegisterNewAccountCommand registerNewAccountCommand = null;

            var orderManagementCommandHandler = new OrderManagementCommandHandlers(
                _orderManagementServiceMock.Object,
                _membershipServiceMock.Object,
                new PostCommitRegistrator()
                );


            ExceptionAssert.Throws<ArgumentNullException>(
                () => orderManagementCommandHandler.Handle(registerNewAccountCommand)
                );
        }

        [TestMethod]
        [TestCategory(TestCategories.OrderManagementCommandHandlers)]
        public void VerifyAccountCommandCallsVerifyEmailFromKey()
        {
            //  Arrange
            var command = new VerifyAccountCommand
            {
                TempPassword = BusinessTestHelper.GetRandomString(8),
                VerificationKey = BusinessTestHelper.GetRandomString(8)

            };

            _membershipServiceMock.Setup(m => m.VerifyEmailFromKey(command.VerificationKey, command.TempPassword))
                .Verifiable();

            var orderManagementCommandHandler = new OrderManagementCommandHandlers(
                _orderManagementServiceMock.Object,
                _membershipServiceMock.Object,
                new PostCommitRegistrator()
                );

            //  Act
            orderManagementCommandHandler.Handle(command);

            //  Assert                        
            _membershipServiceMock.Verify();
        }

        [TestMethod]
        [TestCategory(TestCategories.OrderManagementCommandHandlers)]
        public void VerifyAccountCommandThrowsExceptionWhenPassedNullReference()
        {
            //  Arrange
            VerifyAccountCommand command = null;

            var orderManagementCommandHandler = new OrderManagementCommandHandlers(
                _orderManagementServiceMock.Object,
                _membershipServiceMock.Object,
                new PostCommitRegistrator()
                );


            //  Act
            //  Assert                        
            ExceptionAssert.Throws<ArgumentNullException>(
                () => orderManagementCommandHandler.Handle(command)
                );
        }

        [TestMethod]
        [TestCategory(TestCategories.OrderManagementCommandHandlers)]
        public void AddOrderCommandInvokesCreateNewOrder()
        {
            //  Arrange
            var newOrder = new Order();
            PopulateFields();
            _orderManagementServiceMock.Setup(o =>
                o.CreateNewOrder(_addOrderCommand.Affiliate, _addOrderCommand.WebUser, _addOrderCommand.Webinar,
                    _addOrderCommand.OrderRow))
                    .Returns(newOrder)
                    .Verifiable();


            var orderManagementCommandHandlers = new OrderManagementCommandHandlers(
                _orderManagementServiceMock.Object,
                _membershipServiceMock.Object,
                new PostCommitRegistrator()
                );

            //  Act
            orderManagementCommandHandlers.Handle(_addOrderCommand);

            //  Assert                        
            _orderManagementServiceMock.Verify();
        }
        
        [TestMethod]
        [TestCategory(TestCategories.OrderManagementCommandHandlers)]
        public void AddOrderCommandInvokesSaveOrderChanges()
        {
            //  Arrange
            var newOrder = new Order();
            PopulateFields();
            _orderManagementServiceMock.Setup(o =>
                o.CreateNewOrder(_addOrderCommand.Affiliate, _addOrderCommand.WebUser, _addOrderCommand.Webinar,
                    _addOrderCommand.OrderRow))
                    .Returns(newOrder);

            _orderManagementServiceMock.Setup(
                o => o.SaveOrderChanges(newOrder, _addOrderCommand.VerificationKey, _addOrderCommand.ConfirmChangeEmailUrl))
                .Verifiable();


            var orderManagementCommandHandlers = new OrderManagementCommandHandlers(
                _orderManagementServiceMock.Object,
                _membershipServiceMock.Object,
                new PostCommitRegistrator()
                );

            //  Act
            orderManagementCommandHandlers.Handle(_addOrderCommand);

            //  Assert                        
            _orderManagementServiceMock.Verify();
        }
        
        [TestMethod]
        [TestCategory(TestCategories.OrderManagementCommandHandlers)]
        public void AddOrderCommandAssignsValueToOutParameterOfCommand()
        {
            //  Arrange
            var newOrder = new Order{ idOrder = 1001};
            PopulateFields();
            _orderManagementServiceMock.Setup(o =>
                o.CreateNewOrder(_addOrderCommand.Affiliate, _addOrderCommand.WebUser, _addOrderCommand.Webinar,
                    _addOrderCommand.OrderRow)).Returns(newOrder);
            
            var orderManagementCommandHandler = new OrderManagementCommandHandlers(
                _orderManagementServiceMock.Object,
                _membershipServiceMock.Object,
                new PostCommitRegistrator()
                );

            //  Act
            orderManagementCommandHandler.Handle(_addOrderCommand);

            //  Assert                        
            Assert.AreEqual(_addOrderCommand.OrderId, newOrder.idOrder);
        }

        [TestMethod]
        [TestCategory(TestCategories.OrderManagementCommandHandlers)]
        public void AddOrderCommandThrowsExceptionWhenPassedNullReference()
        {
            //  Arrange
            AddOrderCommand addOrderCommand = null;
            var orderManagementCommandHandler = new OrderManagementCommandHandlers(
                _orderManagementServiceMock.Object,
                _membershipServiceMock.Object,
                new PostCommitRegistrator()
                );

            //  Act
            //  Assert                        
            ExceptionAssert.Throws<ArgumentNullException>(
                () => orderManagementCommandHandler.Handle(addOrderCommand)
                );
        }

        private void PopulateFields()
        {
            _webinar = new Webinar();
            _idRegType = 17;
            _incomingAdditionalLocation = null;
            _additionalLocations = new List<AdditionalLocation>();

            _addOrderRowCommand = new AddOrderRowCommand
            {
                AdditionalLocations = _incomingAdditionalLocation,
                Email = email,
                RegistrationType = _idRegType,
                Webinar = _webinar
            };
            const string firstName = _firstName;
            const string lastName = _lastName;
            var addresses = BusinessTestHelper.GetAddresses(string.Concat(firstName, " ", lastName));

            _addOrderCommand = new AddOrderCommand
            {
                Affiliate = new Affiliate(),
                AffiliateComments = string.Empty,
                BillingAddress = addresses[0],
                ConfirmChangeEmailUrl = string.Empty,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                OrderRow = new OrderRow(),
                ShippingAddress = addresses[1],
                VerificationKey = BusinessTestHelper.GetRandomString(8),
                WebUser = new WebUser{ Institution = new Institution{ idInstitution = 16}},
                Webinar = new Webinar()
            };
        }
    }
}