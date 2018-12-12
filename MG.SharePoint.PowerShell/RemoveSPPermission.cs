using Microsoft.SharePoint.Client;
using System;
using System.Linq;
using System.Management.Automation;

namespace MG.SharePoint.PowerShell
{
    [Cmdlet(VerbsCommon.Remove, "SPPermission", SupportsShouldProcess = true)]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(void))]
    public class RemoveSPPermission : GetSPPermission
    {
        [Parameter(Mandatory = false, Position = 0)]
        public string Principal { get; set; }

        [Parameter(Mandatory = false)]
        public Principal SPPrincipal { get; set; }

        private bool _force;
        [Parameter(Mandatory = false)]
        public SwitchParameter Force
        {
            get => _force;
            set => _force = value;
        }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            if ((!MyInvocation.BoundParameters.ContainsKey("Principal") && !MyInvocation.BoundParameters.ContainsKey("SPPrincipal"))
                || (MyInvocation.BoundParameters.ContainsKey("Principal") && MyInvocation.BoundParameters.ContainsKey("SPPrincipal")))
                throw new ArgumentNullException("Either Principal or SPPrincipal must be specified, but not both!");

            if (MyInvocation.BoundParameters.ContainsKey("Principal"))
                SPPrincipal = CTX.SP1.Web.EnsureUser(Principal);

            CTX.Lae(SPPrincipal, true);
        }

        protected override void ProcessRecord()
        {
            base.CheckParameters();

            if (_force || ShouldContinue("Remove Permissions on " + SPObject.Id.ToString() + "?", "Modify Permissions"))
                SPObject.RemovePermission(SPPrincipal);
        }
    }
}
