using CUWebinars.WebUi.Tests.Infrastructure;
using CUWebinars.WebUi.Tests.Page.Firefox;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CUWebinars.WebUi.Tests.Firefox
{
    [TestClass]
    public class HomeIndexPageTests : FirefoxBaseTest
    {
        [TestMethod]
        [TestCategory(TestCategories.Gui)]
        public void LoadHomePage()
        {
            var homeIndexPage = NavigateToHomeIndexPage();

            Assert.IsTrue(homeIndexPage.PhoneNrLinkIsPresentOnPage);
        }

        [TestMethod]
        [TestCategory(TestCategories.Gui)]
        [TestCategory(TestCategories.Firefox)]
        public void LoginToSite()
        {
            var home = NavigateToHomeIndexPage();
            home.ClickLoginLink( );
            home.LogInToSite(Constants.SitTestEmailAddress, Constants.SitTestPassword);

            Assert.IsTrue(home.LogoutLinkIsPresentOnPage);
            
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
            home.EnterEmailAddressAndEnter(Constants.SitTestEmailAddress);
            home.EnterPasswordWhereUserExists(Constants.SitTestPassword);

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
            home.EnterEmailAddressAndEnter(Constants.SitTestEmailAddress);
            home.ClickResetPasswordButton();

            Assert.IsTrue(home.PasswordResetInstructionsSentLabelPresent); 
        }


        [TestMethod]
        [TestCategory(TestCategories.Gui)]
        [TestCategory(TestCategories.Firefox)]
        public void ClickRegisterUserLinkWithEmailOfExistingDomainOfInstitution()
        {
            var firstName = WebUiTestHelpers.RandomString(8);
            var lastName = WebUiTestHelpers.RandomString(5);
            var email = string.Concat(firstName, "_", lastName, Constants.SitTestEmailAddressDomain);

            var home = NavigateToHomeIndexPage();
            home.ClickLoginLink();
            home.ClickRegisterLinkOnLoginView();
            home.EnterEmailAddress(email, Constants.RegisterFieldsEmail);
            home.ClickSubmitButton();
            home.ClickYesUseAddressButton();
            home.EnterPassword(Constants.SitTestPasswordSameDomainAddress, Constants.RegisterFieldsPassword);
            home.EnterPassword(Constants.SitTestPasswordSameDomainAddress, Constants.RegisterFieldsConfirmPassword);
            home.ClickSubmitButton();
            home.EnterDetail(string.Concat(firstName, " ", lastName), Constants.FullNameInput);
            home.EnterDetail(Constants.Title, Constants.RegisterFieldsTitle);
            home.EnterDetail(Constants.SitTestPhone, Constants.RegisterFieldsPhone);
            home.EnterDetail(Constants.SitTestAddress, Constants.RegisterFieldsStreetAddress);
            home.ClickSubmitButton();

            Assert.IsTrue(home.ManageLoggedInUserLinkIsPresentOnPage);

            home.LogOff();
        }


        public HomeIndexPage NavigateToHomeIndexPage()
        {
            var homeIndexPage = new HomeIndexPage(TestDriver);

            homeIndexPage.Open();

            return homeIndexPage;
        }
    }
}
