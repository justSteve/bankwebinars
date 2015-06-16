using BrockAllen.MembershipReboot;
using System.Collections.Generic;

namespace CUWebinars.Web.Membership.Email
{
    public class TtsTokenizerInert : EmailMessageFormatter.Tokenizer
    {
        public override string Tokenize(
            UserAccountEvent<UserAccount> accountEvent,
            ApplicationInformation appInfo,
            string msg,
            IDictionary<string, string> values)
        {
            return string.Empty;
        }
    }
}