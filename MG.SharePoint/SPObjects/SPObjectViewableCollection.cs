using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Management.Automation;
using System.Reflection;

namespace MG.SharePoint
{
    public class ClientObjectViewableCollection<T> : BaseSPCollection, IEnumerable<T> where T : ClientObject
    {
        #region FIELDS/CONSTANTS
        private const BindingFlags PUB_INST = BindingFlags.Public | BindingFlags.Instance;
        private const string NAME = "Name";

        private List<T> _list;
        private readonly PropertyInfo PredicateProperty;

        #endregion

        #region PROPERTIES
        public override int Count => _list.Count;
        public override object SyncRoot => ((ICollection)_list).SyncRoot;

        #endregion

        #region CONSTRUCTORS
        public ClientObjectViewableCollection()
            : this(CTX.SP1) { }
        public ClientObjectViewableCollection(ClientContext ctx)
            : this(NAME, ctx) { }
        public ClientObjectViewableCollection(string idPropName, ClientContext ctx)
            : base(ctx)
        {
            _list = new List<T>();
            PredicateProperty = typeof(T).GetProperties(PUB_INST).Single(x => x.Name.Equals(idPropName, StringComparison.CurrentCultureIgnoreCase));
        }
        public ClientObjectViewableCollection(int capacity)
            : this(capacity, CTX.SP1) { }
        public ClientObjectViewableCollection(int capacity, ClientContext ctx)
            : this(NAME, capacity, ctx) { }
        public ClientObjectViewableCollection(string idPropName, int capacity, ClientContext ctx)
            : base(ctx)
        {
            _list = new List<T>(capacity);
            PredicateProperty = typeof(T).GetProperties(PUB_INST).Single(x => x.Name.Equals(idPropName, StringComparison.CurrentCultureIgnoreCase));
        }
        public ClientObjectViewableCollection(IEnumerable<T> objs)
            : this(objs, (ClientContext)objs.ToArray()[0].Context) { }
        public ClientObjectViewableCollection(IEnumerable<T> objs, ClientContext ctx)
            : this(NAME, objs, ctx) { }
        public ClientObjectViewableCollection(string idPropName, IEnumerable<T> objs, ClientContext ctx)
            : base(ctx)
        {
            _list = new List<T>(objs);
            PredicateProperty = typeof(T).GetProperties(PUB_INST).Single(x => x.Name.Equals(idPropName, StringComparison.CurrentCultureIgnoreCase));
        }

        #endregion

        #region INDEXERS
        public T this[int index] => _list[index];
        public T this[object identifier] => this.FindByPredicate(identifier, PredicateProperty);
        public ClientObjectViewableCollection<T> this[string propName, object identifier] => this.FindBySpecifiedPredicate(identifier, propName);
        public ClientObjectViewableCollection<T> this[ScriptBlock scriptBlock]
        {
            get
            {
                var list = new ClientObjectViewableCollection<T>();
                for (int i = 0; i < _list.Count; i++)
                {
                    var item = _list[i];
                    object ret = scriptBlock.InvokeReturnAsIs(item);
                    if (ret is PSObject psObj && psObj.ImmediateBaseObject is bool well && well)
                        list.Add(item);
                }
                return list;
            }
        }

        #endregion

        #region METHODS
        public void Add(T item) => _list.Add(item);
        public void AddRange(IEnumerable<T> items) => _list.AddRange(items);
        //public void AddRange(ClientObjectCollection<T> objCol) => _list.AddRange(objCol)

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => ((IEnumerable<T>)this._list).GetEnumerator();
        public override void CopyTo(Array array, int index) => ((ICollection)_list).CopyTo(array, index);
        public override IEnumerator GetEnumerator() => _list.GetEnumerator();
        public override void Sort() => _list.Sort();

        private T FindByPredicate(object valueToSearchBy, PropertyInfo pi)
        {
            var entityParameter = Expression.Parameter(typeof(T), pi.Name);
            var left = Expression.MakeMemberAccess(entityParameter, pi);
            var right = Expression.Constant(valueToSearchBy);
            var body = Expression.Equal(left, right);
            var lambda = Expression.Lambda<Func<T, bool>>(body, entityParameter);
            var pred = new Predicate<T>(lambda.Compile());
            return _list.Find(pred);
        }

        private ClientObjectViewableCollection<T> FindBySpecifiedPredicate(object valueToSearchBy, string propertyToQuery)
        {
            PropertyInfo pi = typeof(T).GetProperties(PUB_INST).Single(x => x.Name.Equals(propertyToQuery, StringComparison.CurrentCultureIgnoreCase));
            var entityParameter = Expression.Parameter(typeof(T), pi.Name);
            var left = Expression.MakeMemberAccess(entityParameter, pi);
            var right = Expression.Constant(valueToSearchBy);
            var body = Expression.Equal(left, right);
            var lambda = Expression.Lambda<Func<T, bool>>(body, entityParameter);
            var pred = new Predicate<T>(lambda.Compile());
            return new ClientObjectViewableCollection<T>(_list.FindAll(pred));
        }
        
        #endregion
    }
}