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
        static void Main(string[] args)
        {
            webDriver = new IeTestDriver{ DriverPath=@"E:\", DriverPort=8889 };
            webDriver.Initialize();

            citrixWebPage = new CitrixWebPage(webDriver, ConfigurationManager.AppSettings["HomeUrl"]);
            
            citrixWebPage.Open();
            Login();

            var webinar = GetWebinarMock();

            var newWebinarKey = ScheduleASimilarWebinar(webinar);
            //ScheduleAWebinar();

            citrixWebPage.Close();
            Console.ReadLine(); 
        }

        private static CitrixWebinar GetWebinarMock()
        {
            var citrixWebinar = new CitrixWebinar
            {
                StartDate = DateTime.Today.AddMonths(1),
                StartHour = "08:30",
                StartMeridian = "PM",
                EndHour = "10:30",
                EndMeridian = "PM",
                RequirePassword = true,
                TemplateTitle = "BaseLive Event",
                TimeZoneKey = 68 // central
                //presenter.webuser.fullname
                //presenter.webuser.email
            };

            return citrixWebinar;
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

        private static string ScheduleASimilarWebinar(CitrixWebinar webinar)
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
