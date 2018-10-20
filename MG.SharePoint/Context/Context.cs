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

        #region Load and Execute
        public static void lae() => SP1.ExecuteQuery();

        public static void lae(ClientObject obj, bool andExecute = true) =>
            lae(new ClientObject[1] { obj }, andExecute);

        public static void lae(ClientObject[] objs, bool andExecute = true)
        {
            for (int i = 0; i < objs.Length; i++)
            {
                var obj = objs[i];
                if (obj != null)
                {
                    SP1.Load(obj);
                    if (andExecute)
                        SP1.ExecuteQuery();
                }
            }
        }
        public static void lae<T>(T obj, bool andExecute, params Expression<Func<T, object>>[] retrievals) where T : ClientObject
        {
            SP1.Load(obj, retrievals);
            if (andExecute)
                SP1.ExecuteQuery();
        }

        #endregion
    }
}
