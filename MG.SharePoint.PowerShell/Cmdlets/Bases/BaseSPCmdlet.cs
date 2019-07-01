using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Management.Automation;
using System.Reflection;

namespace MG.SharePoint.PowerShell
{
    public abstract class BaseSPCmdlet : PSCmdlet
    {
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            if (!CheckSession())
                throw new InvalidOperationException("SPO Context is not set");
        }

        protected internal bool CheckSession() => CTX.Connected;

        protected private void WriteError(string msg, ErrorCategory cat) =>
            this.WriteError(new ArgumentException(msg), cat, null);

        protected private void WriteError(string msg, ErrorCategory cat, object obj) =>
            this.WriteError(new ArgumentException(msg), cat, obj);

        protected private void WriteError(string msg, Exception exception, ErrorCategory cat, object obj)
        {
            var errRec = new ErrorRecord(new InvalidOperationException(msg, exception), exception.GetType().FullName, cat, obj);
            base.WriteError(errRec);
        }

        protected private void WriteError(Exception baseEx, ErrorCategory cat) => this.WriteError(baseEx, cat, null);
        protected private void WriteError(Exception baseEx, ErrorCategory cat, object obj)
        {
            var errRec = new ErrorRecord(baseEx, baseEx.GetType().FullName, cat, obj);
            base.WriteError(errRec);
        }

        public static bool Test<T>(T cliObj, params string[] propName) where T : ClientObject
        {
            object castedObj = GenCast(cliObj);
            Type coe = typeof(ClientObjectExtensions);

            var genMeth = coe.GetMethod("GetPropertyExpressions", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(typeof(T));
            var objs = genMeth.Invoke(null, new object[1] { propName });

            var loadMeth = coe.GetMethod("IsPropertyReady", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(typeof(T));
            var answer = (bool)loadMeth.Invoke(null, new object[2] { castedObj, objs });
            return answer;
        }

        public static object GenCast(object cliObj)
        {
            var genMeth = typeof(BaseSPCmdlet).GetMethod("Cast", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(cliObj.GetType());
            return genMeth.Invoke(null, new object[1] { cliObj });
        }

        private static T Cast<T>(dynamic o) => (T)o;

        public static bool TestThis<T>(T cliObj, Expression<Func<T, object>> exp)
            where T : ClientObject
        {
            return cliObj.IsPropertyReady(exp);
        }
    }
}
