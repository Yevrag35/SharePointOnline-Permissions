using Microsoft.SharePoint.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MG.SharePoint
{
    public class SPFolderCollection : IList<SPFolder>, ICollection
    {
        private protected List<SPFolder> _col;

        #region Constructors
        public SPFolderCollection()
        {
            _col = new List<SPFolder>();
            IsReadOnly = false;
        }

        public SPFolderCollection(int capacity)
        {
            _col = new List<SPFolder>(capacity);
            IsReadOnly = false;
        }

        public SPFolderCollection(IEnumerable<SPFolder> lists)
        {
            _col = new List<SPFolder>(lists);
            IsReadOnly = false;
        }

        public SPFolderCollection(SPFolder list)
            : this(((IEnumerable)list).Cast<SPFolder>())
        {
        }

        #endregion

        #region IList and ICollection Methods

        public SPFolder this[int index]
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

        public void Add(SPFolder item)
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

        public bool Contains(SPFolder item) => _col.Contains(item);

        public void CopyTo(SPFolder[] array, int arrayIndex)
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

        public IEnumerator<SPFolder> GetEnumerator() => _col.GetEnumerator();

        public int IndexOf(SPFolder item) => _col.IndexOf(item);

        public void Insert(int index, SPFolder item)
        {
            if (!IsReadOnly)
                _col.Insert(index, item);
            else
                throw new ReadOnlyCollectionException();
        }

        public bool Remove(SPFolder item)
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
        public SPFolder[] ToArray() =>
            _col.ToArray();

        public void AddRange(IEnumerable<SPFolder> lists) =>
            _col.AddRange(lists);

        public bool TrueForAll(Predicate<SPFolder> match) =>
            _col.TrueForAll(match);

        public bool Exists(Predicate<SPFolder> match) =>
            _col.Exists(match);

        public ReadOnlyCollection<SPFolder> AsReadOnly() =>
            _col.AsReadOnly();

        public void Sort() => _col.Sort();
        public void Sort(Comparison<SPFolder> comparison) =>
            _col.Sort(comparison);

        public void Sort(IComparer<SPFolder> comparer) =>
            _col.Sort(comparer);

        public void RemoveAll(Predicate<SPFolder> match)
        {
            if (!IsReadOnly)
                _col.RemoveAll(match);
            else
                throw new ReadOnlyCollectionException();
        }

        #endregion

        #region Dictionary Indexing
        public SPFolder this[string folderName]
        {
            get
            {
                SPFolder foundya = null;
                for (int i = 0; i < _col.Count; i++)
                {
                    var l = _col[i];
                    if (string.Equals(l.Name, folderName, StringComparison.InvariantCultureIgnoreCase))
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
        public static explicit operator SPFolderCollection(FolderCollection folCol)
        {
            var spList = new SPFolderCollection(folCol.Count);
            for (int i = 0; i < folCol.Count; i++)
            {
                var fol = folCol[i];
                spList.Add((SPFolder)fol);
            }
            return spList;
        }

        #endregion
    }
}
