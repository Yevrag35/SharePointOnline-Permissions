using Microsoft.SharePoint.Client;
using System;
using System.Collections;

namespace MG.SharePoint
{
    public partial class SPFolder : SPSecurable
    {
        #region Upload Document
        public SPFile UploadFile(string localFilePath, bool forceOverwrite = false, bool refreshFiles = true)
        {
            var fileName = localFilePath.Replace(localFilePath.Substring(0, localFilePath.LastIndexOf("\\") + 1), string.Empty);
            var upBytes = System.IO.File.ReadAllBytes(localFilePath);

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

        public SPFile UploadFile(string localFilePath, IDictionary permissionsHash, bool copyRoleAssignments = true,
            bool forceOverwrite = false, bool refreshFiles = true)
        {
            var uploadedFile = UploadFile(localFilePath, forceOverwrite, refreshFiles);
            uploadedFile.BreakInheritance(copyRoleAssignments, true);
            uploadedFile.AddPermission(permissionsHash);
            return uploadedFile;
        }

        public SPFile UploadFile(string localFilePath, SPBindingCollection bindingCol, bool copyRoleAssignments = true,
            bool forceOverwrite = false, bool refreshFiles = true)
        {
            var uploadedFile = UploadFile(localFilePath, forceOverwrite, refreshFiles);
            uploadedFile.BreakInheritance(copyRoleAssignments, true);
            uploadedFile.AddPermission(bindingCol);
            return uploadedFile;
        }

        #endregion
    }
}
