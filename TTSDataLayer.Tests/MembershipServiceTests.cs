using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TTSDataLayer.Account;
using TTSDataLayer.Repository;
using Moq;
using TTSDataLayer.AccountService;
using TTSDataLayer.WebHost;
//using TTSDataLayer.Ef;

namespace TTSDataLayer.Tests
{
    [TestClass]
    public class MembershipServiceTests
    {
        Mock<IRefDataRepository> refDataRepositoryMock;
        Mock<IInstitutionRepository> institutionRepositoryMock;
        Mock<IWebUserRepository> webUserRepositoryMock;
        Mock<IUserAccountService> userAccountServiceMock;
        Mock<UserAccountService> userAccountServiceMock2;
        SamAuthenticationServiceFake samAuthenticationServiceMock;
        MembershipService membershipService;

        [TestInitialize]
        public void SetupTest()
        {
            refDataRepositoryMock = new Mock<IRefDataRepository>();
            institutionRepositoryMock = new Mock<IInstitutionRepository>();
            webUserRepositoryMock = new Mock<IWebUserRepository>();
            userAccountServiceMock = new Mock<IUserAccountService>();
            userAccountServiceMock2 = new Mock<UserAccountService>();
            samAuthenticationServiceMock = new SamAuthenticationServiceFake(
                new UserAccountService(new DefaultUserAccountRepository())
                );

            membershipService = new MembershipService(
                institutionRepositoryMock.Object,
                refDataRepositoryMock.Object,
                samAuthenticationServiceMock,
                userAccountServiceMock.Object,
                webUserRepositoryMock.Object                                
                );
        }


        [TestMethod]
        public void LoginUserWithValidCredentials()
        {
            string email = "avalid@email.com";
            string password = "openSesame";

            UserAccount userAccount = new UserAccount();

            userAccountServiceMock.Setup(u => u.AuthenticateWithEmail(email, password)).Returns(true);
            userAccountServiceMock.Setup(u => u.GetByEmail(email)).Returns(userAccount);

            var result = membershipService.LogInUser(email, password);

            Assert.IsTrue(result);
        }
    }
}
