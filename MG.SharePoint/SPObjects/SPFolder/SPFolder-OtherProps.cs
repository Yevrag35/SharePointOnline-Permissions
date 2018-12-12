using Microsoft.SharePoint.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MG.SharePoint
{
    public partial class SPFolder : SPSecurable
    {
        #region Other Properties
        public IList<ContentTypeId> ContentTypeOrder { get; internal set; }
        public bool? Exists { get; internal set; }
        public SPFileCollection Files { get; internal set; }
        public SPFolderCollection Folders { get; internal set; }
        public bool? IsWOPIEnabled { get; internal set; }
        public int? ItemCount { get; internal set; }
        public SPListItem ListItemAllFields { get; internal set; }
        public SPFolder ParentFolder { get; set; }
        public string ProgID { get; internal set; }
        public PropertyValues Properties { get; internal set; }
        public ResourcePath ServerRelativePath { get; internal set; }
        public StorageMetrics StorageMetrics { get; internal set; }
        public DateTime? TimeCreated { get; internal set; }
        public IList<ContentTypeId> UniqueContentTypeOrder { get; internal set; }
        public Guid? UniqueId { get; internal set; }
        public string WelcomePage { get; internal set; }

        #endregion

        #region Load Property Method
        public override void LoadProperty(params string[] propertyNames)
        {
            if (propertyNames == null)
                return;

            Load(_fol, propertyNames);
        }

        #endregion
    }
}
