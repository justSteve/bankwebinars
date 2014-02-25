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

        public void ClickLoginLink()
        {
            SeleniumTestDriver.FindByXPathClick(@"/html/body/section/nav/a");
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
