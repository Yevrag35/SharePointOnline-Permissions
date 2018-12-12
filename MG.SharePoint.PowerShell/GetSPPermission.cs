using Microsoft.SharePoint.Client;
using System;
using System.Linq;
using System.Management.Automation;

namespace MG.SharePoint.PowerShell
{
    [Cmdlet(VerbsCommon.Get, "SPPermission")]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(SPPermission))]
    public class GetSPPermission : BaseSPCmdlet
    {
        #region ClientObject Pipe Parameters

        [Parameter(Mandatory = false, DontShow = true, ValueFromPipeline = true)]
        public Web Web { get; set; }

        [Parameter(Mandatory = false, DontShow = true, ValueFromPipeline = true)]
        public List List { get; set; }

        [Parameter(Mandatory = false, DontShow = true, ValueFromPipeline = true)]
        public Folder Folder { get; set; }

        [Parameter(Mandatory = false, DontShow = true, ValueFromPipeline = true)]
        public File File { get; set; }

        [Parameter(Mandatory = false, DontShow = true, ValueFromPipeline = true)]
        public ListItem ListItem { get; set; }

        #endregion

        #region ISPPermissions Pipe Parameters

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public ISPPermissions SPObject { get; set; }

        #endregion

        protected override void BeginProcessing() => base.BeginProcessing();

        protected override void ProcessRecord()
        {
            CheckParameters();
            var perms = SPObject.GetPermissions();
            WriteObject(perms, true);
        }

        private protected void CheckParameters()
        {
            if (!MyInvocation.BoundParameters.ContainsKey("SPObject"))
            {
                var keys = MyInvocation.BoundParameters.Keys.Cast<string>().ToArray();
                for (int i = 0; i < keys.Length; i++)
                {
                    var key = keys[i];
                    if (!CommonParameters.Contains(key) && !OptionalCommonParameters.Contains(key))
                        SPObject = WrapObject(MyInvocation.BoundParameters[key]);
                }
            }
            if (SPObject == null)
                throw new ArgumentException("You must specify an input object!");
        }

        private ISPPermissions WrapObject(object cliObj)
        {
            Type ct = cliObj.GetType();
            ISPPermissions outputObj = null;

            switch (ct.Name)
            {
                case "Folder":
                    outputObj = (SPFolder)(Folder)cliObj;
                    break;
                case "List":
                    outputObj = (SPList)(List)cliObj;
                    break;
                case "Web":
                    outputObj = (SPWeb)(Web)cliObj;
                    break;
                case "ListItem":
                    outputObj = (SPListItem)(ListItem)cliObj;
                    break;
                case "File":
                    outputObj = (SPFile)(File)cliObj;
                    break;
            }
            return outputObj;
        }
    }
}
