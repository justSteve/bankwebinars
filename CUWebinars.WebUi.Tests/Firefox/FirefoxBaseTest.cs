using System.Globalization;
using CUWebinars.Selenium.Core.Firefox;
using CUWebinars.WebUi.Tests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CUWebinars.WebUi.Tests.Firefox
{
    [TestClass]
    public class FirefoxBaseTest : BaseTest
    {
        [TestInitialize]
        public void Setup()
        {
            WebUiTestGlobalsTestConfig = WebUiTestGlobals.WebUiTestGlobalsConfigSingleton;
            var port = int.Parse(WebUiTestGlobalsTestConfig.FirefoxBrowserPort, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite);

            var pathToBinary = WebUiTestGlobalsTestConfig.FirefoxExePath;
            TestDriver = new FirefoxTestDriver { DriverPath = pathToBinary, DriverPort = port };

            TestDriver.Initialize();
        }
    }
}
