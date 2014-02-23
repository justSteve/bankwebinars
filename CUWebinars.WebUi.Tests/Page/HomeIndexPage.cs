using System;
using CUWebinars.Selenium.Core;

namespace CUWebinars.WebUi.Tests.Page
{
    public class HomeIndexPage : BasePage
    {
        public HomeIndexPage(ITestDriver seleniumTestDriver)
        {
            this.SeleniumTestDriver = seleniumTestDriver;
            //this.Url = @"/Home/Index";
        }

        public bool HeadingIsPresentOnPage
        {
            get { return SeleniumTestDriver.FindByCssSelector("H3").Text.Equals("Welcome to the South Australian Police Web Request Portal", StringComparison.Ordinal); }
        }

    }
}
