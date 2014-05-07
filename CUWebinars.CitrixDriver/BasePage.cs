using CUWebinars.Selenium.Core;

namespace CUWebinars.CitrixDriver
{
    public class BasePage
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

        public virtual string Title
        {
            get
            {
                return SeleniumTestDriver.GetDocumentTitle();
            }
        }

        public virtual void Close()
        {
            SeleniumTestDriver.Quit();
        }

    }
}