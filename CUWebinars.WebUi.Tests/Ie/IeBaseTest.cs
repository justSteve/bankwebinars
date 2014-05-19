using CUWebinars.Selenium.Core.Ie;
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
            GlobalTestConfig = Global.GlobalConfigSingleton;
            var port = int.Parse(GlobalTestConfig.IeWebDriverPort, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite);
            
            var pathToDriver = GlobalTestConfig.IeWebDriverPath;
            TestDriver = new IeTestDriver { DriverPort = port, DriverPath = pathToDriver };

            TestDriver.Initialize();
        }
    }
}
