using Microsoft.SharePoint.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MG.SharePoint
{
    public class SPWebCollection : IList<SPWeb>, ICollection
    {
        private protected List<SPWeb> _col;

        #region Constructors
        public SPWebCollection()
        {
            _col = new List<SPWeb>();
            IsReadOnly = false;
        }

        public SPWebCollection(int capacity)
        {
            _col = new List<SPWeb>(capacity);
            IsReadOnly = false;
        }

        public SPWebCollection(IEnumerable<SPWeb> lists)
        {
            _col = new List<SPWeb>(lists);
            IsReadOnly = false;
        }

        public SPWebCollection(SPWeb list)
            : this(((IEnumerable)list).Cast<SPWeb>())
        {
        }

        #endregion

        #region IList and ICollection Methods

        public SPWeb this[int index]
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

        public void Add(SPWeb item)
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

        public bool Contains(SPWeb item) => _col.Contains(item);

        public void CopyTo(SPWeb[] array, int arrayIndex)
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

        public IEnumerator<SPWeb> GetEnumerator() => _col.GetEnumerator();

        public int IndexOf(SPWeb item) => _col.IndexOf(item);

        public void Insert(int index, SPWeb item)
        {
            if (!IsReadOnly)
                _col.Insert(index, item);
            else
                throw new ReadOnlyCollectionException();
        }

        public bool Remove(SPWeb item)
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
        public SPWeb[] ToArray() =>
            _col.ToArray();

        public void AddRange(IEnumerable<SPWeb> lists) =>
            _col.AddRange(lists);

        public bool TrueForAll(Predicate<SPWeb> match) =>
            _col.TrueForAll(match);

        public bool Exists(Predicate<SPWeb> match) =>
            _col.Exists(match);

        public ReadOnlyCollection<SPWeb> AsReadOnly() =>
            _col.AsReadOnly();

        public void Sort() => _col.Sort();
        public void Sort(Comparison<SPWeb> comparison) =>
            _col.Sort(comparison);

        public void Sort(IComparer<SPWeb> comparer) =>
            _col.Sort(comparer);

        public void RemoveAll(Predicate<SPWeb> match)
        {
            if (!IsReadOnly)
                _col.RemoveAll(match);
            else
                throw new ReadOnlyCollectionException();
        }

        #endregion

        #region Dictionary Indexing
        public SPWeb this[string webTitle]
        {
            get
            {
                SPWeb foundya = null;
                for (int i = 0; i < _col.Count; i++)
                {
                    SPWeb l = _col[i];
                    if (string.Equals(l.Name, webTitle, StringComparison.InvariantCultureIgnoreCase))
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
        public static explicit operator SPWebCollection(WebCollection webCol)
        {
            var spWeb = new SPWebCollection(webCol.Count);
            for (int i = 0; i < webCol.Count; i++)
            {
                Web web = webCol[i];
                spWeb.Add((SPWeb)web);
            }
            spWeb.IsReadOnly = true;
            return spWeb;
        }

        #endregion
    }
}
