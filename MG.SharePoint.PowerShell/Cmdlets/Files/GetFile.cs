using Microsoft.SharePoint.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Security;

namespace MG.SharePoint.PowerShell.Cmdlets.Files
{
    [Cmdlet(VerbsCommon.Get, "File", ConfirmImpact = ConfirmImpact.None, DefaultParameterSetName = "ByWebInput")]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(File))]
    public class GetFile : PSCmdlet
    {
        #region FIELDS/CONSTANTS


        #endregion

        #region PARAMETERS
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "ByListItemInput")]
        public ListItem ListItem { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "ByFileInput")]
        public File File { get; set; }

        [Parameter(Mandatory = true, DontShow = true, ParameterSetName = "ByFileCollection")]
        public FileCollection FileCollection { get; set; }

        //[Parameter(Mandatory = false, ValueFromPipeline = true, ParameterSetName = "ByWebInput")]


        #endregion

        #region DYNAMIC PARAMETERS


        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing() => base.BeginProcessing();

        protected override void ProcessRecord()
        {
            CTX.SP1.Web.GetFile
            if (this.File != null)
            {
                this.File.LoadFileProps();
                base.WriteObject(this.File);
            }
        }

        #endregion

        #region METHODS


        #endregion
    }
}