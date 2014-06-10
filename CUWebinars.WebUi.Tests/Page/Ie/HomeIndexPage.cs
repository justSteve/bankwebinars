using CUWebinars.Selenium.Core;
using CUWebinars.Selenium.Core.Enums;
using CUWebinars.WebUi.Tests.Infrastructure;

namespace CUWebinars.WebUi.Tests.Page.Ie
{
    public class HomeIndexPage : HomeIndexBasePage
    {
        public HomeIndexPage(ITestDriver seleniumTestDriver)
            : base(seleniumTestDriver)
        {
        }

        public void LogOffIfLoggedIn()
        {
            var logOffLink = SeleniumTestDriver.FindElementWithoutWait(
                SelectorStrategy.PartialLink,
                Constants.LogoffLinkText
                );

            if(!ReferenceEquals(null, logOffLink))
                logOffLink.Click();
        }
    }
}
