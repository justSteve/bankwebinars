using System.IO;
using System.Linq;
using BrockAllen.MembershipReboot;
using System;
using System.Collections.Generic;
using CUWebinars.Business.Constants;
using CUWebinars.Web.Services;

namespace CUWebinars.Web.Membership.Email
{
    public class TtsEmailFormatterInert: EmailMessageFormatter<UserAccount>
    {
        private readonly IStateService _stateService;

        public TtsEmailFormatterInert(ApplicationInformation appInfo, IStateService stateService)
            : base(appInfo)
        {
            _stateService = stateService;
        }

        protected override string LoadBodyTemplate(UserAccountEvent<UserAccount> evt)
        {
            return LoadTemplate(CleanGenericName(evt.GetType()) + "_Body");
        }

        private string CleanGenericName(Type type)
        {
            var name = type.Name;
            var idx = name.IndexOf('`');
            if (idx > 0)
            {
                name = name.Substring(0, idx);
            }
            return name;
        }

        private string LoadTemplate(string name)
        {
            name = "CUWebinars.Web.Membership.Email.EmailTemplates." + name + DomainConstants.RazorExtension;

            var assembly = typeof(TtsEmailFormatter).Assembly;

            using (var s = assembly.GetManifestResourceStream(name))
            {
                if (s == null) throw new FileNotFoundException("Unable to find template.", name);

                using (var sr = new StreamReader(s)) // boom CUWebinars.Web.Membership.Email.EmailTemplates.PasswordResetRequestedEvent_Body
                {
                    return sr.ReadToEnd();
                }
            }
        }

        protected override string LoadSubjectTemplate(UserAccountEvent<UserAccount> evt)
        {
            return string.Empty;
        }

        protected override string GetSubject(UserAccountEvent<UserAccount> evt, IDictionary<string, string> values)
        {
            if (values.Any())
            {
                if (values.ContainsKey(DomainConstants.VerificationKey))
                {
                    var verificationKey = values[DomainConstants.VerificationKey];

                    //  We are adding this to session because it is used in the RegisterUser flow to automatically verify a new user
                    //  in the MembershipReboot sense. CUWebinars verifies it in its own way. This is a hack of MembershipReboot, 
                    //  but necessary to meet the app's requirements.
                    if (!_stateService.HasValue(DomainConstants.VerificationKeyForBatchChangePwd))
                        _stateService.SetValue(DomainConstants.VerificationKeyForBatchChangePwd, verificationKey);
                }
            }

            return string.Empty;
        }
        protected override string GetBody(UserAccountEvent<UserAccount> evt, IDictionary<string, string> values)
        {
            if (values.Any())
            {
                if (values.ContainsKey(DomainConstants.VerificationKey))
                {
                    var verificationKey = values[DomainConstants.VerificationKey];

                    //  We are adding this to session because it is used in the RegisterUser flow to automatically verify a new user
                    //  in the MembershipReboot sense. CUWebinars verifies it in its own way. This is a hack of MembershipReboot, 
                    //  but necessary to meet the app's requirements.
                    if (!_stateService.HasValue(DomainConstants.VerificationKeyForBatchChangePwd))
                        _stateService.SetValue(DomainConstants.VerificationKeyForBatchChangePwd, verificationKey);
                }
            }

            return string.Empty;
        }

        protected override Tokenizer GetTokenizer(UserAccountEvent<UserAccount> evt)
        {
            return new TtsTokenizerInert();
        }

    }
}