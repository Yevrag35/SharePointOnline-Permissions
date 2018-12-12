using Microsoft.SharePoint.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MG.SharePoint
{
    public abstract class SPSecurable : SPObject, ISPPermissions
    {
        public bool? HasUniquePermissions { get; protected internal set; }

        protected internal SecurableObject SecObj { get; }

        public SPPermissionCollection Permissions { get; protected internal set; }

        #region CONSTRUCTORS

        public SPSecurable(SecurableObject so)
        {
            SecObj = so;
            CTX.Lae(SecObj, true, s => s.HasUniqueRoleAssignments, s => s.RoleAssignments);
            HasUniquePermissions = !SecObj.IsPropertyAvailable("HasUniqueRoleAssignments") ?
                null : (bool?)SecObj.HasUniqueRoleAssignments;
        }

        #endregion

        #region ABSTRACT METHODS

        public abstract void Update();

        #endregion

        #region GET PERMISSIONS

        public SPPermissionCollection GetPermissions()
        {
            Permissions = SecObj.RoleAssignments;
            return Permissions;
        }

        #endregion

        #region INHERITANCE METHODS

        public bool BreakInheritance(bool copyRoleAssignments, bool clearSubscopes)
        {
            bool result = true;
            if (HasUniquePermissions.HasValue && HasUniquePermissions.Value)
                throw new InvalidBreakInheritanceException(this.Id);

            SecObj.BreakRoleInheritance(copyRoleAssignments, clearSubscopes);
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
                throw new InvalidResetInheritanceException(this.Id);

            SecObj.ResetRoleInheritance();
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

        #endregion

        #region ADD PERMISSIONS

        public void AddPermission(IDictionary permissionsHash, bool forceBreak) =>
            this.AddPermission(new SPBindingCollection(ResolvePermissions(permissionsHash)), forceBreak);

        public void AddPermission(Principal principal, RoleDefinition roleDefinition, bool forceBreak) =>
            this.AddPermission(new SPBindingCollection(principal, roleDefinition), forceBreak);

        public void AddPermission(SPBinding binding, bool forceBreak) =>
            this.AddPermission(new SPBindingCollection(binding), forceBreak);

        public void AddPermission(string logonName, string roleDefinition, bool forceBreak)
        {
            var user = CTX.SP1.Web.EnsureUser(logonName);
            CTX.Lae(user, true);
            if (CTX.AllRoles == null)
            {
                CTX.AllRoles = CTX.SP1.Web.RoleDefinitions;
                CTX.Lae(CTX.AllRoles, true,
                    ar => ar.Include(
                        r => r.Name
                    )
                );
            }
            RoleDefinition roleDef;
            try
            {
                roleDef = CTX.AllRoles.Single(x => x.Name.Equals(roleDefinition, StringComparison.InvariantCultureIgnoreCase));
            }
            catch (InvalidOperationException)
            {
                throw new ArgumentException(roleDefinition + " is not the name of a valid Role Definition in this site collection.");
            }
            this.AddPermission(new SPBindingCollection(user, roleDef), forceBreak);
        }

        public void AddPermission(SPBindingCollection bindingCol, bool forceBreak)
        {
            if (HasUniquePermissions.HasValue && !HasUniquePermissions.Value)
            {
                if (!forceBreak)
                    throw new NoForceBreakException(this.Id);
                else
                    SecObj.BreakRoleInheritance(true, true);
            }
            else if (!HasUniquePermissions.HasValue)
                throw new InvalidOperationException("The permissions for object id \"" + Convert.ToString(this.Id) + "\" cannot be modified!");

            var list = new List<RoleAssignment>(bindingCol.Count);
            for (int i = 0; i < bindingCol.Count; i++)
            {
                var binding = bindingCol[i];
                var bCol = new RoleDefinitionBindingCollection(CTX.SP1)
                {
                    binding.Definition
                };
                list.Add(SecObj.RoleAssignments.Add(
                    binding.Principal, bCol));

                foreach (var ass in list)
                {
                    CTX.Lae(ass, false);
                }
                Update();
                CTX.Lae();
            }
            if (Permissions != null)
            {
                for (int ra = 0; ra < list.Count; ra++)
                {
                    var r = list[ra];
                    CTX.Lae(r, true, ras => ras.Member.Id, ras => ras.Member.Title);
                }
                Permissions.AddRange(list);
            }
            else
                this.GetPermissions();
        }

        #endregion

        #region REMOVE PERMISSIONS

        public void RemovePermission(Principal principal)
        {
            if (!HasUniquePermissions.HasValue || (HasUniquePermissions.HasValue && !HasUniquePermissions.Value))
                throw new InvalidOperationException("This item does not contain unique permissions.  No permissions were removed.");

            var roleAss = SecObj.RoleAssignments.GetByPrincipal(principal);
            roleAss.DeleteObject();
            CTX.Lae();
        }

        public void RemovePermission(string logonName)
        {
            var principal = CTX.SP1.Web.EnsureUser(logonName);
            RemovePermission(principal);
        }

        #endregion

        #region ISPPermissionResolver Method

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
    }
}
