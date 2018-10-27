using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Management.Automation;

namespace MG.SharePoint.PowerShell
{
    [Cmdlet("Login", "SharePoint", DefaultParameterSetName = "ByAzureLogin")]
    [OutputType(typeof(bool))]
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

        [Parameter(Mandatory = false, ParameterSetName = "ByAzureLogin")]
        public Guid ApplicationId = Guid.Parse(DEFAULT_CLIENT_ID);

        [Parameter(Mandatory = false, ParameterSetName = "ByAzureLogin")]
        public Uri RedirectUrl = new Uri(DEFAULT_REDIRECT_URI, UriKind.Absolute);

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            CTX.Helper = new PSCtxHelper(ApplicationId.ToString(), RedirectUrl);

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
