using CUWebinars.Business.AccountService;
using CUWebinars.Business.Constants;
using CUWebinars.Business.Models;
using CUWebinars.Business.Services;
using CUWebinars.Tests.Common;
using CUWebinars.Web.Core;
using CUWebinars.Web.Core.Orchestrators;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Models;
using CUWebinars.Web.Services;
using CUWebinars.Web.Tests.Fakes;
using CUWebinars.Web.Tests.Infrastructure;
using CUWebinars.Web.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using ClaimTypes = System.IdentityModel.Claims.ClaimTypes;

namespace CUWebinars.Web.Tests.UnitTests
{
    [TestClass]
    public class AccountControllerOrchestratorTests
    {
        private const string AuditInfo = "Account.SignIn Post Success. Session=<AuditInfo><RemoteAddress>::1</RemoteAddress><RemoteHost>::1</RemoteHost><RemoteUser></RemoteUser><UserAgent>Mozilla/5.0 (Windows NT 6.1; WOW64; rv:33.0) Gecko/20100101 Firefox/33.0</UserAgent><Cookie>{0}</Cookie></AuditInfo>, Redirecting to: {1}";
        
        private IAccountControllerOrchestrator _accountControllerOrchestrator;
        private Mock<IAppHelper> _appHelperMock = new Mock<IAppHelper>();
        private Mock<IMembershipService> _membershipServiceMock = new Mock<IMembershipService>();
        private Mock<IOrderManagementService> _orderManagementServiceMock = new Mock<IOrderManagementService>();
        private Mock<ILogger> _loggerMock = new Mock<ILogger>();
        private Mock<IStateService> _stateServiceMock = new Mock<IStateService>();
        
        private GlobalConfig _globals = GlobalConfig.GlobalConfigSingleton;
        private WebTestsGlobalConfig _webTestsGlobals = WebTestsGlobalConfig.WebTestsGlobalConfigSingleton;


        [TestInitialize]
        public void SetUpTest()
        {

        }

        [TestMethod]
        public void BuildBillingAddressModelReturnsEditBillingAddressModel()
        {
            //  Arrange and Act
            var actualModel = GetEditBillingAddressModel();

            //  Assert                        
            Assert.IsInstanceOfType(actualModel, typeof(EditBillingAddressModel));
        }

        [TestMethod]
        public void BuildBillingAddressModelReturnsEditBillingAddressModelWithNonNullBillingAddress()
        {
            //  Arrange and Act
            var actualModel = GetEditBillingAddressModel();

            //  Assert                        
            Assert.IsNotNull(actualModel.BillingAddress);
        }

        [TestMethod]
        public void BuildBillingAddressModelReturnsEditBillingAddressModelWithAddressOfTypeBilling()
        {
            //  Arrange and Act
            var actualModel = GetEditBillingAddressModel();

            //  Assert                        
            Assert.AreEqual(actualModel.BillingAddress.TypeOfAddress.ToString(), DomainConstants.BillingAddress);
        }

        [TestMethod]
        public void SignUserInSignsInUser()
        {
            //  Arrange
            string userMustVerify;
            var signInModel = new SignInModel
            {
                Email = "dave@dave.com",
                Password = "gfhjdg",
                RememberMe = true,
                ReturnUrl = "/"
            };
            
            var request = new HttpRequestFake1();

            _appHelperMock.Setup(i => i.GetUserAuditInfo()).Returns(string.Format(AuditInfo, request.ServerVariables[TestConstants.HttpCookie], signInModel.ReturnUrl)
                );

            _membershipServiceMock.Setup(i => i.LogInUser("CUWebinars", signInModel.Email, signInModel.Password, signInModel.RememberMe, out userMustVerify, false))
                .Returns(true);

            _accountControllerOrchestrator = new AccountControllerOrchestrator(
                _loggerMock.Object,
                _membershipServiceMock.Object,
                _orderManagementServiceMock.Object,
                _stateServiceMock.Object,
                request,
                _appHelperMock.Object
                );

            //  Act
            var result = _accountControllerOrchestrator.SignUserIn(signInModel, out userMustVerify);

            //  Assert                        
            Assert.IsTrue(result);

        }

