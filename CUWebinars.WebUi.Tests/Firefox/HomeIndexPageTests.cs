using CUWebinars.WebUi.Tests.Page.Firefox;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CUWebinars.WebUi.Tests.Firefox
{
    [TestClass]
    public class HomeIndexPageTests : FirefoxBaseTest
    {
        [TestMethod]
        [TestCategory("GUI Tests")]
        public void LoadHomePage()
        {
            var homeIndexPage = NavigateToHomeIndexPage();

            Assert.IsTrue(homeIndexPage.PhoneNrLinkIsPresentOnPage);
        }

        [TestMethod]
        [TestCategory("GUI Tests")]
        public void LoginToSite()
        {
            var home = NavigateToHomeIndexPage();
            home.ClickLoginLink();
            home.LogInToSite(Constants.SitTestEmailAddress, Constants.SitTestPassword);

            Assert.IsTrue(home.LogoutLinkIsPresentOnPage);
        }

        [TestMethod]
        [TestCategory("GUI Tests")]
        public void ClickRegisterUserLinkWithExistingEmailAndLogIn()
        {
            var home = NavigateToHomeIndexPage();
            home.ClickLoginLink();
            home.ClickRegisterLinkOnLoginView();
            home.EnterEmailAddressAndEnter(Constants.SitTestEmailAddress);
            home.EnterPasswordWhereUserExists(Constants.SitTestPassword);

            Assert.IsTrue(home.LogoutLinkIsPresentOnPage);
        }


        public HomeIndexPage NavigateToHomeIndexPage()
        {
            var homeIndexPage = new HomeIndexPage(TestDriver);
            

            homeIndexPage.Open();

            return homeIndexPage;
        }
    }
}
