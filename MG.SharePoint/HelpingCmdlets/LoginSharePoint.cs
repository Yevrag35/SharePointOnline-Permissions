using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.SharePoint.Client;
using System;
using System.Management.Automation;

namespace MG.SharePoint.HelpingCmdlets
{
    [Cmdlet("Login", "SharePoint", DefaultParameterSetName = "ByAzureLogin")]
    [OutputType(typeof(bool))]
    [CmdletBinding(PositionalBinding = false)]
    public class LoginSharePoint : PSCmdlet
    {
        [Parameter(Mandatory = false, Position = 0)]
        public string TenantName = "yevrag35";

        [Parameter(Mandatory = false, Position = 1)]
        public string DestinationSite = string.Empty;

        [Parameter(Mandatory = false, ParameterSetName = "ByAzureLogin")]
        public PromptBehavior PromptBehavior = PromptBehavior.Auto;

        [Parameter(Mandatory = true, ParameterSetName = "ByExplicitLogin")]
        public PSCredential Credential { get; set; }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            switch (ParameterSetName)
            {
                case "ByAzureLogin":
                    bool res1 = CTX.Login(TenantName, DestinationSite, PromptBehavior);
                    WriteObject(res1);
                    break;
                default:
                    bool res2 = CTX.Login(TenantName, DestinationSite, Credential);
                    WriteObject(res2);
                    break;
            }
        }
    }
}
