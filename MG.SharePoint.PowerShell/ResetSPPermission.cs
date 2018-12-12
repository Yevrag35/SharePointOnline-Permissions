using Microsoft.SharePoint.Client;
using System;
using System.Linq;
using System.Management.Automation;

namespace MG.SharePoint.PowerShell
{
    [Cmdlet(VerbsCommon.Reset, "SPPermission", SupportsShouldProcess = true)]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(SPPermission))]
    public class ResetSPPermission : GetSPPermission
    {
        private bool _force;
        [Parameter(Mandatory = false)]
        public SwitchParameter Force
        {
            get => _force;
            set => _force = value;
        }

        protected override void BeginProcessing() => base.BeginProcessing();

        protected override void ProcessRecord()
        {
            CheckParameters();

            if (_force || ShouldContinue("Reset Permissions on " + SPObject.Id.ToString() + "?", "Re-enabling Inheritance"))
                SPObject.ResetInheritance();

            WriteObject(SPObject.GetPermissions(), true);
        }
    }
}
