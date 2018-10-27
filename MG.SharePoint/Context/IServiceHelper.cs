using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.SharePoint.Client;
using System;
using System.Management.Automation;
using System.Management.Automation.Host;

namespace MG.SharePoint
{
    public interface IServiceHelper
    {
        SPOService InstantiateSPOService(Uri destinationUrl, string loginUrl, PSCredential credential, string authenticationUrl, PromptBehavior? behavior);
    }
}
