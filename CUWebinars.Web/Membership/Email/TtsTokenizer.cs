using System.Collections.Generic;
using System.IO;
using System.Linq;
using BrockAllen.MembershipReboot;
using RazorEngine.Templating;

namespace CUWebinars.Web.Membership.Email
{
    public class TtsTokenizer : EmailMessageFormatter.Tokenizer
    {

        const string VerificationKey = "VerificationKey";
        public override string Tokenize(UserAccountEvent<UserAccount> accountEvent, ApplicationInformation appInfo, string msg, IDictionary<string, string> values)
        {
            var body = new TemplateService();
            var user = accountEvent.Account;
            
            var notification = new Membership.Notification
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

            if (values.Any())
            {
                var notificationType = notification.GetType();

                if (values.ContainsKey(VerificationKey))
                {
                    var verificationKey = values[VerificationKey];
                    notification.ConfirmPasswordResetUrl = Path.Combine(notification.ConfirmPasswordResetUrl,
                        verificationKey);
                    notification.ConfirmChangeEmailUrl = Path.Combine(notification.ConfirmChangeEmailUrl,
                        verificationKey);
                    notification.CancelVerificationUrl = Path.Combine(notification.CancelVerificationUrl,
                        verificationKey);
                }

                foreach (var keyValuePair in values)
                {
                    var property = notificationType.GetProperty(keyValuePair.Key);

                    if (property != null)
                        property.SetValue(notification, keyValuePair.Value);
                }
            }

            var b = body.Parse(msg, notification, null, null);

            return b;
        }
    }
}