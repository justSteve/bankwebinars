using System;
using CUWebinars.Selenium.Core;
using CUWebinars.WebUi.Tests.Page;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace CUWebinars.WebUi.Tests.Ie
{
    public class HomeIndexPage : BasePage
    {
        public HomeIndexPage(ITestDriver seleniumTestDriver)
        {
            SeleniumTestDriver = seleniumTestDriver;
            Url = @"http://localhost:5556";
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
                return
                    SeleniumTestDriver.FindByCssSelector("H3")
                        .Text.Equals("Welcome to the South Australian Police Web Request Portal",
                            StringComparison.Ordinal);
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
