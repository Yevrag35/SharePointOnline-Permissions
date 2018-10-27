using Microsoft.SharePoint.Client;
using System;
using System.Management.Automation;

namespace MG.SharePoint.PowerShell
{
    [Cmdlet(VerbsCommon.Get, "SPWeb")]
    [OutputType(typeof(Web))]
    public class GetSPWeb : PSCmdlet
    {
        private protected bool _noex;
        [Parameter(Mandatory = false)]
        public SwitchParameter NoExecute
        {
            get => _noex;
            set => _noex = value;
        }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            if (!CTX.Connected)
                throw new InvalidOperationException("SPO Context is not set");
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            Web web = CTX.SP1.Web;
            if (!_noex)
            {
                CTX.Lae(web, true);
            }
            WriteObject(web, false);
        }
    }
}
