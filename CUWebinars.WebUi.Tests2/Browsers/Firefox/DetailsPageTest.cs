using CUWebinars.Tests.Common;
using CUWebinars.WebUi.Tests2.Infrastructure;
using CUWebinars.WebUi.Tests2.Pages;
using KesselRun.SeleniumCore.TestDrivers.Browsers.Firefox;
using KesselRun.SeleniumCore.TestDrivers.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Linq;


namespace CUWebinars.WebUi.Tests2.Browsers.Firefox
{
    [TestClass]
    public class DetailsPageTest : FirefoxBaseTest
    {
        private const string RegisterFieldsEmailUnderscoreDelimited = "RegisterFields_Email";
        private const string RegisterFieldsEmailDotDelimited = "RegisterFields.Email";
        private const string DotComSuffix = ".com";
        private const string FullName = "John Hancock";
        private const string Title = "Mr";
        private const string Institution = "ACME Bankers";
        private const string StreetAddress = "10 Main St";
        private const string ZipCodeHolmen = "54636";
        private const string PhoneNumber = "(222) 222-2222";
        private const string ZipInput = "getZip";
        private const string FullNameInput = "FullName";
        private const string RegisterfieldsInput = "RegisterFields_Title";
        private const string RegisterfieldsInstitution = "RegisterFields_Institution";
        private const string RegisterfieldsBillingaddressStreetaddress = "RegisterFields_BillingAddress_StreetAddress";
        private const string RegisterfieldsBillingaddressPhone = "RegisterFields_BillingAddress_Phone";
        private const string CommonDomain = "yahoo.com.au";
        private const string TestUserPassword = "Password1";
        //private readonly string TestEmailAddress = "auser@" + CommonDomain;


        [TestMethod]
        [TestCategory(TestCategories.Firefox)]
        public void LoadDetailsPage()
        {
            var page = NavigateToDetailsPage();

            Assert.IsTrue(page.WebinarTitleIsDisplayedOnWebinarDetailsPage);
        }

        [TestMethod]
        [TestCategory(TestCategories.Firefox)]
        public void AnonymousUserWithExistingDomainMakesOrderSuccessfullyUsingSubmit()
        {
            var page = NavigateToDetailsPage();

            page.ClickSignUpButton();

            var email = TestHelper.RandomStringFast(8) + "@" + CommonDomain;

            page.EnterEmailAddressAndClickSubmit(email);
            page.ClickYesUseAddressButton();

            page.EnterDetail(FullName, FullNameInput);
            page.EnterDetail(Title, RegisterfieldsInput);
            page.EnterDetail(StreetAddress, RegisterfieldsBillingaddressStreetaddress);
            page.EnterDetail(PhoneNumber, RegisterfieldsBillingaddressPhone);

            page.ClickSubmit();

            page.ClickBillMeButton();

            Assert.IsTrue(page.WebinarTitleIsDisplayedOnWebinarDetailsPage);
        }

        [TestMethod]
        [TestCategory(TestCategories.Firefox)]
        public void AnonymousUserWithExistingDomainMakesOrderSuccessfullyUsingEnter()
        {
            var page = NavigateToDetailsPage();

            page.ClickSignUpButton();

            var email = TestHelper.RandomStringFast(8) + "@" + CommonDomain;

            page.EnterDetailThenPressEnter(email, RegisterFieldsEmailUnderscoreDelimited);
            page.ClickYesUseAddressButton();

            page.EnterDetail(FullName, FullNameInput);
            page.EnterDetail(Title, RegisterfieldsInput);
            page.EnterDetail(StreetAddress, RegisterfieldsBillingaddressStreetaddress);

            page.EnterDetailThenPressEnter(PhoneNumber, RegisterfieldsBillingaddressPhone);

            page.ClickBillMeButton();

            Assert.IsTrue(page.WebinarTitleIsDisplayedOnWebinarDetailsPage);
        }

