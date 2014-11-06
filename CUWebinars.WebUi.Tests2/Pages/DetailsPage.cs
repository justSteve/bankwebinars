using System;
using KesselRun.SeleniumCore.Enums;
using KesselRun.SeleniumCore.TestDrivers.Contracts;
using OpenQA.Selenium;

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
            SeleniumTestDriver.FindByIdClick("confirmRegistration", ExpectedCondition.ElementIsVisible, 60);
        }

        public void ClickAddAdditionalLocationButton()
        {
            SeleniumTestDriver.FindByIdClick("AddLocationsButton", ExpectedCondition.ElementIsVisible, 5);
        }

        public void ClickTheClickToAddMoreButton()
        {
            SeleniumTestDriver.FindByIdClick("AddInputsButton", ExpectedCondition.ElementIsVisible, 5);
        }

        public void ClickSubmitAdditionalLocationsButton()
        {
            SeleniumTestDriver.FindByIdClick("sumbitAdditionalLocationsButton", ExpectedCondition.ElementIsVisible, 5);
        }

        public void DeleteAdditionalLocation(int id)
        {
            SeleniumTestDriver.FindByIdClick(string.Concat(id, "-AdditionLocationEmail-delete"), ExpectedCondition.ElementIsVisible, 5);
        }

        public int GetNumberOfAdditionalLocationTextBoxes()
        {
            return SeleniumTestDriver.WebDriver.FindElements(By.CssSelector("#collectAdditionalLocations input[type='email']"))
                .Count;
        }
    }
}
