using Microsoft.SharePoint.Client;
using System;
using System.Collections;

namespace MG.SharePoint
{
    public partial class SPFolder : SPSecurable
    {
        #region Upload Document
        public SPFile UploadFile(string localFilePath, bool permissionsApplyRecursively, bool forceOverwrite = false, bool refreshFiles = true)
        {
            string fileName = localFilePath.Replace(localFilePath.Substring(0, localFilePath.LastIndexOf("\\") + 1), string.Empty);
            byte[] upBytes = System.IO.File.ReadAllBytes(localFilePath);

            var fileCreationInfo = new FileCreationInformation()
            {
                Content = upBytes,
                Overwrite = forceOverwrite,
                Url = ServerRelativeUrl + "/" + fileName
            };
            var uploadFile = (SPFile)_fol.Files.Add(fileCreationInfo);
            if (refreshFiles)
            {
                if (this.Files == null)
                    this.LoadProperty("Files");

                else
                    this.Files.Add(uploadFile);
            }

            return uploadFile;
        }

        public SPFile UploadFile(string localFilePath, IDictionary permissionsHash, bool permissionsApplyRecursively, bool copyRoleAssignments = true,
            bool forceOverwrite = false, bool refreshFiles = true)
        {
            SPFile uploadedFile = UploadFile(localFilePath, forceOverwrite, refreshFiles);
            uploadedFile.BreakInheritance(copyRoleAssignments, true);
            uploadedFile.AddPermission(permissionsHash, true, permissionsApplyRecursively);
            return uploadedFile;
        }

        public SPFile UploadFile(string localFilePath, SPBindingCollection bindingCol, bool permissionsApplyRecursively, bool copyRoleAssignments = true,
            bool forceOverwrite = false, bool refreshFiles = true)
        {
            SPFile uploadedFile = UploadFile(localFilePath, forceOverwrite, refreshFiles);
            uploadedFile.BreakInheritance(copyRoleAssignments, true);
            uploadedFile.AddPermission(bindingCol, true, permissionsApplyRecursively);
            return uploadedFile;
        }

        #endregion
    }
}
