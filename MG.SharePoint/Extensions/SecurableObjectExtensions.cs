using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MG.SharePoint
{
    public static class SecurableObjectExtensions
    {
        public static void AddPermission(this SecurableObject secObj, string principal, string roleDefinition, bool forceBreak, bool applyRecursively)
        {
            if (CTX.AllRoles == null)
                CTX.AllRoles = ((ClientContext)secObj.Context).Web.RoleDefinitions;

            User user = ((ClientContext)secObj.Context).Web.EnsureUser(principal);
            user.LoadUserProps();

            RoleDefinition realDef;
            try
            {
                realDef = CTX.AllRoles.GetByName(roleDefinition);
                realDef.LoadDefinition();
            }
            catch (Exception)
            {
                throw new ArgumentException(roleDefinition + " is not the name of a valid RoleDefinition in this site collection.");
            }

            secObj.AddPermission(new SPBindingCollection(user, realDef), forceBreak, applyRecursively);
        }

        public static void AddPermission(this SecurableObject secObj, SPBindingCollection bindingCol, bool forceBreak, bool applyRecursively)
        {
            MethodInfo upMeth = null;
            try
            {
                upMeth = secObj.GetType().GetMethod("Update", BindingFlags.Public | BindingFlags.Instance);
            }
            catch (AmbiguousMatchException)
            {
            }

            if (upMeth == null)
                throw new InvalidOperationException("This specified SecurableObject does not have a method called \"Update\".");

            if (secObj.CanSetPermissions())
            {
                if (!secObj.IsPropertyReady(x => x.HasUniqueRoleAssignments))
                {
                    secObj.LoadProperty("HasUniqueRoleAssignments");
                }
                if (!secObj.HasUniqueRoleAssignments && !forceBreak)
                    throw new InvalidOperationException("You must first break inheritance on this object to apply custom permissions.");

                else if (!secObj.HasUniqueRoleAssignments && forceBreak)
                    secObj.BreakRoleInheritance(true, true);

                for (int i = 0; i < bindingCol.Count; i++)
                {
                    SPBinding binding = bindingCol[i];
                    var bCol = new RoleDefinitionBindingCollection(secObj.Context)
                    {
                        binding.Definition
                    };
                    RoleAssignment roleAss = secObj.RoleAssignments.Add(binding.Principal, bCol);
                    secObj.Context.Load(roleAss);
                    upMeth.Invoke(secObj, null);
                    secObj.Context.ExecuteQuery();
                }
            }
        }

#if DEBUG

        public static void AddPermissionTest(SecurableObject secObj, string principal, string roleDefinition, bool forceBreak, bool applyRecursively)
        {
            if (CTX.AllRoles == null)
                CTX.AllRoles = ((ClientContext)secObj.Context).Web.RoleDefinitions;

            User user = ((ClientContext)secObj.Context).Web.EnsureUser(principal);
            user.LoadUserProps();

            RoleDefinition realDef;
            try
            {
                realDef = CTX.AllRoles.GetByName(roleDefinition);
                realDef.LoadDefinition();
            }
            catch (Exception)
            {
                throw new ArgumentException(roleDefinition + " is not the name of a valid RoleDefinition in this site collection.");
            }

            secObj.AddPermission(new SPBindingCollection(user, realDef), forceBreak, applyRecursively);
        }

#endif

        public static bool CanSetPermissions(this SecurableObject secObj)
        {
            return secObj.IsPropertyAvailable("HasUniqueRoleAssignments");
        }

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
