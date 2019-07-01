using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MG.SharePoint
{
    public static class UserExtensions
    {
        public static bool IsLoaded(this User user)
        {
            return user.IsPropertyReady(x => x.AadObjectId, x => x.Alerts, x => x.Email, x => x.Groups.Include(g => g.Title),
                x => x.IsEmailAuthenticationGuestUser, x => x.Id, x => x.IsHiddenInUI, x => x.IsShareByEmailGuestUser,
                x => x.IsSiteAdmin, x => x.LoginName, x => x.PrincipalType, x => x.Title, x => x.UserId);
        }

        public static void LoadAllUsers(this UserCollection col)
        {
            col.Initialize();
            col.LoadProperty(c => c.Include(
                x => x.AadObjectId, x => x.Alerts, x => x.Email, x => x.Groups.Include(g => g.Title),
                x => x.IsEmailAuthenticationGuestUser, x => x.Id, x => x.IsHiddenInUI, x => x.IsShareByEmailGuestUser,
                x => x.IsSiteAdmin, x => x.LoginName, x => x.PrincipalType, x => x.Title, x => x.UserId));
        }

        public static void LoadUserProps(this User user)
        {
            user.LoadProperty(x => x.AadObjectId, x => x.Alerts, x => x.Email, x => x.Groups.Include(g => g.Title),
                x => x.IsEmailAuthenticationGuestUser, x => x.Id, x => x.IsHiddenInUI, x => x.IsShareByEmailGuestUser,
                x => x.IsSiteAdmin, x => x.LoginName, x => x.PrincipalType, x => x.Title, x => x.UserId);
        }
    }
}