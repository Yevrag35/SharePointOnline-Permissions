using Microsoft.SharePoint.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MG.SharePoint
{
    public class SPPermissionCollection : BaseSPCollection, IEnumerable<SPPermission>
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

        #endregion

        #region INDEXERS
        public SPPermission this[int index] => _list[index];

        #endregion

        #region METHODS
        public override void CopyTo(Array array, int index) => ((ICollection)_list).CopyTo(array, index);
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_list).GetEnumerator();
        IEnumerator<SPPermission> IEnumerable<SPPermission>.GetEnumerator() => ((IEnumerable<SPPermission>)_list).GetEnumerator();
        public override IEnumerator GetEnumerator() => _list.GetEnumerator();
        public override void Sort() => _list.Sort();
        public void Sort(IComparer<SPPermission> comparer) => _list.Sort(comparer);

        #endregion
    }
}