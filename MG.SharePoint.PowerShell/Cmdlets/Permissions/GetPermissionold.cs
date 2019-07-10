using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace MG.SharePoint.PowerShell
{
    [Cmdlet(VerbsCommon.Get, "Permission", ConfirmImpact = ConfirmImpact.None)]
    //[OutputType(typeof(Permission))]
    public class GetPermission : BaseSPCmdlet
    {
        #region PARAMETERS
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public SPSecurable InputObject { get; set; }

        #endregion

        private List<SPSecurable> securables;

        #region CMDLET PROCESSING
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            securables = new List<SPSecurable>();
        }
        protected override void ProcessRecord()
        {
            securables.Add(this.InputObject);
        }

        protected override void EndProcessing()
        {
            var allPerms = new List<SPPermission>();
            for (int i = 1; i <= securables.Count; i++)
            {
                this.UpdateProgress(0, i);
                var sec = securables[i - 1];
                if (sec.CanSetPermissions)
                {
                    SPPermissionCollection perms = this.InputObject.Permissions;
                    if (this.InputObject.Permissions == null)
                        perms = sec.GetPermissions();

                    allPerms.AddRange(perms);
                }
            }
            this.UpdateProgress(0);
            WriteObject(allPerms, true);
        }

        #endregion

        #region METHODS
        private const string COMPLETED = "Completed";
        protected private const double HUNDRED = 100d;
        protected private const MidpointRounding MIDPOINT = MidpointRounding.ToEven;
        protected private const ProgressRecordType REC_TYPE_COMPLETED = ProgressRecordType.Completed;
        protected private const int ROUND_DIGITS = 2;

        protected string StatusFormat => "Fetching permissions from object {0}/{1}...";
        protected string Activity => "Resolving Permissions";

        protected private void UpdateProgress(int id, int on)
        {
            var pr = new ProgressRecord(id, this.Activity, string.Format(
                this.StatusFormat, on, securables.Count)
            );
            this.WriteTheProgress(pr, on);
        }

        protected private void UpdateProgressAndName(int id, int on, string name)
        {
            var pr = new ProgressRecord(id, this.Activity, string.Format(
                this.StatusFormat, on, securables.Count, name)
            );
            this.WriteTheProgress(pr, on);
        }

        protected private void UpdateProgress(int id)
        {
            var pr = new ProgressRecord(id, this.Activity, COMPLETED)
            {
                RecordType = REC_TYPE_COMPLETED
            };
            WriteProgress(pr);
        }

        private void WriteTheProgress(ProgressRecord pr, int on)
        {
            double num = Math.Round(on / (double)securables.Count * HUNDRED, ROUND_DIGITS, MIDPOINT);
            pr.PercentComplete = Convert.ToInt32(num);
            WriteProgress(pr);
        }

        #endregion
    }
}
