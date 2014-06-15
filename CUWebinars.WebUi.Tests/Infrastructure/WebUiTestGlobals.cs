using System;
using System.Collections.Specialized;
using System.Configuration;

namespace CUWebinars.WebUi.Tests.Infrastructure
{
    public class WebUiTestGlobals
    {
        public string ChromeWebDriverPath { get; set; }
        public string ChromeWebDriverPort { get; set; }
        public string DefaultConnection { get; set; }
        public string FirefoxExePath { get; set; }
        public string FirefoxBrowserPort { get; set; }
        public string HomeUrl { get; set; }
        public string IeWebDriverPath { get; set; }
        public string IeWebDriverPort { get; set; }
        public string MembershipNotificationsUrl { get; set; }
        public string MembershipRebootConnection { get; set; }
        public string Tenant { get; set; }

        public string TtsDatabaseConnectionString { get; set; }

        internal class GlobalSingletonCreator
        {
            static GlobalSingletonCreator()
            {
                NameValueCollection applicationSettingsSection = ConfigurationManager.AppSettings;
                ConnectionStringSettingsCollection connectionStringSettingsSection = ConfigurationManager.ConnectionStrings;

                if (ReferenceEquals(null, applicationSettingsSection))
                {
                    throw new ArgumentNullException("AppSettings not found in config file as expected.");
                }

                UniqueInstance.IeWebDriverPath = applicationSettingsSection["IeWebDriverPath"];
                UniqueInstance.IeWebDriverPort = applicationSettingsSection["IeWebDriverPort"];
                UniqueInstance.ChromeWebDriverPort = applicationSettingsSection["ChromeWebDriverPort"];
                UniqueInstance.ChromeWebDriverPath = applicationSettingsSection["ChromeWebDriverPath"];
                UniqueInstance.DefaultConnection = connectionStringSettingsSection["DefaultConnection"].ConnectionString;
                UniqueInstance.FirefoxExePath = applicationSettingsSection["FirefoxExePath"];
                UniqueInstance.FirefoxBrowserPort = applicationSettingsSection["FirefoxBrowserPort"];
                UniqueInstance.HomeUrl = applicationSettingsSection["HomeUrl"];
                UniqueInstance.MembershipNotificationsUrl = applicationSettingsSection["MembershipNotificationsUrl"];
                UniqueInstance.MembershipRebootConnection = connectionStringSettingsSection["MembershipReboot"].ConnectionString;
                UniqueInstance.Tenant = applicationSettingsSection["Tenant"];
                UniqueInstance.TtsDatabaseConnectionString = connectionStringSettingsSection["TTSDataBase"].ConnectionString;
            }

            // Private object instantiated with private constructor
            internal static readonly WebUiTestGlobals UniqueInstance = new WebUiTestGlobals();
        }

        public static WebUiTestGlobals WebUiTestGlobalsConfigSingleton
        {
            get
            {
                return GlobalSingletonCreator.UniqueInstance;
            }
        }

        //private const int IisPort = 5556;
        //private const string ApplicationName = "CUWebinars.Web";
        //private static Process _iisProcess;

        //[AssemblyInitialize]
        //public static void AssemblyInitialize(TestContext context)
        //{
        //    //StartIis();
        //}

        //[AssemblyCleanup]
        //public static void TestCleanup()
        //{
        //    // Ensure IISExpress is stopped
        //    if (_iisProcess.HasExited == false)
        //    {
        //        _iisProcess.Kill();
        //    }
        //}

        //private static void StartIis()
        //{
        //    var applicationPath = GetApplicationPath(ApplicationName);
        //    const string programFiles = @"C:\Program Files";

        //    _iisProcess = new Process
        //    {
        //        StartInfo =
        //        {
        //            FileName = Path.Combine(programFiles, @"IIS Express\iisexpress.exe"),
        //            Arguments = string.Format("/path:{0} /port:{1}", applicationPath, IisPort)
        //        }
        //    };
        //    _iisProcess.Start();
        //}


        //protected static string GetApplicationPath(string applicationName)
        //{
        //    var solutionFolder =
        //        Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory)));

        //    return string.IsNullOrWhiteSpace(solutionFolder) ? null : Path.Combine(solutionFolder, applicationName);
        //}


        //public string GetAbsoluteUrl(string relativeUrl)
        //{
        //    return Path.Combine(String.Format("http://localhost:{0}", IisPort), relativeUrl);
        //}

    }
}
