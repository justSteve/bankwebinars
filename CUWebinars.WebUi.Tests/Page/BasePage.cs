using System;
using CUWebinars.Selenium.Core;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace CUWebinars.WebUi.Tests.Page
{
    public abstract class BasePage
    {
        public BasePage(ITestDriver seleniumTestDriver)
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
            if (null != SeleniumTestDriver)
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
                var logoutLinkWait = new WebDriverWait(SeleniumTestDriver.WebDriver, TimeSpan.FromSeconds(15));

                logoutLinkWait.Until(ExpectedConditions.ElementIsVisible(By.PartialLinkText(Constants.LoginLinkText)));

                var logInLink = SeleniumTestDriver.FindByPartialLinkText(Constants.LoginLinkText);


                return logInLink != null;
            }
        }

        public bool LogoutLinkIsPresentOnPage
        {
            get
            {
                var logoutLinkWait = new WebDriverWait(SeleniumTestDriver.WebDriver, TimeSpan.FromSeconds(15));

                logoutLinkWait.Until(ExpectedConditions.ElementIsVisible(By.PartialLinkText(Constants.LogoffLinkText)));

                var logOffLink = SeleniumTestDriver.FindByPartialLinkText(Constants.LogoffLinkText);

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
            var newAccountWait = new WebDriverWait(SeleniumTestDriver.WebDriver, TimeSpan.FromSeconds(15));
            newAccountWait.Until(ExpectedConditions.ElementExists(By.XPath("/html/body/div/div/div/form/fieldset/div[3]/input")));

            SeleniumTestDriver.FindByXPathClick("/html/body/div/div/div/form/fieldset/div[3]/input");
        }

        public void EnterEmailAddressAndEnter(string email)
        {
            var emailWait = new WebDriverWait(SeleniumTestDriver.WebDriver, TimeSpan.FromSeconds(15));
            emailWait.Until(ExpectedConditions.ElementExists(By.Name("RegisterFields.Email")));

            SeleniumTestDriver.TypeTextWithEnter("RegisterFields.Email", email);
        }

        public void EnterPasswordWhereUserExists(string password)
        {
            var password1Wait = new WebDriverWait(SeleniumTestDriver.WebDriver, TimeSpan.FromSeconds(15));

            password1Wait.Until(ExpectedConditions.ElementExists(By.Name("Password1")));

            SeleniumTestDriver.TypeTextAndTabAway("Password1", password);

            var theSubmitButtonWait = new WebDriverWait(SeleniumTestDriver.WebDriver, TimeSpan.FromSeconds(15));

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
                IWebDriver webDriver = SeleniumTestDriver.WebDriver;

                var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(15));

                var errorMessageElement = wait.Until(d =>
                {
                    var element = webDriver.FindElement(By.XPath(@"//*[@id=""innerContent""]/div[5]/div[2]/span"));
                    return element;
                });

                return errorMessageElement != null &&
                       errorMessageElement.Text.Equals("No Notice exists with supplied criteria",
                           StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
