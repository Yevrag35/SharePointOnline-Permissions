using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Online.SharePoint.PowerShell.Resources;
using Microsoft.SharePoint.Client;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Net;
using System.Reflection;

namespace MG.SharePoint.Backend
{
    public sealed class Test : ClientContext
    {
        private static readonly string FullBuildVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;

        private const string DEFAULT_CLIENT_ID = "9bc3ab49-b65d-410a-85ad-de819febfddc";
        private const string DEFAULT_REDIRECT_URI = "https://oauth.spops.microsoft.com";
        private const int REFRESH_BEFORE_EXPIRATION_SECONDS = 30;
        private const string DEFAULT_AUTHORITY = "https://login.microsoftonline.com/common/oauth2/token";
        private AuthenticationContext _ctx;
        public Uri ResourceUrl { get; private set; }
        public AuthenticationResult Result { get; set; }

        static Test() { }

        public Test(string webFullUrl, PromptBehavior behavior)
            : base(webFullUrl)
        {
            base.ClientTag = string.Empty;
            _ctx = new AuthenticationContext(DEFAULT_AUTHORITY, true);
            this.SignIn(webFullUrl, behavior);
            
            base.ExecutingWebRequest += new EventHandler<WebRequestEventArgs>(this.Test_ExecutingWebRequest);
        }

        public void SignIn(string sharePointUrl, PromptBehavior promptBehavior)
        {
            Uri uri = new Uri(DEFAULT_REDIRECT_URI);
            try
            {
                this.Result = _ctx.AcquireToken(sharePointUrl, DEFAULT_CLIENT_ID, uri, promptBehavior);
                this.ResourceUrl = uri;
            }
            catch (AdalException)
            {
                throw new AuthenticationException(StringResourceManager.GetResourceString("OAuthCouldntSignIn", new object[1] { uri }));
            }
        }

        private void Test_ExecutingWebRequest(object sender, WebRequestEventArgs e)
        {
            if (this.Result != null)
            {
                e.WebRequestExecutor.RequestHeaders.Add(HttpRequestHeader.Authorization, this.Result.CreateAuthorizationHeader());
            }
        }
    }
}