        [TestMethod]
        public void SignUserInSignsInUserAndConstructsReturnUrlForLogging()
        {
            //  Arrange
            string userMustVerify;
            var signInModel = new SignInModel
            {
                Email = "dave@dave.com",
                Password = "gfhjdg",
                RememberMe = true,
                ReturnUrl = "/"
            };
            
            var request = new HttpRequestFake1();

            var retUrl = signInModel.ReturnUrl.Replace(string.Format(@"{0}://{1}{2}/", request.Url.Scheme, request.Url.Authority, request.ApplicationPath.TrimEnd('/')),string.Empty);

            _appHelperMock.Setup(i => i.GetUserAuditInfo()).Returns(string.Format(AuditInfo, request.ServerVariables[TestConstants.HttpCookie], signInModel.ReturnUrl)
                );

            //  This test is testing that the returnUrl is re-constructed from the HttpRequest object and passed to the logger's Info method.
            _loggerMock.Setup(i => i.Info(It.IsAny<string>(), It.IsAny<string>(), retUrl)).Verifiable();

            _membershipServiceMock.Setup(i => i.LogInUser("CUWebinars", signInModel.Email, signInModel.Password, signInModel.RememberMe, out userMustVerify, false))
                .Returns(true);

            _accountControllerOrchestrator = new AccountControllerOrchestrator(
                _loggerMock.Object,
                _membershipServiceMock.Object,
                _orderManagementServiceMock.Object,
                _stateServiceMock.Object,
                request,
                _appHelperMock.Object
                );

            //  Act
            var result = _accountControllerOrchestrator.SignUserIn(signInModel, out userMustVerify);

            //  Assert                        
            _loggerMock.Verify();

        }

        [TestMethod]
        public void SignUserInThrowsNullReferenceExceptionWhereReturnUrlPropertyOnModelIsNull()
        {
            //  Arrange
            string userMustVerify;
            var signInModel = new SignInModel
            {
                Email = "dave@dave.com",
                Password = "gfhjdg",
                RememberMe = true,
                ReturnUrl = null
            };
            
            var request = new HttpRequestFake1();

            //  This test is testing that the returnUrl in the model cannot be null if the RequestContext's ApplicationPath and Url properties are not null.
            _membershipServiceMock.Setup(i => i.LogInUser("CUWebinars", signInModel.Email, signInModel.Password, signInModel.RememberMe, out userMustVerify, false))
                .Returns(true);

            _accountControllerOrchestrator = new AccountControllerOrchestrator(
                _loggerMock.Object,
                _membershipServiceMock.Object,
                _orderManagementServiceMock.Object,
                _stateServiceMock.Object,
                request,
                _appHelperMock.Object
                );

            //  Act
            //  Assert                        
            ExceptionAssert.Throws<NullReferenceException>(() => _accountControllerOrchestrator.SignUserIn(signInModel, out userMustVerify));
        }

        [TestMethod]
        public void SignUserInFailsToSignsInUserWhereInavlidCredentials()
        {
            //  Arrange
            string userMustVerify;
            var signInModel = new SignInModel
            {
                Email = "dave@dave.com",
                Password = "invalid",
                RememberMe = true,
                ReturnUrl = "/"
            };
            
            var request = new HttpRequestFake1();

            _appHelperMock.Setup(i => i.GetUserAuditInfo()).Returns(string.Format(AuditInfo, request.ServerVariables[TestConstants.HttpCookie], signInModel.ReturnUrl)
                );

            _membershipServiceMock.Setup(i => i.LogInUser("CUWebinars", signInModel.Email, signInModel.Password, signInModel.RememberMe, out userMustVerify, false))
                .Returns(false);

            _accountControllerOrchestrator = new AccountControllerOrchestrator(
                _loggerMock.Object,
                _membershipServiceMock.Object,
                _orderManagementServiceMock.Object,
                _stateServiceMock.Object,
                request,
                _appHelperMock.Object
                );

            //  Act
            var result = _accountControllerOrchestrator.SignUserIn(signInModel, out userMustVerify);

            //  Assert                        
            Assert.IsFalse(result);

        }

        [TestMethod]
        public void LogUserInReturnsTrueWhenSucceeds()
        {
            //  Arrange
            _membershipServiceMock.Setup(i => i.LogInUser("CUWebinars", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>() )).Returns(true);

            var signInModel = new SignInModel
            {
                Email = _webTestsGlobals.LoggedInUserEmail,
                Password = _webTestsGlobals.LoggedInUserPassword,
                RememberMe = false,
                SigninAfterCheckout = false
            };

            _accountControllerOrchestrator = new AccountControllerOrchestrator(
                _loggerMock.Object,
                _membershipServiceMock.Object,
                _orderManagementServiceMock.Object,
                _stateServiceMock.Object,
                WebTestHelpers.GetMockedHttpContext().Request,
                _appHelperMock.Object
                );

            //  Act
            var result = _accountControllerOrchestrator.LogUserIn(signInModel);
            
            //  Assert                        
            Assert.IsTrue(result);
        }
        
