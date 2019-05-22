using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MG.SharePoint
{
    public class SPPermissionCollection : SPCollection<SPPermission>
    {
        //private protected List<SPPermission> _list;

        #region Constructors
        private SPPermissionCollection() : base() { }
        private SPPermissionCollection(int capacity) : base(capacity) { }
        private SPPermissionCollection(IEnumerable<SPPermission> items) : base(items) { }

        #endregion

        internal void AddRange(IEnumerable<SPPermission> items) => _list.AddRange(items);
        internal void AddRange(SPSecurable securable, IEnumerable<RoleAssignment> roleAss)
        {
            foreach (RoleAssignment ass in roleAss)
            {
                _list.Add(SPPermission.ResolvePermission(ass, securable));
            }
        }

        #region ICloneable Methods
        public object Clone()
        {
            var newList = new SPPermission[_list.Count];
            _list.CopyTo(newList, 0);
            return new SPPermissionCollection(newList);
        }

        #endregion
        #region CUSTOM METHODS
        public bool ContainsPrincipal(User user, out SPPermissionCollection permissionGroups) =>
            this.ContainsPrincipal((SPUser)user, out permissionGroups);

        public bool ContainsPrincipal(SPUser user, out SPPermissionCollection permissionGroups)
        {
            bool result = false;
            if (user.Groups == null)
                user.LoadProperty("Groups");

            var roleDefs = new SPPermissionCollection();
            for (int i = 0; i < _list.Count; i++)
            {
                var spp = _list[i];
                if ((spp.Type == PrincipalType.User && spp.LoginName.Equals(user.LoginName)) ||
                    (spp.Type == PrincipalType.SecurityGroup || spp.Type == PrincipalType.SharePointGroup &&
                    user.Groups.ContainsGroupByLoginName(spp.LoginName)))
                {
                    result = true;
                    roleDefs._list.Add(spp);
                }
            }
            permissionGroups = roleDefs;
            return result;
        }

        #endregion

        #region Operators
        public static SPPermissionCollection ResolvePermissions(SPSecurable securable)
        {
            var roleAss = securable.SecObj.RoleAssignments;
            if (!roleAss.AreItemsAvailable)
            {
                CTX.Lae(roleAss, true, col => col.Include(
                    ass => ass.Member, ass => ass.RoleDefinitionBindings.Include(
                        d => d.Name, d => d.Description)));
            }

            var permCol = new SPPermissionCollection(roleAss.Count);
            foreach (var ass in roleAss)
            {
                permCol._list.Add(SPPermission.ResolvePermission(ass, securable));
            }
            return permCol;
        }

        //public static explicit operator SPPermissionCollection(RoleAssignmentCollection assCol)
        //{
        //    CTX.Lae(assCol, true,
        //        rCol => rCol.Include(
        //            ass => ass.Member, ass => ass.RoleDefinitionBindings.Include(
        //                d => d.Name, d => d.Description
        //            )
        //        )
        //    );
        //    var permCol = new SPPermissionCollection(assCol.Count);
        //    for (int i = 0; i < assCol.Count; i++)
        //    {
        //        SPPermission p = assCol[i];
        //        permCol.Add(p);
        //    }
        //    return permCol;
        //}

        #endregion
    }
}
