using CUWebinars.Selenium.Core.Chrome;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;

namespace CUWebinars.WebUi.Tests.Chrome
{
    [TestClass]
    public class ChromeBaseTest : BaseTest
    {
        [TestInitialize]
        public void Setup()
        {
            WebUiTestGlobalsTestConfig = WebUiTestGlobals.WebUiTestGlobalsConfigSingleton;
            var port = int.Parse(WebUiTestGlobalsTestConfig.ChromeWebDriverPort, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite);
            var pathToDriver = WebUiTestGlobalsTestConfig.ChromeWebDriverPath;
            TestDriver = new ChromeTestDriver { DriverPort = port, DriverPath = pathToDriver };
            TestDriver.Initialize();
        }
    }
}
