using System;
using CUWebinars.Selenium.Core;

namespace CUWebinars.WebUi.Tests.Page.Firefox
{
   public class HomeIndexPage : BasePage
    {
        public HomeIndexPage(ITestDriver seleniumTestDriver)
        {
            SeleniumTestDriver = seleniumTestDriver;
        }

        public void ClearCookies()
        {
            SeleniumTestDriver.ClearCookies();
        }

        public void ClickLoginLink()
        {
            SeleniumTestDriver.FindByXPathClick(Constants.LoginLinkPath);
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


    }
}
