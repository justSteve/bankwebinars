using System.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;

namespace CUWebinars.Selenium.Core.Chrome
{
    public class ChromeTestDriver : BaseTestDriver
    {
        public ChromeTestDriver() { }

        public override void Initialize()
        {
            ChromeDriverService chromeDriverService = ChromeDriverService.CreateDefaultService(DriverPath);
            var chromeOptions = new ChromeOptions();

            chromeDriverService.Port = DriverPort; // this is the port for the driver, not the webpage
            
            webDriver = new ChromeDriver(chromeDriverService, chromeOptions);
            webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));
            webDriver.Manage().Timeouts().SetScriptTimeout(TimeSpan.FromSeconds(10));            
        }

        public override void GoToUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                //url = ConfigurationManager.AppSettings["HomeUrl"];
                url = "http://127.0.0.1:5556/";
            }

            INavigation navigation = webDriver.Navigate();
            Wait(500);
            navigation.GoToUrl(url);
        }
    }
}
