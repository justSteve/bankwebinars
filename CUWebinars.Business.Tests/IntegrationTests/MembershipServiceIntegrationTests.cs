using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.Ef;
using BrockAllen.MembershipReboot.WebHost;
using CUWebinars.Business.AccountService;
using CUWebinars.Business.Models;
using CUWebinars.Business.Repository;
using CUWebinars.Business.Tests.Config;
using CUWebinars.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject.Extensions.Logging.Log4net.Infrastructure;

namespace CUWebinars.Business.Tests.IntegrationTests
{
    [TestClass]
    public class MembershipServiceIntegrationTests
    {

        private const string TestEmail = "mr_white@saymyname.com";
        private const string TestFirstName = "Walter";
        private const string TestLastName = "White";
        private const string TestPassword = "ghjG7J*";

        [TestInitialize]
        public void Setup()
        {
        }



        [TestMethod]
        [TestCategory(TestCategories.MembershipIntegration)]
        public void CreateANewUserAccount()
        {
            var databaseResources = new DatabaseResources();
            databaseResources.PrimeMembershipTestsDatabases();
            databaseResources.CreateMembershipRebootDb();

            var ctx = new TTSWebinarsContext(Constants.LocalDbConnectionStringName);
            var memRebootCtx = new DefaultMembershipRebootDatabase(Constants.MembershipRebootConnectionStringName);
            var refDataRepository = new RefDataRepository();
            var config = MembershipRebootConfig.Create(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, TestConstants.UpTwoFolders), new StateService(), refDataRepository);
            var userAccountService = new UserAccountService(config, new DefaultUserAccountRepository());
            var logger = new Log4NetLogger(typeof(MembershipService));

            IMembershipService membershipService = new MembershipService(new InstitutionRepository(ctx), 
                new RefDataRepository(), 
                new SamAuthenticationService(userAccountService),
                userAccountService,
                new WebUserRepository(ctx),
                logger
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

            databaseResources.TearDownMembershipRebootDatabase();
        }

        [TestMethod]
        [TestCategory(TestCategories.MembershipIntegration)]
        public void CreateANewWebUser()
        {
            //  Arrange
            var databaseResources = new DatabaseResources();
            databaseResources.PrimeMembershipTestsDatabases();
            databaseResources.CreateCuWebinarsDb();

            //  Act
            var membershipService = CreateMembershipService();

            var webUser = membershipService.CreateWebUser(Globals.Tenant, 
                TestFirstName, 
                TestLastName, 
                TestPassword,
                TestEmail,
                USTimeZone.Central,
                UserType.Customer,
                26,
                //BusinessTestHelper.GetAddresses(TestFirstName + " " + TestLastName),
                null,
                "Mr",
                null,
                null
                );

            var newWebUser = membershipService.GetUserByEmail(TestEmail);

            //  Assert       
            Assert.AreEqual(newWebUser.email, webUser.email);

            databaseResources.TearDownCuWebinarsDatabase();

        }


        [TestMethod]
        [TestCategory(TestCategories.MembershipIntegration)]
        public void CreateUserAccountWithNoTenantThrowsException()
        {
            var membershipService = CreateMembershipService();

            ExceptionAssert.Throws<ArgumentNullException>(
                () =>
                    membershipService.CreateUser(null, TestFirstName, TestLastName, TestFirstName + " " + TestLastName,
                        TestPassword, TestEmail));

        }

        [TestMethod]
        [TestCategory(TestCategories.MembershipIntegration)]
        public void CreateUserAccountWithEmptyStringForTenantThrowsException()
        {
            var membershipService = CreateMembershipService();

            ExceptionAssert.Throws<ArgumentNullException>(() =>
                membershipService.CreateUser(string.Empty, TestFirstName, TestLastName,
                    TestFirstName + " " + TestLastName,
                    TestPassword, TestEmail));

        }

        [TestMethod]
        [TestCategory(TestCategories.MembershipIntegration)]
        public void CreateUserAccountWithEmptyStringForUsernameDoesNotThrowException()
        {
            var databaseResources = new DatabaseResources();
            databaseResources.PrimeMembershipTestsDatabases();
            databaseResources.CreateMembershipRebootDb();

            var membershipService = CreateMembershipService();

            //  Because we have set in App.config emailIsUsername to true, a null or whitespace username
            //  is fine and gets stored as the email address of the user.
            membershipService.CreateUser(Globals.Tenant, TestFirstName, TestLastName, " ",
                TestPassword, TestEmail);

            var context = new DefaultMembershipRebootDatabase();
            var account = context.Users.Where(u => u.Email == TestEmail).SingleOrDefault();

            Assert.IsNotNull(account); 

            databaseResources.TearDownMembershipRebootDatabase();
        }

        [TestMethod]
        [TestCategory(TestCategories.MembershipIntegration)]
        public void CreateUserAccountWithNullStringForUsernameDoesNotThrowException()
        {
            var databaseResources = new DatabaseResources();
            databaseResources.PrimeMembershipTestsDatabases();
            databaseResources.CreateMembershipRebootDb();

            var membershipService = CreateMembershipService();

            //  Because we have set in App.config emailIsUsername to true, a null or whitespace username
            //  is fine and gets stored as the email address of the user.
            membershipService.CreateUser(Globals.Tenant, TestFirstName, TestLastName, null,
                TestPassword, TestEmail);

            var context = new DefaultMembershipRebootDatabase();
            var account = context.Users.Where(u => u.Email == TestEmail).SingleOrDefault();

            Assert.IsNotNull(account);

            databaseResources.TearDownMembershipRebootDatabase();
        }


        private static IMembershipService CreateMembershipService()
        {
            var ctx = new TTSWebinarsContext(Constants.LocalDbConnectionStringName);
            var memRebootCtx = new DefaultMembershipRebootDatabase(Constants.MembershipRebootConnectionStringName);
            var refDataRepository = new RefDataRepository();
            var config =
                MembershipRebootConfig.Create(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, TestConstants.UpTwoFolders),
                    new StateService(), refDataRepository);
            var userAccountService = new UserAccountService(config, new DefaultUserAccountRepository());
            var logger = new Log4NetLogger(typeof(MembershipService));

            IMembershipService membershipService = new MembershipService(new InstitutionRepository(ctx),
                new RefDataRepository(),
                new SamAuthenticationService(userAccountService),
                userAccountService,
                new WebUserRepository(ctx),
                logger
                );
            return membershipService;
        }
    }
}
