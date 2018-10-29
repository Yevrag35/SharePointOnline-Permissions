using Microsoft.SharePoint.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MG.SharePoint
{
    public partial class SPFolder : ISPObject
    {
        #region Permission Methods
        public SPPermissionCollection GetPermissions()
        {
            SPPermissionCollection permCol = _fol.ListItemAllFields.RoleAssignments;
            _perms = permCol;
            return permCol;
        }

        public bool BreakInheritance(bool copyRoleAssignments, bool clearSubscopes = true)
        {
            bool result = true;
            if (HasUniquePermissions.HasValue && HasUniquePermissions.Value)
                throw new InvalidBreakInheritanceException(_fol.UniqueId);

            _fol.ListItemAllFields.BreakRoleInheritance(copyRoleAssignments, clearSubscopes);
            try
            {
                CTX.Lae();
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public bool ResetInheritance()
        {
            bool result = true;
            if (!HasUniquePermissions.HasValue || (HasUniquePermissions.HasValue && !HasUniquePermissions.Value))
                throw new InvalidResetInheritanceException(_fol.UniqueId);

            _fol.ListItemAllFields.ResetRoleInheritance();
            try
            {
                CTX.Lae();
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public void AddFolderPermission(SPBindingCollection bindingCol, bool forceBreak = false)
        {
            if (HasUniquePermissions.HasValue && !HasUniquePermissions.Value)
            {
                if (!forceBreak)
                    throw new NoForceBreakException(_fol.UniqueId);
                else
                    _fol.ListItemAllFields.BreakRoleInheritance(true, true);
            }
            else if (!HasUniquePermissions.HasValue)
                throw new InvalidOperationException("This object's permissions cannot be modified!");

            var list = new List<RoleAssignment>(bindingCol.Count);
            for (int i = 0; i < bindingCol.Count; i++)
            {
                var binding = bindingCol[i];
                var bCol = new RoleDefinitionBindingCollection(CTX.SP1)
                {
                    binding.Definition
                };
                list.Add(_fol.ListItemAllFields.RoleAssignments.Add(
                    binding.Principal, bCol));
                foreach (var ass in list)
                {
                    CTX.Lae(ass, false);
                }
                _fol.Update();
                CTX.Lae();
            }
            if (_perms != null)
                _perms.AddRange(list);
            else
                this.GetPermissions();
        }

        public void AddFolderPermission(SPBinding binding, bool forceBreak = false) =>
            AddFolderPermission(new SPBindingCollection(binding), forceBreak);

        public void AddFolderPermission(Principal principal, RoleDefinition roleDef, bool forceBreak = false) =>
            AddFolderPermission(new SPBindingCollection(principal, roleDef), forceBreak);

        public void AddFolderPermission(string logonName, string roleDefinition, bool forceBreak = false)
        {
            var user = CTX.SP1.Web.EnsureUser(logonName);
            CTX.Lae(user);
            var allRoles = CTX.SP1.Web.RoleDefinitions;
            CTX.Lae(allRoles, true,
                ar => ar.Include(
                    r => r.Name
                )
            );
            var roleDef = allRoles.Where(x => string.Equals(x.Name, roleDefinition, StringComparison.OrdinalIgnoreCase)).Single();
            AddFolderPermission(new SPBindingCollection(user, roleDef), forceBreak);
        }
        #endregion
    }
}
