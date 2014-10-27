using System.Threading;
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
            var textBox = SeleniumTestDriver.FindById(element, ExpectedCondition.ElementIsVisible, 10);
            Wait(); // I don't like these waits, but the moving panels warrant them. O/w, reliability issues.
            SeleniumTestDriver.TypeText(textBox, detail);
            Wait();
        }

        public virtual void Wait(int numberOfMilliSeconds = 250)
        {
            Thread.Sleep(numberOfMilliSeconds);
        }
    }
}
