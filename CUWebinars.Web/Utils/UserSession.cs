using System;
using System.Linq;
using System.Web;
using CUWebinars.Web.Data.Repositories.Interfaces;
using CUWebinars.Business.Models;
using CUWebinars.Business.Repository;

namespace CUWebinars.Web.Utils
{
    public class UserSession : MySession<WebUser>
    {
        public static UserSession Instance = new UserSession();
        private readonly IWebUserRepository _reposWU;
        //private readonly IAffiliateRepository _reposAff;
        // Put additional session objects here
        //public const string SESSION_Affiliate = "CurrentAffiliate";
        public const string SESSION_WebUser = "CurrentWebUser";
        //public const string SESSION_Aff_SESSIONSOURCE = "AffiliateSessionSource";
        //public const string SESSIONOBJECT2 = "CurrentObject2";

        protected override WebUser LoadCurrentUser(string WebUsername)
        {
            WebUser currentWebUser = _reposWU.GetAll().Single(w => w.email == WebUsername);
            return (WebUser)currentWebUser;
        }

        //protected override Affiliate LoadCurrentAffiliate()
        //{
        //    Affiliate currentAffiliate = _reposAff.GetCurrentAffiliate();
        //    return (Affiliate) currentAffiliate;
        //}


        public WebUser CurrentUser
        {
            get
            {
                if (HttpContext.Current.Session[SESSION_WebUser] == null)
                    HttpContext.Current.Session[SESSION_WebUser] = Convert.ToInt32(19);

                return HttpContext.Current.Session[SESSION_WebUser] as WebUser;
            }
            set
            {
                HttpContext.Current.Session[SESSION_WebUser] = value;
            }
        }
        //public Object2 CurrentObject2
        //{
        //    get
        //    {
        //        if (Session[SESSIONOBJECT2] == null)
        //            Session[SESSIONOBJECT2] = new Object2();

        //        return Session[SESSIONOBJECT2] as Object2;
        //    }
        //    set
        //    {
        //        Session[SESSIONOBJECT2] = value;
        //    }
        //}
    }
}