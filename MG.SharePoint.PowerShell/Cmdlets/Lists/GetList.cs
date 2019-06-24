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

        [Parameter(Mandatory = false, Position = 0)]
        [Alias("ListId", "Name", "Url")]
        public Identity[] Identity { get; set; }

        [Parameter(Mandatory = false)]
        public Web Web { get; set; }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            if (this.Web == null)
            {
                this.Web = CTX.SP1.Web;
            }
        }

        protected override void ProcessRecord()
        {
            if (this.InputObject != null)
            {
                this.InputObject.LoadListProps();
                base.WriteObject(this.InputObject);
            }
            else if (this.Identity != null && this.Identity.Length > 0)
            {
                for (int i = 0; i < this.Identity.Length; i++)
                {
                    List list = null;
                    Identity id = this.Identity[i];
                    if (id.IsString)
                    {
                        list = this.Web.Lists.GetByTitle((string)id);
                    }
                    else if (id.IsUrl && id.UriKind == UriKind.Relative)
                    {
                        list = this.Web.GetList(((Uri)id).ToString());
                    }
                    else if (id.IsGuid)
                    {
                        list = this.Web.Lists.GetById((Guid)id);
                    }

                    if (list != null)
                    {
                        try
                        {
                            list.LoadListProps();
                            base.WriteObject(list);
                        }
                        catch (ServerException sex)
                        {
                            base.WriteError(sex, ErrorCategory.ObjectNotFound);
                        }
                    }
                }
            }
            else
            {
                this.Web.Lists.LoadAllLists();
                base.WriteObject(this.Web.Lists, true);
            }
        }

        #endregion

        #region CMDLET METHODS


        #endregion
    }
}