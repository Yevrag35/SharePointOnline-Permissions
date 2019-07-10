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
                ParameterExpression param1 = Expression.Parameter(typeof(T), P);
                MemberExpression name1 = Expression.Property(param1, prop);
                UnaryExpression body1 = Expression.Convert(name1, typeof(object));
                var lambda = Expression.Lambda<Func<T, object>>(body1, param1);

                exprs.Add(lambda);
            }
            return exprs.ToArray();
        }

        public static Expression<Func<T, object>>[] GetClientPropertyExpressions<T>(this T clientObject, params string[] propertyNamesToLoad)
            where T : ClientObject
        {
            return GetPropertyExpressions<T>(propertyNamesToLoad);
        }

        public static void Initialize<T>(this ClientObjectCollection<T> col) where T : ClientObject
        {
            col.Context.Load(col);
            col.Context.ExecuteQuery();
        }

        //public static object GetPermissions<T>(this T secureObj) where T : SecurableObject
        //{
        //    secureObj.
        //}

        /// <summary>
        /// Determines whether Client Object property is loaded
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="clientObject"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static bool IsPropertyReady<T>(this T clientObject, params Expression<Func<T, object>>[] properties)
            where T : ClientObject
        {
            if (properties == null)
                return false;

            bool check = true;
            for (int i = 0; i < properties.Length; i++)
            {
                var property = properties[i];
                string propName = null;
                if (property.Body is MemberExpression expression)
                {
                    propName = expression.Member.Name;
                }
                else if (property.Body is UnaryExpression unExp && unExp.Operand is MemberExpression memEx)
                {
                    propName = memEx.Member.Name;
                }
                bool isCollection = typeof(ClientObjectCollection).IsAssignableFrom(property.Body.Type);

                if (isCollection)
                {
                    try
                    {
                        check = clientObject.IsObjectPropertyInstantiated(propName);
                    }
                    catch (ServerException) { }
                }
                else
                {
                    check = clientObject.IsPropertyAvailable(propName);
                }

                if (!check)
                {
                    break;
                }
            }

            return check;
        }

        public static void LoadProperty<T>(this T clientObject, string propertyName)
            where T : ClientObject
        {
            //clientObject.Initialize();
            Expression<Func<T, object>>[] expressions = clientObject.GetClientPropertyExpressions(propertyName);
            clientObject.LoadProperty(expressions);
        }

        public static void LoadProperty<T>(this T clientObject, string[] propertyNames)
            where T : ClientObject
        {
            Expression<Func<T, object>>[] expressions = clientObject.GetClientPropertyExpressions(propertyNames);
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

        public static void TestLoad<T>(this T cliObj, params string[] propNames)
        {

        }
        public static void TestLoad<T>(this T cliObj, ClientObject parentObj, ClientObject colObj, string parentPropName, params string[] propNames)
        {
            if (propNames == null)
                throw new ArgumentNullException("PropertyNames");

            Type type = colObj.GetType();
            if (colObj is ClientObjectCollection)
                type = type.BaseType.GenericTypeArguments[0];

            Type exprType = typeof(Expression);
            Type paramExprType = typeof(ParameterExpression).MakeArrayType();
            MethodInfo lambdaMethod = exprType.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(
                x => x.Name.Equals("Lambda") &&
                x.IsGenericMethod &&
                x.GetParameters().Length == 2 && x.GetParameters().First().ParameterType.Equals(paramExprType)).First();

            MethodInfo makeGenLambda = typeof(ClientObjectExtensions).GetMethod("MakeFunc", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(type);
            var lambdaMethodGeneric = (MethodInfo)makeGenLambda.Invoke(null, new object[1] { lambdaMethod });

            var list = new List<Expression<Func<T, object>>>(propNames.Length);

        }

        private static MethodInfo MakeFunc<T>(MethodInfo lambdaMethod)
        {
            return lambdaMethod.MakeGenericMethod(typeof(Func<T, object>));
        }

        //private void AddExpression<T>(ref List<Expression<Func<T, object>>> exprs, Type type, string propName)
        //{
        //    var param = Expression.Parameter(type, propName);
        //    param
        //}
    }
}