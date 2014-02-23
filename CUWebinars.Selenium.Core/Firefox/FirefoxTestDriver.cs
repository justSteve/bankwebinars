using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace CUWebinars.Selenium.Core.Firefox
{
   public class FirefoxTestDriver : BaseTestDriver
    {
       public FirefoxTestDriver(string port)
        {
            this.port = port;
            //firefoxBinaryPath = ConfigurationManager.AppSettings["firefox"].ToString();
            const string firefoxBinaryPath = @"C:\Program Files (x86)\Mozilla Firefox\firefox.exe";

            var firefoxBinary = new FirefoxBinary(firefoxBinaryPath);
            var firefoxProfile = new FirefoxProfile();

            webDriver = new FirefoxDriver(firefoxBinary, firefoxProfile);
            webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));
            webDriver.Manage().Timeouts().SetScriptTimeout(TimeSpan.FromSeconds(10));
            
        }

        public override void GoToUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                //url = ConfigurationManager.AppSettings["testServerUrl"].ToString();
                url = "localhost";
            }

            INavigation navigation = webDriver.Navigate();
            navigation.GoToUrl(url);
        }
    }
}
