using CUWebinars.WebUi.Tests2.Infrastructure;
using CUWebinars.WebUi.Tests2.Pages;
using KesselRun.SeleniumCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CUWebinars.WebUi.Tests2.Browsers.Firefox
{
    [TestClass]
    public class HomePageTests : FirefoxBaseTest
    {
        private const string DotComDomain = ".com";
        private const string EdgeCaseResetPasswordButton = "EdgeCaseResetPasswordButton";
        private const string NormalResetPasswordButton = "NormalResetPasswordButton";

        [TestMethod]
        [TestCategory(TestCategories.Gui)]
        [TestCategory(TestCategories.Firefox)]
        public void LoadHomePage()
        {
            var home = NavigateToHomeIndexPage();

            Assert.IsTrue(home.PhoneNrLinkIsPresentOnPage);
        }

        [TestMethod]
        [TestCategory(TestCategories.Gui)]
        [TestCategory(TestCategories.Firefox)]
        public void LoginToSite()
        {
            var home = NavigateToHomeIndexPage();
            home.ClickLoginLink();
            home.LogInToSite(TestConstants.SitTestEmailAddress, TestConstants.SitTestPassword);

            Assert.IsTrue(home.LoginLinkIsPresentOnPage);

            home.LogOff();
        }

        //[TestMethod]
        //[TestCategory(TestCategories.Gui)]
        //[TestCategory(TestCategories.Firefox)]
        //public void ClickRegisterUserLinkWithExistingEmailAndLogIn()
        //{
        //    var home = NavigateToHomeIndexPage();
        //    home.ClickLoginLink();
        //    home.ClickRegisterLinkOnLoginView();
        //    home.EnterEmailAddressAndEnter(TestConstants.SitTestEmailAddress);
        //    home.EnterPasswordWhereUserExists(TestConstants.SitTestPassword);

        //    Assert.IsTrue(home.LogoutLinkIsPresentOnPage);

        //    home.LogOff();
        //}

        public HomePage NavigateToHomeIndexPage()
        {
            var homeIndexPage = new HomePage(TestDriver);

            homeIndexPage.Open();

            return homeIndexPage;
        }
    }
}
