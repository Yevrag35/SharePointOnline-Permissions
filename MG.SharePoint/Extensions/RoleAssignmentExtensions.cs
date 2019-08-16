using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MG.SharePoint
{
    public static class RoleAssignmentExtensions
    {
        public static void LoadAssignment(this RoleAssignment roleAss)
        {
            roleAss.Context.Load(roleAss, ra => ra.Member, ra => ra.Member.Id, ra => ra.Member.IsHiddenInUI, ra => ra.Member.LoginName,
                ra => ra.Member.PrincipalType, ra => ra.Member.Title, ra => ra.PrincipalId, ra => ra.RoleDefinitionBindings.Include(
                    rdb => rdb.Description, rdb => rdb.Hidden, rdb => rdb.Id, rdb => rdb.Name, rdb => rdb.Order, rdb => rdb.RoleTypeKind));

            try
            {
                roleAss.Context.ExecuteQuery();
            }
            catch (ServerException sex)
            {
                throw new InvalidOperationException(string.Format(
                    "An error occurred while loading this RoleAssignment's properties. -- {0}",
                    sex.Message));
            }
        }

        public static void LoadAllAssignments(this RoleAssignmentCollection roleAssCol)
        {
            roleAssCol.Context.Load(roleAssCol, rac => 
                rac.Include(
                    ra => ra.Member, ra => ra.Member.Id, ra => ra.Member.IsHiddenInUI, ra => ra.Member.LoginName,
                    ra => ra.Member.PrincipalType, ra => ra.Member.Title, ra => ra.PrincipalId, ra => ra.RoleDefinitionBindings.Include(
                        rdb => rdb.Description, rdb => rdb.Hidden, rdb => rdb.Id, rdb => rdb.Name, rdb => rdb.Order, rdb => rdb.RoleTypeKind)),
                rac => rac.Groups.Include(
                    g => g.Title, g => g.LoginName, g => g.Id));

            try
            {
                roleAssCol.Context.ExecuteQuery();
            }
            catch (ServerException sex)
            {
                throw new InvalidOperationException(string.Format(
                    "An error occurred while loading this RoleAssignment's properties. -- {0}",
                    sex.Message));
            }
        }
    }
}
