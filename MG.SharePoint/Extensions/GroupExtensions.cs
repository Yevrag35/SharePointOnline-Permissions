using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MG.SharePoint
{
    public static class GroupExtensions
    {
        public static void LoadAllGroups(this GroupCollection col)
        {
            col.Initialize();
            col.LoadProperty(true, c => c.Include(
                g => g.AllowMembersEditMembership, g => g.AllowRequestToJoinLeave,
                g => g.AutoAcceptRequestToJoinLeave, g => g.CanCurrentUserEditMembership, g => g.CanCurrentUserManageGroup,
                g => g.CanCurrentUserViewMembership, g => g.Description, g => g.Id, g => g.IsHiddenInUI, g => g.LoginName,
                g => g.OnlyAllowMembersViewMembership,
                g => g.OwnerTitle, g => g.PrincipalType,
                g => g.RequestToJoinLeaveEmailSetting, g => g.Title,
                g => g.Users.Include(u => u.Title)));
        }

        public static void LoadGroupProps(this Group group)
        {
            group.LoadProperty(true, g => g.AllowMembersEditMembership, g => g.AllowRequestToJoinLeave,
                g => g.AutoAcceptRequestToJoinLeave, g => g.CanCurrentUserEditMembership, g => g.CanCurrentUserManageGroup,
                g => g.CanCurrentUserViewMembership, g => g.Description, g => g.Id, g => g.IsHiddenInUI, g => g.LoginName,
                g => g.OnlyAllowMembersViewMembership, g => g.OwnerTitle, g => g.PrincipalType,
                g => g.RequestToJoinLeaveEmailSetting, g => g.Title,
                g => g.Users.Include(u => u.Title));
        }

        public static void LoadOwner(this Group group)
        {
            group.Context.Load(group.Owner, x => x.Id, x => x.IsHiddenInUI, x => x.LoginName, x => x.PrincipalType, x => x.Title);
            try
            {
                group.Context.ExecuteQuery();
            }
            catch (ServerException) { }
        }
    }
}