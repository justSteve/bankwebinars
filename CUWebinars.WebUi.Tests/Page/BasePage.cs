using System;
using CUWebinars.Selenium.Core;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace CUWebinars.WebUi.Tests.Page
{
    public abstract class BasePage
    {
        protected const int WaitTimeout = 15;

        protected BasePage(ITestDriver seleniumTestDriver)
        {
            SeleniumTestDriver = seleniumTestDriver;
            GlobalTestConfig = Global.GlobalConfigSingleton;
            Url = GlobalTestConfig.HomeUrl;
        }

        protected string Url { get; set; }
        protected Global GlobalTestConfig { get; set; }

        protected ITestDriver SeleniumTestDriver { get; set; }

        public virtual void Open()
        {
            if (!ReferenceEquals(null, SeleniumTestDriver))
            {
                SeleniumTestDriver.GoToUrl(Url);
            }
        }

        public bool PhoneNrLinkIsPresentOnPage
        {
            get { return SeleniumTestDriver.FindByCssSelector("div.phone a.tele") != null; }
        }

        
        public bool LoginLinkIsPresentOnPage
        {
            get
            {
                var logoutLinkWait = new WebDriverWait(SeleniumTestDriver.WebDriver, TimeSpan.FromSeconds(WaitTimeout));

                var logInLink = logoutLinkWait.Until(ExpectedConditions.ElementIsVisible(By.PartialLinkText(Constants.LoginLinkText)));

                return logInLink != null;
            }
        }

        public bool LogoutLinkIsPresentOnPage
        {
            get
            {
                var logoutLinkWait = new WebDriverWait(SeleniumTestDriver.WebDriver, TimeSpan.FromSeconds(WaitTimeout));

                var logOffLink = logoutLinkWait.Until(ExpectedConditions.ElementIsVisible(By.PartialLinkText(Constants.LogoffLinkText)));

                return logOffLink != null;
            }
        }

        public virtual string Title
        {
            get
            {
                return SeleniumTestDriver.GetDocumentTitle();
            }
        }

        public virtual void Close()
        {
            SeleniumTestDriver.CloseWindow();
        }

        public void Wait(int milliSeconds = 1000)
        {
            SeleniumTestDriver.Wait(milliSeconds);
        }

        public void OpenPage(string url)
        {
            SeleniumTestDriver.GoToUrl(url);
        }

        public bool HeadingIsPresentOnPage
        {
            get
            {
                return SeleniumTestDriver
                    .FindByCssSelector("h1")
                    .Text
                    .Equals("Test", StringComparison.Ordinal);
            }
        }

        public void ClearCookies()
        {
            SeleniumTestDriver.ClearFederatedCookies();
        }

        public void ClickLoginLink()
        {
            SeleniumTestDriver.FindByLinkTextClick(Constants.LoginLinkText);
        }

        public void LogInToSite(string email, string password)
        {
            SeleniumTestDriver.TypeTextWithEnter(Constants.EmailInput, email);
            SeleniumTestDriver.TypeTextAndTabAway(Constants.PasswordInput, password);
            SeleniumTestDriver.FindByIdClick(Constants.SignInButtonInput);
        }

        public void ClickRegisterLinkOnLoginView()
        {
            var newAccountWait = new WebDriverWait(SeleniumTestDriver.WebDriver, TimeSpan.FromSeconds(WaitTimeout));
            var element = newAccountWait.Until(ExpectedConditions.ElementExists(By.XPath("/html/body/div/div/div/form/fieldset/div[3]/input")));

            element.Click();
        }

        public void ClickResetPasswordButton()
        {
            SeleniumTestDriver.FindByIdClick("EdgeCaseResetPasswordButton");
        }


        public void EnterEmailAddressAndEnter(string email)
        {
            SeleniumTestDriver.TypeTextAndTabAway("RegisterFields.Email", email);

            var theSubmitButtonWait = new WebDriverWait(SeleniumTestDriver.WebDriver, TimeSpan.FromSeconds(WaitTimeout));
            var theSubmitButton = theSubmitButtonWait.Until(ExpectedConditions.ElementExists(By.Id("TheSubmitButton")));
            theSubmitButton.Click();
        }

        public void EnterPasswordWhereUserExists(string password)
        {
            SeleniumTestDriver.TypeTextAndTabAway("Password1", password);

            var theSubmitButtonWait = new WebDriverWait(SeleniumTestDriver.WebDriver, TimeSpan.FromSeconds(WaitTimeout));
            var theSubmitButton = theSubmitButtonWait.Until(ExpectedConditions.ElementExists(By.Id("TheSubmitButton")));
            theSubmitButton.Click();
        }

        public void LogOff()
        {
            SeleniumTestDriver.FindByPartialLinkText(Constants.LogoffLinkText).Click();
        }

        public bool NoNotceExistsErrorTextIsPresent
        {
            get
            {
                var errorMessageElement =
                    SeleniumTestDriver.FindByXPath(@"//*[@id=""innerContent""]/div[5]/div[2]/span");

                return errorMessageElement != null &&
                       errorMessageElement.Text.Equals("No Notice exists with supplied criteria",
                           StringComparison.OrdinalIgnoreCase);
            }
        }

        public bool PasswordResetInstructionsSentLabelPresent
        {
            get
            {
                return SeleniumTestDriver.FindById("labelEmail") != null;
            }
        }
    }
}
