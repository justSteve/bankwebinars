using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Web;

namespace CUWebinars.Web.Tests.Fakes
{
    /// <summary>
    /// Provides stubs for ApplicationPath, Headers, Url and UrlReferrer
    /// </summary>
    public class HttpRequestFake1 : HttpRequestBase
    {
        public override string ApplicationPath
        {
            get { return System.IO.Path.AltDirectorySeparatorChar.ToString(); }
        }

        public override NameValueCollection Headers
        {
            get
            {
                return new NameValueCollection(); 
            }
        }

        public override Uri Url
        {
            get { return new Uri(ConfigurationManager.AppSettings["SiteUrl"]); }
        }

        public override Uri UrlReferrer
        {
            get
            {
                return new Uri(@"http://localhost:3538/"); 
            }
        }
    }
}