        [TestMethod]
        [TestCategory(TestCategories.Firefox)]
        public void AnonymousUserWithExistingDomainButDoesntUseAddressOfInstitutionMakesOrderSuccessfullyUsingSubmit()
        {
            var page = NavigateToDetailsPage();

            page.ClickSignUpButton();

            var email = TestHelper.RandomStringFast(8) + "@" + CommonDomain;

            page.EnterEmailAddressAndClickSubmit(email);

            page.ClickNoEnterDiffAddressButton();

            page.EnterDetail(email, RegisterFieldsEmailUnderscoreDelimited);
            page.ClickSubmit();

            page.EnterDetail(ZipCodeHolmen, ZipInput);
            page.ClickSubmit();

            page.EnterDetail(FullName, FullNameInput);
            page.EnterDetail(Title, RegisterfieldsInput);
            page.EnterDetail(Institution, RegisterfieldsInstitution);
            page.EnterDetail(StreetAddress, RegisterfieldsBillingaddressStreetaddress);
            page.EnterDetail(PhoneNumber, RegisterfieldsBillingaddressPhone);

            page.ClickSubmit();

            page.ClickBillMeButton();

            Assert.IsTrue(page.WebinarTitleIsDisplayedOnWebinarDetailsPage);
        }

        [TestMethod]
        [TestCategory(TestCategories.Firefox)]
        public void AnonymousUserWithExistingDomainButDoesntUseAddressOfInstitutionMakesOrderSuccessfullyUsingEnter()
        {
            var page = NavigateToDetailsPage();

            page.ClickSignUpButton();

            var email = TestHelper.RandomStringFast(8) + "@" + CommonDomain;

            page.EnterDetailThenPressEnter(email, RegisterFieldsEmailUnderscoreDelimited);

            page.ClickNoEnterDiffAddressButton();

            page.EnterDetailThenPressEnter(email, RegisterFieldsEmailUnderscoreDelimited);

            page.EnterDetailThenPressEnter(ZipCodeHolmen, ZipInput);

            page.EnterDetail(FullName, FullNameInput);
            page.EnterDetail(Title, RegisterfieldsInput);
            page.EnterDetail(Institution, RegisterfieldsInstitution);
            page.EnterDetail(StreetAddress, RegisterfieldsBillingaddressStreetaddress);
            page.EnterDetailThenPressEnter(PhoneNumber, RegisterfieldsBillingaddressPhone);

            page.ClickBillMeButton();

            Assert.IsTrue(page.WebinarTitleIsDisplayedOnWebinarDetailsPage);
        }

        [TestMethod]
        [TestCategory(TestCategories.Firefox)]
        public void AnonymousUserWithExistingDomainButNotFromInstitutionMakesOrderSuccessfullyUsingSubmit()
        {
            var page = NavigateToDetailsPage();

            page.ClickSignUpButton();

            var email = TestHelper.RandomStringFast(8) + "@" + CommonDomain;

            page.EnterEmailAddressAndClickSubmit(email);

            page.ClickNotInstitutionButton();

            page.EnterDetail(email, RegisterFieldsEmailUnderscoreDelimited);
            page.ClickSubmit();

            page.EnterDetail(ZipCodeHolmen, ZipInput);
            page.ClickSubmit();

            page.EnterDetail(FullName, FullNameInput);
            page.EnterDetail(Title, RegisterfieldsInput);
            page.EnterDetail(Institution, RegisterfieldsInstitution);
            page.EnterDetail(StreetAddress, RegisterfieldsBillingaddressStreetaddress);
            page.EnterDetail(PhoneNumber, RegisterfieldsBillingaddressPhone);

            page.ClickSubmit();

            page.ClickBillMeButton();

            Assert.IsTrue(page.WebinarTitleIsDisplayedOnWebinarDetailsPage);
        }

        [TestMethod]
        [TestCategory(TestCategories.Firefox)]
        public void AnonymousUserWithExistingDomainButNotFromInstitutionMakesOrderSuccessfullyUsingEnter()
        {
            var page = NavigateToDetailsPage();

            page.ClickSignUpButton();

            var email = TestHelper.RandomStringFast(8) + "@" + CommonDomain;

            page.EnterDetailThenPressEnter(email, RegisterFieldsEmailUnderscoreDelimited);

            page.ClickNotInstitutionButton();

            page.EnterDetailThenPressEnter(email, RegisterFieldsEmailUnderscoreDelimited);

            page.EnterDetailThenPressEnter(ZipCodeHolmen, ZipInput);

            page.EnterDetail(FullName, FullNameInput);
            page.EnterDetail(Title, RegisterfieldsInput);
            page.EnterDetail(Institution, RegisterfieldsInstitution);
            page.EnterDetail(StreetAddress, RegisterfieldsBillingaddressStreetaddress);
            page.EnterDetailThenPressEnter(PhoneNumber, RegisterfieldsBillingaddressPhone);

            page.ClickBillMeButton();

            Assert.IsTrue(page.WebinarTitleIsDisplayedOnWebinarDetailsPage);
        }

