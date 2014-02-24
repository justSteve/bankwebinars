//using System.Web.Configuration;

using System.Configuration;
using CUWebinars.Selenium.Core.Ie;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CUWebinars.WebUi.Tests.Ie
{
    [TestClass]
    public class IeBaseTest : BaseTest
    {
        [TestInitialize]
        public void Setup()
        {
            var port = int.Parse(ConfigurationManager.AppSettings["IeWebDriverPort"]);
            var pathToDriver = ConfigurationManager.AppSettings["IeWebDriverPath"];
            TestDriver = new IeTestDriver { DriverPort = port, DriverPath = pathToDriver };

            TestDriver.Initialize();
        }
                
    }
}
