using System;
using System.Collections.Specialized;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CUWebinars.WebUi.Tests
{
    [TestClass]
    public class Global
    {
        public string ChromeWebDriverPath { get; set; }
        public string ChromeWebDriverPort { get; set; }
        public string FirefoxExePath { get; set; }
        public string FirefoxBrowserPort { get; set; }
        public string HomeUrl { get; set; }
        public string IeWebDriverPath { get; set; }
        public string IeWebDriverPort { get; set; }


        internal class GlobalSingletonCreator
        {
            static GlobalSingletonCreator()
            {
                NameValueCollection ApplicationSettingsSection = ConfigurationManager.AppSettings;

                if (ReferenceEquals(null, ApplicationSettingsSection))
                {
                    throw new ArgumentNullException("AppSettings not found in config file as expected.");
                }

                UniqueInstance.IeWebDriverPath = ApplicationSettingsSection["IeWebDriverPath"];
                UniqueInstance.IeWebDriverPort = ApplicationSettingsSection["IeWebDriverPort"];
                UniqueInstance.ChromeWebDriverPort = ApplicationSettingsSection["ChromeWebDriverPort"];
                UniqueInstance.ChromeWebDriverPath = ApplicationSettingsSection["ChromeWebDriverPath"];
                UniqueInstance.FirefoxExePath = ApplicationSettingsSection["FirefoxExePath"];
                UniqueInstance.FirefoxBrowserPort = ApplicationSettingsSection["FirefoxBrowserPort"];
                UniqueInstance.HomeUrl = ApplicationSettingsSection["HomeUrl"];
            }

            // Private object instantiated with private constructor
            internal static readonly Global UniqueInstance = new Global();
        }

        public static Global GlobalConfigSingleton
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
