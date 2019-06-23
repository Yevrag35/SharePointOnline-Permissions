using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MG.SharePoint
{
    public static class WebExtensions
    {
        public static void LoadWeb(this Web web)
        {
            web.Context.Load(web, w => w.Alerts.Include(a => a.Title), w => w.AllowAutomaticASPXPageIndexing, w => w.AllowCreateDeclarativeWorkflowForCurrentUser,
                w => w.AllowDesignerForCurrentUser, w => w.AllowMasterPageEditingForCurrentUser, w => w.AllowRevertFromTemplateForCurrentUser,
                w => w.AllowRssFeeds, w => w.AllowSaveDeclarativeWorkflowAsTemplateForCurrentUser, w => w.AllowSavePublishDeclarativeWorkflowForCurrentUser,
                w => w.AlternateCssUrl, w => w.AppInstanceId, w => w.AppTiles.Include(t => t.Title), w => w.AssociatedMemberGroup, w => w.AssociatedOwnerGroup,
                w => w.AssociatedVisitorGroup, w => w.AvailableContentTypes.Include(c => c.Name), w => w.AvailableFields.Include(af => af.Title),
                w => w.CommentsOnSitePagesDisabled, w => w.Configuration, w => w.ContainsConfidentialInfo, w => w.ContentTypes.Include(ct => ct.Name),
                w => w.Created, w => w.CurrentChangeToken, w => w.CurrentUser, w => w.CustomMasterUrl, w => w.DataLeakagePreventionStatusInfo, w => w.Description,
                w => w.DescriptionResource, w => w.DesignerDownloadUrlForCurrentUser, w => w.DesignPackageId, w => w.DisableAppViews, w => w.DisableFlows,
                w => w.DocumentLibraryCalloutOfficeWebAppPreviewersDisabled, w => w.EnableMinimalDownload, w => w.EventReceivers.Include(er => er.ReceiverName),
                w => w.ExcludeFromOfflineClient, w => w.Features.Include(f => f.DisplayName), w => w.Fields.Include(fie => fie.Title),
                w => w.FirstUniqueAncestorSecurableObject, w => w.FooterEnabled, w => w.RecycleBinEnabled, w => w.Lists.Include(list => list.Title),
                w => w.HasUniqueRoleAssignments, w => w.HeaderEmphasis, w => w.HorizontalQuickLaunch, w => w.Id, w => w.IsMultilingual, w => w.Language, w => w.LastItemModifiedDate,
                w => w.LastItemUserModifiedDate, w => w.ListTemplates.Include(lt => lt.Name), w => w.MasterUrl, w => w.MegaMenuEnabled, w => w.MembersCanShare,
                w => w.Navigation, w => w.NoCrawl, w => w.NotificationsInOneDriveForBusinessEnabled, w => w.NotificationsInSharePointEnabled, w => w.ObjectCacheEnabled,
                w => w.OverwriteTranslationsOnChange, w => w.ParentWeb.Title, w => w.PreviewFeaturesEnabled,
                w => w.QuickLaunchEnabled, w => w.RecycleBin.Include(rb => rb.Title), w => w.RegionalSettings, w => w.RequestAccessEmail, w => w.ResourcePath,
                w => w.RoleDefinitions.Include(rd => rd.Name), w => w.RootFolder.Name, w => w.SaveSiteAsTemplateEnabled, w => w.ServerRelativePath, w => w.ServerRelativeUrl,
                w => w.ShowUrlStructureForCurrentUser, w => w.SiteCollectionAppCatalog, w => w.SiteGroups.Include(g => g.Title), w => w.SiteUsers.Include(u => u.Title),
                w => w.SiteLogoDescription, w => w.SiteLogoUrl, w => w.SiteUserInfoList,
                w => w.SupportedUILanguageIds, w => w.SyndicationEnabled, w => w.TenantAppCatalog, w => w.TenantTagPolicyEnabled, w => w.ThemedCssFolderUrl,
                w => w.ThemeInfo, w => w.ThirdPartyMdmEnabled, w => w.Title, w => w.TitleResource, w => w.TreeViewEnabled, w => w.UIVersion, w => w.UIVersionConfigurationEnabled,
                w => w.Url, w => w.UserCustomActions.Include(uca => uca.Title), w => w.Webs.Include(subWeb => subWeb.Title), w => w.WebTemplate, w => w.WelcomePage,
                w => w.WorkflowAssociations.Include(wfa => wfa.Name), w => w.WorkflowTemplates.Include(wt => wt.Name));

            web.Context.ExecuteQuery();

            try
            {
                web.Context.Load(web.Author, a => a.Title);
                web.Context.ExecuteQuery();
            }
            catch (ServerException) { }     // Author may not exist.
            
            try
            {
                web.Context.Load(web, x => x.PushNotificationSubscribers.Include(ps => ps.User.Title));
                web.Context.ExecuteQuery();
            }
            catch (ServerException) { }     // The feature may not be enabled.
        }
    }
}