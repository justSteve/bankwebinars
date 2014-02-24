using System;
using CUWebinars.Selenium.Core;

namespace CUWebinars.WebUi.Tests.Page.Chrome
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
