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
            ScheduleAWebinar();

            citrixWebPage.Close();
            Console.ReadLine(); 
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

        private static void ScheduleAWebinar()
        {
            citrixWebPage.GoToWebinarsPage();
            citrixWebPage.ScheduleAWebinar();
            citrixWebPage.ChooseWebinarTemplate();
            citrixWebPage.CompleteDetails();
        }
    }
}
