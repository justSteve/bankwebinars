using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.Ef;
using BrockAllen.MembershipReboot.Relational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUWebinars.Business.Tests
{
   public class UserAccountServiceFake : UserAccountService
    {
       public UserAccountServiceFake(IUserAccountRepository userAccountRepository)
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
