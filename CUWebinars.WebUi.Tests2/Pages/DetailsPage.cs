using System;
using System.Linq;
using CUWebinars.WebUi.Tests2.Infrastructure;
using KesselRun.SeleniumCore.Enums;
using KesselRun.SeleniumCore.TestDrivers.Contracts;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace CUWebinars.WebUi.Tests2.Pages
{
    public class DetailsPage : HomePage
    {
        public DetailsPage(ITestDriver seleniumTestDriver) : base(seleniumTestDriver)
        {
        }

        public bool SumbitAdditionalLocationsButtonIsNotThere 
        {
            get { return !SeleniumTestDriver.WebDriver.FindElements(By.Id("sumbitAdditionalLocationsButton")).Any(); } 
        }

        public bool SignUpButtonIsNotPresent
        {
            get
            {
                return !SeleniumTestDriver.FindById("AddToCart", ExpectedCondition.ElementIsVisible, 1).Displayed;
            }
        }

        public void ClickSignUpButton()
        {
            SeleniumTestDriver.FindByIdClick("AddToCart", ExpectedCondition.ElementIsVisible, 15);
        }

        public void ClickBillMeButton()
        {
            SeleniumTestDriver.FindByIdClickWithRetries("ConfirmRegistrationBillMe", ExpectedCondition.ElementIsVisible, 5);
        }

        public void ClickConfirmOrderButton()
        {
            SeleniumTestDriver.FindByIdClick("confirmRegistration", ExpectedCondition.ElementIsVisible, 30);
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

        public void ClickCancelAdditionalLocationsModalButton()
        {
            SeleniumTestDriver.FindByIdClick("closeButton", ExpectedCondition.ElementIsVisible, 5);
        }

        public void LogInAndGoToDetailsPage(string email, string password)
        {
            ClickLoginLink();
            LogInToSite(email, password);
            SeleniumTestDriver.FindByPartialLinkText(TestConstants.LogoffLinkText,ExpectedCondition.ElementIsVisible, 15);
            ClickWebinarsMenuItem(3);
        }

        public void WaitForLogOutLink()
        {
            SeleniumTestDriver.FindWithWait(40, (w) =>
                w.FindElements(By.PartialLinkText(TestConstants.LogoffLinkText)).Any()
                    ? SeleniumTestDriver.FindByPartialLinkText(TestConstants.LogoffLinkText)
                    : null);
        }

        public void ClickSignIn()
        {
            SeleniumTestDriver.FindByIdClickWithRetries("SignInButton", ExpectedCondition.ElementIsVisible, 5, 5);
        }

        public void PickRegType(int i)
        {
            SeleniumTestDriver.FindByXPathClick(string.Format(@"//*[@id='RegistrationType']/dl/dt[{0}]/input", i), ExpectedCondition.ElementIsVisible, 5);
        }


        public void EnterDetailThenPressEnter(string phoneNumber, string element)
        {
            EnterDetail(phoneNumber, element);
            SeleniumTestDriver.FindById(element, ExpectedCondition.ElementIsVisible, 10).SendKeys(Keys.Enter);
        }
    }
}
