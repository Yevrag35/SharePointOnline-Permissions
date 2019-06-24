using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MG.SharePoint
{
    public static class FileExtensions
    {
        public static void LoadAllFiles(this FileCollection fileCol)
        {
            fileCol.LoadProperty(c => c.Include(
                x => x.Author.Title, x => x.CheckedOutByUser.Title, x => x.CheckInComment, x => x.CheckOutType, x => x.ContentTag,
                x => x.CustomizedPageStatus, x => x.EffectiveInformationRightsManagementSettings, x => x.ETag, x => x.Exists,
                x => x.InformationRightsManagementSettings, x => x.IrmEnabled, x => x.Length, x => x.Level, x => x.LinkingUri,
                x => x.LinkingUrl, x => x.ListId, x => x.LockedByUser.Title, x => x.MajorVersion, x => x.MinorVersion, x => x.ModifiedBy.Title,
                x => x.Name, x => x.PageRenderType, x => x.ServerRelativePath, x => x.ServerRelativeUrl, x => x.SiteId, x => x.TimeCreated,
                x => x.TimeLastModified, x => x.Title, x => x.UIVersion, x => x.UIVersionLabel, x => x.UniqueId, x => x.VersionEvents.Include(
                    ve => ve.Editor), x => x.Versions.Include(v => v.VersionLabel), x => x.WebId));
        }

        public static void LoadFileProps(this File file)
        {
            file.LoadProperty(x => x.Author.Title, x => x.CheckedOutByUser.Title, x => x.CheckInComment, x => x.CheckOutType,
                x => x.ContentTag, x => x.CustomizedPageStatus, x => x.EffectiveInformationRightsManagementSettings, x => x.ETag,
                x => x.Exists, x => x.InformationRightsManagementSettings, x => x.IrmEnabled, x => x.Length, x => x.Level, x => x.LinkingUri,
                x => x.LinkingUrl, x => x.ListId, x => x.LockedByUser.Title, x => x.MajorVersion, x => x.MinorVersion, x => x.ModifiedBy.Title,
                x => x.Name, x => x.PageRenderType, x => x.ServerRelativePath, x => x.ServerRelativeUrl, x => x.SiteId, x => x.TimeCreated,
                x => x.TimeLastModified, x => x.Title, x => x.UIVersion, x => x.UIVersionLabel, x => x.UniqueId, x => x.VersionEvents.Include(
                    ve => ve.Editor), x => x.Versions.Include(v => v.VersionLabel), x => x.WebId);
        }
    }
}