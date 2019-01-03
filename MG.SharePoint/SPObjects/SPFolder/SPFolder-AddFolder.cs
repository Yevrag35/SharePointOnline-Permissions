using Microsoft.SharePoint.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MG.SharePoint
{
    public partial class SPFolder : SPSecurable
    {
        #region Add SubFolders
        public void AddSubFolder(string folderName) =>
            _fol.AddSubFolder(folderName);

        public SPFolder AddSubFolder(string folderName, string principal, string roleDefinition, bool permissionsApplyRecursively) =>
            AddSubFolder(folderName, permissionsApplyRecursively, new SPBinding(principal, roleDefinition));

        public SPFolder AddSubFolder(string folderName, IDictionary bindingHash, bool permissionsApplyRecursively) =>
            AddSubFolder(folderName, new SPBindingCollection(ResolvePermissions(bindingHash)), permissionsApplyRecursively);
        
        public SPFolder AddSubFolder(string folderName, Principal principal, RoleDefinition roleDefinition, bool permissionsApplyRecursively) =>
            AddSubFolder(folderName, permissionsApplyRecursively, new SPBinding(principal, roleDefinition));

        public SPFolder AddSubFolder(string folderName, bool permissionsApplyRecursively, params SPBinding[] bindings) =>
            AddSubFolder(folderName, new SPBindingCollection(bindings), permissionsApplyRecursively);

        public SPFolder AddSubFolder(string folderName, SPBindingCollection bindingCol, bool permissionsApplyRecursively)
        {
            var newFolder = (SPFolder)CTX.SP1.Web.Folders.Add(_fol.ServerRelativeUrl + "/" + folderName);
            newFolder.AddPermission(bindingCol, true, permissionsApplyRecursively);
            return newFolder;
        }

        #endregion
    }
}
