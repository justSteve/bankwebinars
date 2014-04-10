using System;
using System.IO;
using BrockAllen.MembershipReboot;
using CUWebinars.Business.AccountService;
using CUWebinars.Business.Repository;
using CUWebinars.Web.Services;

namespace CUWebinars.Web.Membership.Email
{
    public class TtsEmailFormatter : EmailMessageFormatter<UserAccount>
    {
        private readonly IStateService _stateService;
        private readonly IRefDataRepository _refDataRepository;
// ReSharper disable once InconsistentNaming
        private string pathToTemplates;

        public TtsEmailFormatter(ApplicationInformation appInfo, IStateService stateService, IRefDataRepository refDataRepository)
            : base(appInfo)
        {
            _stateService = stateService;
            _refDataRepository = refDataRepository;
        }

        public string PathToRoot 
        {
            set { pathToTemplates = Path.Combine(value, @"Membership\Email\EmailTemplates"); }
        }

        
        protected override string LoadBodyTemplate(UserAccountEvent<UserAccount> evt)
        {
            return LoadTemplate(string.Concat(CleanGenericName(evt.GetType()), "_Body.cshtml"));
        }

        protected override string LoadSubjectTemplate(UserAccountEvent<UserAccount> evt)
        {
            return LoadTemplate(string.Concat(CleanGenericName(evt.GetType()), "_Subject.cshtml"));
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
            name = Path.Combine(pathToTemplates, name);
                        
            using (var s = File.OpenRead(name))
            {
                if (s == null) throw new FileNotFoundException("Unable to find template.", name);

                using (var sr = new StreamReader(s))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        protected override Tokenizer GetTokenizer(UserAccountEvent<UserAccount> evt)
        {
            return new TtsTokenizer(_stateService, _refDataRepository);
        }
    }
}
