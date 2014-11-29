using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CUWebinars.Business.Tests
{
    
    public class Globals
    {
        private static string _tenant;
        private static string _localDbConnectionString;
        private static string _membershipRebootConnectionString;
        private static string _ttsDatabaseLocal;
        private static bool _useAzureWebjobs;


        static Globals()
        {
            _tenant = ConfigurationManager.AppSettings["Tenant"];
            _useAzureWebjobs = bool.Parse(ConfigurationManager.AppSettings["UseAzureWebjobs"]);
            _localDbConnectionString = ConfigurationManager.ConnectionStrings[Constants.LocalDbConnectionStringName].ConnectionString;
            _membershipRebootConnectionString = ConfigurationManager.ConnectionStrings[Constants.MembershipRebootConnectionStringName].ConnectionString;
            _ttsDatabaseLocal= ConfigurationManager.ConnectionStrings[Constants.TtsDatabaseLocalName].ConnectionString;
        }

        public static string LocalDbConnectionString
        {
            get { return _localDbConnectionString; }
        }

        public static string MembershipRebootConnectionString
        {
            get { return _membershipRebootConnectionString; }
        }

        public static string Tenant
        {
            get { return _tenant; }
        }

        public static string TtsDatabaseLocal
        {
            get { return _ttsDatabaseLocal; }
        }

        public static bool UseAzureWebjobs
        {
            get { return _useAzureWebjobs; }
        }
    }
}