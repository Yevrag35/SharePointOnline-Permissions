using Microsoft.SharePoint.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MG.SharePoint
{
    public class SPListItemCollection : IList<SPListItem>, ICollection
    {
        private protected List<SPListItem> _col;

        #region Constructors
        public SPListItemCollection()
        {
            _col = new List<SPListItem>();
            IsReadOnly = false;
        }

        public SPListItemCollection(int capacity)
        {
            _col = new List<SPListItem>(capacity);
            IsReadOnly = false;
        }

        public SPListItemCollection(IEnumerable<SPListItem> lists)
        {
            _col = new List<SPListItem>(lists);
            IsReadOnly = false;
        }

        public SPListItemCollection(SPListItem list)
            : this(((IEnumerable)list).Cast<SPListItem>())
        {
        }

        #endregion

        #region IList and ICollection Methods

        public SPListItem this[int index]
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

        public void Add(SPListItem item)
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

        public bool Contains(SPListItem item) => _col.Contains(item);

        public void CopyTo(SPListItem[] array, int arrayIndex)
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

        public IEnumerator<SPListItem> GetEnumerator() => _col.GetEnumerator();

        public int IndexOf(SPListItem item) => _col.IndexOf(item);

        public void Insert(int index, SPListItem item)
        {
            if (!IsReadOnly)
                _col.Insert(index, item);
            else
                throw new ReadOnlyCollectionException();
        }

        public bool Remove(SPListItem item)
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
        public SPListItem[] ToArray() =>
            _col.ToArray();

        public void AddRange(IEnumerable<SPListItem> lists) =>
            _col.AddRange(lists);

        public bool TrueForAll(Predicate<SPListItem> match) =>
            _col.TrueForAll(match);

        public bool Exists(Predicate<SPListItem> match) =>
            _col.Exists(match);

        public ReadOnlyCollection<SPListItem> AsReadOnly() =>
            _col.AsReadOnly();

        public void Sort() => _col.Sort();
        public void Sort(Comparison<SPListItem> comparison) =>
            _col.Sort(comparison);

        public void Sort(IComparer<SPListItem> comparer) =>
            _col.Sort(comparer);

        public void RemoveAll(Predicate<SPListItem> match)
        {
            if (!IsReadOnly)
                _col.RemoveAll(match);
            else
                throw new ReadOnlyCollectionException();
        }

        #endregion

        #region Dictionary Indexing
        public SPListItem this[string listItemName]
        {
            get
            {
                SPListItem foundya = null;
                for (int i = 0; i < _col.Count; i++)
                {
                    var l = _col[i];
                    if (string.Equals(l.Name, listItemName, StringComparison.InvariantCultureIgnoreCase))
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
        public static explicit operator SPListItemCollection(ListItemCollection listItemCol)
        {
            CTX.Lae(listItemCol, true,
                col => col.Include(
                    l => l.DisplayName, l => l.Id,
                    l => l.HasUniqueRoleAssignments
                )
            );
            var spList = new SPListItemCollection(listItemCol.Count);
            for (int i = 0; i < listItemCol.Count; i++)
            {
                var item = listItemCol[i];
                spList.Add((SPListItem)item);
            }
            return spList;
        }

        #endregion
    }
}
