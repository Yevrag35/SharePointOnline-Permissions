using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.SharePoint.Client;
using System;
using System.Management.Automation;

namespace MG.SharePoint.PowerShell
{
    public class PSCtxHelper : IServiceHelper
    {
        private protected readonly Guid _cid;
        private protected readonly Uri _rduri;
        private protected const string nonCommon = "https://login.microsoftonline.com/{0}/oauth2/token";

        public PSCtxHelper() { }
        public PSCtxHelper(string clientId, Uri redirectUri)
        {
            _cid = Guid.Parse(clientId);
            _rduri = redirectUri;
        }
        // The easier methods
        public SPOService NewSPOService(string destinationUrl, string loginUrl, PromptBehavior behavior, string tenantId = null) =>
            InstantiateSPOService(new Uri(destinationUrl), loginUrl, null, tenantId, behavior);

        public SPOService NewSPOService(string destinationUrl, string loginUrl, PSCredential creds) =>
            InstantiateSPOService(new Uri(destinationUrl), loginUrl, creds, null, null);

        // The 'actual' method
        public SPOService InstantiateSPOService(Uri destinationUrl, string loginUrl, PSCredential credential, string tenantId, PromptBehavior? behavior)
        {
            var context = new CmdLetContext(destinationUrl.ToString(), null, null);

            if (credential == null)
            {
                OAuthSession session = string.IsNullOrEmpty(tenantId) ? new OAuthSession() : new OAuthSession(string.Format(nonCommon, tenantId));
                if (_cid == null)
                    session.SignIn(loginUrl, behavior.Value);
                else
                    session.SignIn(loginUrl, behavior.Value, _cid.ToString(), _rduri.ToString());

                context.OAuthSession = session;
            }
            else
            {
                var spCreds = new SharePointOnlineCredentials(credential.UserName, credential.Password);
                context.Credentials = spCreds;
            }
            return new SPOService(context);
        }
    }
}
