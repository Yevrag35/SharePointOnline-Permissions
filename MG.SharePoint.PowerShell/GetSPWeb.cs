using Microsoft.SharePoint.Client;
using System;
using System.Management.Automation;

namespace MG.SharePoint.PowerShell
{
    [Cmdlet(VerbsCommon.Get, "SPWeb")]
    [OutputType(typeof(SPWeb))]
    public class GetSPWeb : PSCmdlet
    {
        private protected bool _withPerms;
        [Parameter(Mandatory = false)]
        public SwitchParameter LoadPermissions
        {
            get => _withPerms;
            set => _withPerms = value;
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
            var web = new SPWeb();
            if (_withPerms)
                web.GetPermissions();

            WriteObject(web, false);
        }
    }
}
