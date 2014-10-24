using KesselRun.SeleniumCore.Enums;
using KesselRun.SeleniumCore.TestDrivers.Contracts;
using OpenQA.Selenium;

namespace CUWebinars.WebUi.Tests2.Pages
{
    public abstract class BasePage
    {
        protected const int WaitTimeout = 15;

        protected BasePage(ITestDriver seleniumTestDriver)
        {
            SeleniumTestDriver = seleniumTestDriver;
            Url = seleniumTestDriver.DefaultUrl;
        }

        protected string Url { get; set; }

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
            SeleniumTestDriver.Quit();
        }

        public virtual void Open()
        {
            if (!ReferenceEquals(null, SeleniumTestDriver))
            {
                SeleniumTestDriver.GoToUrl(Url);
            }
        }


        public virtual void OpenPage(string url)
        {
            SeleniumTestDriver.GoToUrl(url);
        }


        public virtual void EnterDetail(string detail, string element)
        {
            var textBox = SeleniumTestDriver.FindById(element, ExpectedCondition.ElementIsVisible, 5);
            SeleniumTestDriver.TypeText(textBox, detail);
        }

    }
}
