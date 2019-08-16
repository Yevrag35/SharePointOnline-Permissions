using Microsoft.SharePoint.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MG.SharePoint
{
    public class SPPermissionCollection : BaseSPCollection, IEnumerable<SPPermission> //ICollection<SPPermission>
    {
        #region FIELDS/CONSTANTS
        private List<SPPermission> _list;

        #endregion

        #region PROPERTIES
        public override int Count => _list.Count;
        public bool IsReadOnly => false;
        public override object SyncRoot => ((ICollection)_list).SyncRoot;

        #endregion

        #region CONSTRUCTORS
        public SPPermissionCollection()
            : this(CTX.SP1) { }
        public SPPermissionCollection(ClientContext ctx)
            : base(ctx) => _list = new List<SPPermission>();
        public SPPermissionCollection(int capacity)
            : this(capacity, CTX.SP1) { }
        public SPPermissionCollection(int capacity, ClientContext ctx)
            : base(ctx) => _list = new List<SPPermission>(capacity);
        public SPPermissionCollection(IEnumerable<SPPermission> objs)
            : this(objs, CTX.SP1) { }
        public SPPermissionCollection(IEnumerable<SPPermission> objs, ClientContext ctx)
            : base(ctx) => _list = new List<SPPermission>(objs);

        private SPPermissionCollection(RoleAssignmentCollection roleAssCol)
            : base((ClientContext)roleAssCol.Context)
        {
            _list = new List<SPPermission>(roleAssCol.Count);
            for (int i = 0; i < roleAssCol.Count; i++)
            {
                _list.Add(new SPPermission(roleAssCol[i]));
            }
        }

        #endregion

        #region INDEXERS
        public SPPermission this[int index] => _list[index];

        #endregion

        #region COLLECTION METHODS
        //void ICollection<SPPermission>.Add(SPPermission item) => throw new NotImplementedException("This may be available in future releases.");
        //void ICollection<SPPermission>.Clear() => throw new NotImplementedException();
        //bool ICollection<SPPermission>.Contains(SPPermission item) => _list.Contains(item);
        public override void CopyTo(Array array, int index) => ((ICollection)_list).CopyTo(array, index);
        //void ICollection<SPPermission>.CopyTo(SPPermission[] array, int arrayIndex) => ((ICollection<SPPermission>)this._list).CopyTo(array, arrayIndex);
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_list).GetEnumerator();
        IEnumerator<SPPermission> IEnumerable<SPPermission>.GetEnumerator() => ((IEnumerable<SPPermission>)_list).GetEnumerator();
        public override IEnumerator GetEnumerator() => _list.GetEnumerator();
        //bool ICollection<SPPermission>.Remove(SPPermission item) => throw new NotImplementedException("This may be available in future releases.");
        public override void Sort() => _list.Sort();
        public void Sort(IComparer<SPPermission> comparer) => _list.Sort(comparer);

        #endregion

        #region CUSTOM METHODS

        public static SPPermissionCollection ResolvePermissions(SecurableObject securableObject)
        {
            RoleAssignmentCollection roleAssCol = securableObject.RoleAssignments;
            if (!roleAssCol.AreItemsAvailable)
            {
                roleAssCol.LoadAllAssignments();
            }

            var permCol = new SPPermissionCollection(roleAssCol);
            return permCol;
        }

        #endregion
    }
}