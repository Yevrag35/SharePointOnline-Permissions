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
        private const string DEFAULT_CLIENT_ID = "9bc3ab49-b65d-410a-85ad-de819febfddc";
        private const string DEFAULT_REDIRECT_URI = "https://oauth.spops.microsoft.com";
        private const int REFRESH_BEFORE_EXPIRATION_SECONDS = 30;
        private protected const string DEFAULT_AUTHORITY = "https://login.microsoftonline.com/common/oauth2/token";
        private protected string crossTenantAuthenticationURL;
        private AuthenticationContext authContext;
        public AuthenticationResult authResult;
        private string resourceUrl;

        // Methods
        public OAuthSession(string crossTenantAuthenticationURL = DEFAULT_AUTHORITY) => 
            this.crossTenantAuthenticationURL = crossTenantAuthenticationURL;

        public void EnsureValidAuthToken(string clientId = DEFAULT_CLIENT_ID)
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
                        this.RefreshAuthSession(this.authResult.RefreshToken, clientId);
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

        public string GetAuthToken()
        {
            this.EnsureValidAuthToken();
            return this.authResult.AccessToken;
        }

        internal void RefreshAuthSession(string refreshToken, string clientId = DEFAULT_CLIENT_ID)
        {
            this.authResult = this.AuthContext.AcquireTokenByRefreshToken(refreshToken, clientId);
        }

        public void SignIn(string resourceUrl, PromptBehavior behavior, string clientId = DEFAULT_CLIENT_ID, string redirectUri = DEFAULT_REDIRECT_URI)
        {
            this.authResult = null;
            this.resourceUrl = null;
            try
            {
                this.authResult = this.AuthContext.AcquireToken(resourceUrl, clientId, 
                    new Uri(redirectUri), behavior);
                this.resourceUrl = resourceUrl;
            }
            catch (AdalException)
            {
                throw new AuthenticationException(StringResourceManager.GetResourceString("OAuthCouldntSignIn", new object[] { resourceUrl }));
            }
        }

        public void SignIn(string resourceUrl, PSCredential credentials, string clientId = DEFAULT_CLIENT_ID)
        {
            this.authResult = null;
            this.resourceUrl = null;
            var credential = new UserCredential(credentials.UserName, credentials.Password);
            try
            {
                this.authResult = this.AuthContext.AcquireToken(resourceUrl, clientId, credential);
                this.resourceUrl = resourceUrl;
            }
            catch (AdalException)
            {
                throw new AuthenticationException(StringResourceManager.GetResourceString(
                    "OAuthCouldntSignIn", new object[] { resourceUrl }));
            }
        }

        // Properties
        public string TenantId => this.authResult != null ? this.authResult.TenantId : null;

        public string UserId =>  this.authResult?.UserInfo.UniqueId;

        public string UserDisplayableId => this.authResult?.UserInfo.DisplayableId;

        public AuthenticationContext AuthContext
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
