using CUWebinars.Web.Controllers;
using CUWebinars.Web.Models;
using CUWebinars.Web.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Ninject.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CUWebinars.Business.AccountService;

namespace CUWebinars.Web.Tests
{
    [TestClass]
    public class AccountControllerTests
    {
        Mock<IMailService> mailServiceMock;
        Mock<IMembershipService> membershipServiceMock;
        Mock<IOrderService> orderServiceMock;
        Mock<ILogger> loggerMock;


        [TestInitialize]
        public void SetUpTest()
        {
            mailServiceMock = new Mock<IMailService>();
            membershipServiceMock = new Mock<IMembershipService>();
            orderServiceMock = new Mock<IOrderService>();
            loggerMock = new Mock<ILogger>();
        }

        [TestMethod]
        public void LoginReturnsViewResult()
        {
            UrlHelper urlHelper;
            Mock<HttpContextBase> context;

            var accountController = GetNewControllerWithWebInfrastructure(out urlHelper, out context);

            accountController.ControllerContext = new ControllerContext(context.Object, new RouteData(), accountController);
            accountController.Url = urlHelper;

            var result = accountController.Login(null);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void LoginReturnsViewResultWithLoginModel()
        {
            UrlHelper urlHelper;
            Mock<HttpContextBase> context;

            var accountController = GetNewControllerWithWebInfrastructure(out urlHelper, out context);

            accountController.Url = urlHelper;
            accountController.ControllerContext = new ControllerContext(context.Object, new RouteData(), accountController);

            var result = accountController.Login(null);

            Assert.IsInstanceOfType(((ViewResult)result).Model, typeof(LoginModel));
        }

        [TestMethod]
        public void LoginReturnsViewResultWithLoginModelWhereNullParameterPassedIn()
        {
            UrlHelper urlHelper;
            Mock<HttpContextBase> context;

            var accountController = GetNewControllerWithWebInfrastructure(out urlHelper, out context);

            accountController.Url = urlHelper;
            accountController.ControllerContext = new ControllerContext(context.Object, new RouteData(), accountController);

            var result = accountController.Login(null);

            Assert.IsTrue(((LoginModel)((ViewResult)result).Model).ReturnUrl.Equals(@"/") );
        }

        [TestMethod]
        public void LoginReturnsActionResultWhenPostedTo()
        {
            string email = "ever@hopefultoauthenticate.com";
            string password = "tyu567&U";

            membershipServiceMock.Setup(m => m.LogInUser(email, password, false)).Returns(() => true);

            var signInModel = new SignInModel
            {
                Email = email,
                Password = password
            };

            var accountController = new AccountController(mailServiceMock.Object, loggerMock.Object, membershipServiceMock.Object);

            var result = accountController.SignIn(signInModel);

            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
        }

        [TestMethod]
        public void LoginReturnsActionResultWhenPostedToRedirectedToLoginPage()
        {
            string email = "ever@hopefultoauthenticate.com";
            string password = "tyu567&U";

            membershipServiceMock.Setup(m => m.LogInUser(email, password, false)).Returns(() => true);

            var signInModel = new SignInModel
            {
                Email = email,
                Password = password
            };

            var accountController = new AccountController(mailServiceMock.Object, loggerMock.Object, membershipServiceMock.Object);

            var result = accountController.SignIn(signInModel);

            Assert.AreEqual(((RedirectToRouteResult)result).RouteName, string.Empty);
            Assert.AreEqual(((RedirectToRouteResult)result).RouteValues["Action"].ToString(), "Index");
            Assert.AreEqual(((RedirectToRouteResult)result).RouteValues["Controller"].ToString(), "Home");
        }

        [TestMethod]
        public void LoginReturnsViewResultLoginPageWhereLoginFails()
        {
            string email = "authentication@ishopeless.com";
            string password = "tyu567&U";

            membershipServiceMock.Setup(m => m.LogInUser(email, password, true)).Returns(() => false);

            var signInModel = new SignInModel
            {
                Email = email,
                Password = password
            };

            var accountController = new AccountController(mailServiceMock.Object, loggerMock.Object, membershipServiceMock.Object);

            var result = accountController.SignIn(signInModel);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void LoginReturnsViewResultLoginPageWhereLoginFailsWithModelInTact()
        {
            string email = "authentication@ishopeless.com";
            string password = "tyu567&U";

            membershipServiceMock.Setup(m => m.LogInUser(email, password, true)).Returns(() => false);

            var signInModel = new SignInModel
            {
                Email = email,
                Password = password,
                ReturnUrl = null
            };

            var accountController = new AccountController(mailServiceMock.Object, loggerMock.Object, membershipServiceMock.Object);

            var result = accountController.SignIn(signInModel);

            Assert.IsInstanceOfType(((ViewResult)result).Model, typeof(LoginModel));
            Assert.AreEqual(((SignInModel)((LoginModel)((ViewResult)result).Model).SignIn).Password, password);
            Assert.AreEqual(((SignInModel)((LoginModel)((ViewResult)result).Model).SignIn).Email, email);
            Assert.IsNull(((SignInModel)((LoginModel)((ViewResult)result).Model).SignIn).ReturnUrl);
        }

        [TestMethod]
        public void ResetPasswordValidPassword()
        {
            string email = "ever@hopefultoauthenticate.com";

            var resetPasswordModel = new ResetPasswordModel
            {
                Email = email,
                EmailSent = false

            };

            membershipServiceMock.Setup(m => m.ResetPassword(email)).Verifiable();

            var accountController = new AccountController(mailServiceMock.Object, loggerMock.Object, membershipServiceMock.Object);

            var result = accountController.ResetPassword(resetPasswordModel);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            membershipServiceMock.Verify(); 
        }

        [TestMethod]        
        public void ResetPasswordInValidPasswordFails()
        {
            string email = "ever@hopefultoauthenticate.com";

            var resetPasswordModel = new ResetPasswordModel
            {
                Email = email,
                EmailSent = false

            };

            membershipServiceMock.Setup(m => m.ResetPassword(email)).Throws(new ValidationException("Invalid email."));

            var accountController = new AccountController(mailServiceMock.Object, loggerMock.Object, membershipServiceMock.Object);

            var result = accountController.ResetPassword(resetPasswordModel);

            Assert.IsTrue(!accountController.ModelState.IsValid);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void ResetPasswordInValidPasswordFailsAndActiveTabSetToReset()
        {
            string email = "ever@hopefultoauthenticate.com";

            var resetPasswordModel = new ResetPasswordModel
            {
                Email = email,
                EmailSent = false

            };

            membershipServiceMock.Setup(m => m.ResetPassword(email)).Throws(new ValidationException("Invalid email."));

            var accountController = new AccountController(mailServiceMock.Object, loggerMock.Object, membershipServiceMock.Object);

            var result = accountController.ResetPassword(resetPasswordModel);

            Assert.IsTrue(!accountController.ModelState.IsValid);
            Assert.IsTrue(((LoginModel)((ViewResult)result).Model).ActiveTab.Equals("reset", StringComparison.Ordinal));
        }

        [TestMethod]
        public void PasswordResetConfirmReturnsViewResult()
        {
            string randomString = GetRandomString(24);

            var accountController = new AccountController(mailServiceMock.Object, loggerMock.Object, membershipServiceMock.Object);

            var result = accountController.PasswordResetConfirm(randomString);
                        
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void PasswordResetConfirmReturnsViewResultWithKeyFromEmailSentToResetPassword()
        {
            string randomString = GetRandomString(24);

            var accountController = new AccountController(mailServiceMock.Object, loggerMock.Object, membershipServiceMock.Object);

            var result = accountController.PasswordResetConfirm(randomString);

            Assert.IsTrue(((ChangePasswordFromResetKeyInputModel)((ViewResult)result).Model).Key.Equals(randomString, StringComparison.Ordinal));
        }

        [TestMethod]
        public void PasswordResetConfirmModelStateIsFalseWhereNullSentToResetPassword()
        {
            var accountController = new AccountController(mailServiceMock.Object, loggerMock.Object, membershipServiceMock.Object);

            var result = accountController.PasswordResetConfirm(null);

            Assert.IsTrue(!accountController.ModelState.IsValid);
        }

        [TestMethod]
        public void PasswordResetConfirmModelStateIsFalseWhereEmptyStringSentToResetPassword()
        {
            var accountController = new AccountController(mailServiceMock.Object, loggerMock.Object, membershipServiceMock.Object);

            var result = accountController.PasswordResetConfirm(string.Empty);

            Assert.IsTrue(!accountController.ModelState.IsValid);
        }

        private AccountController GetNewControllerWithWebInfrastructure(out UrlHelper urlHelper, out Mock<HttpContextBase> context)
        {
            
            var request = new Mock<HttpRequestBase>();
            var uri = new Uri(@ConfigurationManager.AppSettings["SiteUrl"]);
            var server = new HttpServerUtilityFake();

            request.SetupGet(x => x.UrlReferrer).Returns(uri);

            context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);
            context.SetupGet(x => x.Server).Returns(server);

            RequestContext requestContext = new RequestContext(context.Object, new RouteData());
            urlHelper = new UrlHelper(requestContext);

            return new AccountController(mailServiceMock.Object, loggerMock.Object, membershipServiceMock.Object);
        }

        private string GetRandomString(int size)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();

            return new string(Enumerable.Repeat(chars, size)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray()
                          );
        }
    }
}
