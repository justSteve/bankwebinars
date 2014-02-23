using System;
using CUWebinars.Selenium.Core;
using CUWebinars.WebUi.Tests.Page;

namespace CUWebinars.WebUi.Tests.Firefox
{
   public class HomeIndexPage : BasePage
    {
        public HomeIndexPage(ITestDriver seleniumTestDriver)
        {
            SeleniumTestDriver = seleniumTestDriver;
            Url = @"http://localhost:5556";
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

       

    }
}
