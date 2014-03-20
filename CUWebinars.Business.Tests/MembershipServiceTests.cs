using System.Collections.Generic;
using System.Linq;
using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.Ef;
using CUWebinars.Business.AccountService;
using CUWebinars.Business.Models;
using CUWebinars.Business.Repository;
using CUWebinars.Web.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CUWebinars.Business.Tests
{
    [TestClass]
    public class MembershipServiceTests
    {
        Mock<IRefDataRepository> refDataRepositoryMock;
        Mock<IInstitutionRepository> institutionRepositoryMock;
        Mock<IWebUserRepository> webUserRepositoryMock;
        UserAccountServiceHappyPathFake userAccountServiceFake;
        SamAuthenticationServiceFake samAuthenticationServiceMock;
        MembershipService membershipService;

        public MembershipServiceTests()
        {
        
        }

        const string password = "OpenSesame";
        


        [TestInitialize]
        public void SetupTest()
        {
            refDataRepositoryMock = new Mock<IRefDataRepository>();
            institutionRepositoryMock = new Mock<IInstitutionRepository>();
            webUserRepositoryMock = new Mock<IWebUserRepository>();
            userAccountServiceFake = new UserAccountServiceHappyPathFake(new DefaultUserAccountRepository());
            samAuthenticationServiceMock = new SamAuthenticationServiceFake(userAccountServiceFake);

        }


        [TestMethod]
        [TestCategory("Membership Tests")]
        public void LoginUserWithValidCredentials()
        {
            string email = "avalid@email.com";

            membershipService = new MembershipService(
                institutionRepositoryMock.Object,
                refDataRepositoryMock.Object,
                samAuthenticationServiceMock,
                userAccountServiceFake,
                webUserRepositoryMock.Object
                );

            var result = membershipService.LogInUser(Globals.AppTenant, email, password, true);

            Assert.IsTrue(result);
        }

        [TestMethod]
        [TestCategory("Membership Tests")]
        public void LoginUserWithInValidCredentials()
        {
            string email = "aninvalid@email.com";

            membershipService = new MembershipService(
                institutionRepositoryMock.Object,
                refDataRepositoryMock.Object,
                samAuthenticationServiceMock,
                new UserAccountServiceUnHappyPathFake(new DefaultUserAccountRepository()),
                webUserRepositoryMock.Object
                );

            var result = membershipService.LogInUser(Globals.AppTenant, email, password, true);

            Assert.IsFalse(result);
        }

        [TestMethod]
        [TestCategory("Membership Tests")]
        public void GetDetailsOfUserReturnsWebUser()
        {
            string email = "avalid@email.com";

            refDataRepositoryMock.Setup(r => r.GetWebUserByEmail(email)).Returns(new WebUser { email = email }).Verifiable();

            membershipService = new MembershipService(
                institutionRepositoryMock.Object,
                refDataRepositoryMock.Object,
                samAuthenticationServiceMock,
                new UserAccountServiceHappyPathFake(new DefaultUserAccountRepository()),
                webUserRepositoryMock.Object
                );

            var result = membershipService.GetDetailsOfUser(email);

            refDataRepositoryMock.Verify(r => r.GetWebUserByEmail(email), Times.Exactly(1));
            Assert.AreEqual(result.email, email);
            Assert.IsInstanceOfType(result, typeof(WebUser));
        }

        [TestMethod]
        [TestCategory("Membership Tests")]
        public void GetDetailsOfUserReturnsNullWhereEmailIsWrong()
        {
            string email = "aninvalid@email.com";

            refDataRepositoryMock.Setup(r => r.GetWebUserByEmail(email)).Returns(() => null);

            membershipService = new MembershipService(
                institutionRepositoryMock.Object,
                refDataRepositoryMock.Object,
                samAuthenticationServiceMock,
                new UserAccountServiceHappyPathFake(new DefaultUserAccountRepository()),
                webUserRepositoryMock.Object
                );

            var result = membershipService.GetDetailsOfUser(email);

            refDataRepositoryMock.Verify(r => r.GetWebUserByEmail(email), Times.Exactly(1));
            Assert.IsNull(result);
        }

        [TestMethod]
        [TestCategory("Membership Tests")]
        public void GetDetailsOfUserReturnsNullWhereEmailIsNull()
        {
            string email = null;

            refDataRepositoryMock.Setup(r => r.GetWebUserByEmail(email)).Returns(() => null);

            membershipService = new MembershipService(
                institutionRepositoryMock.Object,
                refDataRepositoryMock.Object,
                samAuthenticationServiceMock,
                new UserAccountServiceHappyPathFake(new DefaultUserAccountRepository()),
                webUserRepositoryMock.Object
                );

            var result = membershipService.GetDetailsOfUser(email);

            refDataRepositoryMock.Verify(r => r.GetWebUserByEmail(email), Times.Exactly(1));
            Assert.IsNull(result);
        }

        [TestMethod]
        [TestCategory("Membership Tests")]
        public void GetDetailsOfUserReturnsNullWhereEmailIsAnEmptyString()
        {
            string email = string.Empty;

            refDataRepositoryMock.Setup(r => r.GetWebUserByEmail(email)).Returns(() => null);

            membershipService = new MembershipService(
                institutionRepositoryMock.Object,
                refDataRepositoryMock.Object,
                samAuthenticationServiceMock,
                new UserAccountServiceHappyPathFake(new DefaultUserAccountRepository()),
                webUserRepositoryMock.Object
                );

            var result = membershipService.GetDetailsOfUser(email);

            refDataRepositoryMock.Verify(r => r.GetWebUserByEmail(email), Times.Exactly(1));
            Assert.IsNull(result);
        }

        [TestMethod]
        [TestCategory("Membership Tests")]
        public void CreateUserAddsClaims()
        {
            string email = string.Empty;
            UserAccountServiceHappyPathFake userAccountService =
                new UserAccountServiceHappyPathFake(new DefaultUserAccountRepository());

            membershipService = new MembershipService(
                institutionRepositoryMock.Object,
                refDataRepositoryMock.Object,
                samAuthenticationServiceMock,
                userAccountService,
                webUserRepositoryMock.Object
                );

            var result = membershipService.CreateUser(Globals.AppTenant, "Bob", "Smith", "BobSmith", password, email);


            Assert.IsTrue(userAccountService.CreateAccountCalled);
            Assert.AreEqual(3, userAccountService.ClaimsAdded);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        [TestCategory("Membership Tests")]
        public void ProcessInstitutionForUserFindsAndReturnsInstitution()
        {
            //  Arrange
            string zipCode = "65748";
            string email = "renoraines@gofarmersbank.com";
            const string _institutionName = "Farmers Bank";

            institutionRepositoryMock
                .Setup(i => i.GetAll())
                .Returns(new List<Institution> { new Institution { Zip = zipCode, InstitutionName = _institutionName } });
            
            membershipService = new MembershipService(
                institutionRepositoryMock.Object,
                refDataRepositoryMock.Object,
                samAuthenticationServiceMock,
                userAccountServiceFake,
                webUserRepositoryMock.Object
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
        [TestCategory("Membership Tests")]
        public void ProcessInstitutionForUserDoesNotFindAndCreatesInstitution()
        {
            //  Arrange
            string zipCode = "65748";
            string email = "renoraines@gofarmersbank.com";
            const string _institutionName = "Farmers Bank";

            institutionRepositoryMock
                .Setup(i => i.GetAll())
                .Returns(new List<Institution> { new Institution { Zip = "11111", InstitutionName = _institutionName } });

            membershipService = new MembershipService(
                institutionRepositoryMock.Object,
                refDataRepositoryMock.Object,
                samAuthenticationServiceMock,
                userAccountServiceFake,
                webUserRepositoryMock.Object
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
        [TestCategory("Membership Tests")]
        public void testtemplate()
        {
            //  Arrange

            //  Act

            //  Assert                        
		
        }

    }
}
