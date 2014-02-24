using CUWebinars.WebUi.Tests.Page.Ie;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CUWebinars.WebUi.Tests.Ie
{
    [TestClass]
    public class HomeIndexPageTests : IeBaseTest
    {
        [TestMethod]
        public void Load_Home_Page()
        {
            var home = NavigateToHomeIndexPage();
            
            Assert.IsTrue(home.LoginLinkIsPresentOnPage);
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
