using Microsoft.SharePoint.Client;
using System;
using System.Collections;

namespace MG.SharePoint
{
    public interface ISPPermissions : ISPPermissionResolver, ISPObject
    {
        SPPermissionCollection Permissions { get; }
        bool? HasUniquePermissions { get; }

        SPPermissionCollection GetPermissions();

        bool ResetInheritance();

        void AddPermission(string principal, string roleDefinition, bool forceBreak, bool permissionsApplyRecursively);
        void AddPermission(SPBinding binding, bool forceBreak, bool permissionsApplyRecursively);
        void AddPermission(Principal principal, RoleDefinition roleDefinition, bool forceBreak, bool permissionsApplyRecursively);
        void AddPermission(SPBindingCollection bindingCol, bool forceBreak, bool permissionsApplyRecursively);
        void AddPermission(IDictionary permissionHash, bool forceBreak, bool permissionsApplyRecursively);

        void RemovePermission(Principal principal);
        void RemovePermission(string logonName);
    }
}
