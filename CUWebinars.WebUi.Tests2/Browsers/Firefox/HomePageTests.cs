using CUWebinars.WebUi.Tests2.Infrastructure;
using CUWebinars.WebUi.Tests2.Pages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace CUWebinars.WebUi.Tests2.Browsers.Firefox
{
    [TestClass]
    public class HomePageTests : FirefoxBaseTest
    {
        private const string DotComSuffix = ".com";
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
            if(home.ClickResetPasswordButton(EdgeCaseResetPasswordButton))
                Assert.IsTrue(home.PasswordResetInstructionsSentLabelPresent);
            else
                Assert.Fail();
        }

        [TestMethod]
        [TestCategory(TestCategories.Gui)]
        [TestCategory(TestCategories.Firefox)]
        public void ClickRegisterUserLinkWithNovelEmail()
        {
            var firstName = WebUiTestHelpers.RandomStringFast(8);
            var lastName = WebUiTestHelpers.RandomStringFast(5);
            var email = string.Concat(firstName, "_", lastName, "@", WebUiTestHelpers.RandomStringFast(5), DotComSuffix);
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

            if (home.ClickSubmit())
            {
                Trace.WriteLine("Click was true");

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
            else
            {
                Assert.Fail();
            }

        }
        
        [TestMethod]
        [TestCategory(TestCategories.Gui)]
        [TestCategory(TestCategories.Firefox)]
        public void ClickRegisterUserLinkWithInValidEmail()
        {
            var firstName = WebUiTestHelpers.RandomStringFast(8);
            var lastName = WebUiTestHelpers.RandomStringFast(5);
            var email = string.Concat(firstName, "_", lastName);


            var home = NavigateToHomeIndexPage();
            home.ClickLoginLink();
            home.ClickRegisterLinkOnLoginView();
            home.EnterDetail(email, TestConstants.RegisterFieldsEmail);
            home.ClickSubmit();

            Assert.IsTrue(true);
            Assert.IsTrue(home.EmailNotValidMessageIsPresent);
        }

        [TestMethod]
        [TestCategory(TestCategories.Gui)]
        [TestCategory(TestCategories.Firefox)]
        public void ClickRegisterUserLinkWithEmailOfExistingDomainOfInstitution()
        {
            var firstName = WebUiTestHelpers.RandomStringFast(8);
            var lastName = WebUiTestHelpers.RandomStringFast(5);
            var email = string.Concat(firstName, "_", lastName, TestConstants.SitTestEmailAddressDomain);

            var home = NavigateToHomeIndexPage();
            home.ClickLoginLink();
            home.ClickRegisterLinkOnLoginView();
            home.EnterDetail(email, TestConstants.RegisterFieldsEmail);
            home.ClickSubmit();
            home.ClickYesUseAddressButton();
            home.EnterDetail(TestConstants.SitTestPasswordSameDomainAddress, TestConstants.RegisterFieldsPassword);
            home.EnterDetail(TestConstants.SitTestPasswordSameDomainAddress, TestConstants.RegisterFieldsConfirmPassword);
            home.ClickSubmit();
            home.EnterDetail(string.Concat(firstName, " ", lastName), TestConstants.FullNameInput);
            home.EnterDetail(TestConstants.Title, TestConstants.RegisterFieldsTitle);
            home.EnterDetail(TestConstants.SitTestPhone, TestConstants.RegisterFieldsPhone);
            home.EnterDetail(TestConstants.SitTestAddress, TestConstants.RegisterFieldsStreetAddress);
            home.ClickSubmit();

            Assert.IsTrue(home.ManageLoggedInUserLinkIsPresentOnPage);

            home.LogOff();
        }

        [TestMethod]
        [TestCategory(TestCategories.Gui)]
        [TestCategory(TestCategories.Firefox)]
        public void ClickRegisterUserLinkWithEmailOfExistingDomainOfInstitutionButNotWantToUseItsAddress()
        {
            var firstName = WebUiTestHelpers.RandomStringFast(8);
            var lastName = WebUiTestHelpers.RandomStringFast(5);
            var email = string.Concat(firstName, "_", lastName, TestConstants.SitTestEmailAddressDomain);
            var institutionNameSansSuffix = WebUiTestHelpers.RandomStringFast(5);
            var institutionName = string.Concat(institutionNameSansSuffix, " ", TestConstants.SitTestInstitutionSuffix);


            var home = NavigateToHomeIndexPage();
            home.ClickLoginLink();
            home.ClickRegisterLinkOnLoginView();
            home.EnterDetail(email, TestConstants.RegisterFieldsEmail);
            home.ClickSubmit();

            home.ClickNoEnterDiffAddressButton();
            home.ClickSubmit();

            home.EnterDetail(TestConstants.SitTestPasswordSameDomainAddress, TestConstants.RegisterFieldsPassword);
            home.EnterDetail(TestConstants.SitTestPasswordSameDomainAddress, TestConstants.RegisterFieldsConfirmPassword);
            home.ClickSubmit();

            home.EnterDetail(TestConstants.SitTestZipCode, TestConstants.GetZipInput);
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

        [TestMethod]
        [TestCategory(TestCategories.Gui)]
        [TestCategory(TestCategories.Firefox)]
        public void ClickRegisterUserLinkWithEmailOfExistingDomainOfInstitutionButActuallyNotTheUsersInstitution()
        {
            var firstName = WebUiTestHelpers.RandomStringFast(8);
            var lastName = WebUiTestHelpers.RandomStringFast(5);
            var email = string.Concat(firstName, "_", lastName, TestConstants.SitTestEmailAddressDomain);
            var institutionNameSansSuffix = WebUiTestHelpers.RandomStringFast(5);
            var institutionName = string.Concat(institutionNameSansSuffix, " ", TestConstants.SitTestInstitutionSuffix);


            var home = NavigateToHomeIndexPage();
            home.ClickLoginLink();
            home.ClickRegisterLinkOnLoginView();
            home.EnterDetail(email, TestConstants.RegisterFieldsEmail);
            home.ClickSubmit();

            home.ClickNotInstitutionButton();
            bool proceedMessageDisplayed = home.ProceedOrEnterDifferentEmailMessagePresent;
            home.ClickSubmit();

            home.EnterDetail(TestConstants.SitTestPasswordSameDomainAddress, TestConstants.RegisterFieldsPassword);
            home.EnterDetail(TestConstants.SitTestPasswordSameDomainAddress, TestConstants.RegisterFieldsConfirmPassword);
            home.ClickSubmit();

            home.EnterDetail(TestConstants.SitTestZipCode, TestConstants.GetZipInput);
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
            Assert.IsTrue(proceedMessageDisplayed);

            home.LogOff();

        }

        [TestMethod]
        [TestCategory(TestCategories.Gui)]
        [TestCategory(TestCategories.Firefox)]
        public void ResetPassword()
        {
            var firstName = WebUiTestHelpers.RandomStringFast(8);
            var lastName = WebUiTestHelpers.RandomStringFast(5);
            var email = string.Concat(firstName, "_", lastName, "@", WebUiTestHelpers.RandomStringFast(5), DotComSuffix);
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
            home.ClickSubmit();

            home.EnterDetail(TestConstants.SitTestFirstName, TestConstants.FullNameInput);
            home.TabAwayFromInput(TestConstants.FullNameInput);
            home.EnterDetail(TestConstants.SitTestLastName, TestConstants.RegisterFieldsLastName);
            home.EnterDetail(TestConstants.Title, TestConstants.RegisterFieldsTitle);
            home.EnterDetail(institutionName, TestConstants.RegisterFieldsInstitution);
            home.EnterDetail(TestConstants.SitTestPhone, TestConstants.RegisterFieldsPhone);
            home.EnterDetail(TestConstants.SitTestAltAddress, TestConstants.RegisterFieldsStreetAddress);

            home.ClickSubmit();

            home.LogOff();
            home.ClickLoginLink();
            home.ClickResetPasswordLink();
            home.EnterDetail(email, "ResetPassEmail");
            home.ClickResetPasswordButton(NormalResetPasswordButton);

            Assert.IsTrue(home.PasswordResetInstructionsInCrunchingLabelPresent);

            home.Close();
        }

        [TestMethod]
        [TestCategory(TestCategories.Gui)]
        [TestCategory(TestCategories.Firefox)]
        public void NavigateToWebinarDetailsPage()
        {
            var home = NavigateToHomeIndexPage();

            var newPageDisplayed = home.ClickUpcomingEventsMenuItem();

            Assert.IsTrue(newPageDisplayed);
        }

        [TestMethod]
        [TestCategory(TestCategories.Gui)]
        [TestCategory(TestCategories.Firefox)]
        public void NavigateToWebinarDetailsPageListedByTopic()
        {
            var home = NavigateToHomeIndexPage();

            var newPageDisplayed = home.ClickTopicsMenuItem();

            Assert.IsTrue(newPageDisplayed);
        }

        [TestMethod]
        [TestCategory(TestCategories.Gui)]
        [TestCategory(TestCategories.Firefox)]
        public void NavigateToWebinarDetailsPageListedByTopics()
        {
            var home = NavigateToHomeIndexPage();

            var newPageDisplayed = home.ClickTopicsMenuItem();

            home.ClickMoreButtonOnTopicsPage();

            Assert.IsTrue(home.WebinarTitleIsDisplayedOnWebinarDetailsPage);
        }

        public HomePage NavigateToHomeIndexPage()
        {
            var homeIndexPage = new HomePage(TestDriver);

            homeIndexPage.Open();

            return homeIndexPage;
        }
    }
}
