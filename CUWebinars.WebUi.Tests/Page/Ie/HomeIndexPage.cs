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
            SeleniumTestDriver.ClearCookies();
        }

        public void ClickLoginLink()
        {
            SeleniumTestDriver.FindByXPathClick(@"/html/body/section/nav/a");
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
            SeleniumTestDriver.Wait(500);
            SeleniumTestDriver.TypeTextAndTabAway("Password1", password);
            SeleniumTestDriver.FindByIdClick("TheSubmitButton");
        }

        public void LogOff()
        {
            SeleniumTestDriver.FindByXPathClick(Constants.LogoffLinkPath);
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
