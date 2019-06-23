using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MG.SharePoint
{
    public static class ListExtensions
    {
        public static void LoadAllLists(this ListCollection listCol)
        {
            listCol.Initialize();
            listCol.LoadProperty(c => c.Include(
                z => z.AllowContentTypes, z => z.AllowDeletion, z => z.BaseTemplate, z => z.BaseType, z => z.BrowserFileHandling,
                z => z.ContentTypes.Include(ct => ct.Name), z => z.ContentTypesEnabled, z => z.CrawlNonDefaultViews, z => z.CreatablesInfo,
                z => z.Created, z => z.CurrentChangeToken, z => z.CustomActionElements, z => z.DataSource, z => z.DefaultContentApprovalWorkflowId,
                z => z.DefaultDisplayFormUrl, z => z.DefaultEditFormUrl, z => z.DefaultItemOpenUseListSetting, z => z.DefaultNewFormUrl, z => z.DefaultView.Title,
                z => z.DefaultViewPath, z => z.DefaultViewUrl, z => z.Description, z => z.DescriptionResource, z => z.Direction, z => z.DisableGridEditing,
                z => z.DocumentTemplateUrl, z => z.DraftVersionVisibility, z => z.EnableAssignToEmail, z => z.EnableAttachments, z => z.EnableFolderCreation,
                z => z.EnableMinorVersions, z => z.EnableModeration, z => z.EnableVersioning, z => z.EntityTypeName, z => z.EventReceivers.Include(evr => evr.ReceiverName),
                z => z.ExcludeFromOfflineClient, z => z.ExemptFromBlockDownloadOfNonViewableFiles, z => z.Fields.Include(fie => fie.Title), z => z.FileSavePostProcessingEnabled,
                z => z.FirstUniqueAncestorSecurableObject, z => z.ForceCheckout, z => z.Forms.Include(form => form.Id), z => z.HasExternalDataSource,
                z => z.HasUniqueRoleAssignments, z => z.Hidden, z => z.Id, z => z.ImagePath, z => z.ImageUrl, z => z.IrmEnabled, z => z.IrmExpire, z => z.IrmReject,
                z => z.IsApplicationList, z => z.IsCatalog, z => z.IsEnterpriseGalleryLibrary, z => z.IsPrivate, z => z.IsSiteAssetsLibrary, z => z.IsSystemList,
                z => z.ItemCount, z => z.LastItemDeletedDate, z => z.LastItemModifiedDate, z => z.LastItemUserModifiedDate, z => z.ListExperienceOptions,
                z => z.ListItemEntityTypeFullName, z => z.MajorVersionLimit, z => z.MajorWithMinorVersionsLimit, z => z.MultipleDataList, z => z.NoCrawl,
                z => z.OnQuickLaunch, z => z.PageRenderType, z => z.ParentWeb.Title, z => z.TitleResource, z => z.UserCustomActions.Include(uca => uca.Title),
                z => z.ValidationFormula, z => z.ValidationMessage, z => z.Views.Include(view => view.Title), z => z.WorkflowAssociations.Include(wfa => wfa.Name),
                z => z.WriteSecurity));
        }

        public static void LoadListProps(this List list)
        {
            list.LoadProperty(z => z.AllowContentTypes, z => z.AllowDeletion, z => z.BaseTemplate, z => z.BaseType, z => z.BrowserFileHandling,
                z => z.ContentTypes.Include(ct => ct.Name), z => z.ContentTypesEnabled, z => z.CrawlNonDefaultViews, z => z.CreatablesInfo,
                z => z.Created, z => z.CurrentChangeToken, z => z.CustomActionElements, z => z.DataSource, z => z.DefaultContentApprovalWorkflowId,
                z => z.DefaultDisplayFormUrl, z => z.DefaultEditFormUrl, z => z.DefaultItemOpenUseListSetting, z => z.DefaultNewFormUrl, z => z.DefaultView.Title,
                z => z.DefaultViewPath, z => z.DefaultViewUrl, z => z.Description, z => z.DescriptionResource, z => z.Direction, z => z.DisableGridEditing,
                z => z.DocumentTemplateUrl, z => z.DraftVersionVisibility, z => z.EnableAssignToEmail, z => z.EnableAttachments, z => z.EnableFolderCreation,
                z => z.EnableMinorVersions, z => z.EnableModeration, z => z.EnableVersioning, z => z.EntityTypeName, z => z.EventReceivers.Include(evr => evr.ReceiverName),
                z => z.ExcludeFromOfflineClient, z => z.ExemptFromBlockDownloadOfNonViewableFiles, z => z.Fields.Include(fie => fie.Title), z => z.FileSavePostProcessingEnabled,
                z => z.FirstUniqueAncestorSecurableObject, z => z.ForceCheckout, z => z.Forms.Include(form => form.Id), z => z.HasExternalDataSource,
                z => z.HasUniqueRoleAssignments, z => z.Hidden, z => z.Id, z => z.ImagePath, z => z.ImageUrl, z => z.IrmEnabled, z => z.IrmExpire, z => z.IrmReject,
                z => z.IsApplicationList, z => z.IsCatalog, z => z.IsEnterpriseGalleryLibrary, z => z.IsPrivate, z => z.IsSiteAssetsLibrary, z => z.IsSystemList,
                z => z.ItemCount, z => z.LastItemDeletedDate, z => z.LastItemModifiedDate, z => z.LastItemUserModifiedDate, z => z.ListExperienceOptions,
                z => z.ListItemEntityTypeFullName, z => z.MajorVersionLimit, z => z.MajorWithMinorVersionsLimit, z => z.MultipleDataList, z => z.NoCrawl,
                z => z.OnQuickLaunch, z => z.PageRenderType, z => z.ParentWeb.Title, z => z.TitleResource, z => z.UserCustomActions.Include(uca => uca.Title),
                z => z.ValidationFormula, z => z.ValidationMessage, z => z.Views.Include(view => view.Title), z => z.WorkflowAssociations.Include(wfa => wfa.Name),
                z => z.WriteSecurity);
        }
    }
}