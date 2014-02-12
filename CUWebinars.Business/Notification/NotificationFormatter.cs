using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using BrockAllen.MembershipReboot;
using CUWebinars.Business.Notification.Email;
using CUWebinars.Web.Notification;

namespace CUWebinars.Business.Notification
{
    public class NotificationFormatter<TAccount> : INotificationFormatter<TAccount>
        where TAccount : UserAccount
    {
        private readonly Lazy<EnvironmentInformation> _environmentInformation;
        private const string ResourcePathTemplate = @"Notification\Templates";
        private const string RazorExtension = ".cshtml";
        private string _emailSubject;
        private string _emailBody;

        #region Constructors

        public NotificationFormatter(EnvironmentInformation environmentInformation)
        {
            if (environmentInformation == null) throw new ArgumentNullException("environmentInformation");
            _environmentInformation = new Lazy<EnvironmentInformation>(() => environmentInformation);
        }

        public NotificationFormatter(Lazy<EnvironmentInformation> environmentInformation)
        {
            if (environmentInformation == null) throw new ArgumentNullException("environmentInformation");
            _environmentInformation = environmentInformation;
        }

        #endregion

        public EnvironmentInformation EnvironmentInformation
        {
            get
            {
                return _environmentInformation.Value;
            }
        }

        public INotificationMessage Format<TBody>(Business.Notification.Events.UserAccountEvent<TAccount> userAccountEvent, TBody objectOfMessage)
        {
            if (userAccountEvent == null) throw new ArgumentNullException("userAccountEvent");

            //  This void method populated the _emailSubject and _emailBody variables which are used further up the stack
            LoadBodyTemplate(userAccountEvent);

            return CreateMessage(GetSubject(userAccountEvent, objectOfMessage), GetBody(userAccountEvent, objectOfMessage));
        }

        protected INotificationMessage CreateMessage(string subject, string body)
        {
            if (subject == null || body == null) return null;

            return new NotificationMessage { Subject = subject.Trim(), Body = body };
        }

        protected virtual string GetSubject<TBody>(Business.Notification.Events.UserAccountEvent<TAccount> userAccountEvent, TBody subjectOfMessage)
        {
            return FormatValue(userAccountEvent, _emailSubject, subjectOfMessage);
        }

        protected virtual string GetBody<T>(Business.Notification.Events.UserAccountEvent<TAccount> userAccountEvent, T bodyOfMessage)
        {
            return FormatValue(userAccountEvent, _emailBody, bodyOfMessage);
        }

        protected string FormatValue<T>(Business.Notification.Events.UserAccountEvent<TAccount> userAccountEvent, string templatedText, T objectOfMessage)
        {
            var renderer = GetRenderer(userAccountEvent);
            return renderer.ConstructMessage(userAccountEvent, EnvironmentInformation, templatedText, objectOfMessage);
        }

        protected virtual IRazorRenderer<TAccount> GetRenderer(Business.Notification.Events.UserAccountEvent<TAccount> userAccountEvent)
        {
            return new RazorRenderer<TAccount>();
        }

        #region Template Utility Helpers

        protected virtual void LoadBodyTemplate(Business.Notification.Events.UserAccountEvent<TAccount> userAccountEvent)
        {
            LoadTemplate(CleanGenericName(userAccountEvent.GetType()) + RazorExtension);
        }

        private string CleanGenericName(Type type)
        {
            var name = type.Name;
            var index = name.IndexOf('`');

            if (index > 0)
            {
                return name.Substring(0, index);
            }

            return name;
        }

        private void LoadTemplate(string name)
        {
            var templatePath = Path.Combine(EnvironmentInformation.BaseUrl, ResourcePathTemplate, name);
            var settings = new XmlReaderSettings{ ConformanceLevel = ConformanceLevel.Fragment };

            using (XmlReader reader = XmlReader.Create(templatePath, settings))
            {
                while (reader.Read())
                {
                    if (reader.Name.Equals("title", StringComparison.OrdinalIgnoreCase))
                    {
                        _emailSubject = reader.ReadElementContentAsString();
                        break;
                    }
                }

                while (reader.Read())
                {
                    if (!reader.Name.Equals("div", StringComparison.OrdinalIgnoreCase)) continue;
                    var div = XNode.ReadFrom(reader) as XElement;

                    if (div != null)
                        _emailBody = div.ToString();
                }
            }
        }

        #endregion


    }

    #region Non-Generic Derived Class

    public class NotificationFormatter : NotificationFormatter<UserAccount>
    {
        public NotificationFormatter(EnvironmentInformation  environmentInformation)
            : base(environmentInformation)
        {
        }

        public NotificationFormatter(Lazy<EnvironmentInformation> environmentInformation)
            : base(environmentInformation)
        {
        }
    }

    public class EnvironmentInformation
    {
        public string BaseUrl { get; set; }
    }

    #endregion

    
}