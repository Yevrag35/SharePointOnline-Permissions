using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MG.SharePoint.PowerShell
{
    public class PrincipalIdentity
    {
        #region FIELDS/CONSTANTS

        #endregion

        #region PROPERTIES
        public bool IsLogonName { get; }
        public bool IsSPPrincipal { get; }
        public Principal Principal { get; }
        public string LogonName { get; set; }

        #endregion

        #region CONSTRUCTORS
        private PrincipalIdentity(string logonName)
        {
            this.LogonName = logonName;
            this.IsLogonName = true;
            this.IsSPPrincipal = false;
        }
        private PrincipalIdentity(Principal principal)
        {
            this.Principal = principal;
            this.IsSPPrincipal = true;
            this.IsLogonName = false;
        }

        #endregion

        #region METHODS
        public static implicit operator PrincipalIdentity(string logonName) => new PrincipalIdentity(logonName);
        public static implicit operator PrincipalIdentity(Principal principal) => new PrincipalIdentity(principal);

        #endregion
    }
}