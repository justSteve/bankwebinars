using CUWebinars.Business.Core;
using CUWebinars.Business.Models;
using CUWebinars.Business.Repository;
using CUWebinars.Business.Services;
using CUWebinars.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Ninject.Extensions.Logging;
using Ninject.Extensions.Logging.Log4net.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;

namespace CUWebinars.Business.Tests.UnitTests
{
    [TestClass]
    public class OrderManagementServiceTests
    {
        private OrderManagementService _orderManagementService;
        private Mock<IAdditionalLocationsRepository> _additionalLocationsRepositoryMock;
        private Mock<IRefDataRepository> _refDataRepositoryMock;
        private Mock<IWebUserRepository> _webUserRepositoryMock;
        private Mock<IAffiliateRepository> _affiliateRepository;
        private Mock<IRegTypeRepository> _regTypeRepository;
        private Mock<IOrderRepository> _orderRepository;
        private Mock<IWebinarRepository> _webinarRepository;
        private ILogger logger = new Log4NetLogger(typeof (OrderManagementService));

        private TtsConfiguration ttsConfig =
            TtsConfig.Create(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, TestConstants.UpTwoFolders), Globals.UseAzureWebjobs, Globals.StorageAccountName, Globals.StorageAccessKey);

        [TestInitialize]
        public void Setup()
        {
            _additionalLocationsRepositoryMock = new Mock<IAdditionalLocationsRepository>();
            _refDataRepositoryMock = new Mock<IRefDataRepository>();
            _webinarRepository = new Mock<IWebinarRepository>();
            _webUserRepositoryMock = new Mock<IWebUserRepository>();
            _affiliateRepository = new Mock<IAffiliateRepository>();
            _regTypeRepository = new Mock<IRegTypeRepository>();
            _orderRepository = new Mock<IOrderRepository>();

            _orderManagementService = new OrderManagementService(_affiliateRepository.Object,
                _regTypeRepository.Object,
                _orderRepository.Object,
                _refDataRepositoryMock.Object,
                _webUserRepositoryMock.Object,
                _webinarRepository.Object,
                _additionalLocationsRepositoryMock.Object,
                logger,
                ttsConfig);
        }


        [TestMethod]
        [TestCategory(TestCategories.Services)]
        public void CreateOrderRowInvokesFindRegType()
        {
            //  Arrange
            _regTypeRepository.Setup(rt => rt.FindRegType(It.IsAny<int>())).Verifiable();

            //  Act
            _orderManagementService.CreateOrderRow(new Webinar(), new List<AdditionalLocation>(), 5);

            //  Assert                        
            _regTypeRepository.Verify();

        }

        [TestMethod]
        [TestCategory(TestCategories.Services)]
        public void CreateOrderRowInvokesCreateOrderRowOnRepository()
        {
            //  Arrange
            _orderRepository.Setup(rt => rt.CreateOrderRow(
                It.IsAny<Webinar>(), 
                It.IsAny<IList<AdditionalLocation>>(), 
                It.IsAny<RegType>()))
                .Verifiable();

            //  Act
            _orderManagementService.CreateOrderRow(new Webinar(), new List<AdditionalLocation>(), 5);

            //  Assert                        
            _orderRepository.Verify();

        }

        [TestMethod]
        [TestCategory(TestCategories.Services)]
        public void AssignUserToOrderInvokesAssignWebUserToOrderOfOrderRepository()
        {
            //  Arrange
            var order = new Order();

            WebUser aWebUser = new WebUser
            {
                email = "avalidemailaddress@test.com",
                FirstName = "John",
                Institution = new Institution {InstitutionName = "ACME Inc"},
                LastName = "Hancock",
                Addresses = BusinessTestHelper.GetAddresses("John Hanckcock")
            };

            //  All we can test is whether the AssignWebUserToOrder method is invoked.
            //  An integration test (elsewhere) could ensure that is working.
            _orderRepository.Setup(r => r.AssignWebUserToOrder(aWebUser, order)).Verifiable();

            //  Act 
            _orderManagementService.AssignWebUserToOrder(aWebUser, order);

            //  Assert                        
            _orderRepository.Verify();
        }

        [TestMethod]
        [TestCategory(TestCategories.Services)]
        public void AssignUserToOrderThrowsExceptionWhenPassedNullReference()
        {
            //  Arrange
            Order order = null;
            WebUser user = null;
            //  Act
            //  Assert                        
            ExceptionAssert.Throws<ArgumentNullException>(
                () => _orderManagementService.AssignWebUserToOrder(user, order)
                );
        }
    }
}