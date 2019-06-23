using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MG.SharePoint
{
    public static class WebExtensions
    {
        public static User GetUserByEmail(this Web web, string email)
        {
            if (!web.SiteUsers.AreItemsAvailable)
            {
                web.SiteUsers.LoadProperty();
            }
            User user = web.SiteUsers.GetByEmail(email);
            user.LoadProperty(x => x.AadObjectId, x => x.Alerts, x => x.Email, x => x.Groups,
                x => x.Id, x => x.IsEmailAuthenticationGuestUser, x => x.IsHiddenInUI, x => x.IsShareByEmailGuestUser,
                x => x.IsSiteAdmin, x => x.LoginName, x => x.PrincipalType, x => x.Title, x => x.UserId);
            return user;
        }


    }
}