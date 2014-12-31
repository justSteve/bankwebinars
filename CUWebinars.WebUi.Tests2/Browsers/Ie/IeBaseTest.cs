using CUWebinars.WebUi.Tests2.Infrastructure;
using KesselRun.SeleniumCore.Infrastructure;
using KesselRun.SeleniumCore.Infrastructure.Factories;
using KesselRun.SeleniumCore.Infrastructure.Factories.Contracts;
using KesselRun.SeleniumCore.TestDrivers.Browsers.Ie;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;

namespace CUWebinars.WebUi.Tests2.Browsers.Ie
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

            ITestDriverFactory foundry = new TestDriverFactory(new DriverOptions
            {
                DriverExePath = pathToDriver,
                Port = port,
                Url = WebUiTestGlobalsTestConfig.HomeUrl
            });

            TestDriver = foundry.CreateTestDriver<IeTestDriver>();
        }
    }
}
