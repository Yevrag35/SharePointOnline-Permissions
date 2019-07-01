using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MG.SharePoint
{
    public static class FolderExtensions
    {
        public static string[] GetFileNames(this FolderCollection folCol)
        {
            folCol.Context.Load(folCol, fc => fc.Include(fol => fol.Files.Include(file => file.Name)));
            folCol.Context.ExecuteQuery();
            return folCol.SelectMany(fol => fol.Files.Select(file => file.Name)).ToArray();
        }

        public static string[] GetFileNames(this Folder fol)
        {
            fol.LoadProperty(x => x.Files.Include(f => f.Name));
            return fol.Files.Select(f => f.Name).ToArray();
        }

        public static string[] GetFolderNames(this FolderCollection folCol)
        {
            folCol.Context.Load(folCol, fc => fc.Include(x => x.Name));
            folCol.Context.ExecuteQuery();
            return folCol.Select(x => x.Name).ToArray();
        }

        public static bool IsLoaded(this Folder fol)
        {
            return fol.IsPropertyReady(x => x.ContentTypeOrder, x => x.Exists, x => x.Files.Include(f => f.Name, f => f.Title, f => f.UniqueId),
                x => x.Folders.Include(sub => sub.Name, sub => sub.UniqueId, sub => sub.ItemCount), x => x.IsWOPIEnabled, x => x.ItemCount,
                x => x.Name, x => x.ParentFolder.Name, x => x.ProgID, x => x.ServerRelativeUrl, x => x.StorageMetrics, x => x.TimeCreated,
                x => x.TimeLastModified, x => x.UniqueContentTypeOrder, x => x.UniqueId, x => x.WelcomePage);
        }

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
