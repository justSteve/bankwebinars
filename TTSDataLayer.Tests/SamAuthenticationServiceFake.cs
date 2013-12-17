
using TTSDataLayer.Account;
using TTSDataLayer.AccountService;
using TTSDataLayer.Authentication;

namespace TTSDataLayer.Tests
{
    public class SamAuthenticationServiceFake : AuthenticationService
    {
        public SamAuthenticationServiceFake(UserAccountService userService) : base(userService)
        {

        }

        public override void SignIn(UserAccount account)
        {
            //  do nothing.
        }

        protected override void IssueToken(System.Security.Claims.ClaimsPrincipal principal, System.TimeSpan? tokenLifetime = null, bool? persistentCookie = null)
        {
            throw new System.NotImplementedException();
        }

        protected override void RevokeToken()
        {
            throw new System.NotImplementedException();
        }
    }
}
