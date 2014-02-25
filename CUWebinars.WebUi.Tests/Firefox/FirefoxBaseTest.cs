using System.Configuration;
using CUWebinars.Selenium.Core.Firefox;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CUWebinars.WebUi.Tests.Firefox
{
    [TestClass]
    public class FirefoxBaseTest : BaseTest
    {
        [TestInitialize]
        public void Setup()
        {
            var port = ConfigurationManager.AppSettings["FirefoxBrowserPort"]; 
            TestDriver = new FirefoxTestDriver(port);
        }
    }
}
