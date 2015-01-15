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

        public INotificationMessage Format<T>(T underPinningObject, string templateName)
        {
            LoadBodyTemplate(templateName);
            Console.WriteLine(templateName);
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

        protected virtual void LoadBodyTemplate(string templateName)
        {
            LoadTemplate(templateName + DomainConstants.RazorExtension);
        }

        private void LoadTemplate(string name)
        {
            var templatePath = "CUWebinars.Business.Notification.Templates." + name;

            var assembly = typeof(Formatter).Assembly;

            var settings = new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment };

            using (XmlReader reader = XmlReader.Create(assembly.GetManifestResourceStream(templatePath), settings))
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
                    if (!reader.Name.Equals("html", StringComparison.OrdinalIgnoreCase)) continue;
                    var div = XNode.ReadFrom(reader) as XElement;

                    if (div != null)
                        _emailBody = div.ToString();
                }
            } 
        }
    }
}
