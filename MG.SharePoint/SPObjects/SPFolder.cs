using Microsoft.SharePoint.Client;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MG.SharePoint
{
    public class SPFolder : ISPObject
    {
        #region Private Fields
        private protected Folder _fol;
        private protected string _name => _fol.Name;
        private protected Guid _id => _fol.UniqueId;
        private protected string _sru => _fol.ServerRelativeUrl;
        private protected int? _filec => _fol.Files.AreItemsAvailable ? _fol.Files.Count : (int?)null;
        private protected int? _folc => _fol.Folders.AreItemsAvailable ? _fol.Folders.Count : (int?)null;
        private protected SPPermission[] _perms;
        private protected bool? _hup;
        private protected string _par;

        #endregion

        #region Public Fields
        public string Name => _name;
        public object Id => _id;
        public string ServerRelativeUrl => _sru;
        public bool? HasUniquePermissions => _hup;
        public string Parent => _par;
        public SPPermission[] Permissions => _perms;
        public int? FileCount => _filec;
        public int? FolderCount => _folc;

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
        {
            CTX.Lae(fol, true, f => f.Name, f => f.UniqueId, f => f.ParentFolder.Name,
                f => f.ServerRelativeUrl, f => f.ListItemAllFields.HasUniqueRoleAssignments);
            if (!string.IsNullOrEmpty(fol.ParentFolder.Name))
                _par = fol.ParentFolder.Name;

            _hup = !fol.ListItemAllFields.IsPropertyAvailable("HasUniqueRoleAssignments") ? 
                null : (bool?)fol.ListItemAllFields.HasUniqueRoleAssignments;

            _fol = fol;
        }

        #endregion

        #region Methods
        public object ShowOriginal() => _fol;

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
                spFols[i] = _fol.Folders[i];
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

        #region Permission Methods
        public SPPermission[] GetPermissions()
        {
            CTX.Lae(_fol.ListItemAllFields, true,
                f => f.RoleAssignments.Include(
                    ass => ass.Member, ass => ass.RoleDefinitionBindings.Include(
                        d => d.Name, d => d.Description
                    )
                )
            );
            var spPerms = new SPPermission[_fol.ListItemAllFields.RoleAssignments.Count];
            for (int i = 0; i < _fol.ListItemAllFields.RoleAssignments.Count; i++)
            {
                SPPermission ass = _fol.ListItemAllFields.RoleAssignments[i];
                spPerms[i] = ass;
            }
            _perms = spPerms;
            return spPerms;
        }

        public bool BreakInheritance(bool copyRoleAssignments, bool clearSubscopes = true)
        {
            bool result = true;
            if (HasUniquePermissions.HasValue && HasUniquePermissions.Value)
                throw new InvalidBreakInheritanceException(_fol.UniqueId);

            _fol.ListItemAllFields.BreakRoleInheritance(copyRoleAssignments, clearSubscopes);
            try
            {
                CTX.Lae();
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public bool ResetInheritance()
        {
            bool result = true;
            if (!HasUniquePermissions.HasValue || (HasUniquePermissions.HasValue && !HasUniquePermissions.Value))
                throw new InvalidResetInheritanceException(_fol.UniqueId);

            _fol.ListItemAllFields.ResetRoleInheritance();
            try
            {
                CTX.Lae();
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public void AddFolderPermission(BindingCollection)

        #endregion

        #region Operators
        public static implicit operator SPFolder(Folder fol) => new SPFolder(fol);
        public static explicit operator Folder(SPFolder spFol) => (Folder)spFol.ShowOriginal();

        #endregion
    }
}
