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
            //var port = ConfigurationManager.AppSettings["port"].ToString();
            var port = "5556";
            this.TestDriver = new FirefoxTestDriver(port);
        }
    }
}
