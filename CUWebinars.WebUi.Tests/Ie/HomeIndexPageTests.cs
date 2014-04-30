using CUWebinars.WebUi.Tests.Page.Ie;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CUWebinars.WebUi.Tests.Ie
{
    [TestClass]
    public class HomeIndexPageTests : IeBaseTest
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
            home.LogInToSite(Constants.SitTestEmailAddress, Constants.SitTestPassword);

            Assert.IsTrue(home.LogoutLinkIsPresentOnPage);

            home.LogOff();
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

            home.LogOff();
        }
        
        public HomeIndexPage NavigateToHomeIndexPage()
        {
            var homeIndexPage = new HomeIndexPage(TestDriver);

            homeIndexPage.Open();

            return homeIndexPage;
        }

        //public PageNotFoundErrorPage NavigateToPageNotFoundErrorPage(string badUrl)
        //{
        //    var pageNotFoundErrorPage = new PageNotFoundErrorPage(TestDriver, badUrl);
        //    pageNotFoundErrorPage.Open();

        //    return pageNotFoundErrorPage;
        //}
    }
}