        [TestMethod]
        public void LogUserInReturnsTrueWhenFails()
        {
            //  Arrange
            _membershipServiceMock.Setup(i => i.LogInUser("CUWebinars", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>() )).Returns(false);

            var signInModel = new SignInModel
            {
                Email = _webTestsGlobals.LoggedInUserEmail,
                Password = _webTestsGlobals.LoggedInUserPassword,
                RememberMe = false,
                SigninAfterCheckout = false
            };

            _accountControllerOrchestrator = new AccountControllerOrchestrator(
                _loggerMock.Object,
                _membershipServiceMock.Object,
                _orderManagementServiceMock.Object,
                _stateServiceMock.Object,
                WebTestHelpers.GetMockedHttpContext().Request,
                _appHelperMock.Object
                );

            //  Act
            var result = _accountControllerOrchestrator.LogUserIn(signInModel);
            
            //  Assert                        
            Assert.IsFalse(result);
        }
        
        [TestMethod]
        public void UpdateNameTitleCallsUpdateNameTitleOfMembershipService()
        {
            //  Arrange
            _membershipServiceMock.Setup(i => i.UpdateNameTitle(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Verifiable();

            _accountControllerOrchestrator = new AccountControllerOrchestrator(
                _loggerMock.Object,
                _membershipServiceMock.Object,
                _orderManagementServiceMock.Object,
                _stateServiceMock.Object,
                WebTestHelpers.GetMockedHttpContext().Request,
                _appHelperMock.Object
                );

            //  Act
            _accountControllerOrchestrator.UpdateNameTitle("FName", "Lname", "email", "title");

            //  Assert                        
		    _membershipServiceMock.Verify();
        }

        [TestMethod]
        public void UpdateBillingEmailOfOrderCallsUpdateBillingEmailOfOrderOfOrderManagementService()
        {
            //  Arrange
            _orderManagementServiceMock.Setup(i => i.UpdateOrderWithUserEmail(It.IsAny<int>(), It.IsAny<string>())).Verifiable();

            //  Act
            _accountControllerOrchestrator = new AccountControllerOrchestrator(
                _loggerMock.Object,
                _membershipServiceMock.Object,
                _orderManagementServiceMock.Object,
                _stateServiceMock.Object,
                WebTestHelpers.GetMockedHttpContext().Request,
                _appHelperMock.Object
                );
            _accountControllerOrchestrator.UpdateBillingEmailOfOrder(5, _webTestsGlobals.LoggedInUserEmail);

            //  Assert                        
		    _orderManagementServiceMock.Verify();
        }

        [TestMethod]
        public void ResetPasswordCallsResetPasswordOfMembershipService()
        {
            //  Arrange
            _membershipServiceMock.Setup(i => i.ResetPassword(It.IsAny<string>(), It.IsAny<string>())).Verifiable();

            //  Act
            _accountControllerOrchestrator = new AccountControllerOrchestrator(
                _loggerMock.Object,
                _membershipServiceMock.Object,
                _orderManagementServiceMock.Object,
                _stateServiceMock.Object,
                WebTestHelpers.GetMockedHttpContext().Request,
                _appHelperMock.Object
                );

            _accountControllerOrchestrator.ResetPassword(_webTestsGlobals.Tenant, _webTestsGlobals.LoggedInUserEmail);

            //  Assert                        
		    _orderManagementServiceMock.Verify();
        }
        
        [TestMethod]
        public void RegisterAndLogInUserCallsProcessInstitutionForUserOfMembershipServiceMock()
        {
            //  Arrange
            var verificationKey = TestHelper.RandomStringFast(5);
            var institution = new Institution {idInstitution = 19};
            _stateServiceMock.Setup(i => i.HasValue(DomainConstants.VerificationKey)).Returns(true);
            _stateServiceMock.Setup(i => i.GetValue<string>(DomainConstants.VerificationKey)).Returns(verificationKey);
            _membershipServiceMock.Setup(
                i =>
                    i.ProcessInstitutionForUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                        It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())
                        ).Returns(institution).Verifiable();

            var registerViewModel = TestHelper.GetRegisterViewModel();

            _accountControllerOrchestrator = new AccountControllerOrchestrator(
                _loggerMock.Object,
                _membershipServiceMock.Object,
                _orderManagementServiceMock.Object,
                _stateServiceMock.Object,
                WebTestHelpers.GetMockedHttpContext().Request,
                _appHelperMock.Object
                );
            
            //  Act
            _accountControllerOrchestrator.RegisterAndLogInUser(registerViewModel);

            //  Assert                        
            _membershipServiceMock.Verify();
        }
        
        [TestMethod]
        public void RegisterAndLogInUserResultsInVerificationKeyPlacedInSession()
        {
            //  Arrange
            var verificationKey = TestHelper.RandomStringFast(5);
            var institution = new Institution {idInstitution = 19};
            _stateServiceMock.Setup(i => i.HasValue(DomainConstants.VerificationKey)).Returns(true);
            _stateServiceMock.Setup(i => i.GetValue<string>(DomainConstants.VerificationKey)).Returns(verificationKey).Verifiable();
            _membershipServiceMock.Setup(
                i =>
                    i.ProcessInstitutionForUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                        It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())
                        ).Returns(institution);

            var registerViewModel = TestHelper.GetRegisterViewModel();

            _accountControllerOrchestrator = new AccountControllerOrchestrator(
                _loggerMock.Object,
                _membershipServiceMock.Object,
                _orderManagementServiceMock.Object,
                _stateServiceMock.Object,
                WebTestHelpers.GetMockedHttpContext().Request,
                _appHelperMock.Object
                );
            
            //  Act
            _accountControllerOrchestrator.RegisterAndLogInUser(registerViewModel);

            //  Assert                        
            _stateServiceMock.Verify(i => i.GetValue<string>(DomainConstants.VerificationKey), Times.Once);
        }
        
