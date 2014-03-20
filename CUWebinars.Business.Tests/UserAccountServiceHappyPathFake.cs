using System;
using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.Relational;

namespace CUWebinars.Business.Tests
{
   public class UserAccountServiceHappyPathFake : UserAccountService
    {
       public bool CreateAccountCalled { get; set; }
       public int ClaimsAdded { get; set; }


       public UserAccountServiceHappyPathFake(IUserAccountRepository userAccountRepository)
           : base(userAccountRepository)
       {

       }
       public override bool AuthenticateWithEmail(string tenant, string email, string password, out UserAccount account)
       {
           account = new RelationalUserAccount();

           return true;
       }

       public override UserAccount CreateAccount(string tenant, string username, string password, string email)
       {
           CreateAccountCalled = true;           
           return new RelationalUserAccount();
       }

       public override void AddClaim(Guid accountID, string type, string value)
       {
           ClaimsAdded += 1;
       }
    }
}
