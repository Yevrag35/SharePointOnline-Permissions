using Microsoft.SharePoint.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MG.SharePoint
{
    public class SPPermissionCollection : IList<SPPermission>, ICollection, ICloneable
    {
        private protected List<SPPermission> _list;

        #region Constructors
        public SPPermissionCollection() =>
            _list = new List<SPPermission>();

        public SPPermissionCollection(int capacity) =>
            _list = new List<SPPermission>(capacity);

        public SPPermissionCollection(SPPermission perm)
            : this(((IEnumerable)perm).Cast<SPPermission>())
        {
        }
        public SPPermissionCollection(IEnumerable<SPPermission> perms) =>
            _list = new List<SPPermission>(perms);

        #endregion

        #region IList and ICollection Methods

        public SPPermission this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

        public int Count => _list.Count;
        public bool IsReadOnly => false;
        public object SyncRoot => ((ICollection)_list).SyncRoot;
        public bool IsSynchronized => ((ICollection)_list).IsSynchronized;

        public void Add(SPPermission item) => _list.Add(item);

        public void AddRange(IEnumerable<SPPermission> items) =>
            _list.AddRange(items);
        public void AddRange(IEnumerable<RoleAssignment> asses)
        {
            var roleAsses = asses.ToArray();
            for (int i = 0; i < roleAsses.Length; i++)
            {
                _list.Add(roleAsses[i]);
            }
        }

        public void Clear() => _list.Clear();

        public bool Contains(SPPermission item) => _list.Contains(item);

        public void CopyTo(SPPermission[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);
        public void CopyTo(Array array, int index) => ((ICollection)_list).CopyTo(array, index);

        public IEnumerator<SPPermission> GetEnumerator() => _list.GetEnumerator();

        public int IndexOf(SPPermission item) => _list.IndexOf(item);

        public void Insert(int index, SPPermission item) => _list.Insert(index, item);

        public bool Remove(SPPermission item) => _list.Remove(item);

        public void RemoveAt(int index) => _list.RemoveAt(index);

        IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();

        #endregion

        #region ICloneable Methods
        public object Clone()
        {
            var newList = new SPPermissionCollection(_list.Count);
            for (int i = 0; i < _list.Count; i++)
            {
                var perm = _list[i];
                newList.Add((SPPermission)perm.Clone());
            }
            return newList;
        }

        #endregion

        #region Other 'List' Methods
        public SPPermission[] ToArray() =>
            _list.ToArray();

        public bool TrueForAll(Predicate<SPPermission> match) =>
            _list.TrueForAll(match);

        public bool Exists(Predicate<SPPermission> match) =>
            _list.Exists(match);

        public ReadOnlyCollection<SPPermission> AsReadOnly() =>
            _list.AsReadOnly();

        public void Sort() => _list.Sort();
        public void Sort(Comparison<SPPermission> comparison) =>
            _list.Sort(comparison);

        public void Sort(IComparer<SPPermission> comparer) =>
            _list.Sort(comparer);

        public void RemoveAll(Predicate<SPPermission> match) =>
            _list.RemoveAll(match);

        #endregion

        #region Operators
        public static implicit operator SPPermissionCollection(RoleAssignmentCollection assCol)
        {
            CTX.Lae(assCol, true,
                rCol => rCol.Include(
                    ass => ass.Member, ass => ass.RoleDefinitionBindings.Include(
                        d => d.Name, d => d.Description
                    )
                )
            );
            var permCol = new SPPermissionCollection(assCol.Count);
            for (int i = 0; i < assCol.Count; i++)
            {
                SPPermission p = assCol[i];
                permCol.Add(p);
            }
            return permCol;
        }

        #endregion
    }
}
