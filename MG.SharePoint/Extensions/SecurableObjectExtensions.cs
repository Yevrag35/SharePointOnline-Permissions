using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MG.SharePoint
{
    public static class SecurableObjectExtensions
    {
        public static SPPermissionCollection GetPermissions(this SecurableObject secObj, string nameProperty, string idProperty)
        {
            secObj.Context.Load(secObj, s => s.HasUniqueRoleAssignments, s => s.RoleAssignments);
            bool canSet = true;
            try
            {
                secObj.Context.ExecuteQuery();
            }
            catch (ServerException ex)
            {
                if (!ex.Message.Contains("does not belong to a list."))
                    canSet = false;
            }

            bool? check = secObj.IsPropertyAvailable("HasUniqueRoleAssignments")
                   ? (bool?)secObj.HasUniqueRoleAssignments
                   : null;

            if (!canSet || !check.HasValue)
            {
                return null;
            }

            Type secType = secObj.GetType();
            MethodInfo genMeth = typeof(ClientObjectExtensions).GetMethod(
                "GetPropertyExpressions", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(secType);

            object expressions = genMeth.Invoke(null, new object[1] { new string[2] { nameProperty, idProperty } });

            MethodInfo specLae = typeof(CTX).GetMethod("SpecialLae", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(secType);
            specLae.Invoke(null, new object[3] { secObj, true, expressions });

            //Console.WriteLine(secObj);
            //return secObj;

            var permissions = SPPermissionCollection.ResolvePermissions(secObj);
            var kvp = GetNameAndIdFromObject(secObj, nameProperty, idProperty);
            permissions.AddObjectNameAndId(kvp.Value, kvp.Key);
            return permissions;
        }

        private static KeyValuePair<object, string> GetNameAndIdFromObject(ClientObject clientObject, string nameProp, string idProp)
        {
            Type objType = clientObject.GetType();
            string[] names = new string[2] { nameProp, idProp };
            //IEnumerable<PropertyInfo> nameAndIdProps = objType.GetProperties().Where(x => names.Contains(x.Name));
            PropertyInfo namePi = objType.GetProperty(nameProp, BindingFlags.Public | BindingFlags.Instance);
            PropertyInfo idPi = objType.GetProperty(idProp, BindingFlags.Public | BindingFlags.Instance);
            string nameVal = namePi.GetValue(clientObject) as string;
            object idVal = idPi.GetValue(clientObject);
            return new KeyValuePair<object, string>(idVal, nameVal);
        }

#if DEBUG

        public static object TestGetPermissions(SecurableObject secObj, string nameProp, string idProp)
        {
            return secObj.GetPermissions(nameProp, idProp);
        }

#endif
    }
}
