using System;
using System.Management.Automation;

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
    }
}
