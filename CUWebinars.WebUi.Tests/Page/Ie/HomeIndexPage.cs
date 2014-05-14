using System;
using CUWebinars.Selenium.Core;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace CUWebinars.WebUi.Tests.Page.Ie
{
    public class HomeIndexPage : BasePage
    {
        public HomeIndexPage(ITestDriver seleniumTestDriver)
        {
            SeleniumTestDriver = seleniumTestDriver;
            GlobalTestConfig = Global.GlobalConfigSingleton;
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
            SeleniumTestDriver.FindByIdClick("btnLogin");
        }

        public void LogInToSite(string email, string password)
        {
            SeleniumTestDriver.TypeTextWithEnter(Constants.EmailInput, email);
            SeleniumTestDriver.TypeTextAndTabAway(Constants.PasswordInput, password);
            SeleniumTestDriver.FindByIdClick(Constants.SignInButtonInput);
        }

        public void ClickRegisterLinkOnLoginView()
        {
            SeleniumTestDriver.FindByXPathClick("/html/body/div/div/div/form/fieldset/div[3]/input");
        }

        public void EnterEmailAddressAndEnter(string email)
        {
            SeleniumTestDriver.Wait(500);
            SeleniumTestDriver.TypeTextWithEnter("RegisterFields.Email", email);
        }

        public void EnterPasswordWhereUserExists(string password)
        {
            var password1Wait = new WebDriverWait(SeleniumTestDriver.WebDriver, TimeSpan.FromSeconds(15));

            var password1 = password1Wait.Until(d =>
            {
                try
                {
                    SeleniumTestDriver.TypeTextAndTabAway("Password1", password);
                    return SeleniumTestDriver.FindByName("Password1");
                }
                catch (Exception e)
                {
                }
                return null;
            });

//            SeleniumTestDriver.FindByIdClick("TheSubmitButton");

            var wait = new WebDriverWait(SeleniumTestDriver.WebDriver, TimeSpan.FromSeconds(5));

            var theSubmitButton = wait.Until(d =>
            {
                var submitButton = SeleniumTestDriver.FindById("TheSubmitButton");
                return submitButton;
            });

            theSubmitButton.Click();
        }

        public void LogOff()
        {
            SeleniumTestDriver.FindByLinkTextClick(Constants.LogoffLinkText);
        }

        public bool NoNotceExistsErrorTextIsPresent
        {
            get
            {
                IWebDriver webDriver = SeleniumTestDriver.WebDriver;

                var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(5));

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