        [TestMethod]
        [TestCategory(TestCategories.Firefox)]
        public void AnonymousUserWithExistingDomainButClicksCrossOnModalMakesOrderSuccessfullyUsingSubmit()
        {
            var page = NavigateToDetailsPage();

            page.ClickSignUpButton();

            var email = TestHelper.RandomStringFast(8) + "@" + CommonDomain;

            page.EnterEmailAddressAndClickSubmit(email);

            page.ClickCancelModalButton();

            page.EnterDetail(email, RegisterFieldsEmailUnderscoreDelimited);
            page.ClickSubmit();

            page.EnterDetail(ZipCodeHolmen, ZipInput);
            page.ClickSubmit();

            page.EnterDetail(FullName, FullNameInput);
            page.EnterDetail(Title, RegisterfieldsInput);
            page.EnterDetail(Institution, RegisterfieldsInstitution);
            page.EnterDetail(StreetAddress, RegisterfieldsBillingaddressStreetaddress);
            page.EnterDetail(PhoneNumber, RegisterfieldsBillingaddressPhone);

            page.ClickSubmit();

            page.ClickBillMeButton();

            Assert.IsTrue(page.WebinarTitleIsDisplayedOnWebinarDetailsPage);
        }

        [TestMethod]
        [TestCategory(TestCategories.Firefox)]
        public void AnonymousUserWithExistingDomainButClicksCrossOnModalMakesOrderSuccessfullyUsingEnter()
        {
            var page = NavigateToDetailsPage();

            page.ClickSignUpButton();

            var email = TestHelper.RandomStringFast(8) + "@" + CommonDomain;

            page.EnterDetailThenPressEnter(email, RegisterFieldsEmailUnderscoreDelimited);

            page.ClickCancelModalButton();

            page.EnterDetailThenPressEnter(email, RegisterFieldsEmailUnderscoreDelimited);

            page.EnterDetailThenPressEnter(ZipCodeHolmen, ZipInput);

            page.EnterDetail(FullName, FullNameInput);
            page.EnterDetail(Title, RegisterfieldsInput);
            page.EnterDetail(Institution, RegisterfieldsInstitution);
            page.EnterDetail(StreetAddress, RegisterfieldsBillingaddressStreetaddress);
            page.EnterDetailThenPressEnter(PhoneNumber, RegisterfieldsBillingaddressPhone);

            page.ClickBillMeButton();

            Assert.IsTrue(page.WebinarTitleIsDisplayedOnWebinarDetailsPage);
        }

        [TestMethod]
        [TestCategory(TestCategories.Firefox)]
        public void AnonymousUserWithoutExistingDomainMakesOrderSuccessfullyUsingSubmit()
        {
            var page = NavigateToDetailsPage();

            page.ClickSignUpButton();

            var email = string.Concat(TestHelper.RandomStringFast(8), "@", TestHelper.RandomStringFast(4), DotComSuffix);

            page.EnterEmailAddressAndClickSubmit(email);

            page.EnterDetail(ZipCodeHolmen, ZipInput);
            page.ClickSubmit();

            page.EnterDetail(FullName, FullNameInput);
            page.EnterDetail(Title, RegisterfieldsInput);
            page.EnterDetail(Institution, RegisterfieldsInstitution);
            page.EnterDetail(StreetAddress, RegisterfieldsBillingaddressStreetaddress);
            page.EnterDetail(PhoneNumber, RegisterfieldsBillingaddressPhone);

            page.ClickSubmit();

            page.ClickBillMeButton();

            Assert.IsTrue(page.WebinarTitleIsDisplayedOnWebinarDetailsPage);
        }

        [TestMethod]
        [TestCategory(TestCategories.Firefox)]
        public void UserAddsTwoLocationsAndMakesOrderSuccessfully()
        {
            var page = NavigateToDetailsPage();

            /*********** Additional Locations modal *****************/
            page.ClickAddAdditionalLocationButton();
            page.ClickTheClickToAddMoreButton();
            page.EnterDetail("drogersbox-test1@yahoo.com.au", "AdditionalLocationEmail_0");
            page.ClickTheClickToAddMoreButton();
            page.EnterDetail("drogersbox-test2@yahoo.com.au", "AdditionalLocationEmail_1");
            page.ClickSubmitAdditionalLocationsButton();
            /********************************************************/

            Assert.AreEqual(2, page.GetNumberOfAdditionalLocationTextBoxes());
        }

