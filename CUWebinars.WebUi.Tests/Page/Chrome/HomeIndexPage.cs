using System;
using CUWebinars.Selenium.Core;

namespace CUWebinars.WebUi.Tests.Page.Chrome
{
    public class HomeIndexPage : BasePage
    {
        public HomeIndexPage(ITestDriver seleniumTestDriver)
        {
            SeleniumTestDriver = seleniumTestDriver;
            GlobalTestConfig = Global.GlobalConfigSingleton;
            Url = GlobalTestConfig.HomeUrl;
        }

        public void ClearCookies()
        {
            SeleniumTestDriver.ClearCookies();
        }

        public void ClickLoginLink()
        {
            SeleniumTestDriver.Wait(1000);
            //SeleniumTestDriver.FindByXPathClick(Constants.LoginLinkPath);
            SeleniumTestDriver.FindByIdClick("btnLogin");
        }

        public void LogInToSite(string email, string password)
        {
            SeleniumTestDriver.Wait(1000);
            SeleniumTestDriver.TypeTextWithEnter(Constants.EmailInput, email);
            SeleniumTestDriver.TypeTextAndTabAway(Constants.PasswordInput, password);
            SeleniumTestDriver.FindByIdClick(Constants.SignInButtonInput);
        }

        public void ClickRegisterLinkOnLoginView()
        {
            SeleniumTestDriver.Wait(1000);
            SeleniumTestDriver.FindByXPathClick("/html/body/div/div/div/form/fieldset/div[3]/input");
        }

        public void EnterEmailAddressAndEnter(string email)
        {
            SeleniumTestDriver.Wait(1000);
            SeleniumTestDriver.TypeTextWithEnter("RegisterFields.Email", email);
        }

        public void EnterPasswordWhereUserExists(string password)
        {
            SeleniumTestDriver.Wait(1000);
            SeleniumTestDriver.TypeTextAndTabAway("Password1", password);
            SeleniumTestDriver.FindByIdClick("TheSubmitButton");
        }

        public void LogOff()
        {
            SeleniumTestDriver.FindByXPathClick(Constants.LogoffLinkPath);
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
    }
}
