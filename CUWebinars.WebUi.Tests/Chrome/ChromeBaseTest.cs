using System.Configuration;
using CUWebinars.Selenium.Core.Chrome;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CUWebinars.WebUi.Tests.Chrome
{
    [TestClass]
    public class ChromeBaseTest : BaseTest
    {
        [TestInitialize]
        public void Setup()
        {
            var port = int.Parse(ConfigurationManager.AppSettings["ChromeWebDriverPort"]);
            var pathToDriver = ConfigurationManager.AppSettings["ChromeWebDriverPath"];
            GlobalTestConfig = Global.GlobalConfigSingleton;
            TestDriver = new ChromeTestDriver { DriverPort = port, DriverPath = pathToDriver };
            TestDriver.Initialize();
        }
    }
}
