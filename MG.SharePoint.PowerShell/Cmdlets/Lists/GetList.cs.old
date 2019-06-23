using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace MG.SharePoint.PowerShell
{
    [Cmdlet(VerbsCommon.Get, "List", ConfirmImpact = ConfirmImpact.None, DefaultParameterSetName = "None")]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(SPList))]
    public class GetList : BaseSPCmdlet
    {
        private static readonly string[] ValidSet = new string[25]
        {
            "ContentTypes", "CreatablesInfo", "CurrentChangeToken",
            "CustomActionElements", "DataSource", "DefaultView", "DefaultViewPath",
            "DescriptionResource", "EffectiveBasePermissions", "EffectiveBasePermissionsForUI",
            "EventReceivers", "Fields", "FirstUniqueAncestorSecurableObject", "Forms",
            "ImagePath", "InformationRightsManagementSettings", "Items", "ParentWeb", "ParentWebPath",
            "RoleAssignments", "RootFolder", "TitleResource", "UserCustomActions", "Views", "WorkflowAssociations"
        };

        #region PARAMETERS
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "ByMSWebInput", DontShow = true)]
        public Web Web { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "ByYEVWebInput", DontShow = true)]
        public SPWeb InputObject { get; set; }

        [Parameter(Mandatory = false, Position = 0)]
        [Alias("i")]
        public Identity Identity { get; set; }

        [Parameter(Mandatory = false)]
        [Alias("p", "Properties", "Property")]
        [ValidateSet("ContentTypes", "CreatablesInfo", "CurrentChangeToken",
            "CustomActionElements", "DataSource", "DefaultView", "DefaultViewPath",
            "DescriptionResource", "EffectiveBasePermissions", "EffectiveBasePermissionsForUI",
            "EventReceivers", "Fields", "FirstUniqueAncestorSecurableObject", "Forms",
            "ImagePath", "InformationRightsManagementSettings", "Items", "ParentWeb", "ParentWebPath",
            "RoleAssignments", "RootFolder", "TitleResource", "UserCustomActions", "Views", "WorkflowAssociations")]
        public string[] LoadExtraProperties { get; set; }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing()
        {
            //if (MyInvocation.BoundParameters.ContainsKey("Identity"))
            //    this.ValidateIdentity(this.Identity);

            base.BeginProcessing();
        }

        protected override void ProcessRecord()
        {
            var lists = this.ExecuteSearch();
            if (MyInvocation.BoundParameters.ContainsKey("LoadExtraProperties"))
                lists.LoadProperty(this.LoadExtraProperties);

            WriteObject(lists, true);
        }

        #endregion

        #region OTHER METHODS
        protected private SPListCollection ExecuteSearch()
        {
            var final = new SPListCollection();
            if (!MyInvocation.BoundParameters.ContainsKey("InputObject"))
            {
                Web workingWeb = this.GetWorkingWeb();
                IEnumerable<SPList> temp = MyInvocation.BoundParameters.ContainsKey("Identity") && this.TryFindList(this.Identity, workingWeb, out SPList spList)
                    ? (new SPList[1] { spList })
                    : (IEnumerable<SPList>)this.GetAllLists(workingWeb);
                final.AddRange(temp);
            }
            else if (MyInvocation.BoundParameters.ContainsKey("InputObject"))
            {
                bool isAvail = ((Web)this.InputObject.ShowOriginal()).Lists.AreItemsAvailable;
                if (MyInvocation.BoundParameters.ContainsKey("Identity"))
                {
                    var outList = new SPList[1];
                    if (!this.Identity.IsGuid)
                    {
                        if (!isAvail)
                        {
                            var listCol = ((Web)this.InputObject.ShowOriginal()).Lists;
                            var list = listCol.GetByTitle((string)this.Identity);
                            if (list != null)
                                outList[0] = (SPList)list;
                        }
                        else
                            outList[0] = this.InputObject.Lists.FindByName((string)this.Identity, StringComparison.CurrentCultureIgnoreCase);
                    }
                    else if (this.Identity.IsGuid)
                    {
                        if (!isAvail)
                        {
                            var listCol = ((Web)this.InputObject.ShowOriginal()).Lists;
                            var list = listCol.GetById((Guid)this.Identity);
                            if (list != null)
                                outList[0] = (SPList)list;
                        }
                    }
                    final.AddRange(outList);
                }
                else
                {
                    if (!isAvail)
                    {
                        this.InputObject.LoadProperty("Lists");
                        final = this.InputObject.Lists;
                    }
                }
            }
            return final;
        }

        //private void ValidateIdentity(object identity)
        //{
        //    if (!(identity is Guid || identity is string))
        //        throw new ArgumentException("'Identity' should be either a GUID or a list name as a string.");
        //}

        private bool TryFindList(Identity identity, Web web, out SPList outList)
        {
            bool result = false;
            SPList spl = null;
            List list = null;
            if (!identity.IsGuid)
                list = FindListByName((string)identity, web);

            else if (identity.IsGuid)
                list = FindListById((Guid)identity, web);

            if (list != null)
            {
                spl = (SPList)list;
                result = true;
            }
            outList = spl;
            return result;
        }

        protected private SPListCollection GetAllLists(SPWeb web)
        {
            if (web.Lists == null)
                web.LoadProperty("Lists");

            return web.Lists;
        }

        protected private SPListCollection GetAllLists(Web web)
        {
            var tempCol = web.Lists;
            if (!tempCol.AreItemsAvailable)
                CTX.Lae(tempCol, true);

            return (SPListCollection)tempCol;
        }

        protected private Web GetWorkingWeb()
        {
            if (MyInvocation.BoundParameters.ContainsKey("InputObject"))
            {
                if (!((Web)this.InputObject.ShowOriginal()).Lists.AreItemsAvailable)
                    this.InputObject.LoadProperty("Lists");

                return this.InputObject.ShowOriginal() as Web;
            }

            else if (MyInvocation.BoundParameters.ContainsKey("Web"))
            {
                if (!this.Web.Lists.AreItemsAvailable)
                {
                    CTX.Lae(this.Web.Lists);
                }
                return this.Web;
            }

            else
            {
                if (!CTX.SP1.Web.Lists.AreItemsAvailable)
                    CTX.Lae(CTX.SP1.Web.Lists);

                return CTX.SP1.Web;
            }
        }

        public static List FindListByName(string name, Web web) => web.Lists.GetByTitle(name);

        public static List FindListById(Guid id, Web web) => web.Lists.GetById(id);

        #endregion
    }
}
