using Microsoft.Online.SharePoint.PowerShell.Resources;
using System;
using System.Management.Automation;
using System.Management.Automation.Host;

namespace MG.SharePoint
{
    public class CredentialCmdletPipeBind : CmdletPipeBind<PSCredential>
    {
        // Fields
        private PSCredential m_credential;
        private string m_userName;

        // Methods
        public CredentialCmdletPipeBind(PSCredential inputObject) : base(inputObject)
        {
        }

        public CredentialCmdletPipeBind(string inputObject)
        {
            this.m_userName = inputObject;
        }

        protected override void Discover(PSCredential instance)
        {
            if (instance != null)
            {
                this.m_userName = instance.UserName;
                this.m_credential = instance;
            }
            else
            {
                this.m_userName = null;
                this.m_credential = null;
            }
        }

        public static bool IsCredentialValid(PSCredential credential, PSHost host)
        {
            if (host == null)
            {
                throw new ArgumentNullException("host");
            }
            if ((credential == null) || ((credential.Password != null) && (credential.Password.Length != 0)))
            {
                return true;
            }
            host.UI.WriteWarningLine(StringResourceManager.GetResourceString("AuthenticationHelperStrAuthenticateEmptyPassword", new object[0]));
            return false;
        }

        public static PSCredential PromptForCredentials(PSHost host, string userName)
        {
            if (host == null)
            {
                throw new ArgumentNullException("host");
            }
            PSCredential credential = null;
            do
            {
                credential = host.UI.PromptForCredential(StringResourceManager.GetResourceString("CmdletContextAuthenticationTitle", new object[0]), StringResourceManager.GetResourceString("CmdletContextAuthenticationMessage", new object[0]), (credential == null) ? userName : credential.UserName, string.Empty);
            }
            while (!IsCredentialValid(credential, host));
            return credential;
        }

        public override PSCredential Read()
        {
            if (this.m_credential == null)
            {
                throw new InvalidOperationException(StringResourceManager.GetResourceString("CredentialCmdletPipebindReadNoPshost", new object[0]));
            }
            return this.m_credential;
        }

        public PSCredential Read(PSHost host)
        {
            if ((this.m_credential == null) || !IsCredentialValid(this.m_credential, host))
            {
                this.m_credential = PromptForCredentials(host, this.m_userName);
            }
            return this.m_credential;
        }
    }


}
