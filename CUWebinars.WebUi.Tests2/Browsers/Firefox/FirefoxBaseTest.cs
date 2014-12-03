using CUWebinars.WebUi.Tests2.Infrastructure;
using KesselRun.SeleniumCore.Infrastructure;
using KesselRun.SeleniumCore.Infrastructure.Factories;
using KesselRun.SeleniumCore.Infrastructure.Factories.Contracts;
using KesselRun.SeleniumCore.TestDrivers.Browsers.Firefox;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;

namespace CUWebinars.WebUi.Tests2.Browsers.Firefox
{
    [TestClass]
    public class FirefoxBaseTest : BaseTest
    {
        protected ITestDriverFactory Foundry;

        [TestInitialize]
        public void Setup()
        {
            WebUiTestGlobalsTestConfig = WebUiTestGlobals.WebUiTestGlobalsConfigSingleton;
            var port = int.Parse(WebUiTestGlobalsTestConfig.FirefoxBrowserPort,
                NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite);

            var pathToDriver = WebUiTestGlobalsTestConfig.FirefoxExePath;

            Foundry = new TestDriverFactory(new DriverOptions
            {
                DriverExePath = pathToDriver,
                Port = port,
                Url = WebUiTestGlobalsTestConfig.HomeUrl
            });

            TestDriver = Foundry.CreateTestDriver<FirefoxTestDriver>();
        }

    }
}