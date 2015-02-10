using CUWebinars.Web.Core;
using System;
using System.Net;

namespace CUWebinars.Web.Infrastructure
{
    public class TtsWebClient : WebClient
    {
        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest webRequest = base.GetWebRequest(uri);
            
            if (!ReferenceEquals(webRequest, null))
            {
                webRequest.Timeout = 1000 * GlobalConfig.GlobalConfigSingleton.GhostRequestTimeout;
            }
            return webRequest;
        }
    }
}