using CUWebinars.Selenium.Core;
using CUWebinars.WebUi.Tests.Infrastructure;
using OpenQA.Selenium;

namespace CUWebinars.WebUi.Tests.Page.Ie
{
    public class HomeIndexPage : HomeIndexBasePage
    {
        public HomeIndexPage(ITestDriver seleniumTestDriver)
            : base(seleniumTestDriver)
        {
        }

        public override void LogOff()
        {
            SeleniumTestDriver.FindByPartialLinkText(Constants.LogoffLinkText).SendKeys(Keys.Enter);
        }

    }
}
