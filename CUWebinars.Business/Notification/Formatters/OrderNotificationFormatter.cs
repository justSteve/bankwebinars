using CUWebinars.Business.Constants;
using CUWebinars.Business.Models;
using CUWebinars.Business.Notification.Email;
using CUWebinars.Business.Notification.Renderers;
using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using RazorEngine.Templating;

namespace CUWebinars.Business.Notification.Formatters
{
    public class OrderNotificationFormatter<TOrder> : IOrderNotificationFormatter<TOrder>
        where TOrder : Order
    {
        private readonly Lazy<EnvironmentInformation> _environmentInformation;

        private string _emailSubject;
        private string _emailBody;

        #region Constructors

        public OrderNotificationFormatter(EnvironmentInformation environmentInformation)
        {
            if (environmentInformation == null) throw new ArgumentNullException("environmentInformation");
            _environmentInformation = new Lazy<EnvironmentInformation>(() => environmentInformation);
        }

        public OrderNotificationFormatter(Lazy<EnvironmentInformation> environmentInformation)
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

        public INotificationMessage Format<TBody>(Events.OrderSubmittedEvent<TOrder> orderSubmittedEvent, TBody objectOfMessage)
        {
            if (orderSubmittedEvent == null) throw new ArgumentNullException("orderSubmittedEvent");
            
            INotificationMessage message = null;

            try
            {
                //  This void method populated the _emailSubject and _emailBody variables which are used further up the stack
                LoadBodyTemplate(orderSubmittedEvent);

                message = CreateMessage(GetSubject(orderSubmittedEvent, objectOfMessage),
                    GetBody(orderSubmittedEvent, objectOfMessage));
            }
            catch (TemplateCompilationException templateCompilationException)
            {

            }
            catch (TemplateParsingException templateParsingException)
            {

            }
            catch (Exception exception)
            {
                
            }
            
            return message;
        }

        protected INotificationMessage CreateMessage(string subject, string body)
        {
            if (subject == null || body == null) return null;

            return new NotificationMessage { Subject = subject.Trim(), Body = body };
        }

        protected virtual string GetSubject<TBody>(Events.OrderSubmittedEvent<TOrder> orderSubmittedEvent, TBody subjectOfMessage)
        {
            return FormatValue(orderSubmittedEvent, _emailSubject, subjectOfMessage);
        }

        protected virtual string GetBody<T>(Events.OrderSubmittedEvent<TOrder> userAccountEvent, T bodyOfMessage)
        {
            return FormatValue(userAccountEvent, _emailBody, bodyOfMessage);
        }

        protected string FormatValue<T>(Events.OrderSubmittedEvent<TOrder> orderSubmittedEvent, string templatedText, T objectOfMessage)
        {
            var renderer = GetRenderer(orderSubmittedEvent);
            return renderer.ConstructMessage(orderSubmittedEvent, EnvironmentInformation, templatedText, objectOfMessage);
        }

        protected virtual IOrderRazorRenderer<TOrder> GetRenderer(Events.OrderSubmittedEvent<TOrder> userAccountEvent)
        {
            return new OrderRazorRenderer<TOrder>();
        }

        #region Template Utility Helpers

        protected virtual void LoadBodyTemplate(Events.OrderSubmittedEvent<TOrder> userAccountEvent)
        {
            LoadTemplate(CleanGenericName(userAccountEvent.GetType()) + DomainConstants.RazorExtension);
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
            var templatePath = Path.Combine(EnvironmentInformation.BaseUrl, DomainConstants.ResourcePathTemplate, name);
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
                    if (!reader.Name.Equals("html", StringComparison.OrdinalIgnoreCase)) continue;
                    var div = XNode.ReadFrom(reader) as XElement;

                    if (div != null)
                        _emailBody = div.ToString();
                }
            }
        }

        #endregion


    }

    #region Non-Generic Derived Class

    public class OrderNotificationFormatter : OrderNotificationFormatter<Order>
    {
        public OrderNotificationFormatter(EnvironmentInformation environmentInformation)
            : base(environmentInformation)
        {
        }

        public OrderNotificationFormatter(Lazy<EnvironmentInformation> environmentInformation)
            : base(environmentInformation)
        {
        }
    }

    #endregion
}