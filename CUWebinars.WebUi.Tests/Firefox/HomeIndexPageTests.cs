using CUWebinars.Selenium.Core.Enums;
using CUWebinars.WebUi.Tests.Infrastructure;
using CUWebinars.WebUi.Tests.Page.Firefox;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace CUWebinars.WebUi.Tests.Firefox
{
    [TestClass]
    public class HomeIndexPageTests : FirefoxBaseTest
    {
        private const string DotComDomain = ".com";
        private const string EdgeCaseResetPasswordButton = "EdgeCaseResetPasswordButton";
        private const string NormalResetPasswordButton = "NormalResetPasswordButton";

        [TestMethod]
        [TestCategory(TestCategories.Gui)]
        [TestCategory(TestCategories.Firefox)]
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
            home.ClickResetPasswordButton(EdgeCaseResetPasswordButton);

            Assert.IsTrue(home.PasswordResetInstructionsSentLabelPresent); 
        }


        [TestMethod]
        [TestCategory(TestCategories.Gui)]
        [TestCategory(TestCategories.Firefox)]
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
        [TestCategory(TestCategories.Firefox)]
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

        [TestMethod]
        [TestCategory(TestCategories.Gui)]
        [TestCategory(TestCategories.Firefox)]
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
        [TestCategory(TestCategories.Firefox)]
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
        [TestCategory(TestCategories.Firefox)]
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

            home.Close();
        }

        [TestMethod]
        [TestCategory(TestCategories.Gui)]
        [TestCategory(TestCategories.Firefox)]
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

            home.Close();
        }

        public HomeIndexPage NavigateToHomeIndexPage()
        {
            var homeIndexPage = new HomeIndexPage(TestDriver);

            homeIndexPage.Open();

            return homeIndexPage;
        }
    }
}
