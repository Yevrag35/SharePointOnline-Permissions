using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MG.SharePoint
{
    public static class FolderExtensions
    {
        public static void LoadAllFolders(this FolderCollection folCol)
        {
            folCol.LoadProperty(fc => fc.Include(
                x => x.ContentTypeOrder, x => x.Exists, x => x.Files.Include(f => f.Name, f => f.Title, f => f.UniqueId),
                x => x.Folders.Include(sub => sub.Name, sub => sub.UniqueId, sub => sub.ItemCount), x => x.IsWOPIEnabled, x => x.ItemCount,
                x => x.Name, x => x.ParentFolder.Name, x => x.ProgID, x => x.ServerRelativeUrl, x => x.StorageMetrics, x => x.TimeCreated,
                x => x.TimeLastModified, x => x.UniqueContentTypeOrder, x => x.UniqueId, x => x.WelcomePage));
        }

        public static void LoadFolderProps(this Folder fol)
        {
            fol.LoadProperty(x => x.ContentTypeOrder, x => x.Exists, x => x.Files.Include(f => f.Name, f => f.Title, f => f.UniqueId),
                x => x.Folders.Include(sub => sub.Name, sub => sub.UniqueId, sub => sub.ItemCount), x => x.IsWOPIEnabled, x => x.ItemCount,
                x => x.Name, x => x.ParentFolder.Name, x => x.ProgID, x => x.ServerRelativeUrl, x => x.StorageMetrics, x => x.TimeCreated,
                x => x.TimeLastModified, x => x.UniqueContentTypeOrder, x => x.UniqueId, x => x.WelcomePage);
        }
    }
}
