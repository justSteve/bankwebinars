using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using CUWebinars.Business.AccountService;
using CUWebinars.Business.Models;
using CUWebinars.Business.Repository;

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


        //    public static Boolean IsCurrentUserPasswordDefault(this UserFacade userFacadeInstance)
        //    {
        //        if (userFacadeInstance.IsAuthenticated() == false)
        //        {
        //            return false;
        //        }

        //        if (userFacadeInstance.IsCurrentUserControlled())
        //        {
        //            return false;
        //        }
        //        else
        //        {
        //            return GetAuthenticatedUser(userFacadeInstance).LastName.ToLower().Trim() == GetAuthenticatedUser(userFacadeInstance).Password.ToLower().Trim();
        //        }
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

        //    public static void TakeControlOverUser(this UserFacade userFacadeInstance, int userID)
        //    {
        //        User authenticatedUser = userFacadeInstance.GetCurrentUser();

        //        if ((authenticatedUser.UserType == UserType.Affiliate ||
        //             authenticatedUser.UserType == UserType.Admin) == false)
        //        {
        //            throw new SecurityException("You do not have permissions to take control over user actions");
        //        }

        //        User controlledUser = userFacadeInstance.Load(userID);
        //        if (controlledUser.Status == UserStatus.DELETED)
        //        {
        //            throw new RulesException(AppConst.GENERAL_RULES_ERROR,
        //                "You can not control deleted users");
        //        }

        //        if (authenticatedUser.UserType == UserType.Affiliate &&
        //            controlledUser.Affiliate.ID != authenticatedUser.ID)
        //        {
        //            throw new RulesException(AppConst.GENERAL_RULES_ERROR,
        //                "Affiliates can control only their customers");
        //        }

        //        if (controlledUser.UserType == UserType.Customer ||
        //            controlledUser.UserType == UserType.Affiliate)
        //        {
        //            HttpContext.Current.Session["CONTROLLED_USER_SESSION_STORE"] = userID;
        //        }
        //        else
        //        {
        //            throw new RulesException(AppConst.GENERAL_RULES_ERROR,
        //                "You can control only ordinal users or affiliates, not admins, presenters, etc.");
        //        }
        //    }

        //    public static void ReleaseControlOverUser(this UserFacade userFacadeInstance)
        //    {
        //        if (userFacadeInstance.IsCurrentUserControlled() == false)
        //        {
        //            throw new TTSException("There is no controlled user to release the control");
        //        }

        //        HttpContext.Current.Session["CONTROLLED_USER_SESSION_STORE"] = null;

        //    }

        //    public static bool IsCurrentUserControlled(this UserFacade userFacadeInstance)
        //    {
        //        if (userFacadeInstance.IsAuthenticated() == false)
        //        {
        //            return false;
        //        }

        //        return HttpContext.Current.Session["CONTROLLED_USER_SESSION_STORE"] != null;
        //    }

        //    //public static IList<Affiliate> CurrentUserIsOwnedByMultipleAffiliates(this UserFacade userFacadeInstance)
        //    //{
        //    //    IList<Affiliate> affiliates =
        //    //        UserFacade.Instance.SelectAffilatesWhoOwnCurrentUser(UserFacade.Instance.GetCurrentUser().Email);


        //    //    return null;
        //    //}



        //    public static int GetMasterUserID(this UserFacade userFacadeInstance)
        //    {
        //        User masterUser = userFacadeInstance.GetMasterUser();

        //        return masterUser.ID;
        //    }

        //    public static User GetMasterUser(this UserFacade userFacadeInstance)
        //    {
        //        if (userFacadeInstance.IsCurrentUserControlled() == false)
        //        {
        //            throw new TTSException("There is no controlled user");
        //        }

        //        return GetAuthenticatedUser(userFacadeInstance);
        //    }

        //    public static UserType GetOrderInitiator(this UserFacade userFacadeInstance)
        //    {
        //        if (userFacadeInstance.IsCurrentUserControlled() == false)
        //        {
        //            User currentUser = userFacadeInstance.GetCurrentUser();
        //            return currentUser.UserType;
        //        }

        //        User masterUser = userFacadeInstance.GetMasterUser();
        //        return masterUser.UserType;
        //    }
        //    public static User GetOrderInitiatorUser(this UserFacade userFacadeInstance)
        //    {
        //        if (userFacadeInstance.IsCurrentUserControlled() == false)
        //        {
        //            User currentUser = userFacadeInstance.GetCurrentUser();
        //            return currentUser;
        //        }

        //        User masterUser = userFacadeInstance.GetMasterUser();
        //        return masterUser;
        //    }
        //    public static User GetAuthenticatedUser(this UserFacade userFacadeInstance)
        //    {
        //        string email = Thread.CurrentPrincipal.Identity.Name;

        //        User user = userFacadeInstance.LoadByEmailAndAffiliate(email, AppHelper.GetCurrentAffiliate().ID);

        //        if (user != null)
        //        {
        //            return user;
        //        }

        //        try
        //        {
        //            if (!CurrentSession.Instance.AffiliateSessionSource.Contains("default")
        //                )
        //            {
        //                user = userFacadeInstance.LoadByEmail(email);
        //                if (user.UserType == UserType.Customer)
        //                {
        //                    Logger.Instance.LogMessage("GetAuthenticatedUser CLONED: " + user.ID + " off: " +
        //                                               CurrentSession.Instance.AffiliateSessionSource);
        //                    return UserFacade.Instance.CloneUserToNewAffiliate(user, AppHelper.GetCurrentAffiliate());
        //                }
        //                else
        //                {
        //                    return user;
        //                }
        //            }
        //            else
        //            {
        //                user = userFacadeInstance.LoadByEmail(email);
        //                return user;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Logger.Instance.LogException(ex);
        //            Logger.Instance.LogMessage("ERROR: Cloning user via GetAuthenticatedUser: " + ex.Message);

        //            return null;
        //        }
        //        Logger.Instance.LogMessage("ERROR: GetAuthUser returned Null. ");
        //        return null;

        //    }

        //    public static int GetAuthenticatedUserID(this UserFacade userFacadeInstance)
        //    {
        //        var authenticatedUser = GetAuthenticatedUser(userFacadeInstance);

        //        return authenticatedUser.ID;
        //    }

        //    public static int GetAffilliateByTTSDomain(string currentHost)
        //    {
        //        if (currentHost.ToLower() == "www" || currentHost.ToLower() == "testing" || currentHost.ToLower() == "bankwebinars" || currentHost == "localhost")
        //        {
        //            return 0;
        //        }
        //        try
        //        {
        //            return AffiliateFacade.Instance.LoadByTTSDomain(currentHost).ID;
        //        }
        //        catch (Exception ex)
        //        {
        //            Logger.Instance.LogException(ex);
        //            Logger.Instance.LogMessage("ERROR: GetAffilliateByTTSDomain: was passed: " + currentHost);
        //            return 0;
        //        }
        //    }
        //}

    }
}
