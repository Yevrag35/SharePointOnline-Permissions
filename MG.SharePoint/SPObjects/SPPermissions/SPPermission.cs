using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MG.SharePoint
{
    public class SPPermission
    {
        #region FIELDS/CONSTANTS

        #endregion

        #region PROPERTIES
        public object Id { get; }
        public string LoginName { get; }
        public string Name { get; }
        public string[] Permissions { get; }
        public int PrincipalId { get; }
        public PrincipalType Type { get; }

        #endregion

        #region CONSTRUCTORS
        public SPPermission(RoleAssignment ass, bool andLoad = true)
        {
            if (andLoad)
            {
                ass.LoadAssignment();
            }

            this.Name = ass.Member.Title;
            this.Id = ass.Member.Id;
            this.LoginName = ass.Member.LoginName;
            this.PrincipalId = ass.PrincipalId;
            this.Type = ass.Member.PrincipalType;
            this.Permissions = this.ParseBindings(ass.RoleDefinitionBindings);
        }

        #endregion

        #region METHODS
        private string[] ParseBindings(RoleDefinitionBindingCollection bindingCol)
        {
            string[] strPerms = new string[bindingCol.Count];
            for (int i = 0; i < bindingCol.Count; i++)
            {
                RoleDefinition bind = bindingCol[i];
                strPerms[i] = bind.Name;
            }
            return strPerms;
        }

        #endregion
    }

    //public class PermissionComparer : IComparer<SPPermission>
    //{
    //    public int Compare(SPPermission x, SPPermission y) => x.
    //}
}