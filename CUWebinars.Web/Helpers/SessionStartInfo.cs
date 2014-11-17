namespace CUWebinars.Web.Helpers
{
    public class SessionStartInfo
    {
       public string remoteAddress { get; set; }
       public string remoteHost { get; set; }
       public string remoteUser { get; set; }
       public string userAgent { get; set; }
       public string userCookie { get; set; }

    }
}