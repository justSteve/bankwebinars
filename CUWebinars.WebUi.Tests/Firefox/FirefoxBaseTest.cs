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
            var port = GlobalTestConfig.FirefoxBrowserPort;
            TestDriver = new FirefoxTestDriver(port);
        }
    }
}
