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
        private const string DotComSuffix = ".com";

        [TestMethod]
        public void LoadDetailsPage()
        {
            var page = NavigateToDetailsPage();

            Assert.IsTrue(page.WebinarTitleIsDisplayedOnWebinarDetailsPage);    
        }

        [TestMethod]
        public void AnonymousUserMakesOrder()
        {
            var page = NavigateToDetailsPage();

            page.ClickSignUpButton();

            var email = TestHelper.RandomString(8) + "@" + "yahoo.com.au";
            
            page.EnterEmailAddressAndClickSubmit(email);
            page.ClickYesUseAddressButton();
            
            page.EnterDetail("Dave Rogers", "FullName");
            page.EnterDetail("Mr", "RegisterFields_Title");
            page.EnterDetail("1 Knox St", "RegisterFields_BillingAddress_StreetAddress");
            page.EnterDetail("56787890", "RegisterFields_BillingAddress_Phone");

            page.ClickSubmit();

            page.ClickBillMeButton();

            page.ClickConfirmOrderButton();

            Assert.IsTrue(page.LogoutLinkIsPresentOnPage);
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
