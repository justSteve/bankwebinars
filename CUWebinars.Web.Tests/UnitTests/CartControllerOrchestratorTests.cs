using System;
using System.Collections.Generic;
using CUWebinars.Business.AccountService;
using CUWebinars.Business.Models;
using CUWebinars.Business.Services;
using CUWebinars.Tests.Common;
using CUWebinars.Web.Core;
using CUWebinars.Web.Core.Orchestrators;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Services;
using CUWebinars.Web.Tests.Infrastructure;
using CUWebinars.Web.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Ninject.Extensions.Logging;

namespace CUWebinars.Web.Tests.UnitTests
{
    [TestClass]
    public class CartControllerOrchestratorTests
    {
        private ICartControllerOrchestrator _cartControllerOrchestrator;
        private Mock<IAppHelper> _appHelperMock = new Mock<IAppHelper>();
        private Mock<IMembershipService> _membershipServiceMock = new Mock<IMembershipService>();
        private Mock<IOrderManagementService> _orderManagementServiceMock = new Mock<IOrderManagementService>();
        private Mock<IWebinarManagementService> _webinarManagementService = new Mock<IWebinarManagementService>();
        private Mock<ILogger> _loggerMock = new Mock<ILogger>();
        private Mock<IStateService> _stateServiceMock = new Mock<IStateService>();

        private GlobalConfig _globals = GlobalConfig.GlobalConfigSingleton;
        private WebTestsGlobalConfig _webTestsGlobals = WebTestsGlobalConfig.WebTestsGlobalConfigSingleton;

        [TestMethod]
        [TestCategory(TestCategories.Orchestrators)]
        public void BuildRegisterViewModelReturnsRegisterViewModel()
        {
            //  Arrange
            _cartControllerOrchestrator = new CartControllerOrchestrator(
                _membershipServiceMock.Object,
                _orderManagementServiceMock.Object,
                _webinarManagementService.Object,
                _stateServiceMock.Object,
                _loggerMock.Object,
                WebTestHelpers.GetMockedHttpContext().Request,
                _appHelperMock.Object
                );

            //  Act
            var model = _cartControllerOrchestrator.BuildRegisterViewModel();

            //  Assert                        
            Assert.IsInstanceOfType(model, typeof (RegisterViewModel));
        }

        [TestMethod]
        [TestCategory(TestCategories.Orchestrators)]
        public void BuildDisplayOptionsInDropDownViewModelInvokesGetOrderRowByIdWhereOrderRowParameterIsNull()
        {
            //  Arrange
            const int idOrderRow = 3453;

            var orderRow = new OrderRow
            {
                idWebinar = 5656,
                RegistrationType = new RegType()
            };

            _orderManagementServiceMock.Setup(i => i.GetOrderRowById(It.IsAny<int>())).Returns(orderRow).Verifiable();

            _cartControllerOrchestrator = new CartControllerOrchestrator(
                _membershipServiceMock.Object,
                _orderManagementServiceMock.Object,
                _webinarManagementService.Object,
                _stateServiceMock.Object,
                _loggerMock.Object,
                WebTestHelpers.GetMockedHttpContext().Request,
                _appHelperMock.Object
                );

            //  Act
            _cartControllerOrchestrator.BuildDisplayOptionsInDropDownViewModel(null, idOrderRow);

            //  Assert                        
            _orderManagementServiceMock.Verify();
        }

        [TestMethod]
        [TestCategory(TestCategories.Orchestrators)]
        public void BuildDisplayOptionsInDropDownViewModelReturnsModelWhereOrderRowParameterIsNull()
        {
            //  Arrange
            const int idOrderRow = 3453;


            var orderRow = new OrderRow
            {
                idWebinar = 5656,
                RegistrationType = new RegType()
            };

            _orderManagementServiceMock.Setup(i => i.GetOrderRowById(It.IsAny<int>())).Returns(orderRow);


            _cartControllerOrchestrator = new CartControllerOrchestrator(
                _membershipServiceMock.Object,
                _orderManagementServiceMock.Object,
                _webinarManagementService.Object,
                _stateServiceMock.Object,
                _loggerMock.Object,
                WebTestHelpers.GetMockedHttpContext().Request,
                _appHelperMock.Object
                );

            //  Act
            var buildDisplayOptionsInDropDownViewModel = _cartControllerOrchestrator.BuildDisplayOptionsInDropDownViewModel(orderRow, idOrderRow);

            //  Assert                        
            Assert.AreEqual(buildDisplayOptionsInDropDownViewModel.OrderRowId, idOrderRow);
        }


