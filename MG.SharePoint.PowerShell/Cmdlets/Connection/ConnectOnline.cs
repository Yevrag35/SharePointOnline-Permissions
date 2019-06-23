using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace MG.SharePoint.PowerShell
{
    [Cmdlet(VerbsCommunications.Connect, "Online", DefaultParameterSetName = "ByAzureLogin")]
    [Alias("Login-SharePoint", "loginsp")]
    [CmdletBinding(PositionalBinding = false)]
    public class ConnectOnline : PSCmdlet
    {
        private const string DEFAULT_CLIENT_ID = "9bc3ab49-b65d-410a-85ad-de819febfddc";
        private const string DEFAULT_REDIRECT_URI = "https://oauth.spops.microsoft.com";

        [Parameter(Mandatory = true, Position = 0)]
        public string TenantName { get; set; }

        [Parameter(Mandatory = false, Position = 1)]
        public string DestinationSite = string.Empty;

        [Parameter(Mandatory = false, ParameterSetName = "ByAzureLogin")]
        public PromptBehavior PromptBehavior = PromptBehavior.Auto;

        [Parameter(Mandatory = true, ParameterSetName = "ByExplicitLogin")]
        public PSCredential Credential { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter PassThru { get; set; }

        protected override void BeginProcessing()
        {
            if (this.TenantName.Contains("."))
            {
                string[] split = this.TenantName.Split(new string[1] { "." }, StringSplitOptions.RemoveEmptyEntries);
                this.TenantName = split.Reverse().Take(2).Last();
            }
        }

        protected override void ProcessRecord()
        {
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
            {
                Web web = CTX.SP1.Web;
                web.LoadWeb();
                base.WriteObject(web);
            }
        }
    }
}
