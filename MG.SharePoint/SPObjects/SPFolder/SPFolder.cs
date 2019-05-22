using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;

namespace MG.SharePoint
{
    public partial class SPFolder : SPSecurable
    {
        #region Private Fields
        private Folder _fol;

        #endregion

        #region Public Fields
        public override object Id { get; internal set; }
        public override string Name { get; internal set; }
        public string ServerRelativeUrl { get; internal set; }
        public DateTime TimeLastModified { get; internal set; }

        public int? FileCount => this.Files != null
            ? this.Files.Count
            : (int?)null;
        public int? FolderCount => this.Folders != null
            ? this.Folders.Count
            : (int?)null;

        #endregion

        #region Constructors
        public SPFolder(Guid folderId)
            : this(CTX.SP1.Web.GetFolderById(folderId))
        {
        }
        public SPFolder(string serverRelativeUrl)
            : this(CTX.SP1.Web.GetFolderByServerRelativeUrl(serverRelativeUrl))
        {
        }
        public SPFolder(Folder fol) 
            : base(fol.ListItemAllFields)
        {
            base.FormatObject(fol, null, "UniqueId");
            this.Id = fol.UniqueId;
            _fol = fol;
        }

        #endregion

        #region Methods
        public override ClientObject ShowOriginal() => _fol;

        public override void Update() => _fol.Update();

        #endregion

        #region Enumerating Content
        public SPFolderCollection GetFolders()
        {
            if (!_fol.Folders.AreItemsAvailable)
                CTX.Lae(_fol.Folders, true, fols => fols.Include(
                    f => f.Name, f => f.UniqueId, f => f.ServerRelativeUrl));
            this.Folders = (SPFolderCollection)_fol.Folders;
            return this.Folders;
        }

        public SPFileCollection GetFiles()
        {
            if (!_fol.Files.AreItemsAvailable)
                CTX.Lae(_fol.Files, true, fis => fis.Include(
                    f => f.Name, f => f.ServerRelativeUrl, f => f.UniqueId));
            this.Files = (SPFileCollection)_fol.Files;
            return this.Files;
        }

        #endregion

        #region Operators
        public static explicit operator SPFolder(Folder fol) => new SPFolder(fol);
        public static explicit operator Folder(SPFolder spFol) => (Folder)spFol.ShowOriginal();
        public static explicit operator SPFolder(string relativeUrl)
        {
            if (relativeUrl.StartsWith(CTX.DestinationSite))
                relativeUrl.Replace(CTX.DestinationSite + "/", string.Empty);
            return new SPFolder(relativeUrl);
        }

        #endregion
    }
}
