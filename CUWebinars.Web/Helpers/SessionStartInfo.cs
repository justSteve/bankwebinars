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
       public string FirstPage { get; set; }
       public string SessionID { get; set; }
       public string AffiliateSessionSource { get; set; }
        public string Elmah { get; set; }
        public string SessionRoot { get; set; }
    }
}