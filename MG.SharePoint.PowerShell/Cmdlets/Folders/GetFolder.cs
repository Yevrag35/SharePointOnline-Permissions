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
        private const string EX_MSG = "The folder \"{0}\" could not be retrieved.  {1}";

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
            if (this.Web == null && CTX.Connected)
                this.Web = CTX.SP1.Web;

            if (this.Folder != null)
            {
                this.Folder.LoadFolderProps();
                base.WriteObject(this.Folder);
            }
            else if (this.FolderCollection != null)
            {
                this.FolderCollection.LoadAllFolders();
                var list = new ClientObjectViewableCollection<Folder>(this.FolderCollection);
                base.WriteObject(list, false);
            }
            else if (this.List != null)
            {
                this.List.RootFolder.LoadFolderProps();
                base.WriteObject(this.List.RootFolder);
            }
            else if (this.Id != null && this.Id.Length > 0)
            {
                var list = new ClientObjectViewableCollection<Folder>(this.Id.Length);
                for (int i = 0; i < this.Id.Length; i++)
                {
                    Folder f = this.Web.GetFolderById(this.Id[i]);
                    try
                    {
                        f.LoadFolderProps();
                        list.Add(f);
                    }
                    catch (ServerException sex)
                    {
                        base.WriteError(sex, ErrorCategory.ObjectNotFound);
                    }
                }
                base.WriteObject(list, false);
            }
            else
            {
                bool hasUrls = false;
                if (this.RelativeUrl != null && this.RelativeUrl.Length > 0)
                {
                    if (!this.Web.IsPropertyReady(x => x.ServerRelativeUrl))
                        this.Web.LoadProperty(x => x.ServerRelativeUrl);

                    string webUrl = this.Web.ServerRelativeUrl;

                    this.RelativeUrl = this.FormatUrls(webUrl, this.RelativeUrl);
                    hasUrls = true;
                }

                if (this.Web == null)
                    this.Web = CTX.SP1.Web;
                
                if (hasUrls)
                {
                    var list = new ClientObjectViewableCollection<Folder>(this.RelativeUrl.Length);
                    for (int i = 0; i < this.RelativeUrl.Length; i++)
                    {
                        string relUrl = this.RelativeUrl[i];
                        Folder f = this.Web.GetFolderByServerRelativeUrl(relUrl);
                        try
                        {
                            f.LoadFolderProps();
                            list.Add(f);
                        }
                        catch (ServerException sex)
                        {
                            string msg = string.Format(EX_MSG, relUrl, sex.Message);
                            base.WriteError(msg, sex, ErrorCategory.ObjectNotFound, f);
                        }
                    }
                    base.WriteObject(list, false);
                }
                else
                {
                    this.Web.Folders.Initialize();
                    this.Web.Context.Load(this.Web.Folders, fols => fols.Include(f => f.Name));
                    this.Web.Context.ExecuteQuery();
                    var list = new ClientObjectViewableCollection<Folder>(this.Web.Folders.Count);
                    foreach (Folder f in this.Web.Folders.Where(x => !x.Name.StartsWith("_")))
                    { 
                        try
                        {
                            f.LoadFolderProps();
                            list.Add(f);
                        }
                        catch (ServerException sex)
                        {
                            f.LoadProperty(x => x.Name);
                            string msg = string.Format(EX_MSG, f.Name, sex.Message);
                            base.WriteError(msg, sex, ErrorCategory.MetadataError, f);
                        }
                    }
                    base.WriteObject(list, false);
                }
            }
        }

        #endregion

        #region METHODS
        private string[] FormatUrls(string webUrl, string[] urls)
        {
            string[] retStrs = new string[urls.Length];
            for (int i = 0; i < urls.Length; i++)
            {
                string url = urls[i];
                if (!url.StartsWith("/"))
                    url = "/" + url;
                
                if (!url.StartsWith(webUrl, StringComparison.CurrentCultureIgnoreCase))
                {
                    url = webUrl + url;
                }

                retStrs[i] = url;
            }
            return retStrs;
        }

        #endregion
    }
}