        [TestMethod]
        [TestCategory(TestCategories.Firefox)]
        public void UserAddsTwoLocationsThenDeletesOneAfterClosingModalAndMakesOrderSuccessfully()
        {
            var page = NavigateToDetailsPage();
            int id = 0;

            /*********** Additional Locations modal *****************/
            page.ClickAddAdditionalLocationButton();
            page.ClickTheClickToAddMoreButton();
            page.EnterDetail("drogersbox-test1@yahoo.com.au", "AdditionalLocationEmail_0");
            page.ClickTheClickToAddMoreButton();
            page.EnterDetail("drogersbox-test2@yahoo.com.au", "AdditionalLocationEmail_1");
            page.ClickSubmitAdditionalLocationsButton();
            page.Wait(1000);
            page.DeleteAdditionalLocation(id);
            page.Wait(500);

            Assert.AreEqual(1, page.GetNumberOfAdditionalLocationTextBoxes());
            /********************************************************/
        }

        [TestMethod]
        [TestCategory(TestCategories.Firefox)]
        public void UserAddsTwoLocationsThenDeletesOneAfterClosingModalAndAddsItAgainAndMakesOrderSuccessfully()
        {
            var page = NavigateToDetailsPage();
            int id = 0;

            /*********** Additional Locations modal *****************/
            page.ClickAddAdditionalLocationButton();
            page.ClickTheClickToAddMoreButton();
            page.EnterDetail("drogersbox-test1@yahoo.com.au", "AdditionalLocationEmail_0");
            page.ClickTheClickToAddMoreButton();
            page.EnterDetail("drogersbox-test2@yahoo.com.au", "AdditionalLocationEmail_1");
            page.ClickSubmitAdditionalLocationsButton();
            page.Wait(1000);
            page.DeleteAdditionalLocation(id);
            page.Wait(500);
            page.ClickAddAdditionalLocationButton();
            page.ClickTheClickToAddMoreButton();
            page.Wait(1000);
            page.EnterDetail("drogersbox-test2@yahoo.com.au", "AdditionalLocationEmail_2");
            page.ClickSubmitAdditionalLocationsButton();
            page.Wait(10000);
            Assert.AreEqual(2, page.GetNumberOfAdditionalLocationTextBoxes());
            /********************************************************/
        }

        [TestMethod]
        [TestCategory(TestCategories.Firefox)]
        public void UserAddsTwoLocationsThenClosesModalThenOpensItThenAddsOneLocationThenClicksCancelLeavingTwoLocations()
        {
            var page = NavigateToDetailsPage();
            int id = 0;

            /*********** Additional Locations modal *****************/
            page.ClickAddAdditionalLocationButton();
            page.ClickTheClickToAddMoreButton();
            page.EnterDetail("drogersbox-test1@yahoo.com.au", "AdditionalLocationEmail_0");
            page.ClickTheClickToAddMoreButton();
            page.EnterDetail("drogersbox-test2@yahoo.com.au", "AdditionalLocationEmail_1");
            page.ClickSubmitAdditionalLocationsButton();
            page.Wait(1000);
            page.DeleteAdditionalLocation(id);
            page.Wait(500);
            page.ClickAddAdditionalLocationButton();
            page.ClickTheClickToAddMoreButton();
            page.EnterDetail("drogersbox-test2@yahoo.com.au", "AdditionalLocationEmail_2");
            page.ClickTheClickToAddMoreButton();
            page.EnterDetail("drogersbox-test2@yahoo.com.au", "AdditionalLocationEmail_3");
            page.ClickCancelAdditionalLocationsModalButton();
            page.Wait(1500);
            Assert.AreEqual(1, page.GetNumberOfAdditionalLocationTextBoxes());
            /********************************************************/
        }

        [TestMethod]
        [TestCategory(TestCategories.Firefox)]
        public void UserAddsTwoLocationsThenClosesModalThenOpensItThenDeletesThemAllAndSubmitButtonDisappears()
        {
            var page = NavigateToDetailsPage();
            int id = 0;

            /*********** Additional Locations modal *****************/
            page.ClickAddAdditionalLocationButton();
            page.ClickTheClickToAddMoreButton();
            page.EnterDetail("drogersbox-test1@yahoo.com.au", "AdditionalLocationEmail_0");
            page.ClickTheClickToAddMoreButton();
            page.EnterDetail("drogersbox-test2@yahoo.com.au", "AdditionalLocationEmail_1");
            page.Wait();
            page.ClickSubmitAdditionalLocationsButton();
            page.Wait(500);
            page.ClickAddAdditionalLocationButton();
            page.Wait(500);
            page.DeleteAdditionalLocation(id);
            page.Wait(500);
            page.DeleteAdditionalLocation(id + 1);
            page.Wait(2000);

            Assert.IsTrue(page.SumbitAdditionalLocationsButtonIsNotThere);
            /********************************************************/
        }

