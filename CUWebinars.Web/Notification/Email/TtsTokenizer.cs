using System.Collections.Generic;
using System.IO;
using BrockAllen.MembershipReboot;
using RazorEngine.Templating;

namespace CUWebinars.Web.Notification.Email
{
    public class TtsTokenizer : EmailMessageFormatter.Tokenizer
    {
        public override string Tokenize(UserAccountEvent<UserAccount> accountEvent, ApplicationInformation appInfo, string msg, IDictionary<string, string> values)
        {
            var body = new TemplateService();
            var user = accountEvent.Account;

            var notification = new Notification
            {
                ApplicationName = appInfo.ApplicationName,
                CancelVerificationUrl = appInfo.CancelVerificationUrl,
                ConfirmChangeEmailUrl = appInfo.ConfirmChangeEmailUrl,
                ConfirmPasswordResetUrl = appInfo.ConfirmPasswordResetUrl,
                Email = user.Email,
                EmailSignature = appInfo.EmailSignature,
                LoginUrl = appInfo.LoginUrl,
                Username = user.Username
            };

            if (values.ContainsKey("VerificationKey"))
            {
                var verificationKey = values["VerificationKey"];
                notification.VerificationKey = verificationKey;
                notification.ConfirmPasswordResetUrl = Path.Combine(notification.ConfirmPasswordResetUrl,
                    verificationKey);
                notification.ConfirmChangeEmailUrl = Path.Combine(notification.ConfirmChangeEmailUrl, verificationKey);
                notification.CancelVerificationUrl = Path.Combine(notification.CancelVerificationUrl, verificationKey);
            }

            //foreach (var item in values)
            //{
            //    msg = msg.Replace("{" + item.Key + "}", item.Value);
            //}

            var b = body.Parse(msg, notification, null, null);

            return b;
        }
    }
}