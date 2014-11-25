using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CUWebinars.Tests.Common;
using CUWebinars.WebUi.Tests2.Infrastructure;
using CUWebinars.WebUi.Tests2.Pages;
using Microsoft.VisualStudio.TestTools.UnitTesting;


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
        private readonly string TestEmailAddress = "auser@" + CommonDomain;


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

            var email = TestHelper.RandomString(8) + "@" + CommonDomain;
            
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

            var email = TestHelper.RandomString(8) + "@" + CommonDomain;
            
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

            var email = TestHelper.RandomString(8) + "@" + CommonDomain;
            
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

            var email = TestHelper.RandomString(8) + "@" + CommonDomain;

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

            var email = TestHelper.RandomString(8) + "@" + CommonDomain;
            
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

            var email = TestHelper.RandomString(8) + "@" + CommonDomain;

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

            var email = TestHelper.RandomString(8) + "@" + CommonDomain;
            
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

            var email = TestHelper.RandomString(8) + "@" + CommonDomain;

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

            var email = string.Concat(TestHelper.RandomString(8), "@", TestHelper.RandomString(4), DotComSuffix);

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
            page.Wait(10000);
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
            page.ClickSubmitAdditionalLocationsButton();
            page.Wait(500);
            page.ClickAddAdditionalLocationButton();
            page.DeleteAdditionalLocation(id);
            page.DeleteAdditionalLocation(id + 1);

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

            page.ClickWebinarsMenuItem();
             
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

            page.ClickWebinarsMenuItem();

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

            page.ClickWebinarsMenuItem();

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

            page.ClickWebinarsMenuItem();

            page.PickRegType(3);
            
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

            page.ClickWebinarsMenuItem();

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

            page.ClickWebinarsMenuItem();

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

            page.ClickWebinarsMenuItem();

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

            page.ClickWebinarsMenuItem();

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

            page.ClickWebinarsMenuItem();

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
            detailsPage.ClickWebinarsMenuItem();
            return detailsPage;
        }
    }
}
