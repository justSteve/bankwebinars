using CUWebinars.Selenium.Core;
using CUWebinars.WebUi.Tests.Infrastructure;
using OpenQA.Selenium;

namespace CUWebinars.WebUi.Tests.Page
{
    public abstract class BasePage
    {
        protected const int WaitTimeout = 15;

        protected BasePage(ITestDriver seleniumTestDriver)
        {
            SeleniumTestDriver = seleniumTestDriver;
            GlobalTestConfig = WebUiTestGlobals.WebUiTestGlobalsConfigSingleton;
            Url = GlobalTestConfig.HomeUrl;
        }

        protected string Url { get; set; }
        protected WebUiTestGlobals GlobalTestConfig { get; set; }

        protected ITestDriver SeleniumTestDriver { get; set; }

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

        public void Wait(int milliSeconds = 1000)
        {
            SeleniumTestDriver.Wait(milliSeconds);
        }

        public virtual void Open()
        {
            if (!ReferenceEquals(null, SeleniumTestDriver))
            {
                SeleniumTestDriver.GoToUrl(Url);
            }
        }


        public void OpenPage(string url)
        {
            SeleniumTestDriver.GoToUrl(url);
        }

        public void ClearCookies()
        {
            SeleniumTestDriver.ClearFederatedCookies();
        }


        public void EnterDetail(string detail, string domItem)
        {
            SeleniumTestDriver.TypeText(By.Id(domItem), detail);
        }

    }
}
