using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Security;

namespace MG.SharePoint.PowerShell
{
    [Cmdlet(VerbsCommon.Get, "Permission", ConfirmImpact = ConfirmImpact.None)]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(SPPermission))]
    public class GetPermission : PSCmdlet
    {
        #region FIELDS/CONSTANTS


        #endregion

        #region PARAMETERS
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public SecurableObject InputObject { get; set; }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing() => base.BeginProcessing();

        protected override void ProcessRecord()
        {
            string nameProp = this.GetNameAndIdProperty(this.InputObject.GetType().FullName);
            base.WriteObject(this.InputObject.GetPermissions(nameProp, "Id"), true);
        }

        #endregion

        #region METHODS
        private string GetNameAndIdProperty(string typeName)
        {
            switch (typeName)
            {
                case "Microsoft.SharePoint.Client.ListItem":
                {
                    return "DisplayName";
                }

                default:
                {
                    return "Title";
                }
            }
        }

        #endregion
    }
}