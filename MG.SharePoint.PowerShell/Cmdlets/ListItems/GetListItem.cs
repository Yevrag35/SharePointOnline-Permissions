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
        private IEnumerable<string> _strCol;

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
        public object[] OrConditions { get; set; }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            if (this.OrConditions != null && this.OrConditions.Length > 0)
            {
                if (this.OrConditions is IEnumerable<string> strCol)
                    _strCol = strCol;

                else
                {
                    try
                    {
                        _strCol = this.OrConditions.Cast<string>();
                    }
                    catch (InvalidCastException ice)
                    {
                        throw new ArgumentException("OrConditions must be an array of strings.", ice);
                    }
                }       
            }
        }

        protected override void ProcessRecord()
        {
            if (this.InputObject != null)
            {
                this.InputObject.LoadListItemProps();
                //this.InputObject.TryLoadAttachments();
                base.WriteObject(this.InputObject);
            }
            else
            {
                object items = null;
                switch (ParameterSetName)
                {
                    case "ByAndConditions":
                    {
                        var query = new Query(this.AndConditions, this.List.Fields);
                        items = this.List.GetItems(query);
                        break;
                    }

                    case "ByOrConditions":
                    {
                        var query = new Query(this.OrFieldName, _strCol, this.List.Fields);
                        items = this.List.GetItems(query);
                        break;
                    }

                    case "ByItemId":
                    {
                        var list = new List<ListItem>(this.ItemId.Length);
                        PopulateListById(ref list, this.List, this.ItemId);
                        items = list;
                        break;
                    }

                    case "ByItemGuid":
                    {
                        var list = new List<ListItem>(this.ItemUniqueId.Length);
                        PopulateListByUniqueId(ref list, this.List, this.ItemUniqueId);
                        items = list;
                        break;
                    }
                    
                    default:
                    {
                        Query query = this.Title != null && this.Title.Length > 0 
                            ? new Query("Title", this.Title, this.List.Fields) 
                            : new Query();

                        items = this.List.GetItems(query);
                        break;
                    }
                }
                
                if (items is ListItemCollection itemCol)
                {
                    itemCol.Initialize();
                    if (itemCol.Count > 0)
                    {
                        itemCol.LoadAllListItems();
                        base.WriteObject(itemCol, true);
                    }
                }
                else if (items is List<ListItem> listItems)
                {
                    for (int i = 0; i < listItems.Count; i++)
                    {
                        ListItem li = listItems[i];
                        li.LoadListItemProps();
                        base.WriteObject(li);
                    }
                }
            }
        }

        #endregion

        #region CMDLET METHODS
        //public static Query GetAndQuery(IDictionary hashtable) => new Query(hashtable);
        //public static Query GetOrQuery(string fieldName, ICollection<string> colStrs) => new Query(fieldName, colStrs);

        private static void PopulateListById(ref List<ListItem> list, List parentList, params int[] ids)
        {
            for (int i = 0; i < ids.Length; i++)
            {
                list.Add(parentList.GetItemById(ids[i]));
            }
        }
        private static void PopulateListByUniqueId(ref List<ListItem> list, List parentList, params Guid[] uniqueIds)
        {
            for (int i = 0; i < uniqueIds.Length; i++)
            {
                list.Add(parentList.GetItemByUniqueId(uniqueIds[i]));
            }
        }

        #endregion
    }
}