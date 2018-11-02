using Microsoft.SharePoint.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MG.SharePoint
{
    public class SPListCollection : IList<SPList>, ICollection
    {
        private protected List<SPList> _col;

        #region Constructors
        public SPListCollection() =>
            _col = new List<SPList>();

        public SPListCollection(int capacity) =>
            _col = new List<SPList>(capacity);

        public SPListCollection(IEnumerable<SPList> lists) =>
            _col = new List<SPList>(lists);

        public SPListCollection(SPList list)
            : this(((IEnumerable)list).Cast<SPList>())
        {
        }

        #endregion

        #region IList and ICollection Methods

        public SPList this[int index]
        {
            get => _col[index];
            set => _col[index] = value;
        }

        public int Count => _col.Count;
        public bool IsReadOnly => false;
        public object SyncRoot => ((ICollection)_col).SyncRoot;
        public bool IsSynchronized => ((ICollection)_col).IsSynchronized;

        public void Add(SPList item) => _col.Add(item);

        public void Clear() => _col.Clear();

        public bool Contains(SPList item) => _col.Contains(item);

        public void CopyTo(SPList[] array, int arrayIndex) => 
            _col.CopyTo(array, arrayIndex);
        public void CopyTo(Array array, int index) =>
            ((ICollection)_col).CopyTo(array, index);

        public IEnumerator<SPList> GetEnumerator() => _col.GetEnumerator();

        public int IndexOf(SPList item) => _col.IndexOf(item);

        public void Insert(int index, SPList item) => _col.Insert(index, item);

        public bool Remove(SPList item) => _col.Remove(item);

        public void RemoveAt(int index) => _col.RemoveAt(index);

        IEnumerator IEnumerable.GetEnumerator() => _col.GetEnumerator();

        #endregion

        #region Other 'List' Methods
        public void AddRange(IEnumerable<SPList> lists) =>
            _col.AddRange(lists);

        #endregion

        #region Operators

        //public static implicit operator SPListCollection(ListCollection listCol)
        //{
        //    CTX.Lae(listCol, true, 
        //        lCol => lCol.Include(
        //            l => l.))
        //}

        #endregion
    }
}
