using System;
using CUWebinars.Selenium.Core;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace CUWebinars.WebUi.Tests.Page
{
    public abstract class BasePage
    {
        protected string Url { get; set; }
        protected Global GlobalTestConfig { get; set; }

        protected ITestDriver SeleniumTestDriver { get; set; }

        public virtual void Open()
        {
            if (null != SeleniumTestDriver)
            {
                SeleniumTestDriver.GoToUrl(Url);
            }
        }

        public bool PhoneNrLinkIsPresentOnPage
        {
            get { return SeleniumTestDriver.FindByCssSelector("div.phone a.tele") != null; }
        }

        
        public bool LoginLinkIsPresentOnPage
        {
            get
            {
                return SeleniumTestDriver.FindByPartialLinkText(Constants.LoginLinkText) != null;

            }
        }

        public bool LogoutLinkIsPresentOnPage
        {
            get
            {
                var logoutLinkWait = new WebDriverWait(SeleniumTestDriver.WebDriver, TimeSpan.FromSeconds(15));

                logoutLinkWait.Until(ExpectedConditions.ElementIsVisible(By.PartialLinkText(Constants.LogoffLinkText)));

                var logOffLink = SeleniumTestDriver.FindByPartialLinkText(Constants.LogoffLinkText);

                return logOffLink != null;
            }
        }

        public virtual string Title
        {
            get
            {
                return SeleniumTestDriver.GetDocumentTitle();
            }
        }

        public virtual void Close()
        {
            SeleniumTestDriver.CloseWindow();
        }
    }
}
