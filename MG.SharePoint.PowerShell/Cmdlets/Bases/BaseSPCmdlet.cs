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
    }
}
