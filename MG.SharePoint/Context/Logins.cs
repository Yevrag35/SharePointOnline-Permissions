using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.SharePoint.Client;
using System;
using System.Management.Automation;

namespace MG.SharePoint
{
    public static partial class CTX
    {
        private const string baseFormat = "https://{0}.sharepoint.com";
        private const string subFormat = baseFormat + "/{1}";

        public static IServiceHelper Helper;

        #region Login Methods
        public static bool Login(string tenantName, string destUrl) =>
            Login(tenantName, destUrl, PromptBehavior.Auto);

        public static bool Login(string tenantName, string destUrl, PromptBehavior behavior)
        {
            if (Helper == null)
                Helper = new SPOServiceHelper();

            var baseLogin = string.Format(baseFormat, tenantName);
            var destSite = new Uri(string.Format(subFormat, tenantName, destUrl));
            try
            {
                var service = Helper.InstantiateSPOService(destSite, baseLogin, null, null, behavior);
                SP1 = service.Context;
            }
            catch
            {
                return false;
            }
            return Connected;
        }

        public static bool Login(string tenantName, string destUrl, PSCredential credential)
        {
            if (Helper == null)
                Helper = new SPOServiceHelper();

            var baseLogin = string.Format(baseFormat, tenantName);
            var destSite = new Uri(string.Format(subFormat, tenantName, destUrl));
            try
            {
                var service = Helper.InstantiateSPOService(destSite, baseLogin, credential, null, null);
                SP1 = service.Context;
            }
            catch
            {
                return false;
            }
            return Connected;
        }
        #endregion
    }
}