        [TestMethod]
        [TestCategory(TestCategories.Firefox)]
        public void UserLogsInAndMakesSimpleOrder()
        {
            var institutionNameSansSuffix = WebUiTestHelpers.RandomStringFast(5);
            var institutionName = string.Concat(institutionNameSansSuffix, " ", Infrastructure.TestConstants.SitTestInstitutionSuffix);

            var firstName = WebUiTestHelpers.RandomStringFast(8);
            var lastName = WebUiTestHelpers.RandomStringFast(5);
            var email = string.Concat(firstName, "_", lastName, "@", WebUiTestHelpers.RandomStringFast(5), DotComSuffix);

            var page = NavigateToDetailsPageViaRegisterWizard();

            page.ClickLoginLink();
            page.ClickRegisterLinkOnLoginView();
            page.EnterDetail(email, Infrastructure.TestConstants.RegisterFieldsEmail);

            page.ClickSubmit();

            page.EnterDetail(Infrastructure.TestConstants.SitTestPassword, Infrastructure.TestConstants.RegisterFieldsPassword);
            page.EnterDetail(Infrastructure.TestConstants.SitTestPassword, Infrastructure.TestConstants.RegisterFieldsConfirmPassword);
            page.ClickSubmit();

            page.EnterDetail(Infrastructure.TestConstants.SitTestZipCode, Infrastructure.TestConstants.GetZipInput);

            page.ClickSubmit();

            page.EnterDetail(Infrastructure.TestConstants.SitTestFirstName + " " + "Last", Infrastructure.TestConstants.FullNameInput);

            page.EnterDetail(Infrastructure.TestConstants.Title, Infrastructure.TestConstants.RegisterFieldsTitle);
            page.EnterDetail(institutionName, Infrastructure.TestConstants.RegisterFieldsInstitution);
            page.EnterDetail(Infrastructure.TestConstants.SitTestPhone, Infrastructure.TestConstants.RegisterFieldsPhone);
            page.EnterDetail(Infrastructure.TestConstants.SitTestAltAddress, Infrastructure.TestConstants.RegisterFieldsStreetAddress);

            page.ClickSubmit();
            page.WaitForLogOutLink();

            page.ClickWebinarsMenuItem(3);

            page.ClickSignUpButton();

            page.ClickBillMeButton();

            page.ClickConfirmOrderButton();

            Assert.IsTrue(page.SignUpButtonIsNotPresent);

            page.LogOff();
        }

        [TestMethod]
        [TestCategory(TestCategories.Notifications)]
        [TestCategory(TestCategories.Firefox)]
        public void ExistingUserLogsInAndMakesLiveDemandOrder()
        {
            var dataOperations = new DataOperations();
            dataOperations.ConnectionString = WebUiTestGlobalsTestConfig.DefaultConnection;

            var page = NavigateToDetailsPageViaRegisterWizard();

            page.ClickLoginLink();
            page.EnterDetail(WebUiTestGlobalsTestConfig.LoggedInUserEmail, Infrastructure.TestConstants.EmailInput);
            page.EnterDetail(WebUiTestGlobalsTestConfig.LoggedInUserPassword, Infrastructure.TestConstants.PasswordInput);
            page.ClickSignIn();

            page.WaitForLogOutLink();

            page.ClickWebinarsMenuItem(3);

            page.PickRegType(2);

            page.ClickSignUpButton();

            page.ClickBillMeButton();

            page.Wait(100);

            dataOperations.DeleteMostRecentOrderOfUser(WebUiTestGlobalsTestConfig.LoggedInUserEmail);

            page.LogOff();
        }

