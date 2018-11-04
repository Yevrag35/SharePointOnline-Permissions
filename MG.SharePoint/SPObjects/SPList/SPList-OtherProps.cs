using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Workflow;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MG.SharePoint
{
    public partial class SPList : SPObject
    {
        #region Properties
        // We'll turn the other properties into separate methods
        public bool? AllowContentTypes { get; internal set; }
        public bool? AllowDeletion { get; internal set; }
        public int? BaseTemplate { get; internal set; }
        public BaseType? BaseType { get; internal set; }
        public BrowserFileHandling? BrowserFileHandling { get; internal set; }
        public ContentTypeCollection ContentTypes { get; internal set; }
        public bool? ContentTypesEnabled { get; internal set; }
        public bool? CrawlNonDefaultViews { get; internal set; }
        public CreatablesInfo CreatablesInfo { get; internal set; }
        public ChangeToken CurrentChangeToken { get; internal set; }
        public CustomActionElementCollection CustomActionElements { get; internal set; }
        public ListDataSource DataSource { get; internal set; }
        public Guid? DefaultContentApprovalWorkflowId { get; internal set; }
        public string DefaultDisplayFormUrl { get; internal set; }
        public string DefaultEditFormUrl { get; internal set; }
        public bool? DefaultItemOpenUseListSetting { get; internal set; }
        public string DefaultNewFormUrl { get; internal set; }
        public View DefaultView { get; internal set; }
        public ResourcePath DefaultViewPath { get; internal set; }
        public string DefaultViewUrl { get; internal set; }
        public string Description { get; internal set; }
        public UserResource DescriptionResource { get; internal set; }
        public string Direction { get; internal set; }
        public bool? DisableGridEditing { get; internal set; }
        public string DocumentTemplateUrl { get; internal set; }
        public DraftVisibilityType? DraftVersionVisibility { get; internal set; }
        public BasePermissions EffectiveBasePermissions { get; internal set; }
        public BasePermissions EffectiveBasePermissionsForUI { get; internal set; }
        public bool? EnableAssignToEmail { get; internal set; }
        public bool? EnableAttachments { get; internal set; }
        public bool? EnableFolderCreation { get; internal set; }
        public bool? EnableMinorVersions { get; internal set; }
        public bool? EnableModeration { get; internal set; }
        public bool? EnableVersioning { get; internal set; }
        public string EntityTypeName { get; internal set; }
        public EventReceiverDefinitionCollection EventReceivers { get; internal set; }
        public bool? ExcludeFromOfflineClient { get; internal set; }
        public bool? ExemptFromBlockDownloadOfNonViewableFiles { get; internal set; }
        public FieldCollection Fields { get; internal set; }
        public bool? FileSavePostProcessingEnabled { get; internal set; }
        public SecurableObject FirstUniqueAncestorSecurableObject { get; internal set; }
        public bool? ForceCheckout { get; internal set; }
        public FormCollection Forms { get; internal set; }
        public bool? HasExternalDataSource { get; internal set; }
        public bool? Hidden { get; internal set; }
        public ResourcePath ImagePath { get; internal set; }
        public string ImageUrl { get; internal set; }
        public InformationRightsManagementSettings InformationRightsManagementSettings { get; internal set; }
        public bool? IrmEnabled { get; internal set; }
        public bool? IrmExpire { get; internal set; }
        public bool? IrmReject { get; internal set; }
        public bool? IsApplicationList { get; internal set; }
        public bool? IsCatalog { get; internal set; }
        public bool? IsEnterpriseGalleryLibrary { get; internal set; }
        public bool? IsPrivate { get; internal set; }
        public bool? IsSiteAssetsLibrary { get; internal set; }
        public bool? IsSystemList { get; internal set; }
        public DateTime? LastItemDeletedDate { get; internal set; }
        public DateTime? LastItemModifiedDate { get; internal set; }
        public DateTime? LastItemUserModifiedDate { get; internal set; }
        public ListExperience? ListExperienceOptions { get; internal set; }
        public string ListItemEntityTypeFullName { get; internal set; }
        public int? MajorVersionLimit { get; internal set; }
        public int? MajorWithMinorVersionsLimit { get; internal set; }
        public bool? MultipleDataList { get; internal set; }
        public bool? NoCrawl { get; internal set; }
        public bool? OnQuickLaunch { get; internal set; }
        public ListPageRenderType? PageRenderType { get; internal set; }
        public Web ParentWeb { get; internal set; }
        public ResourcePath ParentWebPath { get; internal set; }
        public string ParentWebUrl { get; internal set; }
        public bool? ParserDisabled { get; internal set; }
        public int? ReadSecurity { get; internal set; }
        public RoleAssignmentCollection RoleAssignments { get; internal set; }
        public Folder RootFolder { get; internal set; }
        public string SchemaXml { get; internal set; }
        public bool? ServerObjectIsNull { get; internal set; }
        public bool? ServerTemplateCanCreateFolders { get; internal set; }
        public Guid? TemplateFeatureId { get; internal set; }
        public UserResource TitleResource { get; internal set; }
        public UserCustomActionCollection UserCustomActions { get; internal set; }
        public string ValidationFormula { get; internal set; }
        public string ValidationMessage { get; internal set; }
        public ViewCollection Views { get; internal set; }
        public WorkflowAssociationCollection WorkflowAssociations { get; internal set; }
        public int? WriteSecurity { get; internal set; }

        #endregion

        public override void LoadProperty(params string[] propertyNames)
        {
            if (propertyNames == null)
                return;

            Load(_list, propertyNames);
        }
    }
}
