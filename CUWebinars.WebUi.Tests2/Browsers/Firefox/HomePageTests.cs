using System.Threading;
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

        [TestMethod]
        [TestCategory(TestCategories.Gui)]
        [TestCategory(TestCategories.Firefox)]
        public void ClickRegisterUserLinkWithExistingEmailAndLogIn()
        {
            var home = NavigateToHomeIndexPage();
            home.ClickLoginLink();
            home.ClickRegisterLinkOnLoginView();
            home.EnterEmailAddressAndClickSubmit(TestConstants.SitTestEmailAddress);
            home.EnterPasswordWhereUserExists(TestConstants.SitTestPassword);

            Assert.IsTrue(home.LogoutLinkIsPresentOnPage);

            home.LogOff();
        }

        [TestMethod]
        [TestCategory(TestCategories.Gui)]
        [TestCategory(TestCategories.Firefox)]
        public void ClickRegisterUserLinkWithExistingEmailAndRequestPasswordReset()
        {
            var home = NavigateToHomeIndexPage();
            home.ClickLoginLink();
            home.ClickRegisterLinkOnLoginView();
            home.EnterEmailAddressAndClickSubmit(TestConstants.SitTestEmailAddress);
            home.ClickResetPasswordButton(EdgeCaseResetPasswordButton);

            Assert.IsTrue(home.PasswordResetInstructionsSentLabelPresent);
        }

        [TestMethod]
        [TestCategory(TestCategories.Gui)]
        [TestCategory(TestCategories.Firefox)]
        public void ClickRegisterUserLinkWithNovelEmail()
        {
            var firstName = WebUiTestHelpers.RandomStringFast(8);
            var lastName = WebUiTestHelpers.RandomStringFast(5);
            var email = string.Concat(firstName, "_", lastName, "@", WebUiTestHelpers.RandomStringFast(5), DotComDomain);
            var institutionNameSansSuffix = WebUiTestHelpers.RandomStringFast(5);
            var institutionName = string.Concat(institutionNameSansSuffix, " ", TestConstants.SitTestInstitutionSuffix);


            var home = NavigateToHomeIndexPage();
            home.ClickLoginLink();
            home.ClickRegisterLinkOnLoginView();
            home.EnterDetail(email, TestConstants.RegisterFieldsEmail);
            home.ClickSubmit();

            home.EnterDetail(TestConstants.SitTestPassword, TestConstants.RegisterFieldsPassword);
            home.EnterDetail(TestConstants.SitTestPassword, TestConstants.RegisterFieldsConfirmPassword);
            home.ClickSubmit();

            home.EnterDetail(TestConstants.SitTestZipCode, TestConstants.GetZipInput); 
             Thread.Sleep(500);
            home.ClickSubmit();

            home.EnterDetail(TestConstants.SitTestFirstName, TestConstants.FullNameInput);
            home.TabAwayFromInput(TestConstants.FullNameInput);
            home.EnterDetail(TestConstants.SitTestLastName, TestConstants.RegisterFieldsLastName);
            home.EnterDetail(TestConstants.Title, TestConstants.RegisterFieldsTitle);
            home.EnterDetail(institutionName, TestConstants.RegisterFieldsInstitution);
            home.EnterDetail(TestConstants.SitTestPhone, TestConstants.RegisterFieldsPhone);
            home.EnterDetail(TestConstants.SitTestAltAddress, TestConstants.RegisterFieldsStreetAddress);

            home.ClickSubmit();

            Assert.IsTrue(home.LogoutLinkIsPresentOnPage);

            home.LogOff();
        }

        public HomePage NavigateToHomeIndexPage()
        {
            var homeIndexPage = new HomePage(TestDriver);

            homeIndexPage.Open();

            return homeIndexPage;
        }
    }
}
