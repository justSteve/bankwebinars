using System;
using System.Collections.Generic;
using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.Ef;
using CUWebinars.Business.AccountService;
using CUWebinars.Business.Constants;
using CUWebinars.Business.Models;
using CUWebinars.Business.Repository;
using CUWebinars.Business.Tests.Fakes;
using CUWebinars.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Ninject.Extensions.Logging;
using Ninject.Extensions.Logging.Log4net.Infrastructure;

namespace CUWebinars.Business.Tests.UnitTests
{
    [TestClass]
    public class MembershipServiceTests
    {
        private ILogger logger;
        private Mock<IRefDataRepository> refDataRepositoryMock;
        private Mock<IInstitutionRepository> institutionRepositoryMock;
        private Mock<IWebUserRepository> webUserRepositoryMock;
        private UserAccountServiceHappyPathFake userAccountServiceFake;
        private SamAuthenticationServiceFake samAuthenticationServiceMock;
        private MembershipService membershipService;
        private TTSWebinarsContext ttsWebinarsContext;

        public MembershipServiceTests()
        {

        }

        private const string password = "OpenSesame";



        [TestInitialize]
        public void SetupTest()
        {
            logger = new Log4NetLogger(typeof(MembershipService));
            refDataRepositoryMock = new Mock<IRefDataRepository>();
            institutionRepositoryMock = new Mock<IInstitutionRepository>();
            webUserRepositoryMock = new Mock<IWebUserRepository>();
            ttsWebinarsContext = new TTSWebinarsContext();
            userAccountServiceFake = new UserAccountServiceHappyPathFake(new UserAccountRepositoryFake());
            samAuthenticationServiceMock = new SamAuthenticationServiceFake(userAccountServiceFake);

        }


        [TestMethod]
        [TestCategory(TestCategories.Membership)]
        public void LoginUserWithValidCredentials()
        {
            string email = "avalid@email.com";

            membershipService = new MembershipService(
                institutionRepositoryMock.Object,
                refDataRepositoryMock.Object,
                samAuthenticationServiceMock,
                userAccountServiceFake,
                webUserRepositoryMock.Object,
                logger
                );

            var result = membershipService.LogInUser(Globals.Tenant, email, password, true);

            Assert.IsTrue(result);
        }

        [TestMethod]
        [TestCategory(TestCategories.Membership)]
        public void LoginUserWithInValidCredentials()
        {
            string email = "aninvalid@email.com";

            membershipService = new MembershipService(
                institutionRepositoryMock.Object,
                refDataRepositoryMock.Object,
                samAuthenticationServiceMock,
                new UserAccountServiceUnHappyPathFake(new UserAccountRepositoryFake()),
                webUserRepositoryMock.Object,
                logger
                );

            var result = membershipService.LogInUser(Globals.Tenant, email, password, true);

            Assert.IsFalse(result);
        }

        [TestMethod]
        [TestCategory(TestCategories.Membership)]
        public void GetDetailsOfUserReturnsWebUser()
        {
            string email = "avalid@email.com";

            refDataRepositoryMock.Setup(r => r.GetWebUserByEmail(email))
                .Returns(new WebUser {email = email})
                .Verifiable();

            membershipService = new MembershipService(
                institutionRepositoryMock.Object,
                refDataRepositoryMock.Object,
                samAuthenticationServiceMock,
                new UserAccountServiceHappyPathFake(new UserAccountRepositoryFake()),
                webUserRepositoryMock.Object,
                logger
                );

            var result = membershipService.GetDetailsOfUser(email);

            refDataRepositoryMock.Verify(r => r.GetWebUserByEmail(email), Times.Exactly(1));
            Assert.AreEqual(result.email, email);
            Assert.IsInstanceOfType(result, typeof (WebUser));
        }

        [TestMethod]
        [TestCategory(TestCategories.Membership)]
        public void GetDetailsOfUserReturnsNullWhereEmailIsWrong()
        {
            string email = "aninvalid@email.com";

            refDataRepositoryMock.Setup(r => r.GetWebUserByEmail(email)).Returns(() => null);

            membershipService = new MembershipService(
                institutionRepositoryMock.Object,
                refDataRepositoryMock.Object,
                samAuthenticationServiceMock,
                new UserAccountServiceHappyPathFake(new UserAccountRepositoryFake()),
                webUserRepositoryMock.Object,
                logger
                );

            var result = membershipService.GetDetailsOfUser(email);

            refDataRepositoryMock.Verify(r => r.GetWebUserByEmail(email), Times.Exactly(1));
            Assert.IsNull(result);
        }

