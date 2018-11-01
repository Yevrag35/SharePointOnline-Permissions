using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Online.SharePoint.PowerShell.Resources;
using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Utilities;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Net;

namespace MG.SharePoint
{
    public enum AADCrossTenantAuthenticationLocation
    {
        Default,
        ITAR,
        Germany,
        China
    }

    public class SPOServiceHelper : IServiceHelper
    {
        // Fields
        private const string COMMON_AUTH_URL = "https://login.microsoftonline.com/common";
        private const string COMMON_PPE_AUTH_URL = "https://login.windows-ppe.net/common";
        private const string ITAR_AUTH_URL = "https://login.microsoftonline.us/common";
        private const string GERMANY_AUTH_URL = "https://login.microsoftonline.de/common";
        private const string CHINA_AUTH_URL = "https://login.chinacloudapi.cn/common";
        internal string HEADER_SHAREPOINT_VERSION = "MicrosoftSharePointTeamServices";
        private const string TENANT_TEMP = "https://login.microsoftonline.com/{0}";

        public SPOServiceHelper() { }

        // Methods
        private string ConvertAuthenticationLocation2Url(AADCrossTenantAuthenticationLocation region, Uri url)
        {
            switch (region)
            {
                case AADCrossTenantAuthenticationLocation.ITAR:
                    return "https://login.microsoftonline.us/common";

                case AADCrossTenantAuthenticationLocation.Germany:
                    return "https://login.microsoftonline.de/common";

                case AADCrossTenantAuthenticationLocation.China:
                    return "https://login.chinacloudapi.cn/common";
            }
            return url.Host.ToLower().EndsWith("spoppe.com")
                ? "https://login.windows-ppe.net/common"
                : "https://login.microsoftonline.com/common";
        }

        public SPOService SwitchContext(string newWebUrl, CmdLetContext currentContext)
        {
            if (!newWebUrl.EndsWith("/"))
                newWebUrl = newWebUrl + "/";

            OAuthSession oauth = currentContext.OAuthSession;
            var newContext = new CmdLetContext(newWebUrl, null, null);
            if (currentContext.Credentials == null)
            {
                newContext.OAuthSession = oauth;
                newContext.OAuthSession.EnsureValidAuthToken();
            }
            else
                newContext.Credentials = currentContext.Credentials;

            return new SPOService(newContext);
        }

        public SPOService InstantiateSPOService(Uri url, string loginUrl,PSCredential credentials, PromptBehavior? behavior) =>
            InstantiateSPOService(url, loginUrl, credentials, COMMON_AUTH_URL, behavior);

        public SPOService InstantiateSPOService(Uri url, string loginUrl, PSCredential credentials, 
            string authenticationUrl = COMMON_AUTH_URL, PromptBehavior? behavior = null)
        {
            if (!IsValidServerVersion(url))
            {
                throw new InvalidOperationException(StringResourceManager.GetResourceString(
                    "ValidateServerVersionInvalidVersion", 
                    new object[0]));
            }
            var context = new CmdLetContext(url.ToString(), null, null); // this is where the site "sites/Clients" is set.
            if (credentials == null)
            {
                var session = new OAuthSession(authenticationUrl);
                session.SignIn(loginUrl, behavior.Value);                      // the login Url is the base site though.
                context.OAuthSession = session;
            }
            else
            {
                var credentials2 = new SharePointOnlineCredentials(credentials.UserName, credentials.Password);
                context.Credentials = credentials2;
            }
            return new SPOService(context);
        }

        internal bool IsTenantAdminSite(CmdLetContext context)
        {
            try
            {
                new Tenant(context);
                context.ExecuteQuery();
                return true;
            }
            catch (ServerUnauthorizedAccessException)
            {
                return false;
            }
        }

        internal bool IsValidServerVersion(Uri url)
        {
            using (var client = new WebClient())
            {
                client.Headers[HttpRequestHeader.UserAgent] = CmdLetContext.GetUserAgent();
                string str = null;
                try
                {
                    client.DownloadData(url);
                    str = client.ResponseHeaders[HEADER_SHAREPOINT_VERSION];
                }
                catch (WebException exception)
                {
                    if ((exception != null) && (exception.Response != null))
                    {
                        str = exception.Response.Headers[HEADER_SHAREPOINT_VERSION];
                    }
                }
                if (str == null)
                {
                    throw new InvalidOperationException(StringResourceManager.GetResourceString("ValidateServerVersionCouldntConnectToUri", new object[0]));
                }
                if (!string.IsNullOrEmpty(str))
                {
                    string input = str.Split(new char[] { ',' })[0];
                    if (Version.TryParse(input, out Version version))
                    {
                        return (version.Major >= 15);
                    }
                }
            }
            return false;
        }
    }
}