        [TestMethod]
        [TestCategory(TestCategories.Orchestrators)]
        public void BuildDisplayOptionsInDropDownViewModelInvokesGetOptionsByWebinarId()
        {
            //  Arrange
            const int idOrderRow = 3453;


            var orderRow = new OrderRow
            {
                idWebinar = 5656,
                RegistrationType = new RegType()
            };

            var options = 

            _orderManagementServiceMock.Setup(i => i.GetOrderRowById(It.IsAny<int>())).Returns(orderRow);
            _orderManagementServiceMock.Setup(i => i.GetOptionsByWebinarId(It.IsAny<int>(), true)).Verifiable();

            _cartControllerOrchestrator = new CartControllerOrchestrator(
                _membershipServiceMock.Object,
                _orderManagementServiceMock.Object,
                _webinarManagementService.Object,
                _stateServiceMock.Object,
                _loggerMock.Object,
                WebTestHelpers.GetMockedHttpContext().Request,
                _appHelperMock.Object
                );

            //  Act
            _cartControllerOrchestrator.BuildDisplayOptionsInDropDownViewModel(null, idOrderRow);

            //  Assert                        
            _orderManagementServiceMock.Verify();
        }

        [TestMethod]
        [TestCategory(TestCategories.Orchestrators)]
        public void BuildDisplayOptionsInDropDownViewModelReturnsNullWhereidOrderRowIsNull()
        {
            //  Arrange
            int? idOrderRow = null;

            var orderRow = new OrderRow
            {
                idWebinar = 5656,
                RegistrationType = new RegType()
            };

            _cartControllerOrchestrator = new CartControllerOrchestrator(
                _membershipServiceMock.Object,
                _orderManagementServiceMock.Object,
                _webinarManagementService.Object,
                _stateServiceMock.Object,
                _loggerMock.Object,
                WebTestHelpers.GetMockedHttpContext().Request,
                _appHelperMock.Object
                );

            //  Act
            var buildDisplayOptionsInDropDownViewModel = _cartControllerOrchestrator.BuildDisplayOptionsInDropDownViewModel(orderRow, idOrderRow);

            //  Assert                        
            Assert.IsNull(buildDisplayOptionsInDropDownViewModel);
        }

        [TestMethod]
        [TestCategory(TestCategories.Orchestrators)]
        public void BuildDisplayOptionsInDropDownViewModelReturnsNullWhereidOrderRowIsLessThanOne()
        {
            //  Arrange
            int? idOrderRow = -2;

            var orderRow = new OrderRow
            {
                idWebinar = 5656,
                RegistrationType = new RegType()
            };

            _cartControllerOrchestrator = new CartControllerOrchestrator(
                _membershipServiceMock.Object,
                _orderManagementServiceMock.Object,
                _webinarManagementService.Object,
                _stateServiceMock.Object,
                _loggerMock.Object,
                WebTestHelpers.GetMockedHttpContext().Request,
                _appHelperMock.Object
                );

            //  Act
            var buildDisplayOptionsInDropDownViewModel = _cartControllerOrchestrator.BuildDisplayOptionsInDropDownViewModel(orderRow, idOrderRow);

            //  Assert                        
            Assert.IsNull(buildDisplayOptionsInDropDownViewModel);
        } 
        
        [TestMethod]
        [TestCategory(TestCategories.Orchestrators)]
        public void BuildDisplayOptionsInDropDownViewModelReturnsNullWhereidOrderRowIsZero()
        {
            //  Arrange
            int? idOrderRow = 0;

            var orderRow = new OrderRow
            {
                idWebinar = 5656,
                RegistrationType = new RegType()
            };

            _cartControllerOrchestrator = new CartControllerOrchestrator(
                _membershipServiceMock.Object,
                _orderManagementServiceMock.Object,
                _webinarManagementService.Object,
                _stateServiceMock.Object,
                _loggerMock.Object,
                WebTestHelpers.GetMockedHttpContext().Request,
                _appHelperMock.Object
                );

            //  Act
            var buildDisplayOptionsInDropDownViewModel = _cartControllerOrchestrator.BuildDisplayOptionsInDropDownViewModel(orderRow, idOrderRow);

            //  Assert                        
            Assert.IsNull(buildDisplayOptionsInDropDownViewModel);
        }


