using CUWebinars.Selenium.Core.Enums;
using CUWebinars.WebUi.Tests.Infrastructure;
using CUWebinars.WebUi.Tests.Page.Ie;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CUWebinars.WebUi.Tests.Ie
{
    [TestClass]
    public class HomeIndexPageTests : IeBaseTest
    {
        private const string DotComDomain = ".com";
        private const string EdgeCaseResetPasswordButton = "EdgeCaseResetPasswordButton";
        private const string NormalResetPasswordButton = "NormalResetPasswordButton";

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
            home.ClickResetPasswordButton(EdgeCaseResetPasswordButton);

            Assert.IsTrue(home.PasswordResetInstructionsSentLabelPresent);
        }


        [TestMethod]
        [TestCategory(TestCategories.Gui)]
        [TestCategory(TestCategories.IE)]
        public void ClickRegisterUserLinkWithNovelEmail()
        {
            var firstName = WebUiTestHelpers.RandomString(8);
            var lastName = WebUiTestHelpers.RandomString(5);
            var email = string.Concat(firstName, "_", lastName, "@", WebUiTestHelpers.RandomString(5), DotComDomain);
            var institutionNameSansSuffix = WebUiTestHelpers.RandomString(5);
            var institutionName = string.Concat(institutionNameSansSuffix, " ", Constants.SitTestInstitutionSuffix);


            var home = NavigateToHomeIndexPage();
            home.ClickLoginLink();
            home.ClickRegisterLinkOnLoginView();
            home.EnterDetail(email, Constants.RegisterFieldsEmail);
            home.ClickSubmitButton();

            home.EnterDetail(Constants.SitTestPassword, Constants.RegisterFieldsPassword);
            home.EnterDetail(Constants.SitTestPassword, Constants.RegisterFieldsConfirmPassword);
            home.ClickSubmitButton();

            home.EnterDetail(Constants.SitTestZipCode, Constants.GetZipInput);
            home.ClickSubmitButton();

            home.EnterDetail(Constants.SitTestFirstName, Constants.FullNameInput);
            home.TabAwayFromInput(Constants.FullNameInput, SelectorStrategy.Id);
            home.EnterDetail(Constants.SitTestLastName, Constants.RegisterFieldsLastName);
            home.EnterDetail(Constants.Title, Constants.RegisterFieldsTitle);
            home.EnterDetail(institutionName, Constants.RegisterFieldsInstitution);
            home.EnterDetail(Constants.SitTestPhone, Constants.RegisterFieldsPhone);
            home.EnterDetail(Constants.SitTestAltAddress, Constants.RegisterFieldsStreetAddress);

            home.ClickSubmitButton();

            Assert.IsTrue(home.LogoutLinkIsPresentOnPage);

            home.LogOff();
        }

        [TestMethod]
        [TestCategory(TestCategories.Gui)]
        [TestCategory(TestCategories.IE)]
        public void ClickRegisterUserLinkWithInValidEmail()
        {
            var firstName = WebUiTestHelpers.RandomString(8);
            var lastName = WebUiTestHelpers.RandomString(5);
            var email = string.Concat(firstName, "_", lastName);


            var home = NavigateToHomeIndexPage();
            home.ClickLoginLink();
            home.ClickRegisterLinkOnLoginView();
            home.EnterDetail(email, Constants.RegisterFieldsEmail);
            home.ClickSubmitButton();

            Assert.IsTrue(home.EmailNotValidMessageIsPresent);
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

        [TestMethod]
        [TestCategory(TestCategories.Gui)]
        [TestCategory(TestCategories.IE)]
        public void ClickRegisterUserLinkWithEmailOfExistingDomainOfInstitutionButNotWantToUseItsAddress()
        {
            var firstName = WebUiTestHelpers.RandomString(8);
            var lastName = WebUiTestHelpers.RandomString(5);
            var email = string.Concat(firstName, "_", lastName, Constants.SitTestEmailAddressDomain);
            var institutionNameSansSuffix = WebUiTestHelpers.RandomString(5);
            var institutionName = string.Concat(institutionNameSansSuffix, " ", Constants.SitTestInstitutionSuffix);


            var home = NavigateToHomeIndexPage();
            home.ClickLoginLink();

            

            home.ClickRegisterLinkOnLoginView();
            home.EnterEmailAddress(email, Constants.RegisterFieldsEmail);
            home.ClickSubmitButton();

            home.ClickNoEnterDiffAddressButton();
            home.ClickSubmitButton();

            home.EnterPassword(Constants.SitTestPasswordSameDomainAddress, Constants.RegisterFieldsPassword);
            home.EnterPassword(Constants.SitTestPasswordSameDomainAddress, Constants.RegisterFieldsConfirmPassword);
            home.ClickSubmitButton();

            home.EnterDetail(Constants.SitTestZipCode, Constants.GetZipInput);
            home.ClickSubmitButton();

            home.EnterDetail(Constants.SitTestFirstName, Constants.FullNameInput);
            home.TabAwayFromInput(Constants.FullNameInput, SelectorStrategy.Id);
            home.EnterDetail(Constants.SitTestLastName, Constants.RegisterFieldsLastName);
            home.EnterDetail(Constants.Title, Constants.RegisterFieldsTitle);
            home.EnterDetail(institutionName, Constants.RegisterFieldsInstitution);
            home.EnterDetail(Constants.SitTestPhone, Constants.RegisterFieldsPhone);
            home.EnterDetail(Constants.SitTestAltAddress, Constants.RegisterFieldsStreetAddress);

            home.ClickSubmitButton();

            Assert.IsTrue(home.LogoutLinkIsPresentOnPage);

            home.LogOff();

        }

        [TestMethod]
        [TestCategory(TestCategories.Gui)]
        [TestCategory(TestCategories.IE)]
        public void ClickRegisterUserLinkWithEmailOfExistingDomainOfInstitutionButActuallyNotTheUsersInstitution()
        {
            var firstName = WebUiTestHelpers.RandomString(8);
            var lastName = WebUiTestHelpers.RandomString(5);
            var email = string.Concat(firstName, "_", lastName, Constants.SitTestEmailAddressDomain);
            var institutionNameSansSuffix = WebUiTestHelpers.RandomString(5);
            var institutionName = string.Concat(institutionNameSansSuffix, " ", Constants.SitTestInstitutionSuffix);


            var home = NavigateToHomeIndexPage();
            home.ClickLoginLink();

            

            home.ClickRegisterLinkOnLoginView();
            home.EnterEmailAddress(email, Constants.RegisterFieldsEmail);
            home.ClickSubmitButton();

            home.ClickNotInstitutionButton();
            home.Wait(500);
            bool proceedMessageDisplayed = home.ProceedOrEnterDifferentEmailMessagePresent;
            home.ClickSubmitButton();

            home.EnterPassword(Constants.SitTestPasswordSameDomainAddress, Constants.RegisterFieldsPassword);
            home.EnterPassword(Constants.SitTestPasswordSameDomainAddress, Constants.RegisterFieldsConfirmPassword);
            home.ClickSubmitButton();

            home.EnterDetail(Constants.SitTestZipCode, Constants.GetZipInput);
            home.ClickSubmitButton();

            home.EnterDetail(Constants.SitTestFirstName, Constants.FullNameInput);
            home.TabAwayFromInput(Constants.FullNameInput, SelectorStrategy.Id);
            home.Wait(500);
            home.EnterDetail(Constants.SitTestLastName, Constants.RegisterFieldsLastName);
            home.EnterDetail(Constants.Title, Constants.RegisterFieldsTitle);
            home.EnterDetail(institutionName, Constants.RegisterFieldsInstitution);
            home.EnterDetail(Constants.SitTestPhone, Constants.RegisterFieldsPhone);
            home.EnterDetail(Constants.SitTestAltAddress, Constants.RegisterFieldsStreetAddress);

            home.ClickSubmitButton();

            Assert.IsTrue(home.LogoutLinkIsPresentOnPage);
            Assert.IsTrue(proceedMessageDisplayed);

            home.LogOff();

        }

        [TestMethod]
        [TestCategory(TestCategories.Gui)]
        [TestCategory(TestCategories.IE)]
        public void ResetPassword()
        {
            var firstName = WebUiTestHelpers.RandomString(8);
            var lastName = WebUiTestHelpers.RandomString(5);
            var email = string.Concat(firstName, "_", lastName, "@", WebUiTestHelpers.RandomString(5), DotComDomain);
            var institutionNameSansSuffix = WebUiTestHelpers.RandomString(5);
            var institutionName = string.Concat(institutionNameSansSuffix, " ", Constants.SitTestInstitutionSuffix);


            var home = NavigateToHomeIndexPage();

            

            home.ClickLoginLink();
            home.ClickRegisterLinkOnLoginView();
            home.EnterDetail(email, Constants.RegisterFieldsEmail);
            home.ClickSubmitButton();

            home.EnterDetail(Constants.SitTestPassword, Constants.RegisterFieldsPassword);
            home.EnterDetail(Constants.SitTestPassword, Constants.RegisterFieldsConfirmPassword);
            home.ClickSubmitButton();

            home.EnterDetail(Constants.SitTestZipCode, Constants.GetZipInput);
            home.ClickSubmitButton();

            home.EnterDetail(Constants.SitTestFirstName, Constants.FullNameInput);
            home.TabAwayFromInput(Constants.FullNameInput, SelectorStrategy.Id);
            home.EnterDetail(Constants.SitTestLastName, Constants.RegisterFieldsLastName);
            home.EnterDetail(Constants.Title, Constants.RegisterFieldsTitle);
            home.EnterDetail(institutionName, Constants.RegisterFieldsInstitution);
            home.EnterDetail(Constants.SitTestPhone, Constants.RegisterFieldsPhone);
            home.EnterDetail(Constants.SitTestAltAddress, Constants.RegisterFieldsStreetAddress);

            home.ClickSubmitButton();

            home.LogOff();

            home.ClickLoginLink();
            home.ClickResetPasswordLink();
            home.EnterEmailAddress(email, "ResetPassEmail");
            home.ClickResetPasswordButton(NormalResetPasswordButton);

            Assert.IsTrue(home.PasswordResetInstructionsInCrunchingLabelPresent);
        }

        [TestMethod]
        [TestCategory(TestCategories.Gui)]
        [TestCategory(TestCategories.IE)]
        public void ResetPasswordWithInvalidEmail()
        {
            var firstName = WebUiTestHelpers.RandomString(8);
            var lastName = WebUiTestHelpers.RandomString(5);
            var email = string.Concat(firstName, "_", lastName, WebUiTestHelpers.RandomString(5), DotComDomain);


            var home = NavigateToHomeIndexPage();

            

            home.ClickLoginLink();
            home.ClickResetPasswordLink();
            home.EnterEmailAddress(email, "ResetPassEmail");
            home.ClickResetPasswordButton(NormalResetPasswordButton);


            Assert.IsTrue(home.PleaseEnterValidEmailMessageIsPresent);
        }


        [TestMethod]
        [TestCategory(TestCategories.Gui)]
        [TestCategory(TestCategories.IE)]
        public void NavigateToWebinarDetailsPage()
        {
            var home = NavigateToHomeIndexPage();

            var newPageDisplayed = home.ClickUpcomingEventsMenuItem();

            Assert.IsTrue(newPageDisplayed);
        }


        public HomeIndexPage NavigateToHomeIndexPage()
        {
            var homeIndexPage = new HomeIndexPage(TestDriver);

            homeIndexPage.Open();

            return homeIndexPage;
        }

    }
}
