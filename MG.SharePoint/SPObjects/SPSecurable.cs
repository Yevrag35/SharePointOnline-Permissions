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
        private bool _pol = true;

        public bool? HasUniquePermissions { get; protected internal set; }
        public bool CanSetPermissions => _pol;

        protected internal SecurableObject SecObj { get; }

        public SPPermissionCollection Permissions { get; protected internal set; }

        #region CONSTRUCTORS

        public SPSecurable(SecurableObject so)
        {
            SecObj = so;
            try
            {
                CTX.Lae(SecObj, true, s => s.HasUniqueRoleAssignments, s => s.RoleAssignments);
            }
            catch (ServerException ex)
            {
                if (ex.Message.Contains("does not belong to a list."))
                    _pol = false;
            }

            HasUniquePermissions = !SecObj.IsPropertyAvailable("HasUniqueRoleAssignments") ?
                null : (bool?)SecObj.HasUniqueRoleAssignments;

            if (!this.HasUniquePermissions.HasValue)
                _pol = false;
        }

        #endregion

        #region ABSTRACT METHODS

        public abstract void Update();

        #endregion

        #region CHECK PERMISSIONS
        //public SPPermissionCollection GetUserPermissions(string userId)
        //{
        //    if (this.CanSetPermissions)
        //    {
        //        var user = new SPUser(userId, true);
        //        if (this.Permissions == null)
        //            this.GetPermissions();

        //        if (this.Permissions)
        //    }
        //    else
        //        return null;
        //}

        #endregion

        #region GET PERMISSIONS

        public SPPermissionCollection GetPermissions()
        {

            //this.SecObj.
            if (!SecObj.IsPropertyReady(x => x.RoleAssignments))
                CTX.Lae(SecObj, true, s => s.RoleAssignments);

            Type secType = SecObj.GetType();
            MethodInfo genMeth = ExpressionMethod.MakeGenericMethod(secType);
            object expressions = genMeth.Invoke(this, new object[1] { new string[2] { NameProperty, IdProperty }});

            MethodInfo specLae = typeof(CTX).GetMethod("SpecialLae", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(SecObj.GetType());
            specLae.Invoke(null, new object[3] { SecObj, true, expressions });

            Permissions = SPPermissionCollection.ResolvePermissions(this);
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

        public void AddPermission(IDictionary permissionsHash, bool forceBreak, bool permissionsApplyRecursively) =>
            this.AddPermission(new SPBindingCollection(((ISPPermissionResolver)this).ResolvePermissions(permissionsHash)), forceBreak, permissionsApplyRecursively);

        public void AddPermission(Principal principal, RoleDefinition roleDefinition, bool forceBreak, bool permissionsApplyRecursively) =>
            this.AddPermission(new SPBindingCollection(principal, roleDefinition), forceBreak, permissionsApplyRecursively);

        public void AddPermission(SPBinding binding, bool forceBreak, bool permissionsApplyRecursively) =>
            this.AddPermission(new SPBindingCollection(binding), forceBreak, permissionsApplyRecursively);

        public void AddPermission(string logonName, string roleDefinition, bool forceBreak, bool permissionsApplyRecursively)
        {
            User user = CTX.SP1.Web.EnsureUser(logonName);
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
            this.AddPermission(new SPBindingCollection(user, roleDef), forceBreak, permissionsApplyRecursively);
        }

        public void AddPermission(SPBindingCollection bindingCol, bool forceBreak, bool permissionsApplyRecursively)
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
                SPBinding binding = bindingCol[i];
                var bCol = new RoleDefinitionBindingCollection(CTX.SP1)
                {
                    binding.Definition
                };
                list.Add(SecObj.RoleAssignments.Add(
                    binding.Principal, bCol));

                foreach (RoleAssignment ass in list)
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
                    RoleAssignment r = list[ra];
                    CTX.Lae(r, true, ras => ras.Member.Id, ras => ras.Member.Title);
                }
                Permissions.AddRange(this, list);
            }
            else
                this.GetPermissions();

            //if ()
        }

        #endregion

        #region REMOVE PERMISSIONS

        public void RemovePermission(Principal principal)
        {
            if (!HasUniquePermissions.HasValue || (HasUniquePermissions.HasValue && !HasUniquePermissions.Value))
                throw new InvalidOperationException("This item does not contain unique permissions.  No permissions were removed.");

            RoleAssignment roleAss = SecObj.RoleAssignments.GetByPrincipal(principal);
            roleAss.DeleteObject();
            CTX.Lae();
        }

        public void RemovePermission(string logonName)
        {
            User principal = CTX.SP1.Web.EnsureUser(logonName);
            RemovePermission(principal);
        }

        #endregion

        #region ISPPermissionResolver Method

        IEnumerable<SPBinding> ISPPermissionResolver.ResolvePermissions(IDictionary permissions)
        {
            string[] keys = permissions.Keys.Cast<string>().ToArray();
            var bindingCol = new SPBindingCollection();
            for (int i = 0; i < keys.Length; i++)
            {
                string key = keys[i];
                object prins = permissions[key];
                string role = Convert.ToString(key);
                string[] allPrins;
                if (!prins.GetType().IsArray)
                    bindingCol.Add(new SPBinding(Convert.ToString(prins), role));
                else
                {
                    allPrins = ((IEnumerable)prins).Cast<string>().ToArray();
                    for (int p = 0; p < allPrins.Length; p++)
                    {
                        string prin = allPrins[p];
                        bindingCol.Add(new SPBinding(prin, role));
                    }
                }
            }
            return bindingCol;
        }

        #endregion

        #region RECURSIVE PERMISSIONING

        private const string RECURSE_CAML_QUERY = @"<View Scope=""RecursiveAll"">
    <Query>
       <Where>
          <IsNotNull>
             <FieldRef Name = 'SharedWithUsers' />
          </ IsNotNull >
       </ Where >
       < OrderBy >
          < FieldRef Name='SharedWithUsers' Ascending='True' />
       </OrderBy>
    </Query>
</View>";

        private void AddPermissionRecursively(List list, SPBindingCollection bindCol)
        {
            var query = new CamlQuery()
            {
                ViewXml = RECURSE_CAML_QUERY
            };
            var lic = (SPListItemCollection)list.GetItems(query);
            
            for (int i = 0; i < lic.Count; i++)
            {
                SPListItem li = lic[i];
                li.AddPermission(bindCol, true, false);
            }
        }

        private void AddPermissionRecursively(SPBindingCollection bindCol)
        {
            string thisType = this.GetType().Name;
            switch (thisType)
            {
                case "SPFolder":
                    AddPermissionRecursively((SPFolder)this, bindCol);
                    break;
                case "SPFile":
                    AddPermissionRecursively((SPFile)this, bindCol);
                    break;
                default:
                    throw new InvalidOperationException("What the fuck?");
            }
        }

        private void AddPermissionRecursively(SPFile file, SPBindingCollection bindCol) =>
            this.AddPermissionRecursively(((File)file.ShowOriginal()).ListItemAllFields.ParentList, bindCol);

        private void AddPermissionRecursively(SPFolder fol, SPBindingCollection bindCol) =>
            this.AddPermissionRecursively(((Folder)fol.ShowOriginal()).ListItemAllFields.ParentList, bindCol);

        #endregion
    }
}
