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

    public class SPOServiceHelper
    {
        // Fields
        private const string COMMON_AUTH_URL = "https://login.microsoftonline.com/common";
        private const string COMMON_PPE_AUTH_URL = "https://login.windows-ppe.net/common";
        private const string ITAR_AUTH_URL = "https://login.microsoftonline.us/common";
        private const string GERMANY_AUTH_URL = "https://login.microsoftonline.de/common";
        private const string CHINA_AUTH_URL = "https://login.chinacloudapi.cn/common";
        internal static string HEADER_SHAREPOINT_VERSION = "MicrosoftSharePointTeamServices";

        //private static readonly string loginUrl = "https://dgrsystems.sharepoint.com";

        // Methods
        private static string ConvertAuthenticationLocation2Url(AADCrossTenantAuthenticationLocation region, Uri url)
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
            if (url.Host.ToLower().EndsWith("spoppe.com"))
            {
                return "https://login.windows-ppe.net/common";
            }
            return "https://login.microsoftonline.com/common";
        }

        internal static SPOService InstantiateSPOService(Uri url, string loginUrl,PSCredential credentials, PSHost host, 
            PromptBehavior behavior) =>
            InstantiateSPOService(url, loginUrl, credentials, host, COMMON_AUTH_URL, behavior);

        internal static SPOService InstantiateSPOService(Uri url, string loginUrl, PSCredential credentials, PSHost host, 
            string authenticationUrl, PromptBehavior behavior)
        {
            if (!IsValidServerVersion(url))
            {
                throw new InvalidOperationException(StringResourceManager.GetResourceString(
                    "ValidateServerVersionInvalidVersion", 
                    new object[0]));
            }
            bool flag = true;
            bool flag2 = false;
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\SPO\CMDLETS\");
            if (key != null)
            {
                flag = Convert.ToUInt32(key.GetValue("UseOrgID", 1)) != 0;
                flag2 = Convert.ToUInt32(key.GetValue("ForceOAuth", 1)) != 0;
            }
            var context = new CmdLetContext(url.ToString(), host, null); // this is where the site "/Clients" is set.
            if (credentials == null)
            {
                var session = new OAuthSession(authenticationUrl);
                session.SignIn(loginUrl, behavior);                      // the login Url is the base site though.
                context.OAuthSession = session;
            }
            else if (flag2)
            {
                var session2 = new OAuthSession(authenticationUrl);
                session2.SignIn(loginUrl, credentials);
                context.OAuthSession = session2;
            }
            else if (flag)
            {
                var credentials2 = new SharePointOnlineCredentials(credentials.UserName, credentials.Password);
                context.Credentials = credentials2;
            }
            return new SPOService(context);
        }

        internal static bool IsTenantAdminSite(CmdLetContext context)
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

        internal static bool IsValidServerVersion(Uri url)
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
