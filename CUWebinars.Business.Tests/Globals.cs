using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CUWebinars.Business.Tests
{
    
    public class Globals
    {
        private static string _tenant;
        private static string _localDbConnectionString;
        private static string _membershipRebootConnectionString;


        static Globals()
        {
            _tenant = ConfigurationManager.AppSettings["Tenant"];
            _localDbConnectionString = ConfigurationManager.ConnectionStrings[Constants.LocalDbConnectionStringName].ConnectionString;
            _membershipRebootConnectionString = ConfigurationManager.ConnectionStrings[Constants.MembershipRebootConnectionStringName].ConnectionString;
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
    }
}