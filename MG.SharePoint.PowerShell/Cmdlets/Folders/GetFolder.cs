using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;

namespace MG.SharePoint.PowerShell
{
    [Cmdlet(VerbsCommon.Get, "Folder", ConfirmImpact = ConfirmImpact.None, DefaultParameterSetName = "ByUrl")]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(Folder))]
    public class GetFolder : BaseSPCmdlet
    {
        #region PRIVATE FIELDS/CONSTANTS


        #endregion

        #region PARAMETERS
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "ByFolderInput")]
        public Folder Folder { get; set; }

        [Parameter(Mandatory = true, DontShow = true, ParameterSetName = "ByFolderCollectionInput")]
        public FolderCollection FolderCollection { get; set; }

        [Parameter(Mandatory = true, DontShow = true, ValueFromPipeline = true, ParameterSetName = "ByListInput")]
        public List List { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "ByWebAndUrlInput")]
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "ByWebAndIdInput")]
        public Web Web { get; set; }

        [Parameter(Mandatory = false, Position = 0, ParameterSetName = "ByUrl")]
        [Parameter(Mandatory = false, Position = 0, ParameterSetName = "ByWebAndUrlInput")]
        [Parameter(Mandatory = false, Position = 0, ParameterSetName = "ByFolderCollectionInput")]
        public string[] RelativeUrl { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "ByWebAndIdInput")]
        [Parameter(Mandatory = true, ParameterSetName = "ByIdInput")]
        [AllowEmptyCollection]
        public Guid[] Id { get; set; }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing() => base.BeginProcessing();

        protected override void ProcessRecord()
        {
            if (this.Folder != null)
            {
                this.Folder.LoadFolderProps();
                base.WriteObject(this.Folder);
            }
            else if (this.FolderCollection != null)
            {
                this.FolderCollection.LoadAllFolders();
                base.WriteObject(this.FolderCollection, true);
            }
            else if (this.List != null)
            {
                this.List.RootFolder.LoadFolderProps();
                base.WriteObject(this.List.RootFolder);
            }
            else if (this.Id != null && this.Id.Length > 0)
            {
                if (this.Web == null)
                    this.Web = CTX.SP1.Web;
                
                for (int i = 0; i < this.Id.Length; i++)
                {
                    Folder f = this.Web.GetFolderById(this.Id[i]);
                    f.LoadFolderProps();
                    base.WriteObject(f);
                }
            }
            else
            {
                bool hasUrls = false;
                if (this.RelativeUrl != null && this.RelativeUrl.Length > 0)
                {
                    this.RelativeUrl = this.FormatUrls(this.RelativeUrl);
                    hasUrls = true;
                }

                if (this.Web == null)
                    this.Web = CTX.SP1.Web;
                
                if (hasUrls)
                {
                    for (int i = 0; i < this.RelativeUrl.Length; i++)
                    {
                        Folder f = this.Web.GetFolderByServerRelativeUrl(this.RelativeUrl[i]);
                        f.LoadFolderProps();
                        base.WriteObject(f);
                    }
                }
                else
                {
                    this.Web.Folders.Initialize();
                    this.Web.Folders.LoadAllFolders();
                    base.WriteObject(this.Web.Folders, true);
                }
            }
        }

        #endregion

        #region METHODS
        private string[] FormatUrls(string[] urls)
        {
            string[] retStrs = new string[urls.Length];
            for (int i = 0; i < urls.Length; i++)
            {
                string url = urls[i];
                if (!url.StartsWith("/"))
                    url = "/" + url;

                retStrs[i] = url;
            }
            return retStrs;
        }

        #endregion
    }
}
