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
