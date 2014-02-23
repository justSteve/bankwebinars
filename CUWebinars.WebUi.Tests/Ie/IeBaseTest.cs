//using System.Web.Configuration;
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
            TestDriver = new IeTestDriver();
        }
                
    }
}
