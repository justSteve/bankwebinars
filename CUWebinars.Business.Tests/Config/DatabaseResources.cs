using Microsoft.SqlServer.Management.Smo;
using System;
using System.IO;
using System.Reflection;

namespace CUWebinars.Business.Tests.Config
{
    public class DatabaseResources
    {
        private Assembly _assembly;
        private const string DbExtension = ".mdf";
        private const string MsSqlLocalDbServerName = @"(localdb)\MSSQLLocalDB";
        private Server _msSqlLocalDbServer;
        private const string TtsDatabase = "TTSDatabase";
        private const string CUWebinars = "CUWebinars";
        private const string CUWebinarsMemship = "CUWebinarsMemship";
        private const string MembershipReboot = "MembershipReboot";
        private string _dbMembershipRebootFileName;
        private string _dbMembershipRebootFullPath;
        //private string _dbTTSDatabaseFileName;
        //private string _dbTTSDatabaseFullPath;
        private string _dbCUWebinarsFileName;
        private string _dbCUWebinarsFullPath;
        private string _dbCUWebinarsMemshipFileName;
        private string _dbCUWebinarsMemshipFullPath;

        private string _resourceStreamMembershipReboot;
        private string _resourceStreamCuWebinarsMemship;
        private string _resourceStreamCuWebinars;

        public DatabaseResources()
        {
            _assembly = Assembly.GetExecutingAssembly();
            _msSqlLocalDbServer = new Server(MsSqlLocalDbServerName);
        }

        public void PrimeMembershipTestsDatabases()
        {
            _dbMembershipRebootFileName = string.Concat(MembershipReboot, DbExtension);
            _dbCUWebinarsFileName = string.Concat(CUWebinars, DbExtension);
            _dbCUWebinarsMemshipFileName = string.Concat(CUWebinarsMemship, DbExtension);

            _dbMembershipRebootFullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _dbMembershipRebootFileName);
            _dbCUWebinarsMemshipFullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _dbCUWebinarsMemshipFileName);
            _dbCUWebinarsFullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _dbCUWebinarsFileName);

            _resourceStreamMembershipReboot = string.Concat("CUWebinars.Business.Tests.Db.", _dbMembershipRebootFileName);
            _resourceStreamCuWebinarsMemship = string.Concat("CUWebinars.Business.Tests.Db.", _dbCUWebinarsMemshipFileName);
            _resourceStreamCuWebinars = string.Concat("CUWebinars.Business.Tests.Db.", _dbCUWebinarsFileName);
        }

        public void CreateMembershipRebootDb()
        {
            var mdf = _assembly.GetManifestResourceStream(_resourceStreamMembershipReboot);
            var bytes = ReadFully(mdf);
            File.WriteAllBytes(_dbMembershipRebootFullPath, bytes);            
        }
        public void CreateCuWebinarsDb()
        {
            var mdf = _assembly.GetManifestResourceStream(_resourceStreamCuWebinars);
            var bytes = ReadFully(mdf);
            File.WriteAllBytes(_dbCUWebinarsFullPath, bytes);            
        }

        private byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[input.Length];
            input.Read(buffer, 0, buffer.Length);
            return buffer;

        }

        public void TearDownMembershipRebootDatabase()
        {
            _msSqlLocalDbServer = new Server(MsSqlLocalDbServerName);
            _msSqlLocalDbServer.KillDatabase(MembershipReboot);
        }

        public void TearDownCuWebinarsDatabase()
        {
            _msSqlLocalDbServer = new Server(MsSqlLocalDbServerName);
            _msSqlLocalDbServer.KillDatabase(CUWebinars);
        }


    }
}
