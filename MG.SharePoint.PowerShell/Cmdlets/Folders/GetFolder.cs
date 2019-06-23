using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Utilities;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;

namespace MG.SharePoint.PowerShell
{
    [Cmdlet(VerbsCommon.Get, "Folder")]
    [OutputType(typeof(Folder))]
    [CmdletBinding(PositionalBinding = false)]
    public class GetFolder : BaseSPCmdlet
    {
        #region PARAMETERS
        [Parameter(Mandatory = false, Position = 0, ParameterSetName = "WithPipeline")]
        [Parameter(Mandatory = true, Position = 0, ParameterSetName = "WithoutPipeline")]
        public Identity Identity { get; set; }


        [Parameter(Mandatory = false)]
        public SPWeb Web { get; set; }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            CTX.SP1.Web.GetF
        }

        #endregion

        #region METHODS


        #endregion
    }
}
