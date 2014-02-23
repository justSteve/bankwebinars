using System;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;

namespace CUWebinars.Selenium.Core.Ie
{
    public class IeTestDriver : BaseTestDriver
    {
        public IeTestDriver()
        {            
            //ieTestDriverLocation = ConfigurationManager.AppSettings["ieTestDriverLocation"].ToString();
            const string ieTestDriverLocation = @"E:\";
            var internetExplorerDriverService = InternetExplorerDriverService.CreateDefaultService(ieTestDriverLocation);
            var internetExplorerOptions = new InternetExplorerOptions
            {
                IntroduceInstabilityByIgnoringProtectedModeSettings = true
            };

            internetExplorerDriverService.Port = 666; // this is the port for the driver, not the webpage
            //port = ConfigurationManager.AppSettings["port"].ToString(); // this is the port for the dev webserver itself
            port = "5556";

            webDriver = new InternetExplorerDriver(internetExplorerDriverService, internetExplorerOptions);
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
