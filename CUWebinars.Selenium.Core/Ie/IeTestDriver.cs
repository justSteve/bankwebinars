using System.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using System;

namespace CUWebinars.Selenium.Core.Ie
{
    public class IeTestDriver : BaseTestDriver
    {
        public IeTestDriver()
        {            

        }

        public override void Initialize()
        {
            var internetExplorerDriverService = InternetExplorerDriverService.CreateDefaultService(DriverPath);
            var internetExplorerOptions = new InternetExplorerOptions
            {
                InitialBrowserUrl = "http://localhost:5556",
            };

            internetExplorerDriverService.Port = DriverPort; // this is the port for the driver, not the webpage

            //webDriver = new InternetExplorerDriver(internetExplorerDriverService, internetExplorerOptions);
            webDriver = new InternetExplorerDriver(DriverPath);
            
            webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));
            webDriver.Manage().Timeouts().SetScriptTimeout(TimeSpan.FromSeconds(10));            
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
