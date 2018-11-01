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
        void AddPermission(string principal, string roleDefinition, bool forceBreak);
        void AddPermission(SPBinding binding, bool forceBreak);
        void AddPermission(Principal principal, RoleDefinition roleDefinition, bool forceBreak);
        void AddPermission(SPBindingCollection bindingCol, bool forceBreak);
        void AddPermission(IDictionary permissionHash, bool forceBreak);
    }
}
