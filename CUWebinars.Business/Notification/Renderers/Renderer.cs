using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using Encoding = RazorEngine.Encoding;

namespace CUWebinars.Business.Notification.Renderers
{
    public class Renderer : IRenderer
    {
        public string ConstructMessage<T>(EnvironmentInformation applicationInformation, string templatedText, T objectOfMessage)
        {
            //  create a configuration file for the template service
            var config = new FluentTemplateServiceConfiguration(c =>
            {
                c.WithEncoding(Encoding.Raw);
                c.ResolveUsing<TemplateResolver>();
                c.WithBaseTemplateType(typeof(TtsHtmlTemplateBase<>));
                c.IncludeNamespaces("CUWebinars.Business.Core.Helpers",
                    "CUWebinars.Business.Models",
                    "CUWebinars.Business.Notification");
                    //"CUWebinars.Html");
            });

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
