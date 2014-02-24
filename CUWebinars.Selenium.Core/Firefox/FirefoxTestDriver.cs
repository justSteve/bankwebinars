using System;
using System.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace CUWebinars.Selenium.Core.Firefox
{
   public class FirefoxTestDriver : BaseTestDriver
    {
       public FirefoxTestDriver(string port)
        {
            this.port = port;
            var firefoxBinaryPath = ConfigurationManager.AppSettings["FirefoxExePath"]; 

            var firefoxBinary = new FirefoxBinary(firefoxBinaryPath);
            var firefoxProfile = new FirefoxProfile();

            webDriver = new FirefoxDriver(firefoxBinary, firefoxProfile);
            webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));
            webDriver.Manage().Timeouts().SetScriptTimeout(TimeSpan.FromSeconds(10));
            
        }

       public override void Initialize()
       {
           //   do nothing intentionally.
       }

        public override void GoToUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                url = ConfigurationManager.AppSettings["HomeUrl"];
            }

            INavigation navigation = webDriver.Navigate();
            navigation.GoToUrl(url);
        }
    }
}
