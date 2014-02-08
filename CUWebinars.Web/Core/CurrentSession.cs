//http://stackoverflow.com/questions/1710875/better-way-of-doing-strongly-typed-asp-net-mvc-sessions
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using CUWebinars.Business.Models;
using CUWebinars.Business.Repository;

namespace CUWebinars.Web.Core
{
    public class CurrentSession : MySession<WebUser>
    {
        public static CurrentSession Instance = new CurrentSession();
//#pragma warning disable 649
        private readonly IWebUserRepository _reposWu;
//#pragma warning restore 649

        protected WebUser LoadCurrentWebUser(string webUsername)
        {
            WebUser currentWebUser = LoadCurrentUser(webUsername);
            return currentWebUser;
            //return new WebUser(currentWebUser);
        }
// ReSharper disable InconsistentNaming
        // Put additional session objects here
        public const string Session_Affiliate = "CurrentAffiliate";
        public const string Session_WebUser = "CurrentWebUser";
        public const string Session_AffSessionSource = "AffiliateSessionSource";

        public const string Session_FirstCookies = "FirstCookies";
        public const string Session_FirstPage = "FirstPage";
        public const string Session_FirstReferrer = "FirstReferrer";

        public const string Session_SubdomainBranding = "SubdomainBranding";

        public const string Session_InitialQuerystring = "InitialQueryString";
        public const string Session_SessionId = "SessionID";
 // ReSharper restore InconsistentNaming           				            
        //public const string SESSIONOBJECT2 = "CurrentObject2";
        public String FirstPage
        {
            get
            {
                if (HttpContext.Current.Session[Session_FirstPage] == null)
                    HttpContext.Current.Session[Session_FirstPage] = "DEFAULT FirstPage";

                return HttpContext.Current.Session[Session_FirstPage].ToString();
            }
            set
            {
                HttpContext.Current.Session[Session_FirstPage] = value;
            }
        }


        public String FirstReferrer
        {
            get
            {
                if (HttpContext.Current.Session[Session_FirstReferrer] == null)
                    HttpContext.Current.Session[Session_FirstReferrer] = "default FirstReferrer";

                return HttpContext.Current.Session[Session_FirstReferrer].ToString();
            }
            set
            {
                HttpContext.Current.Session[Session_FirstReferrer] = value;
            }
        }

        public String SubdomainBranding
        {
            get
            {
                if (HttpContext.Current.Session[Session_SubdomainBranding] == null)
                    HttpContext.Current.Session[Session_SubdomainBranding] = "default SubdomainBranding";

                return HttpContext.Current.Session[Session_SubdomainBranding].ToString();
            }
            set
            {
                HttpContext.Current.Session[Session_SubdomainBranding] = value;
            }
        }

        public String FirstCookies
        {
            get
            {
                if (HttpContext.Current.Session[Session_FirstCookies] == null)
                    HttpContext.Current.Session[Session_FirstCookies] = "No Cookies";

                return HttpContext.Current.Session[Session_FirstCookies].ToString();
            }
            set
            {
                HttpContext.Current.Session[Session_FirstCookies] = value;
            }
        }
        public NameValueCollection InitialQueryString
        {
            get
            {
                if (HttpContext.Current.Session[Session_InitialQuerystring] == null)
                    HttpContext.Current.Session[Session_InitialQuerystring] = null;

                return (NameValueCollection) HttpContext.Current.Session[Session_InitialQuerystring];
            }
            set
            {
                HttpContext.Current.Session[Session_InitialQuerystring] = value;
            }
        }

        public String SessionID
        {
            get
            {
                if (HttpContext.Current.Session[Session_SessionId] == null)
                    HttpContext.Current.Session[Session_SessionId] = "default SessionID";

                return HttpContext.Current.Session[Session_SessionId].ToString();
            }
            set
            {
                HttpContext.Current.Session[Session_SessionId] = value;
            }
        }
        public Affiliate CurrentAffiliate
        {
            get
            {
                if (HttpContext.Current.Session[Session_Affiliate] == null)
                    HttpContext.Current.Session[Session_Affiliate] = Convert.ToInt32(19);

                return HttpContext.Current.Session[Session_Affiliate] as Affiliate;
            }
            set
            {
                HttpContext.Current.Session[Session_Affiliate] = value;
            }
        }
        public WebUser CurrentWebUser
        {
            get
            {
                if (HttpContext.Current.Session[Session_WebUser] == null)
                    HttpContext.Current.Session[Session_WebUser] = Convert.ToInt32(19);

                return HttpContext.Current.Session[Session_WebUser] as WebUser;
            }
            set
            {
                HttpContext.Current.Session[Session_WebUser] = value;
            }
        }
        public String AffiliateSessionSource
        {
            get
            {
                if (HttpContext.Current.Session[Session_AffSessionSource] == null)
                    HttpContext.Current.Session[Session_AffSessionSource] = "default|" + 19;

                return HttpContext.Current.Session[Session_AffSessionSource].ToString();
            }
            set
            {
                HttpContext.Current.Session[Session_AffSessionSource] = value;
            }
        }

        protected override WebUser LoadCurrentUser(string username)
        {
            return _reposWu.GetAll().Single(u => u.email == username);

        }
    }
}