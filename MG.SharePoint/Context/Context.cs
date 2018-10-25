using Microsoft.SharePoint.Client;
using System;
using System.Linq.Expressions;

namespace MG.SharePoint
{
    public static partial class CTX
    {
        public static ClientContext SP1 { get; set; }
        internal static ClientContext SP2 { get; set; }
        public static bool Connected => SP1 != null;

        #region Load and Execute (LAE)
        public static void Lae() => SP1.ExecuteQuery();

        public static void Lae<T>(T obj, bool andExecute = true, params Expression<Func<T, object>>[] retrievals)
            where T : ClientObject =>
            Lae(new T[1] { obj }, andExecute, retrievals);

        public static void Lae<T>(T[] objs, bool andExecute = true, params Expression<Func<T, object>>[] retrievals)
            where T : ClientObject
        {
            for (int i = 0; i < objs.Length; i++)
            {
                var obj = objs[i];
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
