using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;

namespace MG.SharePoint.PowerShell.Cmdlets.ListItems
{
    [Cmdlet(VerbsCommon.Get, "ListItem", ConfirmImpact = ConfirmImpact.None, DefaultParameterSetName = "ByTitle")]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(ListItem))]
    public class GetListItem : BaseSPCmdlet
    {
        #region PRIVATE FIELDS/CONSTANTS


        #endregion

        #region PARAMETERS
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "ByListItemInput")]
        public ListItem InputObject { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "ByItemGuid")]
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "ByItemId")]
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "ByTitle")]
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "ByAndConditions")]
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "ByOrConditions")]
        public List List { get; set; }

        [Parameter(Mandatory = false, Position = 0, HelpMessage = "The 'title' of the list item.  Not to be confused with its 'Name'.",
            ParameterSetName = "ByTitle")]
        public string[] Title { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "ByItemId")]
        public int[] ItemId { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "ByItemGuid")]
        public Guid[] ItemUniqueId { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "ByAndConditions")]
        public IDictionary AndConditions { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "ByOrConditions")]
        public string OrFieldName { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "ByOrConditions")]
        public ICollection<string> OrConditions { get; set; }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing() => base.BeginProcessing();

        protected override void ProcessRecord()
        {
            if (this.InputObject != null)
            {
                this.InputObject.LoadListItemProps();
                base.WriteObject(this.InputObject);
            }
            else
            {
                switch (ParameterSetName)
                {
                    case "ByAndConditions":
                    {
                        break;
                    }

                    case "ByOrConditions":
                    {

                        break;
                    }

                    default:
                    {
                        break;
                    }
                }
            }
        }

        #endregion

        #region CMDLET METHODS
        public static Query GetAndQuery(IDictionary hashtable) => new Query(hashtable);
        public static Query GetOrQuery(string fieldName, ICollection<string> colStrs) => new Query(fieldName, colStrs);

        #endregion
    }
}