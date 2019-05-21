using Microsoft.SharePoint.Client;
using System;

namespace MG.SharePoint
{
    public partial class SPFile : SPSecurable
    {
        #region PROPERTIES

        public User Author { get; internal set; }
        public User CheckedOutByUser { get; internal set; }
        public string CheckInComment { get; internal set; }
        public CheckOutType? CheckOutType { get; internal set; }
        public string ContentTag { get; internal set; }
        public CustomizedPageStatus? CustomizedPageStatus { get; internal set; }
        public EffectiveInformationRightsManagementSettings EffectiveInformationRightsManagementSettings { get; internal set; }
        public string ETag { get; internal set; }
        public bool Exists { get; internal set; }
        public override object Id { get; internal set; }
        public InformationRightsManagementFileSettings InformationRightsManagementSettings { get; internal set; }
        public bool? IrmEnabled { get; internal set; }
        public long? Length { get; internal set; }
        public FileLevel? Level { get; internal set; }
        public string LinkingUri { get; internal set; }
        public Guid ListId { get; internal set; }
        public SPListItem ListItemAllFields { get; internal set; }
        public User LockedByUser { get; internal set; }
        public int? MajorVersion { get; internal set; }
        public int? MinorVersion { get; internal set; }
        public User ModifiedBy { get; internal set; }
        public override string Name { get; internal set; }
        //public string ObjectVersion { get; internal set; }
        public ListPageRenderType? PageRenderType { get; internal set; }
        public PropertyValues Properties { get; internal set; }
        public ResourcePath ServerRelativePath { get; internal set; }
        public string ServerRelativeUrl { get; internal set; }
        public Guid SiteId { get; internal set; }
        public DateTime? TimeCreated { get; internal set; }
        public DateTime? TimeLastModified { get; internal set; }
        public string Title { get; internal set; }
        public int? UIVersion { get; internal set; }
        public string UIVersionLabel { get; internal set; }
        public FileVersionEventCollection VersionEvents { get; internal set; }
        public FileVersionCollection Versions { get; internal set; }
        public Guid WebId { get; internal set; }

        #endregion

        #region Load Property Method
        public override void LoadProperty(params string[] propertyNames)
        {
            if (propertyNames == null)
                return;

            Load(_file, propertyNames);
        }

        #endregion
    }
}