        [TestMethod]
        [TestCategory(TestCategories.Notifications)]
        [TestCategory(TestCategories.Firefox)]
        public void ExistingUserLogsInAndMakesOnDemandRecordingOrder()
        {
            var dataOperations = new DataOperations();
            dataOperations.ConnectionString = WebUiTestGlobalsTestConfig.DefaultConnection;

            var page = NavigateToDetailsPageViaRegisterWizard();

            page.ClickLoginLink();
            page.EnterDetail(WebUiTestGlobalsTestConfig.LoggedInUserEmail, Infrastructure.TestConstants.EmailInput);
            page.EnterDetail(WebUiTestGlobalsTestConfig.LoggedInUserPassword, Infrastructure.TestConstants.PasswordInput);
            page.ClickSignIn();

            page.WaitForLogOutLink();

            page.ClickWebinarsMenuItem(3);

            page.PickRegType(2);

            page.ClickSignUpButton();

            page.ClickBillMeButton();

            page.Wait(100);

            dataOperations.DeleteMostRecentOrderOfUser(WebUiTestGlobalsTestConfig.LoggedInUserEmail);

            page.LogOff();
        }

        [TestMethod]
        [TestCategory(TestCategories.Notifications)]
        [TestCategory(TestCategories.Firefox)]
        public void ExistingUserLogsInAndMakesCdHardcopyOrder()
        {
            var dataOperations = new DataOperations();
            dataOperations.ConnectionString = WebUiTestGlobalsTestConfig.DefaultConnection;

            var page = NavigateToDetailsPageViaRegisterWizard();

            page.ClickLoginLink();
            page.EnterDetail(WebUiTestGlobalsTestConfig.LoggedInUserEmail, Infrastructure.TestConstants.EmailInput);
            page.EnterDetail(WebUiTestGlobalsTestConfig.LoggedInUserPassword, Infrastructure.TestConstants.PasswordInput);
            page.ClickSignIn();

            page.WaitForLogOutLink();

            page.ClickWebinarsMenuItem(3);

            page.PickRegType(3);

            page.ClickSignUpButton();

            page.ClickBillMeButton();

            page.Wait(100);

            dataOperations.DeleteMostRecentOrderOfUser(WebUiTestGlobalsTestConfig.LoggedInUserEmail);

            page.LogOff();
        }

        [TestMethod]
        //[Ignore]
        [TestCategory(TestCategories.Notifications)]
        [TestCategory(TestCategories.Firefox)]
        public void ExistingUserLogsInAndMakesCdHardcopyOrderWith1AdditionalLocation()
        {
            var dataOperations = new DataOperations();
            dataOperations.ConnectionString = WebUiTestGlobalsTestConfig.DefaultConnection;

            var page = NavigateToDetailsPageViaRegisterWizard();

            page.ClickLoginLink();
            page.EnterDetail(WebUiTestGlobalsTestConfig.LoggedInUserEmail, Infrastructure.TestConstants.EmailInput);
            page.EnterDetail(WebUiTestGlobalsTestConfig.LoggedInUserPassword, Infrastructure.TestConstants.PasswordInput);
            page.ClickSignIn();

            page.WaitForLogOutLink();

            page.ClickWebinarsMenuItem(3);

            /*********** Additional Locations modal *****************/
            page.ClickAddAdditionalLocationButton();
            page.ClickTheClickToAddMoreButton();
            page.EnterDetail("drogersbox-test1@yahoo.com.au", "AdditionalLocationEmail_0");
            page.ClickSubmitAdditionalLocationsButton();
            /*********** ************************** *****************/

            page.Wait(500);

            page.ClickSignUpButton();

            page.ClickBillMeButton();

            page.Wait(100);

            dataOperations.DeleteMostRecentOrderOfUser(WebUiTestGlobalsTestConfig.LoggedInUserEmail);

            page.LogOff();
        }

        [TestMethod]
        [TestCategory(TestCategories.Notifications)]
        [TestCategory(TestCategories.Firefox)]
        public void ExistingUserLogsInAndMakesLivePlusOnDemandOrder()
        {
            var dataOperations = new DataOperations();
            dataOperations.ConnectionString = WebUiTestGlobalsTestConfig.DefaultConnection;

            var page = NavigateToDetailsPageViaRegisterWizard();

            page.ClickLoginLink();
            page.EnterDetail(WebUiTestGlobalsTestConfig.LoggedInUserEmail, Infrastructure.TestConstants.EmailInput);
            page.EnterDetail(WebUiTestGlobalsTestConfig.LoggedInUserPassword, Infrastructure.TestConstants.PasswordInput);
            page.ClickSignIn();

            page.WaitForLogOutLink();

            page.ClickWebinarsMenuItem(3);

            page.PickRegType(4);

            page.ClickSignUpButton();

            page.ClickBillMeButton();

            page.Wait(100);

            dataOperations.DeleteMostRecentOrderOfUser(WebUiTestGlobalsTestConfig.LoggedInUserEmail);

            page.LogOff();
        }

