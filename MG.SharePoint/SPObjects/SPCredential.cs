using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Net;
using System.Security;

namespace MG.SharePoint.Security
{
    public static class CredentialGenerator
    {
        public static ISPCredential NewCredential(string userName, SecureString pass) =>
            SPCredential.NewCreds(userName, pass);

        public static ISPCredential NewCredential(PSCredential psCreds) => SPCredential.NewCreds(psCreds);
        public static ISPCredential NewCredential(NetworkCredential netCreds) => SPCredential.NewCreds(netCreds);

        private class SPCredential : ISPCredential
        {
            #region FIELDS/CONSTANTS


            #endregion

            #region PROPERTIES
            string ISPCredential.UserName { get; set; }
            SecureString ISPCredential.Password { get; set; }

            #endregion

            #region CONSTRUCTORS
            private SPCredential(string userName, SecureString pass)
            {
                ((ISPCredential)this).UserName = userName;
                ((ISPCredential)this).Password = pass;
            }

            #endregion

            #region PUBLIC METHODS
            PSCredential ISPCredential.AsPSCredential() =>
                new PSCredential(((ISPCredential)this).UserName, ((ISPCredential)this).Password);

            internal static ISPCredential NewCreds(string userName, SecureString pass) => new SPCredential(userName, pass);
            internal static ISPCredential NewCreds(PSCredential psCreds) => new SPCredential(psCreds.UserName, psCreds.Password);
            internal static ISPCredential NewCreds(NetworkCredential netCreds) => new SPCredential(netCreds.UserName, netCreds.SecurePassword);

            #endregion
        }
    }
}