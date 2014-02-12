using BrockAllen.MembershipReboot;
using CUWebinars.Business.Notification;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;

namespace CUWebinars.Web.Notification
{
    public class RazorRenderer<TAccount> : IRazorRenderer<TAccount> where TAccount : UserAccount
    {
        public string ConstructMessage<T>(Business.Notification.Events.UserAccountEvent<TAccount> accountEvent,
            EnvironmentInformation applicationInformation,
            string templatedText,
            T objectOfMessage
            )
        {
            var config = new FluentTemplateServiceConfiguration(c => c.WithEncoding(Encoding.Html));
            var user = accountEvent.Account;

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
