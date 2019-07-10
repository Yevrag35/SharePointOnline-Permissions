using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;

namespace MG.SharePoint.PowerShell
{
    [Cmdlet(VerbsCommon.Get, "Web", ConfirmImpact = ConfirmImpact.None, DefaultParameterSetName = "None")]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(Web))]
    public class GetWeb : BaseSPCmdlet
    {
        #region PRIVATE FIELDS/CONSTANTS


        #endregion

        #region PARAMETERS
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "ByWebInput")]
        public Web Web { get; set; }

        [Parameter(Mandatory = true, Position = 0, ParameterSetName = "BySpecificUrl")]
        public Uri Url { get; set; }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing()
        {
        }

        protected override void ProcessRecord()
        {
            if (!CTX.Connected && this.Web == null)
            {
                throw new ContextNotSetException();
            }
            else if (this.Web == null && this.Url == null)
            {
                CTX.SP1.Web.LoadWeb();
                base.WriteObject(CTX.SP1.Web);
            }
            else if (this.Url != null)
            {
                base.WriteObject(CTX.GetWebByUrl(this.Url.ToString()));
            }
            else if (this.Web != null)
            {
                this.Web.LoadWeb();
                base.WriteObject(this.Web);
            }
        }

        #endregion

        #region CMDLET METHODS


        #endregion
    }
}