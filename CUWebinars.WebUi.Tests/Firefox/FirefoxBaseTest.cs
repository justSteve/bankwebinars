using System.Globalization;
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
            GlobalTestConfig = Global.GlobalConfigSingleton;
            var port = int.Parse(GlobalTestConfig.FirefoxBrowserPort, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite);

            var pathToBinary = GlobalTestConfig.FirefoxExePath;
            TestDriver = new FirefoxTestDriver { DriverPath = pathToBinary, DriverPort = port };

            TestDriver.Initialize();
        }
    }
}
