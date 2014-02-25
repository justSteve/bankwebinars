using CUWebinars.WebUi.Tests.Page.Firefox;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CUWebinars.WebUi.Tests.Firefox
{
    [TestClass]
    public class HomeIndexPageTests : FirefoxBaseTest
    {
        [TestMethod]
        [TestCategory("GUI Tests")]
        public void Load_Home_Page()
        {
            var homeIndexPage = NavigateToHomeIndexPage();

            Assert.IsTrue(homeIndexPage.PhoneNrLinkIsPresentOnPage);
        }


        public HomeIndexPage NavigateToHomeIndexPage()
        {
            var homeIndexPage = new HomeIndexPage(TestDriver);
            

            homeIndexPage.Open();

            return homeIndexPage;
        }
    }
}
