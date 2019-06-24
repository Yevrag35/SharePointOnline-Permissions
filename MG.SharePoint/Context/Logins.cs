using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.SharePoint.Client;
using System;
using System.Management.Automation;

namespace MG.SharePoint
{
    public static partial class CTX
    {
        private const string baseFormat = "https://{0}.sharepoint.com";
        private const string subFormat = baseFormat + "{1}";
        private const string COMMON_AUTH_URL = "https://login.microsoftonline.com/common";

        public static IServiceHelper Helper;

        #region Re-Login Method

        public static void NewClientContext(string serverRelativeUrl)
        {
            if (SP1 == null)
                throw new InvalidOperationException("You need to login first.");

            var currentUrl = new Uri(SP1.Url, UriKind.Absolute);
            string uri = currentUrl.PathAndQuery;
            if (string.IsNullOrEmpty(serverRelativeUrl))
                serverRelativeUrl = "/";
            else if (!serverRelativeUrl.StartsWith("/"))
                serverRelativeUrl = "/" + serverRelativeUrl;

            if (string.Equals(serverRelativeUrl, uri, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("The current context is already what is specified.");

            SPOService spo = NewContext(serverRelativeUrl);
            SP1 = spo.Context;
        }

        private static SPOService NewContext(string incoming)
        {
            if (incoming == "/")
                incoming = string.Empty;

            var wholeThing = new Uri(SP1.Url, UriKind.Absolute);
            string hostOnly = wholeThing.Scheme + "//" + wholeThing.Host;
            SPOService service = Helper.SwitchContext(hostOnly + incoming, SP1);
            return service;
        }

        #endregion

        #region Login Methods
        public static bool Login(string tenantName, string destUrl) =>
            Login(tenantName, destUrl, PromptBehavior.Auto);

        public static bool Login(string tenantName, string destUrl, PromptBehavior behavior)
        {
            if (Helper == null)
                Helper = new SPOServiceHelper();

            if (string.IsNullOrEmpty(tenantName) && !string.IsNullOrEmpty(SpecifiedTenantName))
                tenantName = SpecifiedTenantName;

            if (!string.IsNullOrEmpty(destUrl) && !destUrl.StartsWith("/"))
                destUrl = "/" + destUrl;

            string baseLogin = string.Format(baseFormat, tenantName);
            var destSite = new Uri(string.Format(subFormat, tenantName, destUrl));
            try
            {
                SPOService service = Helper.InstantiateSPOService(destSite, baseLogin, null, COMMON_AUTH_URL, behavior);
                SP1 = service.Context;
            }
            catch
            {
                return false;
            }
            SpecifiedTenantName = tenantName;
            return Connected;
        }

        public static bool Login(string tenantName, string destUrl, PSCredential credential)
        {
            if (Helper == null)
                Helper = new SPOServiceHelper();

            string baseLogin = string.Format(baseFormat, tenantName);
            var destSite = new Uri(string.Format(subFormat, tenantName, destUrl));
            try
            {
                SPOService service = Helper.InstantiateSPOService(destSite, baseLogin, credential, null, null);
                SP1 = service.Context;
            }
            catch
            {
                return false;
            }
            SpecifiedTenantName = tenantName;
            return Connected;
        }
        #endregion
    }
}
