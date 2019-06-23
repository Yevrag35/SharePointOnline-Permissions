using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MG.SharePoint
{
    public static class UserCollectionExtensions
    {
        public static User GetUserByEmail(this UserCollection col, string email, bool loadProps)
        {
            User user = col.GetByEmail(email);
            if (loadProps)
            {
                user.LoadUserProps();
            }
            return user;
        }

        public static void LoadUsers(this UserCollection col)
        {
            col.LoadProperty();
            CTX.Lae(col, true, c => c.Include(
                x => x.AadObjectId, x => x.Alerts, x => x.Email, x => x.Groups,
                x => x.Id, x => x.IsEmailAuthenticationGuestUser, x => x.IsHiddenInUI,
                x => x.IsShareByEmailGuestUser, x => x.IsSiteAdmin, x => x.LoginName, x => x.PrincipalType,
                x => x.Title, x => x.UserId));
        }

        public static IEnumerable<User> GetUsersByEmail(this UserCollection col, params string[] emails)
        {
            var list = new List<User>(emails.Length);
            for (int i = 0; i < emails.Length; i++)
            {
                User user = col.GetByEmail(emails[i]);
                user.LoadUserProps();
                list.Add(user);
            }
            return list;
        }

        public static void LoadUserProps(this User user)
        {
            user.LoadProperty(x => x.AadObjectId, x => x.Alerts, x => x.Email, x => x.Groups, 
                x => x.Groups.Include(
                    g => g.Title, g => g.Id
                ),
                x => x.Id, x => x.IsEmailAuthenticationGuestUser, x => x.IsHiddenInUI,
                x => x.IsShareByEmailGuestUser, x => x.IsSiteAdmin, x => x.LoginName, x => x.PrincipalType,
                x => x.Title, x => x.UserId);
        }
    }
}