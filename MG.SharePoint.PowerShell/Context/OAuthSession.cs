using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Management.Automation;

namespace MG.SharePoint.PowerShell
{
    public class PSOAuthSession
    {
        // Fields
        private protected const string DEFAULT_CLIENT_ID = "9bc3ab49-b65d-410a-85ad-de819febfddc";
        private protected const string DEFAULT_REDIRECT_URI = "https://oauth.spops.microsoft.com";
        private protected const int REFRESH_BEFORE_EXPIRATION_SECONDS = 30;
        public AuthenticationContext AuthContext { get; }
        public AuthenticationResult AuthResult;
        private protected string resourceUrl;

        #region Constructors
        public PSOAuthSession(string authority) =>
            AuthContext = new AuthenticationContext(authority, true);

        public PSOAuthSession(string authority, TokenCache cache) =>
            AuthContext = new AuthenticationContext(authority, true, cache);

        #endregion

        #region Token Methods

        public string GetAuthToken() => AuthResult.AccessToken;

        private void RefreshAuthSession(string refreshToken, string clientId = DEFAULT_CLIENT_ID) =>
            AuthResult = AuthContext.AcquireTokenByRefreshToken(refreshToken, clientId);

        internal string GetAuthorizationHeaderValue() => AuthResult.AccessTokenType + " " + AuthResult.AccessToken;

        #endregion

        #region Sign-In Methods
        // Interactive Modern Authentication Sign-In
        internal void SignIn(string resourceUrl, PromptBehavior behavior, string clientId = DEFAULT_CLIENT_ID, string redirectUri = DEFAULT_REDIRECT_URI)
        {
            AuthResult = null;
            this.resourceUrl = null;
            try
            {
                AuthResult = AuthContext.AcquireToken(resourceUrl, clientId,
                    new Uri(redirectUri), behavior);
                this.resourceUrl = resourceUrl;
            }
            catch (AdalException)
            {
                throw new AuthenticationException("OAuth couldn't sign: " + resourceUrl);
            }
        }
        // Non-interactive Credential Sign-In
        internal void SignIn(string resourceUrl, PSCredential credentials, string clientId = DEFAULT_CLIENT_ID)
        {
            AuthResult = null;
            this.resourceUrl = null;
            var credential = new UserCredential(credentials.UserName, credentials.Password);
            try
            {
                AuthResult = AuthContext.AcquireToken(resourceUrl, clientId, credential);
                this.resourceUrl = resourceUrl;
            }
            catch (AdalException)
            {
                throw new AuthenticationException("OAuth couldn't sign: " + resourceUrl);
            }
        }

        #endregion

        #region Properties
        internal string TenantId => AuthResult?.TenantId;

        internal string UserId => AuthResult?.UserInfo.UniqueId;

        internal string UserDisplayableId => AuthResult?.UserInfo.DisplayableId;

        #endregion

        #region Operators
        public static 

        #endregion
    }
}
