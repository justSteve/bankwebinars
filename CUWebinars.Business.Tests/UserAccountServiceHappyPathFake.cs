using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.Relational;

namespace CUWebinars.Business.Tests
{
   public class UserAccountServiceHappyPathFake : UserAccountService
    {
       public UserAccountServiceHappyPathFake(IUserAccountRepository userAccountRepository)
           : base(userAccountRepository)
       {

       }
       public override bool AuthenticateWithEmail(string email, string password, out UserAccount account)
       {
           account = new RelationalUserAccount();

           return true;
       }
 
    }
}
