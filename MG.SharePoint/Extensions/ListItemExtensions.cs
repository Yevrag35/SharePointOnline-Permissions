using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MG.SharePoint
{
    public static class ListItemExtensions
    {
        public static void LoadAllListItems(this ListItemCollection col)
        {
            col.LoadProperty(c => c.Include(
                f => f.Client_Title, f => f.CommentsDisabled,
                f => f.CommentsDisabledScope, f => f.ComplianceInfo, f => f.ContentType, f => f.DisplayName,
                f => f.FieldValuesAsHtml, f => f.FieldValuesAsText, f => f.FieldValuesForEdit, f => f.File, f => f.File.Name,
                f => f.FileSystemObjectType, f => f.FirstUniqueAncestorSecurableObject, f => f.Folder, f => f.GetDlpPolicyTip,
                f => f.HasUniqueRoleAssignments, f => f.Id, f => f.ParentList, f => f.ParentList.Title, f => f.ServerRedirectedEmbedUri,
                f => f.ServerRedirectedEmbedUrl, f => f.Versions.Include(v => v.VersionId, v => v.VersionLabel)));
        }

        public static void LoadListItemProps(this ListItem li)
        {
            li.LoadProperty(f => f.Client_Title, f => f.CommentsDisabled,
                f => f.CommentsDisabledScope, f => f.ComplianceInfo, f => f.ContentType, f => f.DisplayName,
                f => f.FieldValuesAsHtml, f => f.FieldValuesAsText, f => f.FieldValuesForEdit, f => f.File, f => f.File.Name,
                f => f.FileSystemObjectType, f => f.FirstUniqueAncestorSecurableObject, f => f.Folder, f => f.GetDlpPolicyTip,
                f => f.HasUniqueRoleAssignments, f => f.Id, f => f.ParentList, f => f.ParentList.Title, f => f.ServerRedirectedEmbedUri,
                f => f.ServerRedirectedEmbedUrl, f => f.Versions.Include(v => v.VersionId, v => v.VersionLabel));
        }

        public static bool TryLoadAttachments(this ListItem li)
        {
            bool result = false;
            try
            {
                li.LoadProperty(x => x.AttachmentFiles.Include(af => af.FileName));
                result = true;
            }
            catch (ServerException) { }

            return result;
        }
    }
}