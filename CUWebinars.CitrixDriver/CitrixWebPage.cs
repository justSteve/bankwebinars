using CUWebinars.Business.Models;
using KesselRun.SeleniumCore.Enums;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace CUWebinars.CitrixDriver
{
    public class CitrixWebPage : BasePage
    {
        private int count = 0;
        private string _mainWindow;

        public CitrixWebPage(KesselRun.SeleniumCore.TestDrivers.Contracts.ITestDriver seleniumTestDriver, string url)
        {
            Url = url;
            SeleniumTestDriver = seleniumTestDriver;
        }

        public void Wait(int milliSeconds = 1000)
        {
            Thread.Sleep(milliSeconds);
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

        //public void ClearCookies()
        //{
        //    SeleniumTestDriver.ClearFederatedCookies();
        //}

        public void GotToLoginPage()
        {
            var webDriverWait = new WebDriverWait(SeleniumTestDriver.WebDriver, TimeSpan.FromSeconds(20));

            var loginLink = webDriverWait.Until(ExpectedConditions.ElementIsVisible(By.PartialLinkText("Log In")));

            loginLink.SendKeys(Keys.Enter);
        }


        public void EnterEmailAddress(string email)
        {
            Wait(500);
            IWebElement emailInput = SeleniumTestDriver.FindById("emailAddress");
            emailInput.Clear();
            emailInput.SendKeys(email);
        }

        public void EnterPasswordWhereUserExists(string password)
        {
            Wait(500);
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
            var webDriverWait = new WebDriverWait(SeleniumTestDriver.WebDriver, TimeSpan.FromSeconds(20));

            var submitButton = webDriverWait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input#submit")));

            submitButton.Click();
        }

        public void ClickViewLinkForAccessCodeAndPhoneNumbers()
        {
            SeleniumTestDriver.FindByXPathClick("//*[@id='SessionSet1_1']/a");
        }

        public void GoToWebinarsPage()
        {
            SeleniumTestDriver.FindByIdClick("menu-toggle", ExpectedCondition.ElementIsVisible, 15);
            SeleniumTestDriver.FindByCssSelectorClick("#gotowebinar > a");
            SeleniumTestDriver.FindByXPathClick(@"//*[@href='#pastWebinars']", ExpectedCondition.ElementIsVisible, 10);
            //SeleniumTestDriver.FindByPartialLinkTextClick("My Webinars", ExpectedCondition.ElementIsVisible, 10);
            //_mainWindow = SeleniumTestDriver.WebDriver.CurrentWindowHandle;
        }

        public void ScheduleAWebinar()
        {
            SeleniumTestDriver.FindByLinkClick("Schedule a Webinar");
        }

        public void ChooseWebinarTemplate()
        {
            SeleniumTestDriver.FindByIdClick("WebinarChoice");
            Wait(500);
            SeleniumTestDriver.FindByCssSelectorClick("option[value=\"563748522|wid\"]");
        }

        public void PickDate(DateTime date, bool firstRun)
        {
//            var dateWidget = SeleniumTestDriver.FindByCssSelectorClick("div#floatcal table");
            var monthHeader = SeleniumTestDriver.FindByXPath("//*[@id='ui-datepicker-div']/div[1]/div/span[1]"); // month header of popup calendar
            var nextButton = SeleniumTestDriver.FindByXPath("//*[@id='ui-datepicker-div']/div[1]/a[2]/span"); // "next month" button of popup calendar
//            var prevButton = SeleniumTestDriver.FindByCssSelectorClick("div#floatcal div.head img.prev");
            var month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(date.Month);
            var year = date.Year;
            var day = date.Day;

            //if (!firstRun)
            //{
            //    while (count > 0)
            //    {
            //        count--;
            //        prevButton.Click();
            //    }
            //}
            //ClickMonth(monthHeader, month, year, prevButton);
            
            //*[@id="ui-datepicker-div"]/div[1]/div/span[2]
            
            while (monthHeader.Text != month)
            {
                count--;
                nextButton.Click();
            }

            SeleniumTestDriver.FindByXPath("//*[@id='ui-datepicker-div']/table/tbody/tr[4]/td[@data-month='0']/a[child::text()='15']");

            //*[@id="ui-datepicker-div"]/table/tbody/tr[3]/td[4]/a
            //*[@id="ui-datepicker-div"]/table/tbody/tr[4]/td[@data-month="0"]/a[child::text()=15]
            //ReadOnlyCollection<IWebElement> rows = dateWidget.FindElements(By.TagName("tr"));
            //ReadOnlyCollection<IWebElement> cols = dateWidget.FindElements(By.TagName("td"));

            ////  Lets choose the 28th of the current month
            //foreach (IWebElement colWebElement in cols.Where(colWebElement => colWebElement.Text.Equals(day.ToString())))
            //{
            //    colWebElement.Click();
            //    break;
            //}
        }

        private static void ClickMonth(IWebElement monthHeader, string month, int year, IWebElement prevButton)
        {
            try
            {
                if (monthHeader.Text != string.Format("{0} {1}", month, year))
                {
                    prevButton.Click();
                }
            }
            catch (Exception)
            {
                if (monthHeader.Text != string.Format("{0} {1}", month, year))
                {
                    prevButton.Click();
                }
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

        private void AddPanelists(string presenter)
        {

            SeleniumTestDriver.TypeText(FinderStrategy.Name, "Panelist1Name_Full", "Steve Presenter");
            SeleniumTestDriver.TypeText(FinderStrategy.Name, "Panelist1Email", "amSteve@gmail.com");
        }

        public string ScheduleASimilarWebinar(GTWebinar webinar)
        {
            //var div = SeleniumTestDriver.FindByXPath(@"//span/b[contains(text(), '" + webinar.TemplateTitle + "')]/parent::span/parent::p/parent::div/parent::div/parent::div");
            //var footer = div.FindElement(By.XPath(@"/self::node()/descendant::div[@class='scheduleAnotherFooter']"));
            SeleniumTestDriver.WebDriver.FindElements(By.PartialLinkText("BaseLive Event Clone")).First().Click();
            SeleniumTestDriver.FindByIdClick("scheduleSimilar");
            //var scheduleSimilarLink = footer.FindElement(By.XPath(@"/self::node()/descendant::p/descendant::span/descendant::a[contains(text(), 'Schedule Similar Webinar')]"));
            //scheduleSimilarLink.Click();

            var wait = new WebDriverWait(SeleniumTestDriver.WebDriver, TimeSpan.FromSeconds(5));

            //var webinarTitleText = wait.Until(d =>
            //{
            //    var webinarTitleTextInput = SeleniumTestDriver.FindByIdClick("WebinarTitle");
            //    return webinarTitleTextInput;
            //});


            SeleniumTestDriver.TypeText(FinderStrategy.Id, "name", webinar.Title);
            SeleniumTestDriver.TypeText(FinderStrategy.Id, "description", webinar.Description);


            //var confCallRadio = wait.Until(d =>
            //{
            //    var confCallRadioButton = SeleniumTestDriver.FindByIdClick("confCallRadio");
            //    return confCallRadioButton;
            //});

            //confCallRadio.Click();



            SeleniumTestDriver.FindByIdClick("webinarTimesForm.dateTimes_0.baseDate");

            //wait = new WebDriverWait(SeleniumTestDriver.WebDriver, TimeSpan.FromSeconds(5));

            //var calendarImage = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("img.next")));
            //calendarImage.Click();

            PickDate(webinar.StartTime, true);

            //  Start hour
            SeleniumTestDriver.TypeText(FinderStrategy.Name, "StartHour_0", webinar.StartHour);
            var startMeridian = new SelectElement(SeleniumTestDriver.FindById("StartAMPM_0"));
            startMeridian.SelectByText(webinar.StartMeridian);

            //  End hour
            SeleniumTestDriver.TypeText(FinderStrategy.Name, "EndHour_0", webinar.EndHour);
            var endMeridian = new SelectElement(SeleniumTestDriver.FindById("EndAMPM_0"));
            endMeridian.SelectByText(webinar.EndMeridian);

            var timeZoneKey = new SelectElement(SeleniumTestDriver.FindById("TimeZoneKey"));
            timeZoneKey.SelectByValue(webinar.TimeZoneKey.ToString());


            SeleniumTestDriver.FindByIdClick("pstn");
            SeleniumTestDriver.FindByIdClick("pstnTF");

            AddPanelists(webinar.PresenterFirstName + ' ' + webinar.PresenterLastName);

            SeleniumTestDriver.FindByCssSelectorClick("div.submit_bar input[Type=submit]");

            //  Second tab - wait a bit
            wait = new WebDriverWait(SeleniumTestDriver.WebDriver, TimeSpan.FromSeconds(1));

            var submitButtonContinue =
                wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(@"//input[@value='Save and Continue >']")));

            wait = new WebDriverWait(SeleniumTestDriver.WebDriver, TimeSpan.FromSeconds(5));

            IWebElement webinarKeyHiddenInput =
                wait.Until(ExpectedConditions.ElementExists(By.XPath(@"//input[@name='WebinarKey']")));

            string webinarKey = webinarKeyHiddenInput.GetAttribute("value");

            submitButtonContinue.Click();

            //  Third tab - wait a bit
            SeleniumTestDriver.FindByLinkClick("Clear All");

            wait = new WebDriverWait(SeleniumTestDriver.WebDriver, TimeSpan.FromSeconds(1));

            var commitButton =
                wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(@"//input[@value='Save and Email me the Invitation']")));

            SeleniumTestDriver.FindByXPathClick(@"//input[@name='ApprovalRequired'][2]"); // index starts at 1, not 0

            commitButton.Click();

            return webinarKey;
        }

        public void GetPhoneNumbersAndAccessCodes(Dictionary<string, string> webinarDetails, string xPathForType)
        {
            //Perform the click operation that opens new window for details
            SeleniumTestDriver.FindByXPathClick(xPathForType);

            //Switch to new window opened
            foreach (var window in SeleniumTestDriver.WebDriver.WindowHandles)
            {
                SeleniumTestDriver.WebDriver.SwitchTo().Window(window);
            }

            var webDriverWait = new WebDriverWait(SeleniumTestDriver.WebDriver, TimeSpan.FromSeconds(20));


            // Perform the actions on new window
            var phoneNumber =
                webDriverWait.Until(
                    ExpectedConditions.ElementIsVisible(By.XPath("//*[contains(text(),'Toll-free:')]/span"))
                    );

            var accessCode = SeleniumTestDriver.FindByXPath("//*[contains(text(),'Access Code:')]/span");

            webinarDetails.Add("Phone", phoneNumber.Text);
            webinarDetails.Add("AccessCode", accessCode.Text);

            //Close the new window, if that window no more required
            SeleniumTestDriver.Quit();

            //Switch back to original browser (first window)
            SeleniumTestDriver.WebDriver.SwitchTo().Window(_mainWindow);
        }
    }
}
