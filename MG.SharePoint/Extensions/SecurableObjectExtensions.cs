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
            

        }

        private static KeyValuePair<string, string> GetNameAndIdFromObject<T>(T securableObject, string nameProp, string idProp)
            where T : SecurableObject
        {
            
        }

#if DEBUG

        public static object TestGetPermissions(SecurableObject secObj, string nameProp, string idProp)
        {
            return secObj.GetPermissions(nameProp, idProp);
        }

#endif
    }
}