        [TestMethod]
        [TestCategory(TestCategories.Orchestrators)]
        public void BuildCheckoutConfirmViewModelInvokesGetOrderRowById()
        {
            //  Arrange
            const int idOrderRow = 3453;
            Tuple<string, decimal> addressesAndAddLocsPrice = new Tuple<string, decimal>("bla@bla.com", 75.00M);

            var orderRow = new OrderRow
            {
                AdditionalLocation = new List<AdditionalLocation>
                {
                    new AdditionalLocation()
                },
                idWebinar = 5656,
                Order = new Order
                {
                    WebUser = new WebUser
                    {
                        Addresses = TestHelper.GetAddresses("Terry Halpin"),
                        FirstName = "Terry", LastName = "Halpin"
                    }
                },
                RegistrationType = new RegType()
            };

            _orderManagementServiceMock.Setup(i => i.GetOrderRowById(It.IsAny<int>())).Returns(orderRow).Verifiable();
            _orderManagementServiceMock.Setup(i => i.GetCostOfAdditionalLocations(It.IsAny<IEnumerable<AdditionalLocation>>(),orderRow.idWebinar)).Returns(addressesAndAddLocsPrice);


            _cartControllerOrchestrator = new CartControllerOrchestrator(
                _membershipServiceMock.Object,
                _orderManagementServiceMock.Object,
                _webinarManagementService.Object,
                _stateServiceMock.Object,
                _loggerMock.Object,
                WebTestHelpers.GetMockedHttpContext().Request,
                _appHelperMock.Object
                );

            //  Act
            _cartControllerOrchestrator.BuildCheckoutConfirmViewModel(idOrderRow);

            //  Assert                        
            _orderManagementServiceMock.Verify();
        }

        [TestMethod]
        [TestCategory(TestCategories.Orchestrators)]
        public void BuildCheckoutConfirmViewModelAssignsOriginWhereReferrerNotNull()
        { 
            //  Arrange
            const int idOrderRow = 3453;
            Tuple<string, decimal> addressesAndAddLocsPrice = new Tuple<string, decimal>("bla@bla.com", 75.00M);

            var orderRow = new OrderRow
            {
                AdditionalLocation = new List<AdditionalLocation>
                {
                    new AdditionalLocation()
                },
                idWebinar = 5656,
                Order = new Order
                {
                    Origin = string.Empty,
                    WebUser = new WebUser
                    {
                        Addresses = TestHelper.GetAddresses("Terry Halpin"),
                        FirstName = "Terry", LastName = "Halpin"
                    }
                },
                RegistrationType = new RegType()
            };

            _orderManagementServiceMock.Setup(i => i.GetOrderRowById(It.IsAny<int>())).Returns(orderRow);
            _orderManagementServiceMock.Setup(i => i.GetCostOfAdditionalLocations(It.IsAny<IEnumerable<AdditionalLocation>>(),orderRow.idWebinar)).Returns(addressesAndAddLocsPrice);

            var request = WebTestHelpers.GetMockedHttpContext().Request;
            
            _cartControllerOrchestrator = new CartControllerOrchestrator(
                _membershipServiceMock.Object,
                _orderManagementServiceMock.Object,
                _webinarManagementService.Object,
                _stateServiceMock.Object,
                _loggerMock.Object,
                WebTestHelpers.GetMockedHttpContext().Request,
                _appHelperMock.Object
                );

            //  Act
            _cartControllerOrchestrator.BuildCheckoutConfirmViewModel(idOrderRow);

            //  Assert                        

            //  Note: referrer is set in the GetMockedHttpContext method of the WebTestHelpers class
            Assert.AreEqual(orderRow.Order.Origin, string.Format("{0}{1}", WebTestHelpers.ReferrerAddress, Environment.NewLine));
        }

        [TestMethod]
        [TestCategory(TestCategories.Orchestrators)]
        public void BuildCheckoutConfirmViewModelSetsOrderRowHasIdToTrueWhere()
        {
            //  Arrange
            const int idOrderRow = 3453;
            Tuple<string, decimal> addressesAndAddLocsPrice = new Tuple<string, decimal>("bla@bla.com", 75.00M);

            var orderRow = new OrderRow
            {
                AdditionalLocation = new List<AdditionalLocation>
                {
                    new AdditionalLocation()
                },
                idOrderRow = idOrderRow,
                idWebinar = 5656,
                Order = new Order
                {
                    Origin = string.Empty,
                    WebUser = new WebUser
                    {
                        Addresses = TestHelper.GetAddresses("Terry Halpin"),
                        FirstName = "Terry",
                        LastName = "Halpin"
                    }
                },
                RegistrationType = new RegType()
            };

            _orderManagementServiceMock.Setup(i => i.GetOrderRowById(idOrderRow)).Returns(orderRow);
            _orderManagementServiceMock.Setup(i => i.GetCostOfAdditionalLocations(It.IsAny<IEnumerable<AdditionalLocation>>(), orderRow.idWebinar)).Returns(addressesAndAddLocsPrice);

            _cartControllerOrchestrator = new CartControllerOrchestrator(
                _membershipServiceMock.Object,
                _orderManagementServiceMock.Object,
                _webinarManagementService.Object,
                _stateServiceMock.Object,
                _loggerMock.Object,
                WebTestHelpers.GetMockedHttpContext().Request,
                _appHelperMock.Object
                );

            //  Act
            var result = _cartControllerOrchestrator.BuildCheckoutConfirmViewModel(idOrderRow);

            //  Assert                        
            Assert.IsTrue(result.OrderRowHasId);
        }

