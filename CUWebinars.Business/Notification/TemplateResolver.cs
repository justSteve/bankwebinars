using CUWebinars.Business.Constants;
using RazorEngine.Templating;
using System;
using System.IO;
using System.Text;

namespace CUWebinars.Business.Notification
{
    public class TemplateResolver : ITemplateResolver
    {
        public string Resolve(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            var path = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, DomainConstants.ResourcePathTemplate, name);
            return File.ReadAllText(path, Encoding.Default);
        }
    }
}
