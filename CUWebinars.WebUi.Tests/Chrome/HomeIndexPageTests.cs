using CUWebinars.WebUi.Tests.Page.Chrome;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CUWebinars.WebUi.Tests.Chrome
{
    [TestClass]
    public class HomeIndexPageTests : ChromeBaseTest
    {
        [TestMethod]
        [TestCategory("GUI Tests")]
        public void LoadHomePage()
        {
            var home = NavigateToHomeIndexPage();

            Assert.IsTrue(home.PhoneNrLinkIsPresentOnPage);
        }

        [TestMethod]
        [TestCategory("GUI Tests")]
        public void LoginToSite()
        {
            var home = NavigateToHomeIndexPage();
            home.ClickLoginLink();
            home.LogInToSite(Constants.DenzlerEmailAddress, Constants.DenzlerPassword);
            
            Assert.IsTrue(home.LogoutLinkIsPresentOnPage);
        }

        [TestMethod]
        [TestCategory("GUI Tests")]
        public void ClickRegisterUserLinkWithExistingEmailAndLogIn()
        {
            var home = NavigateToHomeIndexPage();
            home.ClickLoginLink();
            home.ClickRegisterLinkOnLoginView();
            home.EnterEmailAddressAndEnter(Constants.DenzlerEmailAddress);
            home.EnterPasswordWhereUserExists(Constants.DenzlerPassword);

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
