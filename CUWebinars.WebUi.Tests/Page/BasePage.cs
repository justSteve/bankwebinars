using CUWebinars.Selenium.Core;

namespace CUWebinars.WebUi.Tests.Page
{
    public abstract class BasePage
    {
        protected string Url { get; set; }
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
                return SeleniumTestDriver.FindByXPath(Constants.LoginLinkPath) != null;

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
