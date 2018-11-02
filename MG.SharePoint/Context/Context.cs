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
        internal static CmdLetContext SP2 { get; set; }
        public static bool Connected => SP1 != null;

        internal static RoleDefinitionCollection allRoles { get; set; }

        #region Load and Execute (LAE)
        public static void Lae() => SP1.ExecuteQuery();

        ////public static void Lae(ClientObject obj, bool andExecute = true) =>
        ////    Lae(obj, andExecute, null);
        //public static void Lae(IEnumerable<ClientObject> objs, bool andExecute = true) =>
        //    Lae(objs, andExecute, null);

        public static void Lae<T>(T obj, bool andExecute = true, params Expression<Func<T, object>>[] retrievals)
            where T : ClientObject =>
            Lae(new T[1] { obj }, andExecute, retrievals);

        public static void Lae<T>(IEnumerable<T> objs, bool andExecute = true, params Expression<Func<T, object>>[] retrievals)
            where T : ClientObject
        {
            var cObjs = ((IEnumerable)objs).Cast<T>().ToArray();
            for (int i = 0; i < cObjs.Length; i++)
            {
                var obj = cObjs[i];
                if (obj != null)
                {
                    SP1.Load(obj, retrievals);
                    if (andExecute)
                        SP1.ExecuteQuery();
                }
            }
        }

        #endregion
    }
}
