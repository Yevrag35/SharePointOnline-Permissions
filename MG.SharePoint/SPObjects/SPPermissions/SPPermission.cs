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
        private static readonly Expression<Func<RoleAssignment, object>>[] PROPS = new Expression<Func<RoleAssignment, object>>[6]
        {
            x => x.Member.Id, x => x.Member.LoginName, x => x.Member.PrincipalType, x => x.Member.Title,
            x => x.PrincipalId, x => x.RoleDefinitionBindings
        };

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
        public SPPermission(RoleAssignment ass)
        {
            if (!ass.IsPropertyReady(PROPS))
            {
                ass.Context.Load(ass, PROPS);
                ass.Context.ExecuteQuery();
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
}