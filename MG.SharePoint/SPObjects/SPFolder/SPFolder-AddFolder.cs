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

        public SPFolder AddSubFolder(string folderName, string principal, string roleDefinition) =>
            AddSubFolder(folderName, new SPBinding(principal, roleDefinition));

        public SPFolder AddSubFolder(string folderName, IDictionary bindingHash) =>
            AddSubFolder(folderName, new SPBindingCollection(ResolvePermissions(bindingHash)));
        
        public SPFolder AddSubFolder(string folderName, Principal principal, RoleDefinition roleDefinition) =>
            AddSubFolder(folderName, new SPBinding(principal, roleDefinition));

        public SPFolder AddSubFolder(string folderName, params SPBinding[] bindings) =>
            AddSubFolder(folderName, new SPBindingCollection(bindings));

        public SPFolder AddSubFolder(string folderName, SPBindingCollection bindingCol)
        {
            var newFolder = (SPFolder)CTX.SP1.Web.Folders.Add(_fol.ServerRelativeUrl + "/" + folderName);
            newFolder.AddPermission(bindingCol, true);
            return newFolder;
        }

        #endregion
    }
}
