using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Online.SharePoint.PowerShell.Resources;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Management.Automation;
using System.Net;

namespace MG.SharePoint
{
    public class OAuthSession
    {
        // Fields
        private const string CLIENT_ID = "9bc3ab49-b65d-410a-85ad-de819febfddc";
        private const string REDIRECT_URI = "https://oauth.spops.microsoft.com";
        private const int REFRESH_BEFORE_EXPIRATION_SECONDS = 30;
        private string crossTenantAuthenticationURL;
        private AuthenticationContext authContext;
        private AuthenticationResult authResult;
        private string resourceUrl;

        // Methods
        public OAuthSession(string crossTenantAuthenticationURL) => 
            this.crossTenantAuthenticationURL = crossTenantAuthenticationURL;

        private void EnsureValidAuthToken()
        {
            if (this.authResult == null)
            {
                throw new AuthenticationException(StringResourceManager.GetResourceString("OAuthNoSession", new object[0]));
            }
            if (this.authResult.ExpiresOn.AddSeconds(-30.0) < DateTimeOffset.Now)
            {
                if (this.authResult.RefreshToken != null)
                {
                    try
                    {
                        this.RefreshAuthSession(this.authResult.RefreshToken);
                        return;
                    }
                    catch (AdalException)
                    {
                        throw new AuthenticationException(StringResourceManager.GetResourceString(
                            "OAuthCouldntRefresh", new object[] { this.resourceUrl }));
                    }
                }
                throw new AuthenticationException(StringResourceManager.GetResourceString(
                    "OAuthNoRefreshToken", new object[] { this.resourceUrl }));
            }
        }

        internal string GetAuthorizationHeaderValue()
        {
            this.EnsureValidAuthToken();
            return (this.authResult.AccessTokenType + " " + this.authResult.AccessToken);
        }

        internal string GetAuthToken()
        {
            this.EnsureValidAuthToken();
            return this.authResult.AccessToken;
        }

        private void RefreshAuthSession(string refreshToken)
        {
            this.authResult = this.AuthContext.AcquireTokenByRefreshToken(refreshToken, "9bc3ab49-b65d-410a-85ad-de819febfddc");
        }

        internal void SignIn(string resourceUrl, PromptBehavior behavior)
        {
            this.authResult = null;
            this.resourceUrl = null;
            try
            {
                this.authResult = this.AuthContext.AcquireToken(resourceUrl, "9bc3ab49-b65d-410a-85ad-de819febfddc", 
                    new Uri("https://oauth.spops.microsoft.com"), behavior);
                this.resourceUrl = resourceUrl;
            }
            catch (AdalException)
            {
                throw new AuthenticationException(StringResourceManager.GetResourceString("OAuthCouldntSignIn", new object[] { resourceUrl }));
            }
        }

        internal void SignIn(string resourceUrl, PSCredential credentials)
        {
            this.authResult = null;
            this.resourceUrl = null;
            var credential = new UserCredential(credentials.UserName, credentials.Password);
            try
            {
                this.authResult = this.AuthContext.AcquireToken(resourceUrl, "9bc3ab49-b65d-410a-85ad-de819febfddc", credential);
                this.resourceUrl = resourceUrl;
            }
            catch (AdalException)
            {
                throw new AuthenticationException(StringResourceManager.GetResourceString(
                    "OAuthCouldntSignIn", new object[] { resourceUrl }));
            }
        }

        // Properties
        internal string TenantId => this.authResult != null ? this.authResult.TenantId : null;

        internal string UserId =>  this.authResult?.UserInfo.UniqueId;

        internal string UserDisplayableId => this.authResult?.UserInfo.DisplayableId;

        private AuthenticationContext AuthContext
        {
            get
            {
                if (this.authContext == null)
                {
                    this.authContext = new AuthenticationContext(this.crossTenantAuthenticationURL);
                }
                return this.authContext;
            }
        }
    }

    public class AuthenticationException : Exception
    {
        // Methods
        public AuthenticationException(string message) : base(message)
        {
        }

        public AuthenticationException(string message, Exception iex) : base(message, iex)
        {
        }
    }
}
