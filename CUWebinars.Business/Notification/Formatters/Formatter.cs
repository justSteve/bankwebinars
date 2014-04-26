using CUWebinars.Business.Constants;
using CUWebinars.Business.Notification.Email;
using CUWebinars.Business.Notification.Renderers;
using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace CUWebinars.Business.Notification.Formatters
{
    public class Formatter : IFormatter
    {
        private readonly Lazy<EnvironmentInformation> _environmentInformation;
        private string _emailSubject;
        private string _emailBody;

        public Formatter(EnvironmentInformation environmentInformation)
        {
            if (environmentInformation == null) throw new ArgumentNullException("environmentInformation");
            _environmentInformation = new Lazy<EnvironmentInformation>(() => environmentInformation);
        }

        public Formatter(Lazy<EnvironmentInformation> environmentInformation)
        {
            if (environmentInformation == null) throw new ArgumentNullException("environmentInformation");
            _environmentInformation = environmentInformation;
        }

        public EnvironmentInformation EnvironmentInformation
        {
            get
            {
                return _environmentInformation.Value;
            }
        }

        public INotificationMessage Format<T>(T underPinningObject)
        {
            LoadBodyTemplate(underPinningObject);

            return CreateMessage(GetSubject(underPinningObject), GetBody(underPinningObject));
        }

        protected INotificationMessage CreateMessage(string subject, string body)
        {
            if (subject == null || body == null) return null;

            return new NotificationMessage { Subject = subject.Trim(), Body = body };
        }

        protected virtual string GetSubject<T>(T subjectOfMessage)
        {
            return FormatValue(_emailSubject, subjectOfMessage);
        }

        protected virtual string GetBody<T>(T bodyOfMessage)
        {
            return FormatValue(_emailBody, bodyOfMessage);
        }

        protected string FormatValue<T>(string templatedText, T objectOfMessage)
        {
            var renderer = GetRenderer();
            return renderer.ConstructMessage(EnvironmentInformation, templatedText, objectOfMessage);
        }

        protected virtual IRenderer GetRenderer()
        {
            return new Renderer();
        }

        protected virtual void LoadBodyTemplate<T>(T evt)
        {
            LoadTemplate(CleanGenericName(evt.GetType()) + DomainConstants.RazorExtension);
        }

        private void LoadTemplate(string name)
        {
            var templatePath = Path.Combine(EnvironmentInformation.BaseUrl, DomainConstants.ResourcePathTemplate, name);
            var settings = new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment };

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

    }
}
