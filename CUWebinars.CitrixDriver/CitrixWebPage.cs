using System.CodeDom;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
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

            var loginLink = webDriverWait.Until(ExpectedConditions.ElementIsVisible(By.PartialLinkText("Sign In")));

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
            SeleniumTestDriver.FindByCssSelectorClick("#gotowebinar > a", ExpectedCondition.ElementIsVisible, 5);
            SeleniumTestDriver.FindByXPathClick(@"//*[@href='#pastWebinars']", ExpectedCondition.ElementIsVisible, 10);
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
            var yearHeader = SeleniumTestDriver.FindByXPath("//*[@id='ui-datepicker-div']/div[1]/div/span[2]", 
                ExpectedCondition.ElementIsVisible, 15); // year header of popup calendar
            var nextButton = SeleniumTestDriver.FindByXPath("//*[@id='ui-datepicker-div']/div[1]/a[2]/span", 
                ExpectedCondition.ElementIsVisible, 15); // "next month" button of popup calendar

            // get day, month and year from the date of the Webinar object
            var month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(date.Month);
            var year = date.Year;
            var day = date.Day;
            var calendarYearFromHeaderParsed = int.Parse(yearHeader.Text.Trim());

            if (year > calendarYearFromHeaderParsed)
            {
                while (yearHeader.Text != year.ToString())
                {
                    nextButton.Click();
                    yearHeader = SeleniumTestDriver.FindByXPath("//*[@id='ui-datepicker-div']/div[1]/div/span[2]");
                    nextButton = SeleniumTestDriver.FindByXPath("//*[@id='ui-datepicker-div']/div[1]/a[2]/span");
                }
            }

            var monthHeader = SeleniumTestDriver.FindByXPath("//*[@id='ui-datepicker-div']/div[1]/div/span[1]"); // month header of popup calendar

            while (monthHeader.Text != month)
            {
                nextButton.Click();
                monthHeader = SeleniumTestDriver.FindByXPath("//*[@id='ui-datepicker-div']/div[1]/div/span[1]");
                nextButton = SeleniumTestDriver.FindByXPath("//*[@id='ui-datepicker-div']/div[1]/a[2]/span");
            }

            // get all active links in the calendar (excludes greyed-out links)
            IEnumerable<IWebElement> dates = SeleniumTestDriver.WebDriver.FindElements(
                By.XPath("//*[@id='ui-datepicker-div']/table/tbody/tr/child::td[@data-handler='selectDay']")
                );

            // find and click the link with the same value as the day part of the date from the Webinar object
            IWebElement dateToClick = dates.Where(d => d.Text.Equals(day.ToString())).Single();
            dateToClick.Click();

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

        public WebinarDetails ScheduleASimilarWebinar(GTWebinar webinar)
        {
            // click the first webinar found with the title "BaseLine Event Clone"
            SeleniumTestDriver.WebDriver.FindElements(By.PartialLinkText(ConfigurationManager.AppSettings["TemplateTitle"])).First().Click();

            // Click the link Schedule a Similar Webinar
            SeleniumTestDriver.FindByIdClick("scheduleSimilar");

            // add title and description from the retrieved Bank Webinars Webinar
            SeleniumTestDriver.TypeText(FinderStrategy.Id, "name", webinar.Title);
            SeleniumTestDriver.TypeText(FinderStrategy.Id, "description", HttpUtility.HtmlEncode(webinar.Description));

            // Click with calendar widget icon to have the calendar expand
            SeleniumTestDriver.FindByIdClick("webinarTimesForm.dateTimes_0.baseDate");

            PickDate(webinar.StartTime, true);

            // click to create Webinar
            SeleniumTestDriver.FindByIdClick("schedule.submit.button");

            IList<string> lines = new List<string>();

            var audioInstructions = SeleniumTestDriver.FindByXPath("//*[@id='audioInstructions']", 
                ExpectedCondition.ElementIsVisible, 
                15).Text;

            using (var streamFromString = GenerateStreamFromString(audioInstructions))
            {
                using (var streamReader = new StreamReader(streamFromString))
                {
                    string line = string.Empty;

                    while ((line = streamReader.ReadLine()) != null)
                    {
                        lines.Add(line);
                    }
                }
            }


            var attendeeDetails = lines[0];
            var organizerDetails = lines[1];
            var panelistDetails = lines[2];


            var webinarDetails = GetAccessDetails(attendeeDetails, organizerDetails, panelistDetails);


            lines.Clear();

            var registrationUrlAndWebinarKeyParagraph =
                SeleniumTestDriver.FindByXPath("//*[@id='manageWebinar']//p[contains(text(), 'Registration URL')]",
                 ExpectedCondition.ElementIsVisible,
                 15).Text;

            using (var streamFromString = GenerateStreamFromString(registrationUrlAndWebinarKeyParagraph))
            {
                using (var streamReader = new StreamReader(streamFromString))
                {
                    string line = string.Empty;

                    while ((line = streamReader.ReadLine()) != null)
                    {
                        lines.Add(line);
                    }
                }
            }

            webinarDetails.WebinarId = lines[1].Substring(lines[1].IndexOf(":") + 1).Trim().Replace("-", string.Empty);
            webinarDetails.WebinarUrl = lines[0].Substring(lines[0].IndexOf("https"));

            return webinarDetails;
        }

        private static WebinarDetails GetAccessDetails(string attendeeDetails, string organizerDetails, string panelistDetails)
        {
            var webinarDetails = new WebinarDetails();

            string phonePattern = "[0-9]{3}-[0-9]{3}-[0-9]{4}";

            var phoneNumberRegex = new Regex(phonePattern);
            var matches = phoneNumberRegex.Matches(attendeeDetails);
            webinarDetails.AttendeePhoneNr = matches[0].Value;

            matches = phoneNumberRegex.Matches(organizerDetails);
            webinarDetails.OrganizerPhoneNr = matches[0].Value;

            matches = phoneNumberRegex.Matches(panelistDetails);
            webinarDetails.PanelistPhoneNr = matches[0].Value;

            string accessCodePattern = @"[0-9]{4,16}\sInternational";

            var accessCodeRegex = new Regex(accessCodePattern);
            matches = accessCodeRegex.Matches(attendeeDetails);
            webinarDetails.AttendeeAccessCode = new string(matches[0].Value.TakeWhile(c => !c.Equals(' ')).ToArray());

            matches = accessCodeRegex.Matches(organizerDetails);
            webinarDetails.OrganizerAccessCode = new string(matches[0].Value.TakeWhile(c => !c.Equals(' ')).ToArray());
            
            matches = accessCodeRegex.Matches(panelistDetails);
            webinarDetails.PanelistAccessCode = new string(matches[0].Value.TakeWhile(c => !c.Equals(' ')).ToArray());

            return webinarDetails;
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

        public Stream GenerateStreamFromString(string str)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(str);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
