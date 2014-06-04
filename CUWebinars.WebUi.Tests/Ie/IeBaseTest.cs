using CUWebinars.Selenium.Core.Ie;
using CUWebinars.WebUi.Tests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;

namespace CUWebinars.WebUi.Tests.Ie
{
    [TestClass]
    public class IeBaseTest : BaseTest
    {
        [TestInitialize]
        public void Setup()
        {
            WebUiTestGlobalsTestConfig = WebUiTestGlobals.WebUiTestGlobalsConfigSingleton;
            var port = int.Parse(WebUiTestGlobalsTestConfig.IeWebDriverPort, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite);
            
            var pathToDriver = WebUiTestGlobalsTestConfig.IeWebDriverPath;
            TestDriver = new IeTestDriver { DriverPort = port, DriverPath = pathToDriver };

            TestDriver.Initialize();
        }
    }
}
