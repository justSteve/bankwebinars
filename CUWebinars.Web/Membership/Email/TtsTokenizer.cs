using BrockAllen.MembershipReboot;
using CUWebinars.Business.Constants;
using CUWebinars.Business.Core.Exceptions;
using CUWebinars.Business.Notification;
using CUWebinars.Web.Services;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CUWebinars.Web.Membership.Email
{
    public class TtsTokenizer : EmailMessageFormatter.Tokenizer
    {
        private readonly IStateService _stateService;

        public TtsTokenizer(IStateService stateService)
        {
            _stateService = stateService;
        }

        public override string Tokenize(
            UserAccountEvent<UserAccount> accountEvent, 
            ApplicationInformation appInfo,
            string msg,
            IDictionary<string, string> values)
        {
            if (accountEvent.GetType().Name.StartsWith("AccountCreatedEvent") &&
                _stateService.HasValue(DomainConstants.ResetPasswordRequested) &&
                _stateService.GetValue<bool>(DomainConstants.ResetPasswordRequested))
            {
                throw new UserCreatedMembershipException("User has requested a password but an AccountCreatedEvent was added to the event bus. This happens when the user is not  yet verified by MembershipReboot. Admin intervention required.");
            }

            //  create a configuration file for the template service
            var config = new FluentTemplateServiceConfiguration(c =>
            {
                c.WithEncoding(Encoding.Raw);
                c.ResolveUsing<TemplateResolver>();
                c.WithBaseTemplateType(typeof(TtsHtmlTemplateBase<>));
                c.IncludeNamespaces("CUWebinars.Business.Core.Helpers",
                    "CUWebinars.Business.Models",
                    "CUWebinars.Business.Notification");
            });

            var templateService = new TemplateService(config);
            var user = accountEvent.Account;

            var notification = new Notification
            {
                Tenant = appInfo.ApplicationName, // This is an MR class, so we are confined to its properties. We need to call it "Tenant".
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

                if (values.ContainsKey(DomainConstants.VerificationKey))
                {
                    var verificationKey = values[DomainConstants.VerificationKey];

                    //  We are adding this to session because it is used in the RegisterUser flow to automatically verify a new user
                    //  in the MembershipReboot sense. CUWebinars verifies it in its own way. This is a hack of MembershipReboot, 
                    //  but necessary to meet the app's requirements.
                    if (!_stateService.HasValue(DomainConstants.VerificationKey))
                        _stateService.SetValue(DomainConstants.VerificationKey, verificationKey);

                    notification.ConfirmPasswordResetUrl = Path.Combine(notification.ConfirmPasswordResetUrl, verificationKey);

                    //  "blank" is concatenated to the end so that it matches the route called EmailLinkRoute (See RouteConfig.cs). 
                    //  The "surname" part of that route is something other than "blank" when the user is registered by way of an 
                    //  Order being imported.
                    notification.ConfirmChangeEmailUrl = string.Concat(
                        Path.Combine(notification.ConfirmChangeEmailUrl, accountEvent.Account.Email),
                        Path.AltDirectorySeparatorChar + DomainConstants.Blank
                        );
                    notification.CancelVerificationUrl = Path.Combine(notification.CancelVerificationUrl, verificationKey);

                    //  We also add the ConfirmChangeEmailLink string to session where user is registered via an Order being imported.
                    //  This is used downstream in the flow with the creation of the Order.
                    if (_stateService.HasValue(DomainConstants.UserCreatedViaNewOrder) && !_stateService.HasValue(DomainConstants.ConfirmChangeEmailLink))
                    {
                        _stateService.SetValue(DomainConstants.ConfirmChangeEmailLink, notification.ConfirmChangeEmailUrl);
                    }
                    
                }

                foreach (var keyValuePair in values)
                {
                    var property = notificationType.GetProperty(keyValuePair.Key);

                    if (property != null)
                        property.SetValue(notification, keyValuePair.Value);
                }
            }

            var razorParsedResult = templateService.Parse(msg, notification, null, null);

            return razorParsedResult.Trim();
        }
    }
}