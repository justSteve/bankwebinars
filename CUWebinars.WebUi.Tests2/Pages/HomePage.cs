using System;
using CUWebinars.WebUi.Tests2.Infrastructure;
using KesselRun.SeleniumCore.Enums;
using KesselRun.SeleniumCore.TestDrivers.Contracts;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace CUWebinars.WebUi.Tests2.Pages
{
    public class HomePage : BasePage
    {
        private const string ForgotPasswordLinkText = "Forgot Password";
        private const string TheSubmitButton = "TheSubmitButton";

        public HomePage(ITestDriver seleniumTestDriver) : base(seleniumTestDriver)
        {
        }

        public bool PhoneNrLinkIsPresentOnPage
        {
            get { return SeleniumTestDriver.FindByCssSelector("div.phone a.tele") != null; }
        }

        public void ClickLoginLink()
        {
            SeleniumTestDriver.FindByLinkClick(TestConstants.LoginLinkText);
        }

        public void LogInToSite(string sitTestEmailAddress, string sitTestPassword)
        {
            SeleniumTestDriver.TypeText(FinderStrategy.Name, TestConstants.EmailInput, sitTestEmailAddress);
            SeleniumTestDriver.TypeText(FinderStrategy.Name, TestConstants.PasswordInput, sitTestPassword);
            SeleniumTestDriver.FindByIdClick(TestConstants.SignInButtonInput);
        }

        public bool LoginLinkIsPresentOnPage
        {
            get
            {
                var loginLink = SeleniumTestDriver.FindByPartialLinkText(TestConstants.LoginLinkText);

                return loginLink.Displayed;
            }
        }

        public bool LogoutLinkIsPresentOnPage
        {
            get { return SeleniumTestDriver.FindByPartialLinkText(TestConstants.LogoffLinkText).Displayed; } 
        }

        public bool PasswordResetInstructionsSentLabelPresent
        {
            get
            {
                WebDriverWait wait = new WebDriverWait(SeleniumTestDriver.WebDriver, TimeSpan.FromSeconds(15));
                
                /*This may look really inefficient (that is, the repeated searches for the element in the retries) but it is necessary to avoid a Selenium StaleElementException*/
                var feedbackLabel = wait.Until((w) => SeleniumTestDriver.FindByCssSelector("#labelEmail > span").Text.Contains("Reset Instructions sent") ? SeleniumTestDriver.FindByCssSelector("#labelEmail > span") : null);

                return feedbackLabel.Text.Contains("Reset Instructions sent"); 
            }
        }

        public virtual void LogOff()
        {
            SeleniumTestDriver.FindByPartialLinkTextClick(TestConstants.LogoffLinkText);
        }

        public void ClickRegisterLinkOnLoginView()
        {
            SeleniumTestDriver.FindByIdClick("registerLinkButton");
        }

        public void EnterEmailAddressAndClickSubmit(string email)
        {
            SeleniumTestDriver.TypeText(FinderStrategy.Name, "RegisterFields.Email", email);
            ClickSubmit();
        }

        public void ClickSubmit()
        {
            IWebElement webElement;

            do
            {
                webElement = SeleniumTestDriver.FindByIdClick(TheSubmitButton);
            } while (webElement == null);
        }

        public void EnterPasswordWhereUserExists(string sitTestPassword)
        {
            SeleniumTestDriver.TypeText(FinderStrategy.Name, "Password1", sitTestPassword);
            ClickSubmit();
        }

        public void ClickResetPasswordButton(string resetPasswordButton)
        {
            var link = SeleniumTestDriver.FindById(resetPasswordButton, ExpectedCondition.ElementIsVisible, 5);
            link.Click();
        }

        public void TabAwayFromInput(string id)
        {
            SeleniumTestDriver.FindById(id).SendKeys(Keys.Tab);
        }
    }
}
