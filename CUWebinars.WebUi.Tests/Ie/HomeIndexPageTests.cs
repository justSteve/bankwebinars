using CUWebinars.WebUi.Tests.Page.Ie;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CUWebinars.WebUi.Tests.Ie
{
    [TestClass]
    public class HomeIndexPageTests : IeBaseTest
    {
        [TestMethod]
        [TestCategory(TestCategories.Gui)]
        [TestCategory(TestCategories.IE)]
        public void LoadHomePage()
        {
            var home = NavigateToHomeIndexPage();

            Assert.IsTrue(home.PhoneNrLinkIsPresentOnPage);
        }

        [TestMethod]
        [TestCategory(TestCategories.Gui)]
        [TestCategory(TestCategories.IE)]
        public void LoginToSite()
        {
            var home = NavigateToHomeIndexPage();
            home.ClickLoginLink();
            home.LogInToSite(Constants.SitTestEmailAddress, Constants.SitTestPassword);

            Assert.IsTrue(home.LogoutLinkIsPresentOnPage);

            home.LogOff();
        }

        [TestMethod]
        [TestCategory(TestCategories.Gui)]
        [TestCategory(TestCategories.IE)]
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
        [TestCategory(TestCategories.IE)]
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
        [TestCategory(TestCategories.IE)]
        public void ClickRegisterUserLinkWithEmailOfExistingDomainOfInstitution()
        {
            var firstName = WebUiTestHelpers.RandomString(8);
            var lastName = WebUiTestHelpers.RandomString(5);
            var email = string.Concat(firstName, "_", lastName, Constants.SitTestEmailAddressDomain);

            var home = NavigateToHomeIndexPage();
            home.ClickLoginLink();
            home.ClickRegisterLinkOnLoginView();
            home.EnterEmailAddress(email, "RegisterFields_Email");
            home.ClickSubmitButton();
            home.ClickYesUseAddressButton();
            home.EnterPassword(Constants.SitTestPasswordSameDomainAddress, "RegisterFields_Password");
            home.EnterPassword(Constants.SitTestPasswordSameDomainAddress, "RegisterFields.ConfirmPassword");
            home.ClickSubmitButton();
            home.EnterDetail(string.Concat(firstName, " ", lastName), "FullName");
            home.EnterDetail("Mr", "RegisterFields_Title");
            home.EnterDetail("555-555-555", "RegisterFields_BillingAddress_Phone");
            home.EnterDetail(Constants.SitTestAddress, "RegisterFields_BillingAddress_StreetAddress");
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
