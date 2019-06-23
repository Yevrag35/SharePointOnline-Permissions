using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MG.SharePoint
{
    public static class UserExtensions
    {
        public static void LoadAllUsers(this UserCollection col)
        {
            col.Initialize();
            col.LoadProperty(c => c.Include(
                x => x.AadObjectId, x => x.Alerts, x => x.Email, x => x.Groups.Include(g => g.Title),
                    //g => g.AllowMembersEditMembership, g => g.AllowRequestToJoinLeave, g => g.AutoAcceptRequestToJoinLeave,
                    //g => g.CanCurrentUserEditMembership, g => g.CanCurrentUserManageGroup, g => g.CanCurrentUserViewMembership,
                    //g => g.Description, g => g.Id, g => g.IsHiddenInUI, g => g.LoginName, g => g.OnlyAllowMembersViewMembership,
                    //g => g.OwnerTitle, g => g.PrincipalType, g => g.RequestToJoinLeaveEmailSetting, g => g.Title,
                    //g => g.Users.Include(
                    //    u => u.Title)
                    //),
                x => x.IsEmailAuthenticationGuestUser, x => x.Id, x => x.IsHiddenInUI, x => x.IsShareByEmailGuestUser,
                x => x.IsSiteAdmin, x => x.LoginName, x => x.PrincipalType, x => x.Title, x => x.UserId));
        }

        public static void LoadUserProps(this User user)
        {
            user.LoadProperty(x => x.AadObjectId, x => x.Alerts, x => x.Email, x => x.Groups.Include(g => g.Title),
                    //g => g.AllowMembersEditMembership, g => g.AllowRequestToJoinLeave, g => g.AutoAcceptRequestToJoinLeave,
                    //g => g.CanCurrentUserEditMembership, g => g.CanCurrentUserManageGroup, g => g.CanCurrentUserViewMembership,
                    //g => g.Description, g => g.Id, g => g.IsHiddenInUI, g => g.LoginName, g => g.OnlyAllowMembersViewMembership,
                    //g => g.OwnerTitle, g => g.PrincipalType, g => g.RequestToJoinLeaveEmailSetting, g => g.Title,
                    //g => g.Users.Include(
                    //    u => u.Title)
                    //),
                x => x.IsEmailAuthenticationGuestUser, x => x.Id, x => x.IsHiddenInUI, x => x.IsShareByEmailGuestUser,
                x => x.IsSiteAdmin, x => x.LoginName, x => x.PrincipalType, x => x.Title, x => x.UserId);
        }
    }
}