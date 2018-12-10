using Microsoft.SharePoint.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MG.SharePoint
{
    public partial class SPFolder : SPSecurable
    {
        #region Private Fields
        private protected Folder _fol;
        private protected string _name => _fol.Name;
        private protected Guid _id => _fol.UniqueId;
        private protected string _sru => _fol.ServerRelativeUrl;
        private protected int? _filec => _fol.Files.AreItemsAvailable ? _fol.Files.Count : (int?)null;
        private protected int? _folc => _fol.Folders.AreItemsAvailable ? _fol.Folders.Count : (int?)null;

        //protected internal SecurableObject SecObj => _fol.ListItemAllFields;

        #endregion

        #region Public Fields
        public override string Name => _name;
        public override object Id => _id;
        public string ServerRelativeUrl => _sru;
        //public bool? HasUniquePermissions => _hup;

        public int? FileCount => (int?)Files.Count;
        public int? FolderCount => (int?)Folders.Count;

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
        public SPFolder(Folder fol) : base(fol.ListItemAllFields)
        {
            CTX.Lae(fol, true, f => f.Name, f => f.UniqueId, f => f.ParentFolder.Name,
                f => f.ServerRelativeUrl);

            //f => f.ListItemAllFields.HasUniqueRoleAssignments)
            //_hup = !fol.ListItemAllFields.IsPropertyAvailable("HasUniqueRoleAssignments") ? 
            //    null : (bool?)fol.ListItemAllFields.HasUniqueRoleAssignments;

            _fol = fol;
        }

        #endregion

        #region Methods
        public override object ShowOriginal() => _fol;

        public override void Update() => _fol.Update();

        #endregion

        #region Enumerating Content
        public FolderCollection GetFolders()
        {
            if (!_fol.Folders.AreItemsAvailable)
                CTX.Lae(_fol.Folders, true, fols => fols.Include(
                    f => f.Name, f => f.UniqueId, f => f.ServerRelativeUrl));
            return _fol.Folders;
        }

        public SPFolder[] LoadAllFolders()
        {
            var spFols = new SPFolder[_fol.Folders.Count];
            for (int i = 0; i < _fol.Folders.Count; i++)
            {
                var fol = _fol.Folders[i];
                spFols[i] = (SPFolder)fol;
            }
            return spFols;
        }

        public FileCollection GetFiles()
        {
            if (!_fol.Files.AreItemsAvailable)
                CTX.Lae(_fol.Files, true, fis => fis.Include(
                    f => f.Name, f => f.ServerRelativeUrl, f => f.UniqueId));
            return _fol.Files;
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
