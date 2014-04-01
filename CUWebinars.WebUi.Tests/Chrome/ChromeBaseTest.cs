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
            GlobalTestConfig = Global.GlobalConfigSingleton;
            var port = int.Parse(GlobalTestConfig.ChromeWebDriverPort, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite);
            var pathToDriver = GlobalTestConfig.ChromeWebDriverPath;
            TestDriver = new ChromeTestDriver { DriverPort = port, DriverPath = pathToDriver };
            TestDriver.Initialize();
        }
    }
}
