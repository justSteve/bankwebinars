using CUWebinars.Business.AccountService;
using CUWebinars.Business.Models;
using CUWebinars.Business.Services;
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

    }
}
