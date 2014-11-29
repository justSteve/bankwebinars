using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Web;
using CUWebinars.Web.Tests.Infrastructure;

namespace CUWebinars.Web.Tests.Fakes
{
    /// <summary>
    /// Provides stubs for ApplicationPath, Headers, Url and UrlReferrer
    /// </summary>
    public class HttpRequestFake1 : HttpRequestBase
    {
        private WebTestsGlobalConfig _webTestsGlobals = WebTestsGlobalConfig.WebTestsGlobalConfigSingleton;

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

        public override NameValueCollection ServerVariables
        {
            get
            {
                return new NameValueCollection
                {
                    {"HTTP_COOKIE",                    "ASP.NET_SessionId=ztwj2r1d3oxfxwno0hgqrdju;__RequestVerificationToken=Yqbn2w5yVR3260rQhbHNRZ6aHrssoy6I40KGRGk8fwlKpVFAzEs24Udr81qM11HsMDZhoBAYRPTGtee1WUYcUlVH2SUvIZJFpGpuMrRMPLg1"}
                };
            }
        }

        public override Uri Url
        {
            get { return new Uri(_webTestsGlobals.SiteUrl); }
        }

        public override Uri UrlReferrer
        {
            get
            {
                return new Uri(_webTestsGlobals.SiteUrl); 
            }
        }
    }
}
