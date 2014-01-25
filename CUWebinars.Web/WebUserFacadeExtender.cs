using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using CUWebinars.Business.AccountService;
using CUWebinars.Business.Models;
using CUWebinars.Business.Repository;
//TODO: Review these methods from the legacy site and decide best way to implement
namespace CUWebinars.Web
{
    public static class WebUserFacadeExtender
    {
        public static bool IsAuthenticated(this MembershipService userFacadeInstance)
        {
            bool isAuthenticated = Thread.CurrentPrincipal.Identity.IsAuthenticated;
            return isAuthenticated;
        }

        //    public static int GetCurrentUserID(this UserFacade userFacadeInstance)
        //    {
        //        User currentUser = userFacadeInstance.GetCurrentUser();

        //        if (currentUser == null)
        //        {
        //            return 0;
        //        }
        //        else
        //        {
        //            return currentUser.ID;
        //        }
        //    }

        //    public static USTimeZone GetCurrentUserTimeZone(this UserFacade userFacadeInstance)
        //    {
        //        User currentUser = userFacadeInstance.GetCurrentUser();

        //        return currentUser.TimeZone;
        //    }

        //    public static User GetCurrentUser(this UserFacade userFacadeInstance)
        //    {
        //        if (userFacadeInstance.IsAuthenticated() == false)
        //        {
        //            Logger.Instance.LogMessage("ERROR: GetCurrentUser called but user not Auth");
        //            return null;
        //        }

        //        return GetAuthenticatedUser(userFacadeInstance);

        //        //if (userFacadeInstance.IsCurrentUserControlled())
        //        //{
        //        //    try
        //        //    {
        //        //        int userID = (int)HttpContext.Current.Session["CONTROLLED_USER_SESSION_STORE"];
        //        //        return userFacadeInstance.Load(userID);
        //        //    }
        //        //    catch (Exception ex)
        //        //    {
        //        //        Logger.Instance.LogException(ex);

        //        //        ReleaseControlOverUser(userFacadeInstance);
        //        //        return GetAuthenticatedUser(userFacadeInstance);
        //        //    }
        //        //}
        //        //else
        //        //{
        //        //    return GetAuthenticatedUser(userFacadeInstance);
        //        //}
        //    }


    }
}
