using System;

namespace CUWebinars.Web.Helpers
{
    [Serializable]
    public class SessionStartInfo
    {
       public string RemoteAddress { get; set; }
       public string RemoteHost { get; set; }
       public string RemoteUser { get; set; }
       public string UserAgent { get; set; }
       public string UserCookie { get; set; }


    }
}