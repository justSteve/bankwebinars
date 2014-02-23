//using System.Web.Configuration;
using System;
using CUWebinars.Selenium.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CUWebinars.WebUi.Tests
{
    [TestClass]
    public class BaseTest
    {
        protected ITestDriver TestDriver { get; set; }

        [TestCleanup]
        public void TearDown()
        {
            try
            {
                TestDriver.CloseWindow();
                TestDriver.Quit();
            }
            catch (Exception)
            {
                // Ignore errors if unable to close the browser
            }
        }
    }
}
