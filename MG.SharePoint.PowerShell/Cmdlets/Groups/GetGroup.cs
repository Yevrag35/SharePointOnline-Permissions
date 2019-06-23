using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Management.Automation;
using System.Reflection;

namespace MG.SharePoint.PowerShell.Cmdlets.Groups
{
    [Cmdlet(VerbsCommon.Get, "Group", ConfirmImpact = ConfirmImpact.None, DefaultParameterSetName = "None")]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(Group))]
    public class GetGroup : BaseSPCmdlet
    {
        #region PRIVATE FIELDS/CONSTANTS
        private const string SYSTEM_ACCOUNT = "System Account";

        #endregion

        #region PARAMETERS
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "ByGroup")]
        public Group Group { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "ByGroupCollection", Position = 0)]
        public GroupCollection GroupCollection { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "ByWeb")]
        public Web Web { get; set; }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing() => base.BeginProcessing();

        protected override void ProcessRecord()
        {
            if (ParameterSetName == "None")
            {
                this.GroupCollection = CTX.SP1.Web.SiteGroups;
            }
            else if (this.Web != null)
            {
                this.GroupCollection = this.Web.SiteGroups;
            }
            
            if (this.Group != null)
            {
                this.Group.LoadGroupProps();
                base.WriteObject(this.Group);
            }
            else if (this.GroupCollection != null)
            {
                this.GroupCollection.LoadAllGroups();
                for (int i = 0; i < this.GroupCollection.Count; i++)
                {
                    Group g = this.GroupCollection[i];
                    if (!string.IsNullOrEmpty(g.OwnerTitle) && g.OwnerTitle != SYSTEM_ACCOUNT)
                        g.LoadOwner();

                    base.WriteObject(g);
                }

                base.WriteObject(this.GroupCollection, true);
            }
        }

        #endregion
    }
}