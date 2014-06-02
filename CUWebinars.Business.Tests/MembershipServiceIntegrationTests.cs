using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.Ef;
using BrockAllen.MembershipReboot.WebHost;
using CUWebinars.Business.AccountService;
using CUWebinars.Business.Models;
using CUWebinars.Business.Repository;
using CUWebinars.Business.Tests.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CUWebinars.Business.Tests
{
    [TestClass]
    public class MembershipServiceIntegrationTests
    {

        private const string TestEmail = "mr_white@saymyname.com";
        private const string TestFirstName = "Walter";
        private const string TestLastName = "White";
        private const string TestPassword = "ghjG7J*";
        public MembershipServiceIntegrationTests()
        {
            //  MembershipReboot Database
            var databaseSetup = new DatabaseSetup {ConnectionString = Globals.MembershipRebootConnectionString};
            databaseSetup.InstallDatabase(Constants.CreateMemRebootDb);

            //  CUWebinars Database
            databaseSetup.ConnectionString = Globals.LocalDbConnectionString;
            databaseSetup.InstallDatabase(Constants.CreateDbDefault);
            
        }

        [TestInitialize]
        public void Setup()
        {
        }



        [TestMethod]
        [TestCategory(TestCategories.MembershipIntegration)]
        public void CreateANewUserAccount()
        {
            var ctx = new TTSWebinarsContext(Constants.LocalDbConnectionStringName);
            var memRebootCtx = new DefaultMembershipRebootDatabase();
            var refDataRepository = new RefDataRepository();
            var config = MembershipRebootConfig.Create(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Constants.UpTwoFolders), new StateService(), refDataRepository);
            var userAccountService = new UserAccountService(config, new DefaultUserAccountRepository());
            
            IMembershipService membershipService = new MembershipService(new InstitutionRepository(ctx), 
                new RefDataRepository(), 
                new SamAuthenticationService(userAccountService),
                userAccountService,
                new WebUserRepository(ctx)
                );

            var userAccount = membershipService.CreateUser(Globals.Tenant, 
                TestFirstName, 
                TestLastName, 
                string.Empty, 
                TestPassword,
                TestEmail
                );

            var account = userAccountService.GetByEmail(Globals.Tenant, TestEmail);

            Assert.AreEqual(userAccount.Email, account.Email);

        }

        [TestMethod]
        [TestCategory(TestCategories.MembershipIntegration)]
        public void CreateANewWebUser()
        {
            var ctx = new TTSWebinarsContext(Constants.LocalDbConnectionStringName);
            var memRebootCtx = new DefaultMembershipRebootDatabase();
            var refDataRepository = new RefDataRepository();
            var config = MembershipRebootConfig.Create(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Constants.UpTwoFolders), new StateService(), refDataRepository);
            var userAccountService = new UserAccountService(config, new DefaultUserAccountRepository());
            
            IMembershipService membershipService = new MembershipService(new InstitutionRepository(ctx), 
                new RefDataRepository(), 
                new SamAuthenticationService(userAccountService),
                userAccountService,
                new WebUserRepository(ctx)
                );

            var webUser = membershipService.CreateWebUser(Globals.Tenant, 
                TestFirstName, 
                TestLastName, 
                TestPassword,
                TestEmail,
                USTimeZone.Central,
                UserType.Customer,
                26,
                GetAddresses(TestFirstName + " " + TestLastName),
                "Mr",
                null,
                null
                );

            var newWebUser = membershipService.GetUserByEmail(TestEmail);

            Assert.AreEqual(newWebUser.email, webUser.email);

        }

        private IList<Address> GetAddresses(string fullName)
        {
            var billingAddress = new Address
            {
               AddressType = "Billing",
               Name = fullName,
               City = "Dallas",
               Zip = "75201",
               State= "TX",
               StreetAddress = "1 Liberty St",
               Country = "USA"
            };
            
            var shippingAddress = new Address
            {
               AddressType = "Shipping",
               Name = fullName,
               City = "Dallas",
               Zip = "75201",
               State= "TX",
               StreetAddress = "1 Liberty St",
               Country = "USA"
            };

            return new[] {billingAddress, shippingAddress};
        }
    }
}
