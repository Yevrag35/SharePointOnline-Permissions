using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Workflow;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MG.SharePoint
{
    public partial class SPWeb : SPObject, ISPPermissions
    {
        #region Other Properties
        public AlertCollection Alerts { get; internal set; }
        public bool? AllowAutomaticASPXPageIndexing { get; internal set; }
        public bool? AllowCreateDeclarativeWorkflowForCurrentUser { get; internal set; }
        public bool? AllowDesignerForCurrentUser { get; internal set; }
        public bool? AllowMasterPageEditingForCurrentUser { get; internal set; }
        public bool? AllowRevertFromTemplateForCurrentUser { get; internal set; }
        public bool? AllowRssFeeds { get; internal set; }
        public bool? AllowSaveDeclarativeWorkflowAsTemplateForCurrentUser { get; internal set; }
        public bool? AllowSavePublishDeclarativeWorkflowForCurrentUser { get; internal set; }
        public PropertyValues AllProperties { get; internal set; }
        public string AlternateCssUrl { get; internal set; }
        public Guid? AppInstanceId { get; internal set; }
        public AppTileCollection AppTiles { get; internal set; }
        public Group AssociatedMemberGroup { get; internal set; }
        public Group AssociatedOwnerGroup { get; internal set; }
        public Group AssociatedVisitorGroup { get; internal set; }
        public User Author { get; internal set; }
        public ContentTypeCollection AvailableContentTypes { get; internal set; }
        public FieldCollection AvailableFields { get; internal set; }
        public bool? CommentsOnSitePagesDisabled { get; internal set; }
        public short? Configuration { get; internal set; }
        public bool? ContainsConfidentialInfo { get; internal set; }
        public ContentTypeCollection ContentTypes { get; internal set; }
        public ChangeToken CurrentChangeToken { get; internal set; }
        public User CurrentUser { get; internal set; }
        public string CustomMasterUrl { get; internal set; }
        public SPDataLeakagePreventionStatusInfo DataLeakagePreventionStatusInfo { get; internal set; }
        public string Description { get; internal set; }
        public UserResource DescriptionResource { get; internal set; }
        public string DesignerDownloadUrlForCurrentUser { get; internal set; }
        public Guid? DesignPackageId { get; internal set; }
        public bool? DisableAppViews { get; internal set; }
        public bool? DisableFlows { get; internal set; }
        public bool? DocumentLibraryCalloutOfficeWebAppPreviewersDisabled { get; internal set; }
        public BasePermissions EffectiveBasePermissions { get; internal set; }
        public bool? EnableMinimalDownload { get; internal set; }
        public EventReceiverDefinitionCollection EventReceivers { get; internal set; }
        public bool? ExcludeFromOfflineClient { get; internal set; }
        public FeatureCollection Features { get; internal set; }
        public FieldCollection Fields { get; internal set; }
        public SecurableObject FirstUniqueAncestorSecurableObject { get; internal set; }
        public bool? FooterEnabled { get; internal set; }
        public SPVariantThemeType? HeaderEmphasis { get; internal set; }
        public bool? HorizontalQuickLaunch { get; internal set; }
        public bool? IsMultilingual { get; internal set; }
        public uint? Language { get; internal set; }
        public DateTime? LastItemModifiedDate { get; internal set; }
        public DateTime? LastItemUserModifiedDate { get; internal set; }
        public ListTemplateCollection ListTemplates { get; internal set; }
        public string MasterUrl { get; internal set; }
        public bool? MegaMenuEnabled { get; internal set; }
        public bool? MembersCanShare { get; internal set; }
        public Navigation Navigation { get; internal set; }
        public bool? NoCrawl { get; internal set; }
        public bool? NotificationsInOneDriveForBusinessEnabled { get; internal set; }
        public bool? NotificationsInSharePointEnabled { get; internal set; }
        public bool? ObjectCacheEnabled { get; internal set; }
        public bool? OverwriteTranslationsOnChange { get; internal set; }
        public WebInformation ParentWeb { get; internal set; }
        public bool? PreviewFeaturesEnabled { get; internal set; }
        public PushNotificationSubscriberCollection PushNotificationSubscribers { get; internal set; }
        public bool? QuickLaunchEnabled { get; internal set; }
        public RecycleBinItemCollection RecycleBin { get; internal set; }
        public bool? RecycleBinEnabled { get; internal set; }
        public RegionalSettings RegionalSettings { get; internal set; }
        public string RequestAccessEmail { get; internal set; }
        public ResourcePath ResourcePath { get; internal set; }
        public RoleDefinitionCollection RoleDefinitions { get; internal set; }
        public SPFolder RootFolder { get; internal set; }
        public bool? SaveSiteAsTemplateEnabled { get; internal set; }
        public ResourcePath ServerRelativePath { get; internal set; }
        public bool? ShowUrlStructureForCurrentUser { get; internal set; }
        public Microsoft.SharePoint.Marketplace.CorporateCuratedGallery.SiteCollectionCorporateCatalogAccessor SiteCollectionAppCatalog { get; internal set; }
        public GroupCollection SiteGroups { get; internal set; }
        public string SiteLogoDescription { get; internal set; }
        public string SiteLogoUrl { get; internal set; }
        public SPList SiteUserInfoList { get; internal set; }
        public UserCollection SiteUsers { get; internal set; }
        public IEnumerable<int> SupportedUILanguageIds { get; internal set; }
        public bool? SyndicationEnabled { get; internal set; }
        public Microsoft.SharePoint.Marketplace.CorporateCuratedGallery.TenantCorporateCatalogAccessor TenantAppCatalog { get; internal set; }
        public bool? TenantTagPolicyEnabled { get; internal set; }
        public string ThemedCssFolderUrl { get; internal set; }
        public ThemeInfo ThemeInfo { get; internal set; }
        public bool? ThirdPartyMdmEnabled { get; internal set; }
        public UserResource TitleResource { get; internal set; }
        public bool? TreeViewEnabled { get; internal set; }
        public int? UIVersion { get; internal set; }
        public bool? UIVersionConfigurationEnabled { get; internal set; }
        public string Url { get; internal set; }
        public UserCustomActionCollection UserCustomActions { get; internal set; }
        public SPWebCollection Webs { get; internal set; }
        public string WebTemplate { get; internal set; }
        public string WelcomePage { get; internal set; }
        public WorkflowAssociationCollection WorkflowAssociations { get; internal set; }
        public WorkflowTemplateCollection WorkflowTemplates { get; internal set; }

        #endregion

        #region Load Property Method
        public override void LoadProperty(params string[] propertyNames)
        {
            if (propertyNames == null)
                return;

            Load(_web, propertyNames);
        }

        #endregion

    }
}
