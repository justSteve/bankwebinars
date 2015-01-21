using CUWebinars.Business.Core.Helpers;
using CUWebinars.Business.Models;
using KesselRun.SeleniumCore.Enums;
using KesselRun.SeleniumCore.Infrastructure;
using KesselRun.SeleniumCore.Infrastructure.Factories;
using KesselRun.SeleniumCore.Infrastructure.Factories.Contracts;
using System;
using System.Configuration;

namespace CUWebinars.CitrixDriver
{
    class Program
    {
        private const string Attendee = "Attendee";
        private const string Organizer = "Organizer";
        private const string Panelist = "Panelist";

        private static KesselRun.SeleniumCore.TestDrivers.Contracts.ITestDriver webDriver;
        private static CitrixWebPage citrixWebPage;
        private static TTSWebinarsContext context = new TTSWebinarsContext();
        private static DateTimeHelper dateTimeHelper = new DateTimeHelper();

        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green; // love the old green screen!

            var ttsWebinar = GetWebinar();

            var webinar = GetGTWebinar(ttsWebinar);

            if (webinar != null)
            {
                ITestDriverFactory foundry = new TestDriverFactory(new DriverOptions
                {
                    DriverEngine = DriverEngine.AutoDetect,
                    DriverExePath = @ConfigurationManager.AppSettings["FireFoxBinaryPath"],
                    Port = 8889
                });

                webDriver = foundry.CreateTestDriver(DriverType.Firefox);

                citrixWebPage = new CitrixWebPage(webDriver, ConfigurationManager.AppSettings["HomeUrl"]);

                //  Open the page for login details
                citrixWebPage.Open();

                //  Login, using the credentials in App.config
                Login();

                citrixWebPage.GoToWebinarsPage();

                var newWebinarKey = ScheduleASimilarWebinar(webinar);

                ttsWebinar.AccessCodeAttendee = newWebinarKey.AttendeeAccessCode;
                ttsWebinar.AccessCodeOrganizer = newWebinarKey.OrganizerAccessCode;
                ttsWebinar.AccessCodePresenter = newWebinarKey.PanelistAccessCode;

                ttsWebinar.AccessPhone = newWebinarKey.AttendeePhoneNr;
                
                ttsWebinar.WebinarKey = newWebinarKey.WebinarId;
                ttsWebinar.ConnectionInfo = newWebinarKey.WebinarUrl;

                context.SaveChanges();

                citrixWebPage.Close();

                Console.WriteLine("{0} Press any key to close...", Environment.NewLine);
                Console.ReadLine();
            }
        }

        private static Webinar GetWebinar()
        {
            Console.Write("Enter Webinar Number: ");
            var textEntered = Console.ReadLine();
            int webinarId;
            while (!int.TryParse(textEntered, out webinarId))
            {
                Console.Write("Invalid input. The Webinar Number must be a valid Integer: ");
                textEntered = Console.ReadLine();
            }

            var webinar = context.Webinars.Find(webinarId);

            return webinar;
        }

        private static GTWebinar GetGTWebinar(Webinar webinar)
        {
            var GTWebinar = new GTWebinar
            {
                //StartTime = DateTime.Now.AddMonths(2),
                StartTime = webinar.Date,
                StartHour = webinar.Date.ToString("hh:mm").ToLower(),
                StartMeridian = webinar.Date.ToString("tt"),
                EndHour = webinar.Date.AddHours((double)webinar.Duration).ToString("hh:mm"),
                EndMeridian = webinar.Date.ToString("tt"),
                PresenterFirstName =  webinar.Presenter.WebUser.FirstName,
                PresenterLastName=  webinar.Presenter.WebUser.LastName,
                RequirePassword = true,
                TemplateTitle = ConfigurationManager.AppSettings["TemplateTitle"],
                TimeZoneKey = 68,
                Description = webinar.Description,
                Title = webinar.Title // central
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

        private static WebinarDetails ScheduleASimilarWebinar(GTWebinar webinar)
        {
            
            return citrixWebPage.ScheduleASimilarWebinar(webinar);

        }
    }
}
