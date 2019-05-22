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
        public bool Exists { get; internal set; }
        public SPFileCollection Files { get; internal set; }
        public SPFolderCollection Folders { get; internal set; }
        public bool? IsWOPIEnabled { get; internal set; }
        public int ItemCount { get; internal set; }
        public SPListItem ListItemAllFields { get; internal set; }
        public SPFolder ParentFolder { get; set; }
        public string ProgID { get; internal set; }
        public PropertyValues Properties { get; internal set; }
        public ResourcePath ServerRelativePath { get; internal set; }
        public StorageMetrics StorageMetrics { get; internal set; }
        public DateTime TimeCreated { get; internal set; }
        public IList<ContentTypeId> UniqueContentTypeOrder { get; internal set; }
        public string WelcomePage { get; internal set; }

        #endregion

        #region Load Property Method
        public override void LoadProperty(params string[] propertyNames)
        {
            if (propertyNames.Length > 0)
            {
                var list = propertyNames.ToList();
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    string name = list[i];
                    if (name.Equals("Folders", StringComparison.CurrentCultureIgnoreCase))
                    {
                        this.GetFolders();
                        list.Remove(name);
                    }
                    else if (name.Equals("Files", StringComparison.CurrentCultureIgnoreCase))
                    {
                        this.GetFiles();
                        list.Remove(name);
                    }
                    else if (name.Equals("Permissions", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (this.CanSetPermissions)
                            this.GetPermissions();

                        list.Remove(name);
                    }
                    else if (name.Equals("ParentFolder", StringComparison.CurrentCultureIgnoreCase) && this.ServerRelativeUrl.Equals("/"))
                    {
                        list.Remove(name);
                    }
                }
                base.Load(_fol, list.ToArray());
            }
        }

        #endregion
    }
}
