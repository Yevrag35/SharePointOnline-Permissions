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
        public static object GetPermissions(this SecurableObject secObj, string nameProperty, string idProperty)
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
            Expression<Func<SecurableObject, object>>[] expressions = secObj.GetClientPropertyExpressions(nameProperty, idProperty);
            MethodInfo specLae = typeof(CTX).GetMethod("SpecialLae", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(secType);
            specLae.Invoke(null, new object[3] { secObj, true, expressions });

            Console.WriteLine(secObj);
            return secObj;
        }

#if DEBUG

        public static object TestGetPermissions(SecurableObject secObj, string nameProp, string idProp)
        {
            return secObj.GetPermissions(nameProp, idProp);
        }

#endif
    }
}
