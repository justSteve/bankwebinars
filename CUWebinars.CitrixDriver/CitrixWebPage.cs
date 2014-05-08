using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using CUWebinars.Selenium.Core;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace CUWebinars.CitrixDriver
{
    public class CitrixWebPage : BasePage
    {
        public CitrixWebPage(ITestDriver seleniumTestDriver, string url)
        {
            Url = url;
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

        public void ClearCookies()
        {
            SeleniumTestDriver.ClearCookies();
        }
        
        public void GotToLoginPage()
        {
            SeleniumTestDriver.FindByLinkTextClick("Log In");
        }


        public void EnterEmailAddress(string email)
        {
            SeleniumTestDriver.Wait(500);
            IWebElement emailInput = SeleniumTestDriver.FindById("emailAddress");
            emailInput.Clear();
            emailInput.SendKeys(email);
        }

        public void EnterPasswordWhereUserExists(string password)
        {
            SeleniumTestDriver.Wait(500);
            IWebElement passwordInput = SeleniumTestDriver.FindById("password");
            passwordInput.Clear();
            passwordInput.SendKeys(password);
        }

        public void LogOff()
        {
            //SeleniumTestDriver.FindByXPathClick(Constants.LogoffLinkPath);
        }

        public void ClickSubmit()
        {
            SeleniumTestDriver.FindByIdClick("submit");
        }

        public void GoToWebinarsPage()
        {
            SeleniumTestDriver.FindByLinkTextClick("My Webinars");
        }

        public void ScheduleAWebinar()
        {
            SeleniumTestDriver.FindByLinkTextClick("Schedule a Webinar");
        }

        public void ChooseWebinarTemplate()
        {
            SeleniumTestDriver.FindByIdClick("WebinarChoice");
            SeleniumTestDriver.Wait(500);
            SeleniumTestDriver.FindByCssSelectorClick("option[value=\"563748522|wid\"]");
        }

        public void CompleteDetails()
        {
            SeleniumTestDriver.FindByIdClick("privateConfCallRadio");
            SeleniumTestDriver.FindByIdClick("StartDate_Cal_0");
            SeleniumTestDriver.FindByCssSelectorClick("img.next");

            var dateWidget = SeleniumTestDriver.FindByCssSelectorClick("div#floatcal table");
            var month = SeleniumTestDriver.FindByCssSelectorClick("div#floatcal div.head");
            var nextButton = SeleniumTestDriver.FindByCssSelectorClick("div#floatcal div.head img.next");
            var prevButton = SeleniumTestDriver.FindByCssSelectorClick("div#floatcal div.head img.prev");

            int count = 0;
            while (month.Text != "September 2014")
            {
                count++;
                nextButton.Click();
            }

            ReadOnlyCollection<IWebElement> rows = dateWidget.FindElements(By.TagName("tr"));
            ReadOnlyCollection<IWebElement> cols = dateWidget.FindElements(By.TagName("td"));

            //  Lets choose the 28th of the current month
            foreach (IWebElement webElement in cols)
            {
                if (webElement.Text.Equals("28"))
                {
                    webElement.Click();
                    break;
                }
            }
            
            SeleniumTestDriver.TypeText("StartHour_0", "06:00");
            
            var startMeridian = new SelectElement(SeleniumTestDriver.FindById("StartAMPM_0"));
            startMeridian.SelectByText("PM");
            SeleniumTestDriver.TypeText("EndHour_0", "11:00");

            var endMeridian = new SelectElement(SeleniumTestDriver.FindById("EndAMPM_0"));
            endMeridian.SelectByText("PM");

            var timeZoneKey = new SelectElement(SeleniumTestDriver.FindById("TimeZoneKey"));
            timeZoneKey.SelectByValue("68");

            var recursDropList = new SelectElement(SeleniumTestDriver.FindById("Recurs"));
            recursDropList.SelectByText("Monthly");

            SeleniumTestDriver.FindByIdClick("monthly_enddate");
            SeleniumTestDriver.FindByCssSelectorClick("img.next");

            //dateWidget = SeleniumTestDriver.FindByCssSelectorClick("div#floatcal table");
            //month = SeleniumTestDriver.FindByCssSelectorClick("div#floatcal div.head");

            while (count > 0)
            {
                count--;
                prevButton.Click();
            }

            while (month.Text != "December 2014")
            {
                count++;
                nextButton.Click();
            }

            //rows = dateWidget.FindElements(By.TagName("tr"));
            cols = dateWidget.FindElements(By.TagName("td"));

            //  Lets choose the 28th of the current month
            foreach (IWebElement webElement in cols)
            {
                if (webElement.Text.Equals("22"))
                {
                    webElement.Click();
                    break;
                }
            }

            SeleniumTestDriver.FindByIdClick("attendee_type_1");

            SeleniumTestDriver.FindByCssSelectorClick("div.submit_bar input[type=submit]");

        }
    }
}
