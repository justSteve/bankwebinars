using System;
using KesselRun.SeleniumCore.Enums;
using KesselRun.SeleniumCore.TestDrivers.Contracts;

namespace CUWebinars.WebUi.Tests2.Pages
{
    public class DetailsPage : HomePage
    {
        public DetailsPage(ITestDriver seleniumTestDriver) : base(seleniumTestDriver)
        {
        }

        public void ClickSignUpButton()
        {
            SeleniumTestDriver.FindByIdClick("AddToCart", ExpectedCondition.ElementIsVisible, 5);
        }

        public void ClickBillMeButton()
        {
            SeleniumTestDriver.FindByIdClickWithRetries("ConfirmRegistrationBillMe", ExpectedCondition.ElementIsVisible, 5);
        }

        public void ClickConfirmOrderButton()
        {
            try
            {
                SeleniumTestDriver.FindByIdClick("confirmRegistration", ExpectedCondition.ElementIsVisible, 60);
            }
            catch (Exception e)
            {
                SeleniumTestDriver.WebDriver.SwitchTo().Alert().Accept();
                SeleniumTestDriver.FindByIdClick("confirmRegistration");
            }
        }
    }
}
