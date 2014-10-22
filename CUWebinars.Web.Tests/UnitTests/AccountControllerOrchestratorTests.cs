using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Routing;
using BrockAllen.MembershipReboot;
using CUWebinars.Business.AccountService;
using CUWebinars.Business.Constants;
using CUWebinars.Business.Models;
using CUWebinars.Business.Services;
using CUWebinars.Web.App_Start;
using CUWebinars.Web.Core;
using CUWebinars.Web.Core.Orchestrators;
using CUWebinars.Web.Models;
using CUWebinars.Web.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Ninject.Extensions.Logging;
using ClaimTypes = System.IdentityModel.Claims.ClaimTypes;

namespace CUWebinars.Web.Tests.UnitTests
{
    [TestClass]
    public class AccountControllerOrchestratorTests
    {
        private IAccountControllerOrchestrator _accountControllerOrchestrator;
        private Mock<IMembershipService> _membershipServiceMock;
        private Mock<IOrderManagementService> _orderManagementServiceMock;
        private Mock<ILogger> _loggerMock;
        private Mock<IStateService> _stateServiceMock;
        private Mock<HttpRequestBase> _requestMock;
        private GlobalConfig _globals = GlobalConfig.GlobalConfigSingleton;


        [TestInitialize]
        public void SetUpTest()
        {
            _membershipServiceMock = new Mock<IMembershipService>();
            _orderManagementServiceMock = new Mock<IOrderManagementService>();
            _loggerMock = new Mock<ILogger>();
            _stateServiceMock = new Mock<IStateService>();
            HttpContextFactory.SetCurrentContext(GetMockedHttpContext());
            _requestMock = new Mock<HttpRequestBase>(HttpContextFactory.Current.Request);

            _accountControllerOrchestrator = new AccountControllerOrchestrator(_loggerMock.Object, _membershipServiceMock.Object, _orderManagementServiceMock.Object, _stateServiceMock.Object, new HttpRequestWrapper(HttpContext.Current.Request));
        }

        [TestMethod, Ignore]
        public void BuildBillingAddressModelReturnsEditBillingAddressModel()
        {
            //  Arrange

            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.Email;

            var cp = ClaimsPrincipal.Current;
            var identity = cp.Identity as ClaimsIdentity;

            identity.AddClaim(new Claim(ClaimTypes.Email, "dave@dave.com"));

            //  Act
            var actualModel = _accountControllerOrchestrator.BuildBillingAddressModel();

            //  Assert                        
            Assert.IsInstanceOfType(actualModel, typeof(EditBillingAddressModel));
        }

        [TestMethod, Ignore]
        public void BuildBillingAddressModelReturnsEditBillingAddressModelWithNonNullBillingAddress()
        {
            //  Arrange
            _membershipServiceMock.Setup(m => m.GetUserAccountByUserId(It.IsAny<Guid>())).Returns(It.IsAny<UserAccount>);
            _membershipServiceMock.Setup(m => m.GetUserByEmail(It.IsAny<string>()))
                .Returns(new WebUser
                {
                    Addresses = new List<Address> {new Address {AddressType = DomainConstants.BillingAddress}}
                });
            
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.Email;
            
            var cp = ClaimsPrincipal.Current;
            var identity = cp.Identity as ClaimsIdentity;
            
            identity.AddClaim(new Claim(ClaimTypes.Email, "dave@dave.com"));
            
            //  Act
            var actualModel = _accountControllerOrchestrator.BuildBillingAddressModel();

            //  Assert                        
            Assert.IsNotNull(actualModel.BillingAddress);
            Assert.AreEqual(actualModel.BillingAddress.TypeOfAddress, DomainConstants.BillingAddress);
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
            _membershipServiceMock.Setup(i => i.LogInUser("CUWebinars", signInModel.Email, signInModel.Password, signInModel.RememberMe, out userMustVerify))
                .Returns(true);
            
            //  Act
            var result = _accountControllerOrchestrator.SignUserIn(signInModel, out userMustVerify);

            //  Assert                        
            Assert.IsTrue(result);

        }

        [TestMethod]
        public void BuildLoginModelReturnsLoginModel()
        {
            //  Arrange
            //  Act
            var logInModel = _accountControllerOrchestrator.BuildLoginModel(null);

            //  Assert                        
            Assert.IsTrue(logInModel.ReturnUrl.Equals(Path.AltDirectorySeparatorChar.ToString()));
        }

        private HttpContextBase GetMockedHttpContext()
        {
            var context = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            var response = new Mock<HttpResponseBase>();
            var session = new Mock<HttpSessionStateBase>();
            var server = new Mock<HttpServerUtilityBase>();
            var user = new Mock<IPrincipal>();
            var identity = new Mock<IIdentity>();
            var urlHelper = new Mock<UrlHelper>();

            
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            
            var requestContext = new Mock<RequestContext>();
            requestContext.Setup(x => x.HttpContext).Returns(context.Object);
            context.Setup(ctx => ctx.Request).Returns(request.Object);
            context.Setup(ctx => ctx.Response).Returns(response.Object);
            context.Setup(ctx => ctx.Session).Returns(session.Object);
            context.Setup(ctx => ctx.Server).Returns(server.Object);
            context.Setup(ctx => ctx.User).Returns(user.Object);
            user.Setup(ctx => ctx.Identity).Returns(identity.Object);
            identity.Setup(id => id.IsAuthenticated).Returns(true);
            identity.Setup(id => id.Name).Returns("test");
            request.Setup(req => req.Url).Returns(new Uri(ConfigurationManager.AppSettings["SiteUrl"]));
            request.Setup(req => req.ApplicationPath).Returns(Path.AltDirectorySeparatorChar.ToString);
            request.Setup(req => req.RequestContext).Returns(requestContext.Object);
            request.Setup(req => req.UrlReferrer).Returns(new Uri(@"http://localhost:3538/"));
            requestContext.Setup(x => x.RouteData).Returns(new RouteData());
            request.SetupGet(req => req.Headers).Returns(new NameValueCollection());

            //  we also need to assig a value to HttpContext.Current as it is used in the AppHelper.GetUserAuditInfo method
            HttpContext.Current = new HttpContext(new HttpRequest("", ConfigurationManager.AppSettings["SiteUrl"], ""), new HttpResponse(new StringWriter())
    );

            return context.Object;
        }

        public class HttpContextFactory
        {
            private static HttpContextBase m_context;
            public static HttpContextBase Current
            {
                get
                {
                    if (m_context != null)
                        return m_context;

                    if (HttpContext.Current == null)
                        throw new InvalidOperationException("HttpContext not available");

                    return new HttpContextWrapper(HttpContext.Current);
                }
            }

            public static void SetCurrentContext(HttpContextBase context)
            {
                m_context = context;
            }
        }

    }
}
