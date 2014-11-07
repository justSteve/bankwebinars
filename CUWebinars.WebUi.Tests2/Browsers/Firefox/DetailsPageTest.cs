using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CUWebinars.Tests.Common;
using CUWebinars.WebUi.Tests2.Pages;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CUWebinars.WebUi.Tests2.Browsers.Firefox
{
    [TestClass]
    public class DetailsPageTest : FirefoxBaseTest
    {
        private string RegisterFieldsEmailUnderscoreDelimited = "RegisterFields_Email";
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

        [TestMethod]
        public void LoadDetailsPage()
        {
            var page = NavigateToDetailsPage();

            Assert.IsTrue(page.WebinarTitleIsDisplayedOnWebinarDetailsPage);    
        }

        [TestMethod]
        public void AnonymousUserWithExistingDomainMakesOrderSuccessfully()
        {
            var page = NavigateToDetailsPage();

            page.ClickSignUpButton();

            var email = TestHelper.RandomString(8) + "@" + CommonDomain;
            
            page.EnterEmailAddressAndClickSubmit(email);
            page.ClickYesUseAddressButton();
            
            page.EnterDetail("Dave Rogers", FullNameInput);
            page.EnterDetail(Title, RegisterfieldsInput);
            page.EnterDetail("1 Knox St", RegisterfieldsBillingaddressStreetaddress);
            page.EnterDetail("56787890", RegisterfieldsBillingaddressPhone);

            page.ClickSubmit();

            page.ClickBillMeButton();

            Assert.IsTrue(page.LogoutLinkIsPresentOnPage);
        }
        
        [TestMethod]
        public void AnonymousUserWithExistingDomainButDoesntUseAddressOfInstitutionMakesOrderSuccessfully()
        {
            var page = NavigateToDetailsPage();

            page.ClickSignUpButton();

            var email = TestHelper.RandomString(8) + "@" + CommonDomain;
            
            page.EnterEmailAddressAndClickSubmit(email);

            page.ClickNoEnterDiffAddressButton();

            page.EnterDetail(email, RegisterFieldsEmailUnderscoreDelimited); // not sure why, but id changes from a "." to a "_"
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

            Assert.IsTrue(page.LogoutLinkIsPresentOnPage);
        }
        
        [TestMethod]
        public void AnonymousUserWithExistingDomainButNotFromInstitutionMakesOrderSuccessfully()
        {
            var page = NavigateToDetailsPage();

            page.ClickSignUpButton();

            var email = TestHelper.RandomString(8) + "@" + CommonDomain;
            
            page.EnterEmailAddressAndClickSubmit(email);

            page.ClickNotInstitutionButton();

            page.EnterDetail(email, RegisterFieldsEmailUnderscoreDelimited); // not sure why, but id changes from a "." to a "_"
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

            Assert.IsTrue(page.LogoutLinkIsPresentOnPage);
        }
        
        [TestMethod]
        public void AnonymousUserWithExistingDomainButClicksCrossOnModalMakesOrderSuccessfully()
        {
            var page = NavigateToDetailsPage();

            page.ClickSignUpButton();

            var email = TestHelper.RandomString(8) + "@" + CommonDomain;
            
            page.EnterEmailAddressAndClickSubmit(email);

            page.ClickCancelModalButton();

            page.EnterDetail(email, RegisterFieldsEmailUnderscoreDelimited); // not sure why, but id changes from a "." to a "_"
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

            Assert.IsTrue(page.LogoutLinkIsPresentOnPage);
        }

        [TestMethod]
        public void AnonymousUserWithoutExistingDomainMakesOrderSuccessfully()
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

            Assert.IsTrue(page.LogoutLinkIsPresentOnPage);
        }
        
        [TestMethod]
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

        public DetailsPage NavigateToDetailsPage()
        {
            var detailsPage = new DetailsPage(TestDriver);
            detailsPage.Open();
            detailsPage.ClickWebinarsMenuItem();
            return detailsPage;
        }
    }
}
