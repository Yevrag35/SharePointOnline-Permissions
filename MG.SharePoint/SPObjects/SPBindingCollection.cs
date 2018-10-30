using Microsoft.SharePoint.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MG.SharePoint
{
    public class SPBindingCollection : IList<SPBinding>, ICollection, ICloneable
    {
        private protected List<SPBinding> _list;

        #region Constructors
        public SPBindingCollection() => 
            _list = new List<SPBinding>();

        public SPBindingCollection(int capacity) =>
            _list = new List<SPBinding>(capacity);

        public SPBindingCollection(Principal prin, RoleDefinition roleDef)
            : this(new SPBinding(prin, roleDef))
        {
        }
        public SPBindingCollection(SPBinding spBind)
            : this(((IEnumerable<SPBinding>)spBind).Cast<SPBinding>().ToArray())
        {
        }
        public SPBindingCollection(IEnumerable<SPBinding> items) => 
            _list = new List<SPBinding>(items);

        public SPBindingCollection(IDictionary bindingHashtable)
        {
            var keys = bindingHashtable.Keys.Cast<string>().ToArray();
            var bindings = new SPBinding[keys.Length];
            for (int i = 0; i < keys.Length; i++)
            {
                var key = keys[i];
                var prin = Convert.ToString(key);
                var role = Convert.ToString(bindingHashtable[key]);
                bindings[i] = new SPBinding(prin, role);
            }
            _list = new List<SPBinding>(bindings);
        }

        #endregion

        #region IList and ICollection Methods
        SPBinding IList<SPBinding>.this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }
        public SPBinding this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

        public bool IsReadOnly => false;
        public bool IsFixedSize => false;

        public int Count => _list.Count;

        public object SyncRoot => ((ICollection)_list).SyncRoot;

        public bool IsSynchronized => ((ICollection)_list).IsSynchronized;

        public void Add(SPBinding item) => _list.Add(item);

        public void Add(Principal prin, RoleDefinition roleDef) =>
            this.Add(new SPBinding(prin, roleDef));

        public void AddRange(IEnumerable<SPBinding> items) =>
            _list.AddRange(items);

        public void Clear() => _list.Clear();

        public bool Contains(SPBinding item) => _list.Contains(item);

        public bool Contains(Principal prin, RoleDefinition roleDef) =>
            this.Contains(new SPBinding(prin, roleDef));

        public void CopyTo(Array array, int index)
        {
            SPBinding[] bindings = array.OfType<SPBinding>().ToArray();
            _list.CopyTo(bindings, index);
        }

        public void CopyTo(SPBinding[] array, int arrayIndex) => 
            _list.CopyTo(array, arrayIndex);

        public IEnumerator GetEnumerator() => _list.GetEnumerator();

        public int IndexOf(SPBinding item) => _list.IndexOf(item);

        public int IndexOf(Principal prin, RoleDefinition roleDef) =>
            this.IndexOf(new SPBinding(prin, roleDef));

        public void Insert(int index, SPBinding item) => _list.Insert(index, item);

        public void Insert(int index, Principal prin, RoleDefinition roleDef) =>
            this.Insert(index, new SPBinding(prin, roleDef));

        public bool Remove(SPBinding item) => _list.Remove(item);

        public bool Remove(Principal prin, RoleDefinition roleDef) =>
            this.Remove(new SPBinding(prin, roleDef));

        public void RemoveAt(int index) => _list.RemoveAt(index);

        IEnumerator<SPBinding> IEnumerable<SPBinding>.GetEnumerator() => 
            _list.GetEnumerator();

        #endregion

        #region ICloneable Methods
        public object Clone()
        {
            var newList = new SPBindingCollection(_list.Count);
            for (int i = 0; i < _list.Count; i++)
            {
                var binding = _list[i];
                newList.Add((SPBinding)binding.Clone());
            }
            return newList;
        }

        #endregion

        #region Other 'List' Methods
        public SPBinding[] ToArray() =>
            _list.ToArray();

        public bool TrueForAll(Predicate<SPBinding> match) =>
            _list.TrueForAll(match);

        public bool Exists(Predicate<SPBinding> match) =>
            _list.Exists(match);

        public ReadOnlyCollection<SPBinding> AsReadOnly() =>
            _list.AsReadOnly();

        public void Sort() => _list.Sort();

        public void Sort(Comparison<SPBinding> comparison) => 
            _list.Sort(comparison);

        public void Sort(IComparer<SPBinding> comparer) =>
            _list.Sort(comparer);

        public void RemoveAll(Predicate<SPBinding> match) =>
            _list.RemoveAll(match);

        #endregion
    }
}
