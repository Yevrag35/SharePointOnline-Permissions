using Microsoft.SharePoint.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Management.Automation;
using System.Reflection;
using System.Security;

namespace MG.SharePoint.PowerShell.Cmdlets.Files
{
    [Cmdlet(VerbsCommon.Get, "File", ConfirmImpact = ConfirmImpact.None, DefaultParameterSetName = "ByWebInput")]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(File))]
    public class GetFile : BaseSPCmdlet
    {
        #region FIELDS/CONSTANTS


        #endregion

        #region PARAMETERS
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "ByListItemInput")]
        public ListItem ListItem { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "ByFileInput")]
        public File File { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "ByFolderInput")]
        public Folder Folder { get; set; }

        [Parameter(Mandatory = true, DontShow = true, ParameterSetName = "ByFileCollection")]
        public FileCollection FileCollection { get; set; }

        [Parameter(Mandatory = false, ValueFromPipeline = true, ParameterSetName = "ByWebInput")]
        public Web Web { get; set; }

        [Parameter(Mandatory = false, ParameterSetName = "ByWebInput")]
        [Parameter(Mandatory = false, ParameterSetName = "ByFileCollection")]
        public Identity[] Identity { get; set; }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing() => base.BeginProcessing();

        protected override void ProcessRecord()
        {
            if (this.File != null)
            {
                this.File.LoadFileProps();
                base.WriteObject(this.File);
            }
            else if (this.ListItem != null)
            {
                this.ListItem.LoadProperty(x => x.File);
                this.ListItem.File.LoadFileProps();
                base.WriteObject(this.ListItem.File);
            }
            else
            {
                if (ParameterSetName == "ByWebInput" && this.Web == null)
                    this.Web = CTX.SP1.Web;

                IQueryable<File> items = null;
                switch (ParameterSetName)
                {
                    case "ByWebInput":
                    {
                        if (this.Identity != null && this.Identity.Length > 0)
                        {
                            for (int i = 0; i < this.Identity.Length; i++)
                            {
                                Identity id = this.Identity[i];
                                File file = null;
                                if (id.IsUrl)
                                {
                                    file = this.Web.GetFileByServerRelativeUrl(((Uri)id).ToString());
                                }
                                else if (id.IsGuid)
                                {
                                    file = this.Web.GetFileById((Guid)id);
                                }
                                else
                                {
                                    base.WriteError("The specified identity can't be used for searching with a Web object.", ErrorCategory.InvalidArgument);
                                }

                                if (file != null)
                                {
                                    file.LoadFileProps();
                                    base.WriteObject(file);
                                }
                            }
                        }
                        else  // this is going to take a while...
                        {
                            var folderCol = this.Web.Folders;
                            folderCol.Initialize();
                            for (int i = 0; i < folderCol.Count; i++)
                            {
                                Folder fol = folderCol[i];
                                fol.LoadProperty(x => x.Files);
                                fol.Files.LoadAllFiles();
                                base.WriteObject(fol.Files, true);
                            }
                        }
                        break;
                    }
                    case "ByFileCollection":
                    {
                        this.FileCollection.LoadAllFiles();
                        if (this.Identity != null && this.Identity.Length > 0)
                        {
                            for (int t = 0; t < this.Identity.Length; t++)
                            {
                                Identity id = this.Identity[t];
                                for (int i = 0; i < this.FileCollection.Count; i++)
                                {
                                    File f = this.FileCollection[i];
                                    f.LoadProperty(x => x.ListItemAllFields.Id);
                                    if (id.IsGuid && f.UniqueId.Equals((Guid)id))
                                        base.WriteObject(f);

                                    else if (id.IsNumeric && f.ListItemAllFields.Id.Equals((int)id))
                                        base.WriteObject(f);

                                    else if (id.IsUrl && f.ServerRelativeUrl.Equals(((Uri)id).ToString(), StringComparison.CurrentCultureIgnoreCase))
                                        base.WriteObject(f);

                                    else if (id.IsString && (f.Title.Equals((string)id, StringComparison.CurrentCultureIgnoreCase) ||
                                        f.Name.Equals((string)id, StringComparison.CurrentCultureIgnoreCase)))
                                    {
                                        base.WriteObject(f);
                                    }
                                }
                            }
                        }
                        else
                        {
                            base.WriteObject(this.FileCollection, true);
                        }
                        break;
                    }
                }
            }
        }

        #endregion

        #region METHODS


        #endregion
    }
}