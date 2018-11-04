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
        public SPListCollection()
        {
            _col = new List<SPList>();
            IsReadOnly = false;
        }

        public SPListCollection(int capacity)
        {
            _col = new List<SPList>(capacity);
            IsReadOnly = false;
        }
            
        public SPListCollection(IEnumerable<SPList> lists)
        {
            _col = new List<SPList>(lists);
            IsReadOnly = false;
        }

        public SPListCollection(SPList list)
            : this(((IEnumerable)list).Cast<SPList>())
        {
        }

        #endregion

        #region IList and ICollection Methods

        public SPList this[int index]
        {
            get => _col[index];
            set
            {
                if (!IsReadOnly)
                    _col[index] = value;
            }
        }

        public int Count => _col.Count;
        public bool IsReadOnly { get; internal set; }
        public object SyncRoot => ((ICollection)_col).SyncRoot;
        public bool IsSynchronized => ((ICollection)_col).IsSynchronized;

        public void Add(SPList item)
        {
            if (!IsReadOnly)
                _col.Add(item);
            else
                throw new ReadOnlyCollectionException();
        }

        public void Clear()
        {
            if (!IsReadOnly)
                _col.Clear();
            else
                throw new ReadOnlyCollectionException();
        }

        public bool Contains(SPList item) => _col.Contains(item);

        public void CopyTo(SPList[] array, int arrayIndex)
        {
            if (!IsReadOnly)
                _col.CopyTo(array, arrayIndex);
            else
                throw new ReadOnlyCollectionException();
        }
        public void CopyTo(Array array, int index)
        {
            if (!IsReadOnly)
                ((ICollection)_col).CopyTo(array, index);
            else
                throw new ReadOnlyCollectionException();
        }

        public IEnumerator<SPList> GetEnumerator() => _col.GetEnumerator();

        public int IndexOf(SPList item) => _col.IndexOf(item);

        public void Insert(int index, SPList item)
        {
            if (!IsReadOnly)
                _col.Insert(index, item);
            else
                throw new ReadOnlyCollectionException();
        }

        public bool Remove(SPList item)
        {
            if (!IsReadOnly)
                return _col.Remove(item);
            else
                throw new ReadOnlyCollectionException();
        }

        public void RemoveAt(int index)
        {
            if (!IsReadOnly)
                _col.RemoveAt(index);
            else
                throw new ReadOnlyCollectionException();
        }

        IEnumerator IEnumerable.GetEnumerator() => _col.GetEnumerator();

        #endregion

        #region Other 'List' Methods
        public SPList[] ToArray() =>
            _col.ToArray();

        public void AddRange(IEnumerable<SPList> lists) =>
            _col.AddRange(lists);

        public bool TrueForAll(Predicate<SPList> match) =>
            _col.TrueForAll(match);

        public bool Exists(Predicate<SPList> match) =>
            _col.Exists(match);

        public ReadOnlyCollection<SPList> AsReadOnly() =>
            _col.AsReadOnly();

        public void Sort() => _col.Sort();
        public void Sort(Comparison<SPList> comparison) =>
            _col.Sort(comparison);

        public void Sort(IComparer<SPList> comparer) =>
            _col.Sort(comparer);

        public void RemoveAll(Predicate<SPList> match)
        {
            if (!IsReadOnly)
                _col.RemoveAll(match);
            else
                throw new ReadOnlyCollectionException();
        }

        #endregion

        #region Dictionary Indexing
        public SPList this[string listName]
        {
            get
            {
                SPList foundya = null;
                for (int i = 0; i < _col.Count; i++)
                {
                    var l = _col[i];
                    if (string.Equals(l.Name, listName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        foundya = l;
                        break;
                    }
                }
                return foundya;
            }
        }

        #endregion

        #region Operators

        public static explicit operator SPListCollection(ListCollection listCol)
        {
            var spListCol = new SPListCollection(listCol.Count);
            for (int i = 0; i < listCol.Count; i++)
            {
                var list = listCol[i];
                spListCol.Add((SPList)list);              
            }
            spListCol.IsReadOnly = true;
            return spListCol;
        }

        #endregion
    }
}
