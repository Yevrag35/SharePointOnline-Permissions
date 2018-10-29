using Microsoft.SharePoint.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MG.SharePoint
{
    public partial class SPFolder : ISPObject
    {
        #region Add SubFolders
        public void AddSubFolder(string folderName) =>
            _fol.AddSubFolder(folderName);

        public SPFolder AddSubFolder(string folderName, string principal, string roleDefinition) =>
            AddSubFolder(folderName, new SPBinding(principal, roleDefinition));

        public SPFolder AddSubFolder(string folderName, Principal principal, RoleDefinition roleDefinition) =>
            AddSubFolder(folderName, new SPBinding(principal, roleDefinition));

        public SPFolder AddSubFolder(string folderName, SPBinding binding) =>
            AddSubFolder(folderName, new SPBindingCollection(binding));

        public SPFolder AddSubFolder(string folderName, SPBindingCollection bindingCol)
        {
            SPFolder newFolder = CTX.SP1.Web.Folders.Add(_fol.ServerRelativeUrl + "/" + folderName);
            newFolder.AddFolderPermission(bindingCol, true);
            return newFolder;
        }

        #endregion
    }
}
