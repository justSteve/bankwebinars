using CUWebinars.Business.Core;
using CUWebinars.Business.Models;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace CUWebinars.Business.Tests.IntegrationTests
{
    [TestClass]
    public class DataOperationsIntegrationTests
    {
        [TestInitialize]
        public void SetUp()
        {
            var databaseSetup = new DatabaseSetup { ConnectionString = Globals.TtsDatabaseLocal};
            databaseSetup.InstallDatabase(Constants.TtsDatabaseResourceName);
        }

        [TestCleanup]
        public void TearDown()
        {
            var databaseSetup = new DatabaseSetup { ConnectionString = Globals.TtsDatabaseLocal };
            databaseSetup.UninstallDatabase(Constants.TtsDatabaseResourceName);
        }

        [TestMethod]
        //[Ignore]
        public void GetTimeZoneByZipCodeReturnsValidTimeZone()
        {
            var dataOperations = new DataOperations(
                ConfigurationManager.ConnectionStrings[Constants.TtsDatabaseLocalName].ConnectionString
                );

            var timeZone = dataOperations.GetTimeZoneByZipCode("00210");

            Assert.AreEqual(USTimeZone.Alaska, timeZone);
        }

    }
}
