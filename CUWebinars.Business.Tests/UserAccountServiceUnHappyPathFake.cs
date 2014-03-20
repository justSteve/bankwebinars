using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.Relational;

namespace CUWebinars.Business.Tests
{
    public class UserAccountServiceUnHappyPathFake : UserAccountService
    {
        public UserAccountServiceUnHappyPathFake(IUserAccountRepository userAccountRepository)
            : base(userAccountRepository)
        {

        }

        public override bool AuthenticateWithEmail(string tenant, string email, string password, out UserAccount account)
        {
            account = new RelationalUserAccount();

            return false;
        }
    }
}
