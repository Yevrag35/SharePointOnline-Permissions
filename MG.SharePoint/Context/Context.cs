using Microsoft.SharePoint.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MG.SharePoint
{
    public static partial class CTX
    {
        public static CmdLetContext SP1 { get; set; }
        public static CmdLetContext SP2 { get; set; }
        public static bool Connected => SP1 != null;
        public static bool DestConnected => SP2 != null;

        internal static string SpecifiedTenantName { get; set; }
        internal static string DestinationSite =>
            !string.IsNullOrEmpty(SpecifiedTenantName) ?
                SP1.Url.Replace(
                    "https://" + SpecifiedTenantName +
                    ".sharepoint.com", string.Empty) :
                null;

        public static RoleDefinitionCollection AllRoles { get; set; }

        public static User GetUserByEmail(string email) => SP1.Web.GetUserByEmail(email);

        #region Load and Execute (LAE)
        public static void Lae() => SP1.ExecuteQuery();
        public static void DestLae() => SP2.ExecuteQuery();

        public static void Lae<T>(T obj, bool andExecute = true, params Expression<Func<T, object>>[] retrievals)
            where T : ClientObject =>
            Lae(new T[1] { obj }, andExecute, retrievals);

        internal static void SpecialLae<T>(T obj, bool andExecute, object retrievals) where T : ClientObject
        {
            var rets = (Expression<Func<T, object>>[])retrievals;
            Lae(new T[1] { obj }, SP1, andExecute, rets);
        }

        public static void Lae<T>(IEnumerable<T> objs, bool andExecute = true, params Expression<Func<T, object>>[] retrievals)
            where T : ClientObject =>
            Lae(objs, SP1, andExecute, retrievals);

        public static void DestLae<T>(T obj, bool andExecute, params Expression<Func<T, object>>[] retrievals) where T : ClientObject =>
            DestLae(new T[1] { obj }, andExecute, retrievals);

        public static void DestLae<T>(IEnumerable<T> objs, bool andExecute, params Expression<Func<T, object>>[] retrievals)
            where T : ClientObject =>
            Lae(objs, SP2, andExecute, retrievals);

        private static void Lae<T>(IEnumerable<T> objs, CmdLetContext ctx, bool andExecute, params Expression<Func<T, object>>[] retrievals) where T : ClientObject
        {
            var cObjs = objs.ToArray();
            for (int i = 0; i < cObjs.Length; i++)
            {
                var obj = cObjs[i];
                if (obj != null)
                {
                    try
                    {
                        ctx.Load(obj, retrievals);
                        if (andExecute)
                            ctx.ExecuteQuery();
                    }
                    catch (InvalidQueryExpressionException e)
                    {
                        if (e.Message.Contains("The query expression") && e.Message.Contains("is not supported"))
                            return;

                        else
                            throw new AggregateException("An invalid query was attempted.", e);
                    }
                }
            }
        }

        #endregion
    }
}
