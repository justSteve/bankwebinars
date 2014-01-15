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
        public void LoginUserWithValidCredentials()
        {
            string email = "avalid@email.com";
            string password = "openSesame";

            membershipService = new MembershipService(
                institutionRepositoryMock.Object,
                refDataRepositoryMock.Object,
                samAuthenticationServiceMock,
                userAccountServiceFake,
                webUserRepositoryMock.Object
                );

            var result = membershipService.LogInUser(email, password, true);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void LoginUserWithInValidCredentials()
        {
            string email = "aninvalid@email.com";
            string password = "openSesame";

            membershipService = new MembershipService(
                institutionRepositoryMock.Object,
                refDataRepositoryMock.Object,
                samAuthenticationServiceMock,
                new UserAccountServiceUnHappyPathFake(new DefaultUserAccountRepository()),
                webUserRepositoryMock.Object
                );

            var result = membershipService.LogInUser(email, password, true);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetDetailsOfUserReturnsWebUser()
        {
            string email = "avalid@email.com";

            refDataRepositoryMock.Setup(r => r.GetWebUserByEmail(email)).Returns(new WebUser { email = email}).Verifiable();

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
    }
}
