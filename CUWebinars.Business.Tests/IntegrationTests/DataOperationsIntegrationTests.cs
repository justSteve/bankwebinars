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
        private const string DbExtension = ".mdf";
        private const string DbName = "TTSDatabase";
        private readonly string _dbFileName;
        private readonly string _dbFullPath;
        private readonly string _resourceStreamMemoryReference;

        public DataOperationsIntegrationTests()
        {
            _dbFileName = string.Concat(DbName, DbExtension);
            _dbFullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _dbFileName);
            _resourceStreamMemoryReference = string.Concat("CUWebinars.Business.Tests.Db.", _dbFileName);
        }

        [TestMethod]
        //[Ignore]
        public void GetTimeZoneByZipCodeReturnsValidTimeZone()
        {
            var _assembly = Assembly.GetExecutingAssembly();
            var mdf = _assembly.GetManifestResourceStream(string.Concat(_resourceStreamMemoryReference));
            var bytes = ReadFully(mdf);
            File.WriteAllBytes(_dbFullPath, bytes);

            DataOperations dataOperations = new DataOperations(
                ConfigurationManager.ConnectionStrings["TtsDatabaseLocal"].ConnectionString
                );

            var bla = dataOperations.GetTimeZoneByZipCode("00211");

            Assert.AreEqual(USTimeZone.Alaska, bla);
        }

        [TestCleanup]
        public void TearDown()
        {
            var server = new Server(@"(localdb)\MSSQLLocalDB");
            //server.Databases.Refresh();
            //server.DetachDatabase("TTSDatabase", false);
            server.KillDatabase(DbName);
        }


        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[input.Length];
            input.Read(buffer, 0, buffer.Length);
            return buffer;

        }

    }
}
