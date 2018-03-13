using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CUWebinars.Web
{
    public class Log4NetAspNetProperty
    {
        private const string PropertyNamePrefix = "log4net_app_";
        private const string PropertyDefaultValue = null;

        private readonly string propertyName;
        private readonly object propertyValue;

        public string Name { get { return propertyName; } }

        private Log4NetAspNetProperty(string propertyName, object propertyValue)
        {
            if (String.IsNullOrWhiteSpace(propertyName)) throw new ArgumentNullException("propertyName");

            this.propertyName = propertyName;
            this.propertyValue = propertyValue;

            if (HttpContext.Current != null)
                HttpContext.Current.Items[GetPrefixedPropertyName()] = propertyValue;
        }

        public override string ToString()
        {
            if (HttpContext.Current == null)
                return PropertyDefaultValue;

            var item = HttpContext.Current.Items[GetPrefixedPropertyName()];
            return item != null ? item.ToString() : PropertyDefaultValue;
        }

        private static string GetPrefixedPropertyName()
        {
            return String.Format("{0}{1}", PropertyNamePrefix, PropertyDefaultValue);
        }

        public static void CurrentUserId(object userId)
        {
            var property = new Log4NetAspNetProperty("CurrentUserId", userId);
            log4net.ThreadContext.Properties[property.Name] = property;
        }

        public static void CurrentUrl(object url)
        {
            var property = new Log4NetAspNetProperty("CurrentUrl", url);
            log4net.ThreadContext.Properties[property.Name] = property;
        }
        public static void CurrentIp(object ip)
        {
            var property = new Log4NetAspNetProperty("CurrentIp", ip);
            log4net.ThreadContext.Properties[property.Name] = property;
        }
    }
}