        [TestMethod]
        [TestCategory(TestCategories.Orchestrators)]
        public void BuildCheckoutConfirmViewModelSetsOptionLabelToOptionLabelOfRegType()
        {
            //  Arrange
            const int idOrderRow = 3453;
            const string optionText = "Text goes here";
            Tuple<string, decimal> addressesAndAddLocsPrice = new Tuple<string, decimal>("bla@bla.com", 75.00M);

            var orderRow = new OrderRow
            {
                AdditionalLocation = new List<AdditionalLocation>
                {
                    new AdditionalLocation()
                },
                idOrderRow = idOrderRow,
                idWebinar = 5656,
                Order = new Order
                {
                    Origin = string.Empty,
                    WebUser = new WebUser
                    {
                        Addresses = TestHelper.GetAddresses("Terry Halpin"),
                        FirstName = "Terry",
                        LastName = "Halpin"
                    }
                },
                RegistrationType = new RegType
                {
                    OptionLabel = optionText
                }
            };

            _orderManagementServiceMock.Setup(i => i.GetOrderRowById(idOrderRow)).Returns(orderRow);
            _orderManagementServiceMock.Setup(i => i.GetCostOfAdditionalLocations(It.IsAny<IEnumerable<AdditionalLocation>>(), orderRow.idWebinar)).Returns(addressesAndAddLocsPrice);

            _cartControllerOrchestrator = new CartControllerOrchestrator(
                _membershipServiceMock.Object,
                _orderManagementServiceMock.Object,
                _webinarManagementService.Object,
                _stateServiceMock.Object,
                _loggerMock.Object,
                WebTestHelpers.GetMockedHttpContext().Request,
                _appHelperMock.Object
                );

            //  Act
            var result = _cartControllerOrchestrator.BuildCheckoutConfirmViewModel(idOrderRow);

            //  Assert                        
            Assert.IsTrue(result.OptionLabel.Equals(optionText, StringComparison.Ordinal));
        }

        [TestMethod]
        [TestCategory(TestCategories.Orchestrators)]
        public void BuildCheckoutConfirmViewModelSetsTextOfEditButtonWhereOrderInProcess()
        {
            //  Arrange
            const int idOrderRow = 3453;
            const string optionText = "Text goes here";
            Tuple<string, decimal> addressesAndAddLocsPrice = new Tuple<string, decimal>("bla@bla.com", 75.00M);

            var orderRow = new OrderRow
            {
                AdditionalLocation = new List<AdditionalLocation>
                {
                    new AdditionalLocation()
                },
                idOrderRow = idOrderRow,
                idWebinar = 5656,
                Order = new Order
                {
                    OrderStatus = OrderStatus.InProcess,
                    Origin = string.Empty,
                    WebUser = new WebUser
                    {
                        Addresses = TestHelper.GetAddresses("Terry Halpin"),
                        FirstName = "Terry",
                        LastName = "Halpin"
                    }
                },
                RegistrationType = new RegType
                {
                    OptionLabel = optionText
                }
            };

            _orderManagementServiceMock.Setup(i => i.GetOrderRowById(idOrderRow)).Returns(orderRow);
            _orderManagementServiceMock.Setup(i => i.GetCostOfAdditionalLocations(It.IsAny<IEnumerable<AdditionalLocation>>(), orderRow.idWebinar)).Returns(addressesAndAddLocsPrice);

            _cartControllerOrchestrator = new CartControllerOrchestrator(
                _membershipServiceMock.Object,
                _orderManagementServiceMock.Object,
                _webinarManagementService.Object,
                _stateServiceMock.Object,
                _loggerMock.Object,
                WebTestHelpers.GetMockedHttpContext().Request,
                _appHelperMock.Object
                );

            //  Act
            var result = _cartControllerOrchestrator.BuildCheckoutConfirmViewModel(idOrderRow);

            //  Assert                        
            Assert.IsTrue(result.UserDetails.EndsWith(
                    " - <a id='editUserDetails' role='button' class='btn btn-mini' target='new'> Edit?</a>",
                    StringComparison.Ordinal));
        }

    }
}