        [TestMethod]
        [TestCategory(TestCategories.Membership)]
        public void GetDetailsOfUserReturnsThrowsNullArgumentExceptionWhereEmailIsNull()
        {
            string email = null;

            refDataRepositoryMock.Setup(r => r.GetWebUserByEmail(email)).Returns(() => null);

            membershipService = new MembershipService(
                institutionRepositoryMock.Object,
                refDataRepositoryMock.Object,
                samAuthenticationServiceMock,
                new UserAccountServiceHappyPathFake(new UserAccountRepositoryFake()),
                webUserRepositoryMock.Object,
                logger
                );
            
            ExceptionAssert.Throws<ArgumentNullException>(() => membershipService.GetDetailsOfUser(email));
        }

        [TestMethod]
        [TestCategory(TestCategories.Membership)]
        public void GetDetailsOfUserReturnsNullWhereEmailIsAnEmptyString()
        {
            string email = string.Empty;

            refDataRepositoryMock.Setup(r => r.GetWebUserByEmail(email)).Returns(() => null);

            membershipService = new MembershipService(
                institutionRepositoryMock.Object,
                refDataRepositoryMock.Object,
                samAuthenticationServiceMock,
                new UserAccountServiceHappyPathFake(new UserAccountRepositoryFake()),
                webUserRepositoryMock.Object,
                logger
                );

            var result = membershipService.GetDetailsOfUser(email);

            refDataRepositoryMock.Verify(r => r.GetWebUserByEmail(email), Times.Exactly(1));
            Assert.IsNull(result);
        }

