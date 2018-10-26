using Microsoft.SharePoint.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
        private protected SPPermissionCollection _perms;
        private protected bool? _hup;
        private protected string _par;

        #endregion

        #region Public Fields
        public string Name => _name;
        public object Id => _id;
        public string ServerRelativeUrl => _sru;
        public bool? HasUniquePermissions => _hup;
        public string Parent => _par;
        public SPPermissionCollection Permissions => _perms;
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
        public SPPermissionCollection GetPermissions()
        {
            SPPermissionCollection permCol = _fol.ListItemAllFields.RoleAssignments;
            _perms = permCol;
            return permCol;
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

        public void AddFolderPermission(SPBindingCollection bindingCol, bool forceBreak = false)
        {
            if (HasUniquePermissions.HasValue && !HasUniquePermissions.Value)
            {
                if (!forceBreak)
                    throw new NoForceBreakException(_fol.UniqueId);
                else
                    _fol.ListItemAllFields.BreakRoleInheritance(true, true);
            }
            else if (!HasUniquePermissions.HasValue)
                throw new InvalidOperationException("This object's permissions cannot be modified!");

            var list = new List<RoleAssignment>(bindingCol.Count);
            for (int i = 0; i < bindingCol.Count; i++)
            {
                var binding = bindingCol[i];
                var bCol = new RoleDefinitionBindingCollection(CTX.SP1)
                {
                    binding.Definition
                };
                list.Add(_fol.ListItemAllFields.RoleAssignments.Add(
                    binding.Principal, bCol));
                foreach (var ass in list)
                {
                    CTX.Lae(ass, false);
                }
                _fol.Update();
                CTX.Lae();
            }
            if (_perms != null)
                _perms.AddRange(list);
            else
                this.GetPermissions();
        }

        public void AddFolderPermission(SPBinding binding, bool forceBreak = false) =>
            AddFolderPermission(new SPBindingCollection(binding), forceBreak);

        public void AddFolderPermission(Principal principal, RoleDefinition roleDef, bool forceBreak = false) =>
            AddFolderPermission(new SPBindingCollection(principal, roleDef), forceBreak);

        public void AddFolderPermission(string logonName, string roleDefinition, bool forceBreak = false)
        {
            var user = CTX.SP1.Web.EnsureUser(logonName);
            CTX.Lae(user);
            var allRoles = CTX.SP1.Web.RoleDefinitions;
            CTX.Lae(allRoles, true,
                ar => ar.Include(
                    r => r.Name
                )
            );
            var roleDef = allRoles.Where(x => string.Equals(x.Name, roleDefinition, StringComparison.OrdinalIgnoreCase)).Single();
            AddFolderPermission(new SPBindingCollection(user, roleDef), forceBreak);
        }
        #endregion

        #region Operators
        public static implicit operator SPFolder(Folder fol) => new SPFolder(fol);
        public static explicit operator Folder(SPFolder spFol) => (Folder)spFol.ShowOriginal();

        #endregion
    }
}
