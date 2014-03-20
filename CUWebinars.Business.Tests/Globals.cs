using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CUWebinars.Business.Tests
{
    [TestClass]
    public class Globals
    {
        public const string AppTenant = "BankWebinars";

        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            //Database.SetInitializer(new DefaultMembershipRebootDatabase);

        }

    }
}