using Microsoft.SharePoint.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MG.SharePoint
{
    public abstract class SPObject : ISPObject
    {
        #region Constants
        private protected const BindingFlags getProp = BindingFlags.GetProperty;
        private protected const BindingFlags setFlags = BindingFlags.NonPublic | BindingFlags.Instance;

        #endregion

        internal PropertyInfo[] allPropInfo;

        public abstract string Name { get; }
        public abstract object Id { get; }

        public abstract object ShowOriginal();

        internal List<Expression<Func<T, object>>> GetPropertyExpressions<T>(params string[] propertyNamesToLoad)
            where T : ClientObject
        {
            var exprs = new List<Expression<Func<T, object>>>(propertyNamesToLoad.Length);
            for (int i = 0; i < propertyNamesToLoad.Length; i++)
            {
                var prop = propertyNamesToLoad[i];
                var param1 = Expression.Parameter(typeof(T), "p");
                var name1 = Expression.Property(param1, prop);
                var body1 = Expression.Convert(name1, typeof(object));
                var lambda = Expression.Lambda<Func<T, object>>(body1, param1);

                exprs.Add(lambda);
            }
            return exprs;
        }

        public abstract void LoadProperty(params string[] propertyNames);

        private protected Type GetSPType<T>() where T : ClientObject
        {
            switch (typeof(T).Name)
            {
                case "Web":
                    return typeof(SPWeb);
                case "ListCollection":
                    return typeof(SPListCollection);
                case "ListItemCollection":
                    return typeof(SPListItemCollection);
                case "List":
                    return typeof(SPList);
                case "Folder":
                    return typeof(SPFolder);
                case "ListItem":
                    return typeof(SPListItem);
                default:
                    return null;
            }
        }

        internal void Load<T>(T original, params string[] propertyNames) where T : ClientObject
        {
            var expressions = GetPropertyExpressions<T>(propertyNames).ToArray();
            CTX.Lae(original, true, expressions);
            var thisType = this.GetType();
            var thatType = typeof(T);
            for (int i = 0; i < propertyNames.Length; i++)
            {
                var prop = propertyNames[i];
                var propInfo = thisType.GetProperty(prop);
                if (propInfo == null)
                {
                    if (allPropInfo == null)
                        allPropInfo = thisType.GetProperties();

                    for (int p = 0; p < allPropInfo.Length; p++)
                    {
                        var pi = allPropInfo[p];
                        if (string.Equals(pi.Name, prop, StringComparison.InvariantCultureIgnoreCase))
                        {
                            propInfo = pi;
                            break;
                        }
                    }
                    if (propInfo == null)
                        throw new ArgumentException(prop + " was not recognized as a valid property name for this object!");
                }
                var thatObj = thatType.InvokeMember(propInfo.Name, getProp, null, original, null);
                propInfo.SetValue(this, thatObj, setFlags,
                    null, null, CultureInfo.CurrentCulture);
            }
        }
    }
}
