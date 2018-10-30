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

        public SPFolder AddSubFolder(string folderName, IDictionary bindingHash)
        {
            var keys = bindingHash.Keys.Cast<string>().ToArray();
            var bindingCol = new SPBindingCollection();
            for (int i = 0; i < keys.Length; i++)
            {
                var key = keys[i];
                var prins = bindingHash[key];
                var role = Convert.ToString(key);
                string[] allPrins;
                if (!prins.GetType().IsArray)
                    allPrins = new string[1] { Convert.ToString(prins) };
                else
                    allPrins = ((IEnumerable)prins).Cast<string>().ToArray();

                for (int p = 0; p < allPrins.Length; p++)
                {
                    var prin = allPrins[p];
                    bindingCol.Add(new SPBinding(prin, role));
                }
            }
            return AddSubFolder(folderName, bindingCol);
        }   // @{ "Role" = "Principal"; "Role" = @("Principal", "Principal") }

        public SPFolder AddSubFolder(string folderName, Principal principal, RoleDefinition roleDefinition) =>
            AddSubFolder(folderName, new SPBinding(principal, roleDefinition));

        public SPFolder AddSubFolder(string folderName, params SPBinding[] bindings) =>
            AddSubFolder(folderName, new SPBindingCollection(bindings));

        public SPFolder AddSubFolder(string folderName, SPBindingCollection bindingCol)
        {
            SPFolder newFolder = CTX.SP1.Web.Folders.Add(_fol.ServerRelativeUrl + "/" + folderName);
            newFolder.AddFolderPermission(bindingCol, true);
            return newFolder;
        }

        #endregion
    }
}
