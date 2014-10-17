using CUWebinars.WebUi.Tests2.Infrastructure;
using KesselRun.SeleniumCore.TestDrivers.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CUWebinars.WebUi.Tests2.Browsers
{
    [TestClass]
    public class BaseTest
    {
        protected ITestDriver TestDriver { get; set; }
        protected WebUiTestGlobals WebUiTestGlobalsTestConfig { get; set; }

        [TestCleanup]
        public void TearDown()
        {
            try
            {
                if(!ReferenceEquals(null, TestDriver))
                    TestDriver.Quit();
            }
            catch (Exception)
            {
                // Ignore errors if unable to close the browser
            }
        }

    }
}