        [TestMethod]
        [Ignore]
        [TestCategory(TestCategories.Notifications)]
        [TestCategory(TestCategories.Firefox)]
        public void ExistingUserLogsInAndMakesLivePlusOnDemandOrderWith1AdditionalLocation()
        {
            var dataOperations = new DataOperations();
            dataOperations.ConnectionString = WebUiTestGlobalsTestConfig.DefaultConnection;

            var page = NavigateToDetailsPageViaRegisterWizard();

            page.ClickLoginLink();
            page.EnterDetail(WebUiTestGlobalsTestConfig.LoggedInUserEmail, Infrastructure.TestConstants.EmailInput);
            page.EnterDetail(WebUiTestGlobalsTestConfig.LoggedInUserPassword, Infrastructure.TestConstants.PasswordInput);
            page.ClickSignIn();

            page.WaitForLogOutLink();

            page.ClickWebinarsMenuItem(3);

            page.PickRegType(4);

            /*********** Additional Locations modal *****************/
            page.ClickAddAdditionalLocationButton();
            page.ClickTheClickToAddMoreButton();
            page.EnterDetail("drogersbox-test1@yahoo.com.au", "AdditionalLocationEmail_0");
            page.ClickSubmitAdditionalLocationsButton();
            /*********** ************************** *****************/

            page.Wait(500);

            page.ClickSignUpButton();

            page.ClickBillMeButton();

            page.Wait(100);

            dataOperations.DeleteMostRecentOrderOfUser(WebUiTestGlobalsTestConfig.LoggedInUserEmail);

            page.LogOff();
        }

        [TestMethod]
        [TestCategory(TestCategories.Notifications)]
        [TestCategory(TestCategories.Firefox)]
        public void ExistingUserLogsInAndMakesPremierPackageOrder()
        {
            var dataOperations = new DataOperations();
            dataOperations.ConnectionString = WebUiTestGlobalsTestConfig.DefaultConnection;

            var page = NavigateToDetailsPageViaRegisterWizard();

            page.ClickLoginLink();
            page.EnterDetail(WebUiTestGlobalsTestConfig.LoggedInUserEmail, Infrastructure.TestConstants.EmailInput);
            page.EnterDetail(WebUiTestGlobalsTestConfig.LoggedInUserPassword, Infrastructure.TestConstants.PasswordInput);
            page.ClickSignIn();

            page.WaitForLogOutLink();

            page.ClickWebinarsMenuItem(3);

            page.PickRegType(5);

            page.ClickSignUpButton();

            page.ClickBillMeButton();

            page.Wait(100);

            dataOperations.DeleteMostRecentOrderOfUser(WebUiTestGlobalsTestConfig.LoggedInUserEmail);

            page.LogOff();
        }

        [TestMethod]
        [Ignore]
        [TestCategory(TestCategories.Notifications)]
        [TestCategory(TestCategories.Firefox)]
        public void ExistingUserLogsInAndMakesPremierPackageOrderWith1AdditionalLocation()
        {
            var dataOperations = new DataOperations();
            dataOperations.ConnectionString = WebUiTestGlobalsTestConfig.DefaultConnection;

            var page = NavigateToDetailsPageViaRegisterWizard();

            page.ClickLoginLink();
            page.EnterDetail(WebUiTestGlobalsTestConfig.LoggedInUserEmail, Infrastructure.TestConstants.EmailInput);
            page.EnterDetail(WebUiTestGlobalsTestConfig.LoggedInUserPassword, Infrastructure.TestConstants.PasswordInput);
            page.ClickSignIn();

            page.WaitForLogOutLink();

            page.ClickWebinarsMenuItem(3);

            page.PickRegType(5);

            /*********** Additional Locations modal *****************/
            page.ClickAddAdditionalLocationButton();
            page.ClickTheClickToAddMoreButton();
            page.EnterDetail("drogersbox-test1@yahoo.com.au", "AdditionalLocationEmail_0");
            page.ClickSubmitAdditionalLocationsButton();
            /*********** ************************** *****************/

            page.Wait(500);

            page.ClickSignUpButton();

            page.ClickBillMeButton();

            page.Wait(100);

            dataOperations.DeleteMostRecentOrderOfUser(WebUiTestGlobalsTestConfig.LoggedInUserEmail);

            page.LogOff();
        }

