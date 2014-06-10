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
            var internetExplorerOptions = new InternetExplorerOptions();
            
            webDriver = new InternetExplorerDriver(internetExplorerDriverService, internetExplorerOptions);
            
            //webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));
            //webDriver.Manage().Timeouts().SetScriptTimeout(TimeSpan.FromSeconds(10));            
        }
    }
}
