using System;
using System.Configuration;
using Microsoft.Azure.WebJobs;

namespace CUWebinars.Azure.Common
{
    public class ConfigNameResolver : INameResolver
    {
        public string Resolve(string name)
        {
            string resolvedName = ConfigurationManager.AppSettings[name];
            if (string.IsNullOrWhiteSpace(resolvedName))
            {
                throw new InvalidOperationException("Cannot resolve " + name);
            }

            return resolvedName;
        }
    }
}
