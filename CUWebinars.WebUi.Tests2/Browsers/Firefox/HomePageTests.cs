using CUWebinars.WebUi.Tests2.Infrastructure;
using CUWebinars.WebUi.Tests2.Pages;
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

        public HomePage NavigateToHomeIndexPage()
        {
            var homeIndexPage = new HomePage(TestDriver);

            homeIndexPage.Open();

            return homeIndexPage;
        }
    }
}
