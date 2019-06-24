using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MG.SharePoint
{
    public static class ListItemExtensions
    {
        public static void LoadListItemProps(this ListItem li)
        {
            li.LoadProperty(f => f.AttachmentFiles.Include(af => af.FileName), f => f.Client_Title, f => f.CommentsDisabled,
                f => f.CommentsDisabledScope, f => f.ComplianceInfo, f => f.ContentType, f => f.DisplayName, f => f.FieldValues,
                f => f.FieldValuesAsHtml, f => f.FieldValuesAsText, f => f.FieldValuesForEdit, f => f.File, f => f.File.Name,
                f => f.FileSystemObjectType, f => f.FirstUniqueAncestorSecurableObject, f => f.Folder, f => f.GetDlpPolicyTip,
                f => f.HasUniqueRoleAssignments, f => f.IconOverlay, f => f.Id, f => f.ParentList, f => f.ServerRedirectedEmbedUri,
                f => f.ServerRedirectedEmbedUrl, f => f.Versions.Include(v => v.VersionId, v => v.VersionLabel));
        }
    }
}