using CUWebinars.Business.Constants;
using CUWebinars.Business.Notification.Email;
using CUWebinars.Business.Notification.Renderers;
using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
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

            var returnThis = CreateMessage(GetSubject(underPinningObject), GetBody(underPinningObject));
            return returnThis;
        }

        public INotificationMessage FormatV2<T>(T underPinningObject, string templatePath)
        {
            // using MVC for text template rendering "outside" of the web project (based on http://www.codemag.com/article/1312081)
            var controller = ViewRenderer.CreateController<EmptyController>(); // Create an arbitrary controller instance

            string markup = ViewRenderer.RenderPartialView(
                             templatePath,
                             underPinningObject,
                             controller.ControllerContext);

            var returnThis = CreateMessage(markup);
            return returnThis;
        }

        private INotificationMessage CreateMessage(string markup)
        {
            string subject = null;
            string body = null;
            
            // grab inner text of Title tag as subject
            Regex reFindTitle = new Regex("<title>([^<>]*)</title>", RegexOptions.Multiline);
            Match matches = reFindTitle.Match(markup);
            if (matches.Success)
            {
                subject = matches.Groups[1].Value.Trim();

                // html decode the subject in case the administrator included special characters in the 
                //  textbox which were automatically HTML Encoded on submit
                subject = WebUtility.HtmlDecode(subject);
            }

            body = reFindTitle.Replace(markup, "").Trim(); // delete the title tag and use the rest as the body

            return CreateMessage(subject, body);
        }

        public string FormatToString<T>(T objectOfMessage, string templateName)
        {
            throw new NotImplementedException();
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
                    if (!reader.Name.Equals("html", StringComparison.OrdinalIgnoreCase)) continue;
                    var div = XNode.ReadFrom(reader) as XElement;

                    if (div != null)
                        _emailBody = div.ToString();
                }
            }
        }
    }
}