        [TestMethod]
        [TestCategory(TestCategories.Membership)]
        public void CreateUserAddsClaims()
        {
            string email = string.Empty;
            UserAccountServiceHappyPathFake userAccountService =
                userAccountServiceFake = new UserAccountServiceHappyPathFake(new UserAccountRepositoryFake());

            membershipService = new MembershipService(
                institutionRepositoryMock.Object,
                refDataRepositoryMock.Object,
                samAuthenticationServiceMock,
                userAccountService,
                webUserRepositoryMock.Object,
                logger
                );

            var result = membershipService.CreateUser(Globals.Tenant, "Bob", "Smith", "BobSmith", password, email);


            Assert.IsTrue(userAccountService.CreateAccountCalled);
            Assert.AreEqual(2, userAccountService.ClaimsAdded);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        [TestCategory(TestCategories.Membership)]
        public void ProcessInstitutionForUserFindsAndReturnsInstitution()
        {
            //  Arrange
            string zipCode = "65748";
            string email = "renoraines@gofarmersbank.com";
            const string _institutionName = "Farmers Bank";

            institutionRepositoryMock
                .Setup(i => i.GetAll())
                .Returns(new List<Institution> {new Institution {Zip = zipCode, InstitutionName = _institutionName}});

            membershipService = new MembershipService(
                institutionRepositoryMock.Object,
                refDataRepositoryMock.Object,
                samAuthenticationServiceMock,
                userAccountServiceFake,
                webUserRepositoryMock.Object,
                logger
                );

            //  Act
            var institution = membershipService.ProcessInstitutionForUser(
                _institutionName,
                email,
                "Greenwood",
                "AR",
                "1296",
                "FDIC",
                zipCode
                );

            //  Assert
            Assert.IsNotNull(institution);
            Assert.AreEqual(zipCode, institution.Zip);
            Assert.AreEqual(_institutionName, institution.InstitutionName);
        }

        [TestMethod]
        [TestCategory(TestCategories.Membership)]
        public void ProcessInstitutionForUserDoesNotFindAndCreatesInstitution()
        {
            //  Arrange
            string zipCode = "65748";
            string email = "renoraines@gofarmersbank.com";
            const string _institutionName = "Farmers Bank";

            institutionRepositoryMock
                .Setup(i => i.GetAll())
                .Returns(new List<Institution> {new Institution {Zip = "11111", InstitutionName = _institutionName}});

            membershipService = new MembershipService(
                institutionRepositoryMock.Object,
                refDataRepositoryMock.Object,
                samAuthenticationServiceMock,
                userAccountServiceFake,
                webUserRepositoryMock.Object,
                logger
                );

            //  Act
            var institution = membershipService.ProcessInstitutionForUser(
                _institutionName,
                email,
                "Greenwood",
                "AR",
                "1296",
                "FDIC",
                zipCode
                );

            //  Assert
            Assert.IsNotNull(institution);
            Assert.AreEqual(zipCode, institution.Zip);
            Assert.AreEqual(_institutionName, institution.InstitutionName);
            Assert.AreEqual(institution.domainName, "gofarmersbank.com");
        }

        [TestMethod]
        [TestCategory(TestCategories.Membership)]
        public void
            CreateWebUserCreatesWebUserWithIdOneBiggerThanExistingBiggestIdWhenZeroPassedForidUserImportedParameter()
        {
            //  Arrange
            string email = "renoraines@gofarmersbank.com";
            const int maxUser = 1000;
            refDataRepositoryMock.Setup(r => r.GetMaxWebUserId()).Returns(maxUser);

            //  Act
            membershipService = new MembershipService(
                institutionRepositoryMock.Object,
                refDataRepositoryMock.Object,
                samAuthenticationServiceMock,
                userAccountServiceFake,
                webUserRepositoryMock.Object,
                logger
                );

            var newGuy = membershipService.CreateWebUser(Globals.Tenant,
                "John",
                "Doh",
                password,
                email,
                USTimeZone.Central,
                UserType.Customer,
                20,
                null,
                "Dr",
                0,
                null);

            //  Assert                   
            Assert.AreEqual(maxUser + 1, newGuy.idUser);
        }

        [TestMethod]
        [TestCategory(TestCategories.Membership)]
        public void
            CreateWebUserCreatesWebUserWithIdOneBiggerThanExistingBiggestIdWhenNullPassedForidUserImportedParameter()
        {
            //  Arrange
            string email = "renoraines@gofarmersbank.com";
            const int maxUser = 1000;
            refDataRepositoryMock.Setup(r => r.GetMaxWebUserId()).Returns(maxUser);

            //  Act
            membershipService = new MembershipService(
                institutionRepositoryMock.Object,
                refDataRepositoryMock.Object,
                samAuthenticationServiceMock,
                userAccountServiceFake,
                webUserRepositoryMock.Object,
                logger
                );

            var newGuy = membershipService.CreateWebUser(Globals.Tenant,
                "John",
                "Doh",
                password,
                email,
                USTimeZone.Central,
                UserType.Customer,
                20,
                null,
                "Dr",
                null,
                null);

            //  Assert                   
            Assert.AreEqual(maxUser + 1, newGuy.idUser);
        }

        [TestMethod]
        [TestCategory(TestCategories.Membership)]
        public void AddRegistrationTypeNotVerifiedClaimAddsClaim()
        {
            var userAccountService =
                userAccountServiceFake = new UserAccountServiceHappyPathFake(new UserAccountRepositoryFake());

            membershipService = new MembershipService(
                institutionRepositoryMock.Object,
                refDataRepositoryMock.Object,
                samAuthenticationServiceMock,
                userAccountService,
                webUserRepositoryMock.Object,
                logger
                );

            membershipService.AddAccountTypeNotVerifiedClaim(
                GetExampleUserAccount(),
                ClaimValues.ManualRegistration
                );

            Assert.AreEqual(3, userAccountService.ClaimsAdded);
        }

        [TestMethod]
        [TestCategory(TestCategories.Membership)]
        [ExpectedException(typeof(ArgumentException), "String parameter cannot be white space or null.")]
        public void AddRegistrationTypeNotVerifiedClaimThrowsExceptionWhere2ndParamIsNull()
        {
            var userAccountService =
                userAccountServiceFake = new UserAccountServiceHappyPathFake(new UserAccountRepositoryFake());

            membershipService = new MembershipService(
                institutionRepositoryMock.Object,
                refDataRepositoryMock.Object,
                samAuthenticationServiceMock,
                userAccountService,
                webUserRepositoryMock.Object,
                logger
                );

            membershipService.AddAccountTypeNotVerifiedClaim(
                GetExampleUserAccount(),
                null
                );

            Assert.Fail();
        }
        
        [TestMethod]
        [TestCategory(TestCategories.Membership)]
        [ExpectedException(typeof(ArgumentException), "String parameter cannot be white space or null.")]
        public void AddRegistrationTypeNotVerifiedClaimThrowsExceptionWhere2ndParamIsWhiteSpace()
        {
            var userAccountService =
                userAccountServiceFake = new UserAccountServiceHappyPathFake(new UserAccountRepositoryFake());

            membershipService = new MembershipService(
                institutionRepositoryMock.Object,
                refDataRepositoryMock.Object,
                samAuthenticationServiceMock,
                userAccountService,
                webUserRepositoryMock.Object,
                logger
                );

            membershipService.AddAccountTypeNotVerifiedClaim(
                GetExampleUserAccount(),
                string.Empty
                );

            Assert.Fail();
        }
        
        [TestMethod]
        [TestCategory(TestCategories.Membership)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddRegistrationTypeNotVerifiedClaimThrowsExceptionWhere1stParamIsNull()
        {
            var userAccountService =
                userAccountServiceFake = new UserAccountServiceHappyPathFake(new UserAccountRepositoryFake());

            membershipService = new MembershipService(
                institutionRepositoryMock.Object,
                refDataRepositoryMock.Object,
                samAuthenticationServiceMock,
                userAccountService,
                webUserRepositoryMock.Object,
                logger
                );

            membershipService.AddAccountTypeNotVerifiedClaim(
                null,
                ClaimValues.ManualRegistration
                );

            Assert.Fail();
        }

        private UserAccount GetExampleUserAccount()
        {
            string email = "renoraines@gofarmersbank.com";

            return membershipService.CreateUser(Globals.Tenant,
                "John",
                "Doh",
                string.Empty,
                password,
                email);

        }

    }
}
