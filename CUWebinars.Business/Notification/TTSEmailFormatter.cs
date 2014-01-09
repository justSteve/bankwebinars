using BrockAllen.MembershipReboot;
using System;
using System.IO;

namespace CUWebinars.Business.Notification
{
    public class TTSEmailFormatter : EmailMessageFormatter<UserAccount>
    {
        private string pathToTemplates;
        

        public TTSEmailFormatter(ApplicationInformation appInfo)
            : base(appInfo)
        {

        }

        public string PathToRoot 
        {
            set { pathToTemplates = Path.Combine(value, "EmailTemplates"); }
        }

        protected override string LoadBodyTemplate(UserAccountEvent<UserAccount> evt)
        {
            return LoadTemplate(string.Concat(CleanGenericName(evt.GetType()), "_Body.txt"));
        }

        protected override string LoadSubjectTemplate(UserAccountEvent<UserAccount> evt)
        {
            return LoadTemplate(string.Concat(CleanGenericName(evt.GetType()), "_Subject.txt"));
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
    }
}
