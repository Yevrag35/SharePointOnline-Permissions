using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Online.SharePoint.PowerShell.Resources;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Utilities;
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
    public sealed class CmdLetContext : ClientContext
    {
        // Fields
        private const string USER_AGENT_STRING_FORMAT = "SharePoint Online PowerShell ({0})";
        private const string CLIENT_TAG_FORMAT = "TAPS ({0})";
        private static string s_userAgent = string.Empty;
        private static string s_clientTag = string.Empty;
        private PSHost m_powerShellHost;

        // Methods
        public CmdLetContext(string webFullUrl, CmdLetContext rootContext) : base(webFullUrl)
        {
            base.ClientTag = GetClientTag();
            ApplicationName = GetUserAgent();
            OAuthSession = rootContext.OAuthSession;
            base.ExecutingWebRequest += new EventHandler<WebRequestEventArgs>(this.CmdLetContext_ExecutingWebRequest);
        }

        public CmdLetContext(string webFullUrl, PSHost host, string clientTag) : base(webFullUrl)
        {
            this.Host = host;
            base.ClientTag = GetClientTag();
            base.ClientTag = GetClientTag() + (clientTag ?? "");
            base.ApplicationName = GetUserAgent();
            base.ExecutingWebRequest += new EventHandler<WebRequestEventArgs>(this.CmdLetContext_ExecutingWebRequest);
        }

        public CmdLetContext(Uri webFullUrl, PSHost host, string clientTag) : base(webFullUrl)
        {
            this.Host = host;
            base.ClientTag = GetClientTag() + (clientTag ?? "");
            base.ApplicationName = GetUserAgent();
        }

        private void CmdLetContext_ExecutingWebRequest(object sender, WebRequestEventArgs e)
        {
            if (this.OAuthSession != null)
            {
                e.WebRequestExecutor.RequestHeaders.Add(HttpRequestHeader.Authorization, 
                    this.OAuthSession.GetAuthorizationHeaderValue());
            }
        }

        internal static string GetClientTag()
        {
            if (string.IsNullOrWhiteSpace(s_clientTag))
            {
                s_clientTag = string.Format(CultureInfo.InvariantCulture, 
                    "TAPS ({0})", 
                    new object[] { GetVersionString() });
            }
            return s_clientTag;
        }

        internal static string GetUserAgent()
        {
            if (string.IsNullOrWhiteSpace(s_userAgent))
            {
                s_userAgent = string.Format(CultureInfo.InvariantCulture, 
                    "SharePoint Online PowerShell ({0})", 
                    new object[] { GetVersionString() });
            }
            return s_userAgent;
        }

        private static string GetVersionString() =>
            OfficeVersion.FullBuildBase;

        // Properties
        internal PSHost Host
        {
            get =>
                this.m_powerShellHost;
            private set =>
                this.m_powerShellHost = this.Host;
        }

        public OAuthSession OAuthSession { get; set; }

        internal bool ServerSupportsGroupIdFilter =>
            base.ServerVersion >= new Version(0x10, 0, 0x1e1c, 0x4b0);
    }
}
