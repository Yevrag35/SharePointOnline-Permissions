using Microsoft.SharePoint.Client;
using System;
using System.Linq;
using System.Management.Automation;

namespace MG.SharePoint.PowerShell
{
    [Cmdlet(VerbsCommon.Get, "SPWeb")]
    [OutputType(typeof(SPWeb))]
    public class GetSPWeb : BaseSPCmdlet
    {
        private static readonly string[] ValidSet = new string[28]
        {
            "Alerts", "AllProperties", "AppTiles", "AvailableContentTypes", "AvailableFields",
            "ContentTypes", "CurrentChangeToken", "DataLeakagePreventionStatusInfo", "DescriptionResource",
            "EffectiveBasePermissions", "EventReceivers", "Features", "Fields", "FirstUniqueAncestorSecurableObject",
            "Lists", "ListTemplates", "ParentWeb", "PushNotificationSubscribers", "RecycleBin", "RootFolder",
            "SiteCollectionAppCatalog", "SiteGroups", "SiteUserInfoList", "SiteUsers", "TenantAppCatalog",
            "Webs", "WorkflowAssociations", "WorkflowTemplates"
        };

        #region PARAMETERS
        [Parameter(Mandatory = false, Position = 0, ValueFromPipeline = true)]
        [AllowEmptyString]
        public string RelativeUrl = string.Empty;

        [Parameter(Mandatory = false, Position = 1)]
        [Alias("p", "Properties", "Property")]
        [ValidateSet("*", "Alerts", "AllProperties", "AppTiles", "AvailableContentTypes", "AvailableFields",
            "ContentTypes", "CurrentChangeToken", "DataLeakagePreventionStatusInfo", "DescriptionResource",
            "EffectiveBasePermissions", "EventReceivers", "Features", "Fields", "FirstUniqueAncestorSecurableObject",
            "Lists", "ListTemplates", "ParentWeb", "PushNotificationSubscribers", "RecycleBin", "RootFolder",
            "SiteCollectionAppCatalog", "SiteGroups", "SiteUserInfoList", "SiteUsers", "TenantAppCatalog",
            "Webs", "WorkflowAssociations", "WorkflowTemplates")]
        public string[] LoadExtraProperties = new string[] { };

        #endregion

        protected override void BeginProcessing() => base.BeginProcessing();

        protected override void ProcessRecord()
        {
            var spWeb = new SPWeb(this.RelativeUrl);
            if (MyInvocation.BoundParameters.ContainsKey("LoadExtraProperties"))
                LoadExtraWebProperties(spWeb, LoadExtraProperties);

            WriteObject(spWeb);
        }

        public static void LoadExtraWebProperties(SPWeb web, string[] properties)
        {
            if (!properties.Contains("*"))
                web.LoadProperty(properties);

            else
                web.LoadProperty(ValidSet);
        }
    }
}
