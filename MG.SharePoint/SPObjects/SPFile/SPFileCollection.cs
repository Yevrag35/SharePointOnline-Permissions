using Microsoft.SharePoint.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MG.SharePoint
{
    public partial class SPFileCollection : IList<SPFile>, ICollection
    {
        private protected List<SPFile> _col;
        protected internal FileCollection Original;     // Just used for its methods

        public string FolderPath =>
            _col.Count > 0 ?
                _col[0].ServerRelativeUrl.Substring(
                    0, _col[0].ServerRelativeUrl.LastIndexOf("/")) :
                null;

        #region Constructors
        public SPFileCollection()
        {
            _col = new List<SPFile>();
            IsReadOnly = false;
        }

        public SPFileCollection(int capacity)
        {
            _col = new List<SPFile>(capacity);
            IsReadOnly = false;
        }

        public SPFileCollection(IEnumerable<SPFile> lists)
        {
            _col = new List<SPFile>(lists);
            IsReadOnly = false;
        }

        public SPFileCollection(SPFile list)
            : this(((IEnumerable)list).Cast<SPFile>())
        {
        }

        #endregion

        #region IList and ICollection Methods

        public SPFile this[int index]
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

        public void Add(SPFile item)
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

        public bool Contains(SPFile item) => _col.Contains(item);

        public void CopyTo(SPFile[] array, int arrayIndex)
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

        public IEnumerator<SPFile> GetEnumerator() => _col.GetEnumerator();

        public int IndexOf(SPFile item) => _col.IndexOf(item);

        public void Insert(int index, SPFile item)
        {
            if (!IsReadOnly)
                _col.Insert(index, item);
            else
                throw new ReadOnlyCollectionException();
        }

        public bool Remove(SPFile item)
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
        public SPFile[] ToArray() =>
            _col.ToArray();

        public void AddRange(IEnumerable<SPFile> lists) =>
            _col.AddRange(lists);

        public bool TrueForAll(Predicate<SPFile> match) =>
            _col.TrueForAll(match);

        public bool Exists(Predicate<SPFile> match) =>
            _col.Exists(match);

        public ReadOnlyCollection<SPFile> AsReadOnly() =>
            _col.AsReadOnly();

        public void Sort() => _col.Sort();
        public void Sort(Comparison<SPFile> comparison) =>
            _col.Sort(comparison);

        public void Sort(IComparer<SPFile> comparer) =>
            _col.Sort(comparer);

        public void RemoveAll(Predicate<SPFile> match)
        {
            if (!IsReadOnly)
                _col.RemoveAll(match);
            else
                throw new ReadOnlyCollectionException();
        }

        #endregion

        #region Dictionary Indexing
        public SPFile this[string fileName]
        {
            get
            {
                SPFile foundya = null;
                for (int i = 0; i < _col.Count; i++)
                {
                    var l = _col[i];
                    if (string.Equals(l.Name, fileName, StringComparison.InvariantCultureIgnoreCase))
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
        public static explicit operator SPFileCollection(FileCollection fileCol)
        {
            var spList = new SPFileCollection();
            foreach (File f in fileCol)
            {
                spList.Add((SPFile)f);
            }
            spList.Original = fileCol;
            return spList;
        }

        #endregion

    }
}
