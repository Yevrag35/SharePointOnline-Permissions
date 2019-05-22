using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.SharePoint.Client;
using System;
using System.Management.Automation;

namespace MG.SharePoint.PowerShell
{
    [Cmdlet(VerbsCommunications.Connect, "ToSharePoint", DefaultParameterSetName = "ByAzureLogin")]
    [Alias("Login-SharePoint", "loginsp")]
    [CmdletBinding(PositionalBinding = false)]
    public class LoginSharePoint : PSCmdlet
    {
        private protected const string DEFAULT_CLIENT_ID = "9bc3ab49-b65d-410a-85ad-de819febfddc";
        private protected const string DEFAULT_REDIRECT_URI = "https://oauth.spops.microsoft.com";

        [Parameter(Mandatory = false, Position = 0)]
        public string TenantName = "yevrag35";

        [Parameter(Mandatory = false, Position = 1)]
        public string DestinationSite = string.Empty;

        [Parameter(Mandatory = false, ParameterSetName = "ByAzureLogin")]
        public PromptBehavior PromptBehavior = PromptBehavior.Auto;

        [Parameter(Mandatory = true, ParameterSetName = "ByExplicitLogin")]
        public PSCredential Credential { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter PassThru { get; set; }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            bool result = false;
            switch (ParameterSetName)
            {
                case "ByAzureLogin":
                    result = CTX.Login(TenantName, DestinationSite, PromptBehavior);
                    break;
                default:
                    result = CTX.Login(TenantName, DestinationSite, Credential);
                    break;
            }
            if (!result)
                throw new Exception("Something went trying to authenticate!");

            else if (PassThru)
                WriteObject((SPWeb)CTX.SP1.Web);
        }
    }
}
