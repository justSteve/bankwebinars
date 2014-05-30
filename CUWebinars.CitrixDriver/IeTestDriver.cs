using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CUWebinars.Selenium.Core;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;

namespace CUWebinars.CitrixDriver
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
                InitialBrowserUrl = "http://www.gotomeeting.com/online/",
            };

            internetExplorerDriverService.Port = DriverPort; // this is the port for the driver, not the webpage

            webDriver = new InternetExplorerDriver(internetExplorerDriverService, internetExplorerOptions);

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
