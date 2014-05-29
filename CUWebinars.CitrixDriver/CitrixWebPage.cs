using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CUWebinars.Business.Models;
using CUWebinars.Selenium.Core;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.ObjectModel;

namespace CUWebinars.CitrixDriver
{
    public class CitrixWebPage : BasePage
    {
        private int count = 0;
        private string _mainWindow;

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
            SeleniumTestDriver.ClearFederatedCookies();
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

        public void ClickViewLinkForAccessCodeAndPhoneNumbers()
        {
            SeleniumTestDriver.FindByXPathClick("//*[@id='SessionSet1_1']/a");
        }

        public void GoToWebinarsPage()
        {
            SeleniumTestDriver.FindByLinkTextClick("My Webinars");
            _mainWindow = SeleniumTestDriver.CurrentWindowHandle;
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

        public void PickDate(DateTime date, bool firstRun)
        {
            var dateWidget = SeleniumTestDriver.FindByCssSelectorClick("div#floatcal table");
            var monthHeader = SeleniumTestDriver.FindByCssSelectorClick("div#floatcal div.head");
            var nextButton = SeleniumTestDriver.FindByCssSelectorClick("div#floatcal div.head img.next");
            var prevButton = SeleniumTestDriver.FindByCssSelectorClick("div#floatcal div.head img.prev");
            var month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(date.Month);
            var year = date.Year;
            var day = date.Day;

            if (!firstRun)
            {
                while (count > 0)
                {
                    count--;
                    prevButton.Click();
                }
            }

            while (monthHeader.Text != string.Format("{0} {1}", month, year))
            {
                count++;
                nextButton.Click();
            }

            ReadOnlyCollection<IWebElement> rows = dateWidget.FindElements(By.TagName("tr"));
            ReadOnlyCollection<IWebElement> cols = dateWidget.FindElements(By.TagName("td"));

            //  Lets choose the 28th of the current month
            foreach (IWebElement colWebElement in cols.Where(colWebElement => colWebElement.Text.Equals(day.ToString())))
            {
                colWebElement.Click();
                break;
            }
        }

        //public void CompleteDetails()
        //{
        //    SeleniumTestDriver.FindByIdClick("privateConfCallRadio");
        //    SeleniumTestDriver.FindByIdClick("StartDate_Cal_0");
        //    SeleniumTestDriver.FindByCssSelectorClick("img.next");

            
        //    SeleniumTestDriver.TypeText("StartHour_0", "06:00");
            
        //    var startMeridian = new SelectElement(SeleniumTestDriver.FindById("StartAMPM_0"));
        //    startMeridian.SelectByText("PM");
        //    SeleniumTestDriver.TypeText("EndHour_0", "11:00");

        //    var endMeridian = new SelectElement(SeleniumTestDriver.FindById("EndAMPM_0"));
        //    endMeridian.SelectByText("PM");

        //    var timeZoneKey = new SelectElement(SeleniumTestDriver.FindById("TimeZoneKey"));
        //    timeZoneKey.SelectByValue("68");

        //    var recursDropList = new SelectElement(SeleniumTestDriver.FindById("Recurs"));
        //    recursDropList.SelectByText("Monthly");

        //    SeleniumTestDriver.FindByIdClick("monthly_enddate");
        //    SeleniumTestDriver.FindByCssSelectorClick("img.next");

        //    SeleniumTestDriver.FindByIdClick("attendee_type_1");

        //    SeleniumTestDriver.FindByCssSelectorClick("div.submit_bar input[Type=submit]");

        //}

        private void AddPanelists(IList<Presenter> presenters)
        {/*
            var nrOfPresenters = presenters.Count();

            if (nrOfPresenters > 3)
                SeleniumTestDriver.FindByIdClick("addPanelistsLink"); 


            for (int i = 0; i < nrOfPresenters; i++)
            {
                SeleniumTestDriver.TypeText("Panelist" + i + "Name_Full", presenters[i].WebUser.FullName);
                SeleniumTestDriver.TypeText("Panelist" + i + "Email", presenters[i].WebUser.email);
            }
           */
            SeleniumTestDriver.TypeText("Panelist1Name_Full", "Steve Presenter");
            SeleniumTestDriver.TypeText("Panelist1Email", "amSteve@gmail.com");
        }

        public string ScheduleASimilarWebinar(GTWebinar webinar)
        {
            var div = SeleniumTestDriver.FindByXPath(@"//span/b[contains(text(), '" + webinar.TemplateTitle + "')]/parent::span/parent::p/parent::div/parent::div/parent::div");
            var footer = div.FindElement(By.XPath(@"/self::node()/descendant::div[@class='scheduleAnotherFooter']"));
            var scheduleSimilarLink = footer.FindElement(By.XPath(@"/self::node()/descendant::p/descendant::span/descendant::a[contains(text(), 'Schedule Similar Webinar')]"));
            scheduleSimilarLink.Click();

            var wait = new WebDriverWait(SeleniumTestDriver.WebDriver, TimeSpan.FromSeconds(5));

            var confCallRadio = wait.Until(d =>
            {
                var confCallRadioButton = SeleniumTestDriver.FindByIdClick("confCallRadio");
                return confCallRadioButton;
            });

            confCallRadio.Click();

            SeleniumTestDriver.FindByIdClick("StartDate_Cal_0");
            SeleniumTestDriver.FindByCssSelectorClick("img.next");

            PickDate(webinar.StartTime, true);

            //  Start hour
            SeleniumTestDriver.TypeText("StartHour_0", webinar.StartHour);
            var startMeridian = new SelectElement(SeleniumTestDriver.FindById("StartAMPM_0"));
            startMeridian.SelectByText(webinar.StartMeridian);

            //  End hour
            SeleniumTestDriver.TypeText("EndHour_0", webinar.EndHour);
            var endMeridian = new SelectElement(SeleniumTestDriver.FindById("EndAMPM_0"));
            endMeridian.SelectByText(webinar.EndMeridian);

            var timeZoneKey = new SelectElement(SeleniumTestDriver.FindById("TimeZoneKey"));
            timeZoneKey.SelectByValue(webinar.TimeZoneKey.ToString());

            
            SeleniumTestDriver.FindByIdClick("pstn");
            SeleniumTestDriver.FindByIdClick("pstnTF");

            AddPanelists(null);

            SeleniumTestDriver.FindByCssSelectorClick("div.submit_bar input[Type=submit]");

            //  Second tab - wait a bit
            wait = new WebDriverWait(SeleniumTestDriver.WebDriver, TimeSpan.FromSeconds(1));
            IWebElement webinarKeyHiddenInput = null;
            string webinarKey = null;

            var submitButtonContinue = wait.Until(d =>
            {
                var submitButton = SeleniumTestDriver.FindByXPath(@"//input[@value='Save and Continue >']");
                webinarKeyHiddenInput = SeleniumTestDriver.FindByXPath(@"//input[@name='WebinarKey']");
                return submitButton;
            });

            webinarKey = webinarKeyHiddenInput.GetAttribute("value");
            submitButtonContinue.Click();

            SeleniumTestDriver.FindByLinkTextClick("Clear All");

            //  Third tab - wait a bit
            wait = new WebDriverWait(SeleniumTestDriver.WebDriver, TimeSpan.FromSeconds(1));

            var commitButton = wait.Until(d =>
            {
                var commitInput = SeleniumTestDriver.FindByXPath(@"//input[@value='Save and Email me the Invitation']");
                return commitInput;
            });

            SeleniumTestDriver.FindByXPathClick(@"//input[@name='ApprovalRequired'][2]"); // index starts at 1, not 0

            commitButton.Click();

            return webinarKey;
        }

        public void GetPhoneNumbersAndAccessCodes(Dictionary<string, string> webinarDetails, string xPathForType)
        {
            //Perform the click operation that opens new window for details
            SeleniumTestDriver.FindByXPathClick(xPathForType);

            //Switch to new window opened
            //SeleniumTestDriver.SwitchToWindow(SeleniumTestDriver.Windows.First());
            foreach (var window in SeleniumTestDriver.Windows)
            {
                SeleniumTestDriver.SwitchToWindow(window);
            }

            SeleniumTestDriver.Wait(500);

            // Perform the actions on new window
            var phoneNumber = SeleniumTestDriver.FindByCssSelector("#parentWraper > div:nth-child(3) > p:nth-child(6) > span");
            var accessCode = SeleniumTestDriver.FindByCssSelector("#parentWraper > div:nth-child(3) > p:nth-child(7) > span");

            webinarDetails.Add("Phone", phoneNumber.Text);
            webinarDetails.Add("AccessCode", accessCode.Text);

            //Close the new window, if that window no more required
            SeleniumTestDriver.CloseWindow();

            //Switch back to original browser (first window)
            SeleniumTestDriver.SwitchToWindow(_mainWindow);
        }
    }
}
