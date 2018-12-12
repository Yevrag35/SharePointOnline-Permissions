using Microsoft.SharePoint.Client;
using System;
using System.Management.Automation;

namespace MG.SharePoint.PowerShell
{
    [Cmdlet(VerbsCommon.Get, "SPWeb")]
    [OutputType(typeof(SPWeb))]
    public class GetSPWeb : PropertyLoadingCmdlet
    {
        protected internal override string[] SkipThese => 
        protected internal override Type ThisType => throw new NotImplementedException();

        protected override void BeginProcessing() => base.BeginProcessing();

        //protected override void ProcessRecord()
        //{
        //    base.ProcessRecord();
        //    var web = new SPWeb();
        //    if (_withPerms)
        //        web.GetPermissions();

        //    WriteObject(web, false);
        //}
    }
}