        [TestMethod]
        public void RegisterAndLogInUserClearsTempPasswordFromSession()
        {
            //  Arrange
            var verificationKey = TestHelper.RandomStringFast(5);
            var institution = new Institution {idInstitution = 19};
            _stateServiceMock.Setup(i => i.HasValue(DomainConstants.TempPassword)).Returns(true);
            _stateServiceMock.Setup(i => i.ClearValue(DomainConstants.TempPassword)).Verifiable();
            _stateServiceMock.Setup(i => i.HasValue(DomainConstants.VerificationKey)).Returns(true);
            _stateServiceMock.Setup(i => i.GetValue<string>(DomainConstants.VerificationKey)).Returns(verificationKey);
            _membershipServiceMock.Setup(
                i =>
                    i.ProcessInstitutionForUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                        It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())
                        ).Returns(institution);

            var registerViewModel = TestHelper.GetRegisterViewModel();

            _accountControllerOrchestrator = new AccountControllerOrchestrator(
                _loggerMock.Object,
                _membershipServiceMock.Object,
                _orderManagementServiceMock.Object,
                _stateServiceMock.Object,
                WebTestHelpers.GetMockedHttpContext().Request,
                _appHelperMock.Object
                );
            
            //  Act
            _accountControllerOrchestrator.RegisterAndLogInUser(registerViewModel);

            //  Assert                        
            _stateServiceMock.Verify();
        }

        [TestMethod]
        public void RegisterAndLogInUserDoesNotClearsTempPasswordFromSessionWhenNotInSession()
        {
            //  Arrange
            var verificationKey = TestHelper.RandomStringFast(5);
            var institution = new Institution { idInstitution = 19 };
            _stateServiceMock.Setup(i => i.HasValue(DomainConstants.TempPassword)).Returns(false);
            _stateServiceMock.Setup(i => i.HasValue(DomainConstants.VerificationKey)).Returns(true);
            _stateServiceMock.Setup(i => i.GetValue<string>(DomainConstants.VerificationKey)).Returns(verificationKey);
            _membershipServiceMock.Setup(
                i =>
                    i.ProcessInstitutionForUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                        It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())
                        ).Returns(institution);

            var registerViewModel = TestHelper.GetRegisterViewModel();

            _accountControllerOrchestrator = new AccountControllerOrchestrator(
                _loggerMock.Object,
                _membershipServiceMock.Object,
                _orderManagementServiceMock.Object,
                _stateServiceMock.Object,
                WebTestHelpers.GetMockedHttpContext().Request,
                _appHelperMock.Object);

            //  Act
            _accountControllerOrchestrator.RegisterAndLogInUser(registerViewModel);

