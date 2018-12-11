using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Comments;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MG.SharePoint
{
    public partial class SPListItem : SPSecurable
    {
        #region Other Properties
        public AttachmentCollection AttachmentFiles { get; internal set; }
        public string Client_Title { get; internal set; }
        public bool? CommentsDisabled { get; internal set; }
        public CommentsDisabledScope? CommentsDisabledScope { get; internal set; }
        public ListItemComplianceInfo ComplianceInfo { get; internal set; }
        public ContentType ContentType { get; internal set; }
        public BasePermissions EffectiveBasePermissions { get; internal set; }
        public BasePermissions EffectiveBasePermissionsForUI { get; internal set; }
        public Dictionary<string, object> FieldValues { get; internal set; }
        public FieldStringValues FieldValuesAsHtml { get; internal set; }
        public FieldStringValues FieldValuesAsText { get; internal set; }
        public FieldStringValues FieldValuesForEdit { get; internal set; }
        public SPFile File { get; internal set; }
        public FileSystemObjectType? FileSystemObjectType { get; internal set; }
        public SecurableObject FirstUniqueAncestorSecurableObject { get; internal set; }
        public SPFolder Folder { get; internal set; }
        public DlpPolicyTip GetDlpPolicyTip { get; internal set; }
        public string IconOverlay { get; internal set; }
        public SPList ParentList { get; internal set; }
        public PropertyValues Properties { get; internal set; }
        public string ServerRedirectedEmbedUri { get; internal set; }
        public string ServerRedirectedEmbedUrl { get; internal set; }
        public ListItemVersionCollection Versions { get; internal set; }

        #endregion

        #region Load Property Method
        public override void LoadProperty(params string[] propertyNames)
        {
            if (propertyNames == null)
                return;

            Load(_li, propertyNames);
        }

        #endregion
    }
}
