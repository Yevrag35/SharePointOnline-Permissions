using Microsoft.SharePoint.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MG.SharePoint
{
    public class SPBindingCollection : IList<SPBinding>, ICollection, ICloneable, ISPPermissionResolver
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
            : this(new SPBinding[1] { spBind })
        {
        }
        public SPBindingCollection(IEnumerable<SPBinding> items) => 
            _list = new List<SPBinding>(items);

        public SPBindingCollection(IDictionary bindingHashtable) => 
            _list = new List<SPBinding>(ResolvePermissions(bindingHashtable));

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

        public void AddRange(IDictionary permissions) =>
            _list.AddRange(ResolvePermissions(permissions));

        public void Clear() => _list.Clear();

        public bool Contains(SPBinding item) => _list.Contains(item);

        public bool Contains(Principal prin, RoleDefinition roleDef) =>
            this.Contains(new SPBinding(prin, roleDef));

        public bool ContainsPrincipal(string principal)
        {
            bool result = false;
            for (int i = 0; i < _list.Count; i++)
            {
                SPBinding item = _list[i];
                if (string.Equals(principal, item.Name, StringComparison.OrdinalIgnoreCase))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        public bool ContainsRole(string roleDefinitionName)
        {
            bool result = false;
            for (int i = 0; i < _list.Count; i++)
            {
                SPBinding item = _list[i];
                if (string.Equals(item.Definition.Name, roleDefinitionName, StringComparison.OrdinalIgnoreCase))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        public bool ContainsRole(RoleDefinition roleDef)
        {
            if (!roleDef.IsPropertyReady(x => x.Name))
                CTX.Lae(roleDef, true, r => r.Name);
            return ContainsRole(roleDef.Name);
        }

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
                SPBinding binding = _list[i];
                newList.Add((SPBinding)binding.Clone());
            }
            return newList;
        }

        #endregion

        #region IPermissionResolver Method
        public IEnumerable<SPBinding> ResolvePermissions(IDictionary bindingHashtable)
        {
            string[] keys = bindingHashtable.Keys.Cast<string>().ToArray();
            var bindings = new SPBinding[keys.Length];
            for (int i = 0; i < keys.Length; i++)
            {
                string key = keys[i];
                string prin = Convert.ToString(key);
                string role = Convert.ToString(bindingHashtable[key]);
                bindings[i] = new SPBinding(prin, role);
            }
            return bindings;
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

        // Remove duplicate entries on construction and adding
        //private IEnumerable<SPBinding> RemoveDuplicates(IEnumerable<SPBinding> bindings)
        //{
        //    var names = new string[_list.Count];
        //    for (int n = 0; n < _list.Count; n++)
        //    {
        //        names[n] = _list[n].Name;
        //    }
        //    for (int i = _list.Count - 1; i >=0; i--)
        //    {
        //        bool dup = false;
        //        bool found = false;
        //        var item = _list[i];
        //        foreach (string name in names)
        //        {
        //            if (name == item.Name && !found)
        //            {
        //                found = true;
        //            }
        //            else if (name == item.Name && found &&
        //                )
                        
        //        }
        //    }
        //}

        #endregion
    }
}