            //  Assert                        
            _stateServiceMock.Verify(i => i.ClearValue(DomainConstants.TempPassword), Times.Never);
        }

        [TestMethod]
        public void RegisterAndLogInUserCallsCreateWebUser()
        {
            //  Arrange
            var verificationKey = TestHelper.RandomStringFast(5);
            var institution = new Institution { idInstitution = 19 };
            _stateServiceMock.Setup(i => i.HasValue(DomainConstants.TempPassword)).Returns(false);
            _stateServiceMock.Setup(i => i.HasValue(DomainConstants.VerificationKey)).Returns(true);
            _stateServiceMock.Setup(i => i.GetValue<string>(DomainConstants.VerificationKey)).Returns(verificationKey);
            _membershipServiceMock.Setup(
                i =>
                    i.ProcessInstitutionForUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                        It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())
                        ).Returns(institution);

            _membershipServiceMock.Setup(i => i.CreateWebUser(It.IsAny<string>(),It.IsAny<string>(),It.IsAny<string>(),It.IsAny<string>(),It.IsAny<string>(),It.IsAny<USTimeZone>(), It.IsAny<UserType>(), It.IsAny<int>(), It.IsAny<IList<Address>>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>())).Verifiable();

            var registerViewModel = TestHelper.GetRegisterViewModel();

            _accountControllerOrchestrator = new AccountControllerOrchestrator(
                _loggerMock.Object,
                _membershipServiceMock.Object,
                _orderManagementServiceMock.Object,
                _stateServiceMock.Object,
                WebTestHelpers.GetMockedHttpContext().Request,
                _appHelperMock.Object
                );

            //  Act
            _accountControllerOrchestrator.RegisterAndLogInUser(registerViewModel);

            //  Assert                        
            _membershipServiceMock.Verify();
        }

        [TestMethod]
        public void RegisterAndLogInUserCallsCreateWebUserOfMembershipServiceMock()
        {
            //  Arrange
            var verificationKey = TestHelper.RandomStringFast(5);
            var institution = new Institution { idInstitution = 19 };
            _stateServiceMock.Setup(i => i.HasValue(DomainConstants.TempPassword)).Returns(false);
            _stateServiceMock.Setup(i => i.HasValue(DomainConstants.VerificationKey)).Returns(true);
            _stateServiceMock.Setup(i => i.GetValue<string>(DomainConstants.VerificationKey)).Returns(verificationKey);
            _membershipServiceMock.Setup(
                i =>
                    i.ProcessInstitutionForUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                        It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())
                        ).Returns(institution);

            _stateServiceMock.Setup(i => i.HasValue(Constants.CurrentUser)).Returns(false);
            _stateServiceMock.Setup(i => i.SetValue(Constants.CurrentUser, It.IsAny<WebUser>())).Verifiable();

            var registerViewModel = TestHelper.GetRegisterViewModel();

            _accountControllerOrchestrator = new AccountControllerOrchestrator(
                _loggerMock.Object,
                _membershipServiceMock.Object,
                _orderManagementServiceMock.Object,
                _stateServiceMock.Object,
                WebTestHelpers.GetMockedHttpContext().Request,
                _appHelperMock.Object
                );

            //  Act
            _accountControllerOrchestrator.RegisterAndLogInUser(registerViewModel);

            //  Assert                        
            _stateServiceMock.Verify(i => i.SetValue(Constants.CurrentUser, It.IsAny<WebUser>()), Times.Once);
        }

        [TestMethod]
        public void GetZipAddressReturnsZipWhereZipGetsHit()
        {
            //  Arrange
            _appHelperMock.Setup(i => i.GetCityStateFromZip(It.IsAny<int>())).Returns("WILLISTON,ND,-6");

            _accountControllerOrchestrator = new AccountControllerOrchestrator(
                _loggerMock.Object,
                _membershipServiceMock.Object,
                _orderManagementServiceMock.Object,
                _stateServiceMock.Object,
                WebTestHelpers.GetMockedHttpContext().Request,
                _appHelperMock.Object
                );
            //  Act
            var zipAddress = _accountControllerOrchestrator.GetZipAddress(54535);

            //  Assert                        
            Assert.IsInstanceOfType(zipAddress, typeof (string));
        }

        [TestMethod]
        public void GetZipAddressReturnsZipWhereZipGetsNoHits()
        {
            //  Arrange
            _appHelperMock.Setup(i => i.GetCityStateFromZip(It.IsAny<int>())).Returns(string.Empty);

            _accountControllerOrchestrator = new AccountControllerOrchestrator(
                _loggerMock.Object,
                _membershipServiceMock.Object,
                _orderManagementServiceMock.Object,
                _stateServiceMock.Object,
                WebTestHelpers.GetMockedHttpContext().Request,
                _appHelperMock.Object
                );
            //  Act
            var zipAddress = _accountControllerOrchestrator.GetZipAddress(54535);
            
            //  Assert                        
            Assert.AreEqual(zipAddress, string.Empty);
        }


        [TestMethod]
        public void UserConfirmedReturnsTrueWhenVerifiedSuccessfully()
        {
            //  Arrange
            var model = new CreateUserConfirmedViewModel
            {
                NewPassword = TestConstants.PasswordNew,
                Email = TestConstants.Email,
                OldPassword = TestConstants.PasswordOld
            };

            var request = new HttpRequestFake1();
            var verificationKey = TestHelper.RandomStringFast(8);

            _membershipServiceMock.Setup(i => i.VerifyUserByEmail(_webTestsGlobals.Tenant, model.Email)).Returns(true);
            _stateServiceMock.Setup(i => i.SetValue(DomainConstants.UserCreatedViaNewOrder, true));
            _stateServiceMock.Setup(i => i.GetValue<string>(DomainConstants.VerificationKey)).Returns(verificationKey);
            
            _membershipServiceMock.Setup(i => i.ChangePasswordFromResetKey(verificationKey, model.NewPassword)).Returns(true);
            _membershipServiceMock.Setup(i => i.LogInUser(_webTestsGlobals.Tenant,model.Email, model.NewPassword, true)).Returns(true);
            
            _appHelperMock.Setup(i => i.GetUserAuditInfo()).Returns(string.Format(AuditInfo, request.ServerVariables[TestConstants.HttpCookie], TestConstants.HomeUrlRelative));

            _accountControllerOrchestrator = new AccountControllerOrchestrator(
                _loggerMock.Object,
                _membershipServiceMock.Object,
                _orderManagementServiceMock.Object,
                _stateServiceMock.Object,
                WebTestHelpers.GetMockedHttpContext().Request,
                _appHelperMock.Object
                );

            //  Act
            var result = _accountControllerOrchestrator.UserConfirmed(model);
            
            //  Assert                        
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void UserConfirmedReturnsFalseWhenVerifiedSuccessfully()
        {
            //  Arrange
            var model = new CreateUserConfirmedViewModel
            {
                NewPassword = TestConstants.PasswordNew,
                Email = TestConstants.Email,
                OldPassword = TestConstants.PasswordOld
            };

            _membershipServiceMock.Setup(i => i.VerifyUserByEmail(_webTestsGlobals.Tenant, model.Email)).Returns(false);

            _accountControllerOrchestrator = new AccountControllerOrchestrator(
                _loggerMock.Object,
                _membershipServiceMock.Object,
                _orderManagementServiceMock.Object,
                _stateServiceMock.Object,
                WebTestHelpers.GetMockedHttpContext().Request,
                _appHelperMock.Object
                );

            //  Act
            var result = _accountControllerOrchestrator.UserConfirmed(model);

            //  Assert                        
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void StateServiceSetsUserCreatedViaNewOrderValueWhenUserVerifiedSuccessfully()
        {
            //  Arrange
            var model = new CreateUserConfirmedViewModel
            {
                NewPassword = TestConstants.PasswordNew,
                Email = TestConstants.Email,
                OldPassword = TestConstants.PasswordOld
            };

            var request = new HttpRequestFake1();
            var verificationKey = TestHelper.RandomStringFast(8);

            _membershipServiceMock.Setup(i => i.VerifyUserByEmail(_webTestsGlobals.Tenant, model.Email)).Returns(true);
            
            //  This is the bit we are verifying and asserting against. It may seem like a private implementation detail,
            //  but it is actually important in downstream operations that the StateService sets that value.
            _stateServiceMock.Setup(i => i.SetValue(DomainConstants.UserCreatedViaNewOrder, true)).Verifiable();
            _stateServiceMock.Setup(i => i.GetValue<string>(DomainConstants.VerificationKey)).Returns(verificationKey);

            _membershipServiceMock.Setup(i => i.ChangePasswordFromResetKey(verificationKey, model.NewPassword)).Returns(true);
            _membershipServiceMock.Setup(i => i.LogInUser(_webTestsGlobals.Tenant, model.Email, model.NewPassword, true)).Returns(true);

            _appHelperMock.Setup(i => i.GetUserAuditInfo()).Returns(string.Format(AuditInfo, request.ServerVariables[TestConstants.HttpCookie], TestConstants.HomeUrlRelative));

            _accountControllerOrchestrator = new AccountControllerOrchestrator(
                _loggerMock.Object,
                _membershipServiceMock.Object,
                _orderManagementServiceMock.Object,
                _stateServiceMock.Object,
                WebTestHelpers.GetMockedHttpContext().Request,
                _appHelperMock.Object
                );

            //  Act
            var result = _accountControllerOrchestrator.UserConfirmed(model);

            //  Assert                        
            _stateServiceMock.Verify();
        }
        
        [TestMethod]
        public void StateServiceClearsUserCreatedViaNewOrderValueWhenUserVerifiedSuccessfully()
        {
            //  Arrange
            var model = new CreateUserConfirmedViewModel
            {
                NewPassword = TestConstants.PasswordNew,
                Email = TestConstants.Email,
                OldPassword = TestConstants.PasswordOld
            };

            var request = new HttpRequestFake1();
            var verificationKey = TestHelper.RandomStringFast(8);

            _membershipServiceMock.Setup(i => i.VerifyUserByEmail(_webTestsGlobals.Tenant, model.Email)).Returns(true);
            
            _stateServiceMock.Setup(i => i.GetValue<string>(DomainConstants.VerificationKey)).Returns(verificationKey);

            //  This is the bit we are verifying and asserting against. It may seem like a private implementation detail,
            //  but it is actually important that after the notifications are sent, the StateService clears that value.
            _stateServiceMock.Setup(i => i.ClearValue(DomainConstants.UserCreatedViaNewOrder)).Verifiable();

            _membershipServiceMock.Setup(i => i.ChangePasswordFromResetKey(verificationKey, model.NewPassword)).Returns(true);
            _membershipServiceMock.Setup(i => i.LogInUser(_webTestsGlobals.Tenant, model.Email, model.NewPassword, true)).Returns(true);

            _appHelperMock.Setup(i => i.GetUserAuditInfo()).Returns(string.Format(AuditInfo, request.ServerVariables[TestConstants.HttpCookie], TestConstants.HomeUrlRelative));

            _accountControllerOrchestrator = new AccountControllerOrchestrator(
                _loggerMock.Object,
                _membershipServiceMock.Object,
                _orderManagementServiceMock.Object,
                _stateServiceMock.Object,
                WebTestHelpers.GetMockedHttpContext().Request,
                _appHelperMock.Object
                );

            //  Act
            var result = _accountControllerOrchestrator.UserConfirmed(model);

            //  Assert                        
            _stateServiceMock.Verify();
        }


        [TestMethod]
        public void BuildLoginModelReturnsLoginModel()
        {
            //  Arrange
            var fake = new HttpRequestFake1();
            _accountControllerOrchestrator = new AccountControllerOrchestrator(
                _loggerMock.Object,
                _membershipServiceMock.Object,
                _orderManagementServiceMock.Object,
                _stateServiceMock.Object,
                WebTestHelpers.GetMockedHttpContext().Request,
                _appHelperMock.Object
                );

            //  Act
            var result = _accountControllerOrchestrator.BuildLoginModel(TestConstants.HomeUrlRelative);

            //  Assert                        
            Assert.IsInstanceOfType(result, typeof (LoginModel));
        }

        [TestMethod]
        public void BuildLoginModelLogsReturnUrlWhereNullPassedAsParameter()
        {
            //  Arrange
            var request = new HttpRequestFake1();
            _appHelperMock.Setup(i => i.GetUserAuditInfo()).Returns(string.Format(AuditInfo, request.ServerVariables[TestConstants.HttpCookie], TestConstants.HomeUrlRelative)
    );

            _loggerMock.Setup(i => i.Info(It.IsAny<string>())).Verifiable();
            
            _accountControllerOrchestrator = new AccountControllerOrchestrator(
                _loggerMock.Object,
                _membershipServiceMock.Object,
                _orderManagementServiceMock.Object,
                _stateServiceMock.Object,
                WebTestHelpers.GetMockedHttpContext().Request,
                _appHelperMock.Object
                );

            //  Act
            var result = _accountControllerOrchestrator.BuildLoginModel(null);

            //  Assert                        
            _loggerMock.Verify();
        }

        [TestMethod]
        public void BuildLoginModelReturnsLoginModelWithNonNullReturnUrlWhereNullPassedAsParameter()
        {
            //  Arrange
            var request = new HttpRequestFake1();
            _appHelperMock.Setup(i => i.GetUserAuditInfo()).Returns(string.Format(AuditInfo, request.ServerVariables[TestConstants.HttpCookie], TestConstants.HomeUrlRelative)
    );

            _loggerMock.Setup(i => i.Info(It.IsAny<string>())).Verifiable();
            
            _accountControllerOrchestrator = new AccountControllerOrchestrator(
                _loggerMock.Object,
                _membershipServiceMock.Object,
                _orderManagementServiceMock.Object,
                _stateServiceMock.Object,
                WebTestHelpers.GetMockedHttpContext().Request,
                _appHelperMock.Object
                );

            //  Act
            var result = _accountControllerOrchestrator.BuildLoginModel(null);

            //  Assert                        
            Assert.IsNotNull(result.ReturnUrl);
        }

        [TestMethod]
        public void BuildLoginModelReturnsLoginModelWithReturnUrlWhichIsSameAsPassedAsParameter()
        {
            //  Arrange
            var request = new HttpRequestFake1();
            _appHelperMock.Setup(i => i.GetUserAuditInfo()).Returns(string.Format(AuditInfo, request.ServerVariables[TestConstants.HttpCookie], TestConstants.HomeUrlRelative)
    );

            _loggerMock.Setup(i => i.Info(It.IsAny<string>())).Verifiable();
            
            _accountControllerOrchestrator = new AccountControllerOrchestrator(
                _loggerMock.Object,
                _membershipServiceMock.Object,
                _orderManagementServiceMock.Object,
                _stateServiceMock.Object,
                WebTestHelpers.GetMockedHttpContext().Request,
                _appHelperMock.Object
                );

            //  Act
            var result = _accountControllerOrchestrator.BuildLoginModel(TestConstants.HomeUrlRelative);

            //  Assert                        
            Assert.AreEqual(result.ReturnUrl, TestConstants.HomeUrlRelative);
        }

        [TestMethod]
        public void BuildLoginModelReturnsLoginModelWithMyWebinarUrlWhereReturnUrlIsAccountSlashSignin()
        {
            //  Arrange
            var request = new HttpRequestFake1();
            _appHelperMock.Setup(i => i.GetUserAuditInfo()).Returns(string.Format(AuditInfo, request.ServerVariables[TestConstants.HttpCookie], TestConstants.HomeUrlRelative)
    );

            _loggerMock.Setup(i => i.Info(It.IsAny<string>())).Verifiable();
            
            _accountControllerOrchestrator = new AccountControllerOrchestrator(
                _loggerMock.Object,
                _membershipServiceMock.Object,
                _orderManagementServiceMock.Object,
                _stateServiceMock.Object,
                WebTestHelpers.GetMockedHttpContext().Request,
                _appHelperMock.Object
                );

            //  Act
            var result = _accountControllerOrchestrator.BuildLoginModel("/Account/Signin");

            //  Assert                        
            Assert.AreEqual(result.ReturnUrl, "/Account/MyWebinars");
        }

        [TestMethod]
        public void BuildLoginModelReturnsLoginModelWithMyWebinarUrlWhereReturnUrlIsAccountSlashLogin()
        {
            //  Arrange
            var request = new HttpRequestFake1();
            _appHelperMock.Setup(i => i.GetUserAuditInfo()).Returns(string.Format(AuditInfo, request.ServerVariables[TestConstants.HttpCookie], TestConstants.HomeUrlRelative)
    );

            _loggerMock.Setup(i => i.Info(It.IsAny<string>())).Verifiable();
            
            _accountControllerOrchestrator = new AccountControllerOrchestrator(
                _loggerMock.Object,
                _membershipServiceMock.Object,
                _orderManagementServiceMock.Object,
                _stateServiceMock.Object,
                WebTestHelpers.GetMockedHttpContext().Request,
                _appHelperMock.Object
                );

            //  Act
            var result = _accountControllerOrchestrator.BuildLoginModel("/Account/Login");

            //  Assert                        
            Assert.AreEqual(result.ReturnUrl, "/Account/MyWebinars");
        }

        [TestMethod]
        public void BuildLoginModelReturnsLoginModelWithMyWebinarUrlWhereReturnUrlContainsPasswordResetConfirm()
        {
            //  Arrange
            var request = new HttpRequestFake1();
            _appHelperMock.Setup(i => i.GetUserAuditInfo()).Returns(string.Format(AuditInfo, request.ServerVariables[TestConstants.HttpCookie], TestConstants.HomeUrlRelative)
    );

            _loggerMock.Setup(i => i.Info(It.IsAny<string>())).Verifiable();
            
            _accountControllerOrchestrator = new AccountControllerOrchestrator(
                _loggerMock.Object,
                _membershipServiceMock.Object,
                _orderManagementServiceMock.Object,
                _stateServiceMock.Object,
                WebTestHelpers.GetMockedHttpContext().Request,
                _appHelperMock.Object
                );

            //  Act
            var result = _accountControllerOrchestrator.BuildLoginModel("/Account/PasswordResetConfirm");

            //  Assert                        
            Assert.AreEqual(result.ReturnUrl, "/Account/MyWebinars");
        }



        #region Helper methods

        private EditBillingAddressModel GetEditBillingAddressModel()
        {
            var userAccountFake = new UserAccountFake1();

            _membershipServiceMock.Setup(m => m.GetUserAccountByUserId(It.IsAny<Guid>())).Returns(userAccountFake);
            _membershipServiceMock.Setup(m => m.GetUserByEmail(It.IsAny<string>()))
                .Returns(new WebUser
                {
                    Addresses = new List<Address> {new Address {AddressType = DomainConstants.BillingAddress}}
                });

            HttpContextFactory.SetCurrentContext(WebTestHelpers.GetMockedHttpContext());


            _accountControllerOrchestrator = new AccountControllerOrchestrator(
                _loggerMock.Object,
                _membershipServiceMock.Object,
                _orderManagementServiceMock.Object,
                _stateServiceMock.Object,
                HttpContextFactory.Current.Request,
                _appHelperMock.Object
                );

            var data = new DataOperations {ConnectionString = _globals.MembershipConnectionString};
            var idOfTestUser = data.GetIdOfTestUser(_webTestsGlobals.LoggedInUserEmail);

            // updates current thread principal 
            var principal = Thread.CurrentPrincipal as ClaimsPrincipal;
            if (principal != null)
            {
                // Need to add this claim as GetWebUserFromIPrincipal method of AccountControllerOrchestrator needs a valid NameIdentifier to retrieve the Guid
                ClaimsIdentity threadIdentity = principal.Identities.First();
                threadIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, idOfTestUser.ToString()));
            }

            //  Act
            var actualModel = _accountControllerOrchestrator.BuildBillingAddressModel();
            return actualModel;
        }

        

        #endregion
        

    }
}
