using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Management.Automation;
using System.Reflection;

namespace MG.SharePoint.PowerShell
{
    [Cmdlet(VerbsCommon.Find, "File", ConfirmImpact = ConfirmImpact.None, DefaultParameterSetName = "ByName")]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(File))]
    public class FindFile : BaseSPCmdlet
    {
        #region PRIVATE FIELDS/CONSTANTS
        private WildcardOptions _opts;

        #endregion

        #region PARAMETERS
        [Parameter(Mandatory = false, DontShow = true, ValueFromPipeline = true)]
        public Web Web { get; set; }

        [Parameter(Mandatory = true, Position = 0, ParameterSetName = "ByName")]
        [SupportsWildcards]
        public string[] Name { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter IsCaseSensitive { get; set; }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            if (this.Web == null)
                this.Web = CTX.SP1.Web;

            if (Cache.SearchCache == null)
                Cache.SearchCache = new SearchCollection();

            if (!Cache.CurrentWeb.Equals(this.Web.Id))
            {
                Cache.SearchCache.Clear();
                Cache.CurrentWeb = this.Web.Id;
            }

            if (!Cache.SearchCache.ContainsFiles)
                this.PopulateSearchCache();

            _opts = this.OptionsFromBool(this.IsCaseSensitive.ToBool());
        }

        protected override void ProcessRecord()
        {
            base.WriteVerbose("Performing search against file cache...");
            var wcps = new WildcardPattern[this.Name.Length];
            for (int i = 0; i < this.Name.Length; i++)
            {
                wcps[i] = new WildcardPattern(this.Name[i], _opts);
            }

            ClientObject[] found = Cache.SearchCache.FindAll(x => x.NameMatchesPattern(SearchObjectType.File, wcps));
            base.WriteObject(found, true);
        }

        #endregion

        #region CMDLET METHODS
        private WildcardOptions OptionsFromBool(bool sp)
        {
            return !sp 
                ? WildcardOptions.Compiled | WildcardOptions.IgnoreCase 
                : WildcardOptions.Compiled;
        }

        private void PopulateSearchCache()
        {
            base.WriteWarning("Loading cache... this could take a while.");
            Cache.SearchCache.AddFileFromFolderCollection(this.Web.Folders);
        }

        #endregion
    }
}