using System;
using CUWebinars.Selenium.Core;
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
                SeleniumTestDriver.Wait(1000);
                var logoutLinkWait = new WebDriverWait(SeleniumTestDriver.WebDriver, TimeSpan.FromSeconds(15));

                var logoutLink = logoutLinkWait.Until(d => SeleniumTestDriver.FindByPartialLinkText(Constants.LogoffLinkText));

                return logoutLink != null;
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
