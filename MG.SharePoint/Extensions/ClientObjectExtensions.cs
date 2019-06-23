using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MG.SharePoint
{
    public static class ClientObjectExtensions
    {
        private const string P = "p";

        public static Expression<Func<T, object>>[] GetPropertyExpressions<T>(params string[] propertyNamesToLoad)
        {
            if (propertyNamesToLoad == null || propertyNamesToLoad.Length <= 0)
                throw new ArgumentNullException("propertyNamesToLoad");

            var exprs = new List<Expression<Func<T, object>>>(propertyNamesToLoad.Length);
            for (int i = 0; i < propertyNamesToLoad.Length; i++)
            {
                string prop = propertyNamesToLoad[i];
                var param1 = Expression.Parameter(typeof(T), P);
                var name1 = Expression.Property(param1, prop);
                var body1 = Expression.Convert(name1, typeof(object));
                var lambda = Expression.Lambda<Func<T, object>>(body1, param1);

                exprs.Add(lambda);
            }
            return exprs.ToArray();
        }

        public static Expression<Func<T, object>>[] GetPropertyExpressions<T>(this T clientObject, params string[] propertyNamesToLoad)
            where T : ClientObject
        {
            return GetPropertyExpressions<T>(propertyNamesToLoad);
        }

        public static void Initialize<T>(this ClientObjectCollection<T> col) where T : ClientObject
        {
            col.Context.Load(col);
            col.Context.ExecuteQuery();
        }

        /// <summary>
        /// Determines whether Client Object property is loaded
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="clientObject"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static bool IsPropertyReady<T>(this T clientObject, Expression<Func<T, object>> property)
            where T : ClientObject
        {
            var expression = (MemberExpression)property.Body;
            var propName = expression.Member.Name;
            var isCollection = typeof(ClientObjectCollection).IsAssignableFrom(property.Body.Type);
            return isCollection ?
                clientObject.IsObjectPropertyInstantiated(propName) :
                clientObject.IsPropertyAvailable(propName);
        }

        public static void LoadProperty<T>(this T clientObject, string propertyName)
            where T : ClientObject
        {
            //clientObject.Initialize();
            var expressions = clientObject.GetPropertyExpressions(propertyName);
            clientObject.LoadProperty(expressions);
        }

        public static void LoadProperty<T>(this T clientObject, string[] propertyNames)
            where T : ClientObject
        {
            var expressions = clientObject.GetPropertyExpressions(propertyNames);
            clientObject.LoadProperty(expressions);
        }

        public static void LoadProperty<T>(this T clientObject, params Expression<Func<T, object>>[] property)
            where T : ClientObject
        {
            clientObject.Context.Load(clientObject, property);
            clientObject.Context.ExecuteQuery();
        }

        public static void LoadProperty<T>(this T clientObject, bool andExecute, params Expression<Func<T, object>>[] property)
            where T : ClientObject
        {
            clientObject.Context.Load(clientObject, property);
            if (andExecute)
            {
                clientObject.Context.ExecuteQuery();
            }
        }
    }
}