using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.Ef;
using CUWebinars.Business.AccountService;
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
        UserAccountServiceFake userAccountServiceFake;
        SamAuthenticationServiceFake samAuthenticationServiceMock;
        MembershipService membershipService;

        [TestInitialize]
        public void SetupTest()
        {
            refDataRepositoryMock = new Mock<IRefDataRepository>();
            institutionRepositoryMock = new Mock<IInstitutionRepository>();
            webUserRepositoryMock = new Mock<IWebUserRepository>();
            userAccountServiceFake = new UserAccountServiceFake(new DefaultUserAccountRepository());
            samAuthenticationServiceMock = new SamAuthenticationServiceFake(userAccountServiceFake);

            membershipService = new MembershipService(
                institutionRepositoryMock.Object,
                refDataRepositoryMock.Object,
                samAuthenticationServiceMock,
                userAccountServiceFake,
                webUserRepositoryMock.Object
                );
        }


        [TestMethod]
        public void LoginUserWithValidCredentials()
        {
            string email = "avalid@email.com";
            string password = "openSesame";
            
            var result = membershipService.LogInUser(email, password, true);

            Assert.IsTrue(result);
        }
    }
}
