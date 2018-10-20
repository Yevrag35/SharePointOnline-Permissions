using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.SharePoint.Client;
using System;
using System.Management.Automation;

namespace MG.SharePoint.HelpingCmdlets
{
    [Cmdlet("Login", "SharePoint", DefaultParameterSetName = "ByAzureLogin")]
    [OutputType(typeof(Web))]
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
                    Web web1 = CTX.Login(TenantName, DestinationSite, PromptBehavior);
                    WriteObject(web1);
                    break;
                default:
                    Web web2 = CTX.Login(TenantName, DestinationSite, Credential);
                    WriteObject(web2);
                    break;
            }
        }
    }
}
