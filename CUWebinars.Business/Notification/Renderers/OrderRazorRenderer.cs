using CUWebinars.Business.Models;
using CUWebinars.Business.Notification.Events;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;

namespace CUWebinars.Business.Notification.Renderers
{
    public class OrderRazorRenderer<TOrder> : IOrderRazorRenderer<TOrder> where TOrder : Order
    {
        public string ConstructMessage<T>(OrderSubmittedEvent<TOrder> orderSubmittedEvent,
            EnvironmentInformation applicationInformation,
            string templatedText,
            T objectOfMessage
            )
        {
            //  create a configuration file for the template service
            var config = new FluentTemplateServiceConfiguration(c =>
            {
                c.WithEncoding(Encoding.Html);
                c.ResolveUsing<TemplateResolver>();
            });

            var order = orderSubmittedEvent.Order;

            // create a new TemplateService and pass in the configuration to the constructor
            var myConfiguredTemplateService = new TemplateService(config);

            // set the template service to our configured one
            Razor.SetTemplateService(myConfiguredTemplateService);

            // start parsing templates
            var result = Razor.Parse(templatedText, objectOfMessage);

            return result;
        }
    }
}
