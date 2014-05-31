using System.Collections.Generic;
using System.Diagnostics;
using CUWebinars.Business.Core.Helpers;
using CUWebinars.Business.Models;
using CUWebinars.Selenium.Core;
using System;
using System.Configuration;
using CUWebinars.Selenium.Core.Firefox;

namespace CUWebinars.CitrixDriver
{
    class Program
    {
        private const string Attendee = "Attendee";
        private const string Organizer = "Organizer";
        private const string Panelist = "Panelist";

        private static ITestDriver webDriver;
        private static CitrixWebPage citrixWebPage;
        private static TTSWebinarsContext context = new TTSWebinarsContext();
        private static DateTimeHelper dateTimeHelper = new DateTimeHelper();
        private static Dictionary<string, Dictionary<string, string>> webinarDetails;
        static void Main(string[] args)
        {
            PrimeAccessCodeAndPhoneBucket();

            var ttsWebinar = GetWebinar();
            var webinar = GetGTWebinar(ttsWebinar);

            //webDriver = new IeTestDriver { DriverPath = @ConfigurationManager.AppSettings["IeDriverPath"], DriverPort = 8889 };
            webDriver = new FirefoxTestDriver { DriverPath = @ConfigurationManager.AppSettings["FireFoxBinaryPath"], DriverPort = 8889 };
            webDriver.Initialize();


            citrixWebPage = new CitrixWebPage(webDriver, ConfigurationManager.AppSettings["HomeUrl"]);
            
            citrixWebPage.Open();
            Login();


            var newWebinarKey = ScheduleASimilarWebinar(webinar);
            citrixWebPage.ClickViewLinkForAccessCodeAndPhoneNumbers();

            //  First get Organizer details
            citrixWebPage.GetPhoneNumbersAndAccessCodes(webinarDetails[Organizer], "//*[@id='ConfCallNumbersDiv1_1']/div[1]/a");

            //  Second get Panelist details
            citrixWebPage.GetPhoneNumbersAndAccessCodes(webinarDetails[Panelist], "//*[@id='ConfCallNumbersDiv1_1']/div[2]/a");

            //  Third get Attendee details
            citrixWebPage.GetPhoneNumbersAndAccessCodes(webinarDetails[Attendee], "//*[@id='ConfCallNumbersDiv1_1']/div[3]/a");

            Trace.WriteLine(webinarDetails[Organizer]["Phone"]);
            Trace.WriteLine(webinarDetails[Organizer]["AccessCode"]);
            Trace.WriteLine(webinarDetails[Panelist]["Phone"]);
            Trace.WriteLine(webinarDetails[Panelist]["AccessCode"]);
            Trace.WriteLine(webinarDetails[Attendee]["Phone"]);
            Trace.WriteLine(webinarDetails[Attendee]["AccessCode"]);

            citrixWebPage.Close();

            Console.ReadLine(); 
        }

        private static void PrimeAccessCodeAndPhoneBucket()
        {
            webinarDetails = new Dictionary<string, Dictionary<string, string>>(3)
            {
                {Organizer, new Dictionary<string, string>(2)},
                {Panelist, new Dictionary<string, string>(2)},
                {Attendee, new Dictionary<string, string>(2)}
            };
        }

        private static Webinar GetWebinar()
        {
            var webinarId = int.Parse(ConfigurationManager.AppSettings["WebinarId"]);
            var webinar = context.Webinars.Find(webinarId);
            return webinar;
        }

        private static GTWebinar GetGTWebinar(Webinar webinar)
        {
            var GTWebinar = new GTWebinar
            {
                StartTime = DateTime.Now.AddMonths(2),
                //StartTime = webinar.Date,
                StartHour = webinar.Date.ToString("hh:mm").ToLower(),
                StartMeridian = webinar.Date.ToString("tt"),
                EndHour = webinar.Date.AddHours((double)webinar.Duration).ToString("hh:mm"),
                EndMeridian = webinar.Date.ToString("tt"),
                PresenterFirstName =  webinar.Presenter.WebUser.FirstName,
                PresenterLastName=  webinar.Presenter.WebUser.LastName,
                RequirePassword = true,
                TemplateTitle = ConfigurationManager.AppSettings["TemplateTitle"],
                TimeZoneKey = 68 // central
            };

            return GTWebinar;
        }

        private static void Login()
        {
            var userName = ConfigurationManager.AppSettings["UserName"];
            var passWord = ConfigurationManager.AppSettings["Password"];

            citrixWebPage.GotToLoginPage();
            citrixWebPage.EnterEmailAddress(userName);
            citrixWebPage.EnterPasswordWhereUserExists(passWord);
            citrixWebPage.ClickSubmit();
        }

        private static string ScheduleASimilarWebinar(GTWebinar webinar)
        {
            citrixWebPage.GoToWebinarsPage();
            return citrixWebPage.ScheduleASimilarWebinar(webinar);

        }
        
        //private static void ScheduleAWebinar()
        //{
        //    citrixWebPage.GoToWebinarsPage();
        //    citrixWebPage.ScheduleAWebinar();
        //    citrixWebPage.ChooseWebinarTemplate();
        //    citrixWebPage.CompleteDetails();
        //}
    }
}
