using CUWebinars.WebUi.Tests.Page.Chrome;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CUWebinars.WebUi.Tests.Chrome
{
    [TestClass]
    public class HomeIndexPageTests : ChromeBaseTest
    {
        [TestMethod]
        [TestCategory("GUI Tests")]
        public void Load_Home_Page()
        {
            var home = NavigateToHomeIndexPage();

            Assert.IsTrue(home.PhoneNrLinkIsPresentOnPage);
        }

        public HomeIndexPage NavigateToHomeIndexPage()
        {
            var homeIndexPage = new HomeIndexPage(TestDriver);

            homeIndexPage.Open();

            return homeIndexPage;
        }
    }
}
