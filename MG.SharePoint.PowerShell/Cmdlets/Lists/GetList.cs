using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;

namespace MG.SharePoint.PowerShell.Cmdlets.Lists
{
    [Cmdlet(VerbsCommon.Get, "List", ConfirmImpact = ConfirmImpact.None, DefaultParameterSetName = "None")]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(List))]
    public class GetList : BaseSPCmdlet
    {
        #region PRIVATE FIELDS/CONSTANTS


        #endregion

        #region PARAMETERS
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "ByListInput")]
        public List InputObject { get; set; }

        [Parameter(Mandatory = false, ParameterSetName = "ByDifferentWeb")]
        public Web Web { get; set; }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing() => base.BeginProcessing();

        protected override void ProcessRecord()
        {
            if (this.InputObject != null)
            {
                this.InputObject.LoadListProps();
                base.WriteObject(this.InputObject);
            }
            else
            {
                ListCollection listCol = CTX.SP1.Web.Lists;
                listCol.LoadAllLists();
                base.WriteObject(listCol, true);
            }
        }

        #endregion

        #region CMDLET METHODS


        #endregion
    }
}