        [TestMethod]
        [TestCategory(TestCategories.Notifications)]
        [TestCategory(TestCategories.Firefox)]
        public void UserLogsInDuringCheckoutAndMakesASimpleOrder()
        {
            //
            string[] testUsers = new[]
            {
                //"1-1@ttstester.com",
                //"1-2@ttstester.com",
                //"1-3@ttstester.com",
                //"1-4@ttstester.com",
                //"1-5@ttstester.com",
                //"2-1@ttstester.com",
                //"2-2@ttstester.com",
                "3-1@ttstester.com",
                "3-2@ttstester.com",
                "3-3@ttstester.com",
                "3-4@ttstester.com",
                "3-5@ttstester.com",
                "4-1@ttstester.com",
                "4-2@ttstester.com",
                //"5-1@ttstester.com",
                //"5-2@ttstester.com",
                //"5-3@ttstester.com",
                //"5-4@ttstester.com",
                //"5-5@ttstester.com",
                //"6-1@ttstester.com",
                //"6-2@ttstester.com",
                //"7-1@ttstester.com",
                //"7-2@ttstester.com",
                //"7-3@ttstester.com",
                //"7-4@ttstester.com",
                //"7-5@ttstester.com",
                //"8-1@ttstester.com",
                //"8-2@ttstester.com",
                //"9-1@ttstester.com",
                //"9-2@ttstester.com",
                "9-3@ttstester.com",
                "9-4@ttstester.com",
                "9-5@ttstester.com",
                "10-1@ttstester.com",
                "10-2@ttstester.com"
            };
            for (var i = 0; i < testUsers.Count(); i++)
            {
                int testWebinar = Convert.ToInt32(testUsers[i].Split('@')[0].Split('-')[0]); ;
                int regType =  Convert.ToInt32(testUsers[i].Split('@')[0].Split('-')[1]); ;
                if (testWebinar < 11)
                {
                    var freshTestDriver = Foundry.CreateTestDriver<FirefoxTestDriver>();

                    int idWebinar = testWebinar;

                    string email = testUsers[i];
                    string password = "kkkkkk";

                    var page =
                        NavigateToDetailsPageByUrl(string.Format(@"http://localhost:3538/Webinar/Details/{0}", idWebinar), freshTestDriver);

                    //  use this to choose an option. Lowest parameter is 1 (not 0). 
                    //  The first radio button is selected on load, so don't even call this method 
                    //  if you do want to choose that 1st radio button.

                    if (regType > 1)
                    {
                        try
                        {
                            page.PickRegType(regType);
                            page.Wait(500);
                        }
                        catch (Exception)
                        {
                            Trace.Write(string.Format("Failed to pick radio button for regType {0}", regType));
                        }
                    }

                    try
                    {
                        // click the big green Signup button
                        page.ClickSignUpButton();

                        // enter UserName of test user for this test (parameters are {username, idOfInput} )
                        page.EnterDetail(email, RegisterFieldsEmailUnderscoreDelimited);
                        page.ClickSubmit();

                        // enter Password of test user for this test (parameters are {password, idOfInput} )
                        page.EnterDetail(password, "Password1");
                        page.ClickSubmit();
                        page.Wait(6000);

                        // Finally, click the Bill Me button on the final tab.
                        page.ClickBillMeButton();

                        page.Wait(3000);

                        page.QuitPage();

                        regType++;
                        testWebinar++;
                    }
                    catch (Exception)
                    {
                        regType++;
                        testWebinar++;
                        page.LogOff();
                    }

                }
            }
        }

        public DetailsPage NavigateToDetailsPageViaRegisterWizard()
        {
            var detailsPage = new DetailsPage(TestDriver);
            detailsPage.Open();

            return detailsPage;
        }

        public DetailsPage NavigateToDetailsPage()
        {
            var detailsPage = new DetailsPage(TestDriver);
            detailsPage.Open();
            detailsPage.ClickWebinarsMenuItem(3);
            return detailsPage;
        }

        public DetailsPage NavigateToDetailsPageByUrl(string url, ITestDriver driver = null)
        {
            DetailsPage detailsPage;

            if (ReferenceEquals(driver, null))
            {
                detailsPage = new DetailsPage(TestDriver);
            }
            else
            {
                detailsPage = new DetailsPage(driver);
            }


            detailsPage.OpenPage(url);

            return detailsPage;
        }
    }
}
