using CUWebinars.Business.Core.Helpers;
using CUWebinars.Business.Models;
using CUWebinars.Selenium.Core;
using System;
using System.Configuration;

namespace CUWebinars.CitrixDriver
{
    class Program
    {
        private static ITestDriver webDriver;
        private static CitrixWebPage citrixWebPage;
        private static TTSWebinarsContext context = new TTSWebinarsContext();
        private static DateTimeHelper dateTimeHelper = new DateTimeHelper();
        static void Main(string[] args)
        {
            var ttsWebinar = GetWebinar();
            var webinar = GetGTWebinar(ttsWebinar);

            webDriver = new IeTestDriver { DriverPath = @"E:\", DriverPort = 8889 };
            webDriver.Initialize();


            citrixWebPage = new CitrixWebPage(webDriver, ConfigurationManager.AppSettings["HomeUrl"]);
            
            citrixWebPage.Open();
            Login();


            var newWebinarKey = ScheduleASimilarWebinar(webinar);

            citrixWebPage.Close();

            Console.ReadLine(); 
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
                StartTime = webinar.Date,
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
