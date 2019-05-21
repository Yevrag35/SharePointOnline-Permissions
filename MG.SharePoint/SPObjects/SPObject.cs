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
        private const BindingFlags GETPROP = BindingFlags.GetProperty;
        private const BindingFlags INST = BindingFlags.Instance;
        private const BindingFlags NONPUBINST = BindingFlags.NonPublic | INST;
        private const BindingFlags PUBINST = BindingFlags.Public | INST;

        #endregion

        protected internal PropertyInfo[] allPropInfo;

        public abstract string Name { get; internal set; }
        public abstract object Id { get; internal set; }

        bool ISPObject.IsObjectPropertyInstantiated(string propertyName) => this.ShowOriginal().IsObjectPropertyInstantiated(propertyName);
        void ISPObject.RefreshLoad() => this.ShowOriginal().RefreshLoad();

        public abstract ClientObject ShowOriginal();
        public ClientContext GetContext() => (ClientContext)this.ShowOriginal().Context;

        protected private void FormatObject<T>(T obj, string[] skipThese, params string[] includeThese) where T : ClientObject
        {
            Type origType = typeof(T);
            IEnumerable<PropertyInfo> origProps = null;
            if (skipThese == null)
            {
                origProps = origType.GetProperties(
                    PUBINST).Where(
                        x => includeThese.Contains(x.Name) || (
                            x.CanRead &&
                            (x.PropertyType.GetInterfaces().Contains(typeof(IConvertible)) ||
                                x.PropertyType.IsValueType))
                        );
            }
            else
            {
                origProps = origType.GetProperties(
                    PUBINST).Where(
                        x => !skipThese.Contains(x.Name) &&
                        x.CanRead &&
                        (x.PropertyType.GetInterfaces().Contains(typeof(IConvertible)) ||
                            x.PropertyType.IsValueType));
            }

            allPropInfo = this.GetType().GetProperties(PUBINST).Where(x => x.CanWrite).ToArray();

            var propList = origProps.Select(x => x.Name).ToList();

            //propList.Add("Title");
            propList.Remove("ObjectVersion");
            propList.Remove("ServerObjectIsNull");
            var expressions = this.GetPropertyExpressions<T>(origType, propList.ToArray());
            try
            {
                CTX.SP1.Load(obj, expressions);
            }
            catch (InvalidQueryExpressionException) { }
            
            CTX.SP1.ExecuteQuery();

            for (int i = 0; i < allPropInfo.Length; i++)
            {
                PropertyInfo thisProp = allPropInfo[i];

                foreach (PropertyInfo origProp in origProps)
                {
                    if (thisProp.Name.Equals(origProp.Name))
                    {
                        thisProp.SetValue(this, origProp.GetValue(obj));
                        break;
                    }
                }
            }
        }

        protected private Expression<Func<T, object>>[] GetPropertyExpressions<T>(Type type, params string[] propertyNamesToLoad)
            //where T : ClientObject
        {
            var exprs = new List<Expression<Func<T, object>>>(propertyNamesToLoad.Length);
            for (int i = 0; i < propertyNamesToLoad.Length; i++)
            {
                var prop = propertyNamesToLoad[i];
                var param1 = Expression.Parameter(type, "p");
                var name1 = Expression.Property(param1, prop);
                var body1 = Expression.Convert(name1, typeof(object));
                var lambda = Expression.Lambda<Func<T, object>>(body1, param1);

                exprs.Add(lambda);
            }

            return exprs.ToArray();
        }

        protected private Expression<Func<T, object>>[] GetPropertyExpressions<T>(params string[] propertyNamesToLoad) =>
            this.GetPropertyExpressions<T>(typeof(T), propertyNamesToLoad);

        public abstract void LoadProperty(params string[] propertyNames);

        private bool ToSPType(Type t, out Type returnType)
        {
            MethodInfo mi = this.GetType().GetMethod(
                "GetSPType", NONPUBINST).MakeGenericMethod(t);
            returnType = (Type)mi.Invoke(this, null);
            return returnType != null;
        }

        private Type GetSPType<T>() where T : ClientObject
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

                case "FolderCollection":
                    return typeof(SPFolderCollection);

                case "File":
                    return typeof(SPFile);

                case "FileCollection":
                    return typeof(SPFileCollection);

                case "ListItem":
                    return typeof(SPListItem);

                case "WebCollection":
                    return typeof(SPWebCollection);

                case "User":
                    return typeof(SPUser);

                case "UserCollection":
                    return typeof(SPUserCollection);

                //case "Group":
                //    return typeof(SPGroup);

                default:
                    return typeof(T);
            }
        }

        protected internal T Cast<T>(dynamic o) => (T)o;

        protected internal void Load<T>(T original, params string[] propertyNames) where T : ClientObject
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
                var thatObj = thatType.InvokeMember(propInfo.Name, GETPROP, null, original, null);
                if (thatObj is ClientObject && ToSPType(thatObj.GetType(), out Type newType))
                {
                    MethodInfo GenericCast = this.GetType().GetMethod(
                        "Cast", NONPUBINST).MakeGenericMethod(newType);
                    thatObj = GenericCast.Invoke(this, new object[1] { thatObj });
                }
                propInfo.SetValue(this, thatObj, NONPUBINST,
                    null, null, CultureInfo.CurrentCulture);
            }
        }
    }
}
