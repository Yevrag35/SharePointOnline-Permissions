﻿using Microsoft.SharePoint.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MG.SharePoint
{
    public partial class SPWeb : SPObject, ISPPermissions
    {
        #region ISPPermissions
        public SPPermissionCollection Permissions { get; internal set; }

        public SPPermissionCollection GetPermissions()
        {
            Permissions = _web.RoleAssignments;
            return Permissions;
        }

        #region AddPermission
        public void AddPermission(string principal, string roleDefinition, bool forceBreak = false)
        {
            var user = _web.EnsureUser(principal);
            CTX.Lae(user);
            if (CTX.AllRoles == null)
            {
                CTX.AllRoles = _web.RoleDefinitions;
                CTX.Lae(CTX.AllRoles, true,
                    ar => ar.Include(
                        r => r.Name
                    )
                );
            }
            var roleDef = CTX.AllRoles.Single(r => string.Equals(r.Name, roleDefinition, StringComparison.OrdinalIgnoreCase));
            AddPermission(new SPBindingCollection(user, roleDef), forceBreak);
        }

        public void AddPermission(SPBinding binding, bool forceBreak = false) =>
            AddPermission(new SPBindingCollection(binding), forceBreak);

        public void AddPermission(Principal principal, RoleDefinition roleDefinition, bool forceBreak = false) =>
            AddPermission(new SPBindingCollection(principal, roleDefinition), forceBreak);

        public void AddPermission(IDictionary permissionHash, bool forceBreak = false) =>
            AddPermission(new SPBindingCollection(ResolvePermissions(permissionHash)), forceBreak);

        public void AddPermission(SPBindingCollection bindingCol, bool forceBreak = false)
        {
            // This is the main "AddPermission" method that the other methods call.
            if (HasUniquePermissions.HasValue && !HasUniquePermissions.Value)
            {
                if (!forceBreak)
                    throw new NoForceBreakException(_web.Id);
                else
                    _web.BreakRoleInheritance(true, true);
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
                list.Add(_web.RoleAssignments.Add(
                    binding.Principal, bCol));
                foreach (var ass in list)
                {
                    CTX.Lae(ass, false);
                }
                _web.Update();
                CTX.Lae();
            }
            if (Permissions == null)
                this.GetPermissions();
            else
                Permissions.AddRange(list);
        }

        #endregion

        #region ResolvePermissions
        public IEnumerable<SPBinding> ResolvePermissions(IDictionary permissions)
        {
            var keys = permissions.Keys.Cast<string>().ToArray();
            var bindingCol = new SPBindingCollection();
            for (int i = 0; i < keys.Length; i++)
            {
                var key = keys[i];
                var prins = permissions[key];
                var role = Convert.ToString(key);
                string[] allPrins;
                if (!prins.GetType().IsArray)
                    bindingCol.Add(new SPBinding(Convert.ToString(prins), role));
                else
                {
                    allPrins = ((IEnumerable)prins).Cast<string>().ToArray();
                    for (int p = 0; p < allPrins.Length; p++)
                    {
                        var prin = allPrins[p];
                        bindingCol.Add(new SPBinding(prin, role));
                    }
                }
            }
            return bindingCol;
        }

        #endregion

        #endregion
    }
}
