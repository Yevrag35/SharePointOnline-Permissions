using Microsoft.SharePoint.Client;
using System;
using System.Management.Automation;

namespace MG.SharePoint.PowerShell
{
    [Cmdlet(VerbsCommon.Get, "SPWeb")]
    [OutputType(typeof(SPWeb))]
    public class GetSPWeb : PropertyLoadingCmdlet
    {
        protected internal override string[] SkipThese => new string[5]
            { "Created", "HasUniquePemrissions", "Id", "Name", "RelativeUrl" };
        protected internal override Type ThisType => typeof(SPWeb);

        protected override void BeginProcessing() => base.BeginProcessing();

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            var web = new SPWeb();
            if (MyInvocation.BoundParameters.ContainsKey("Property"))
                LoadWithDynamic(pName, web);
            WriteObject(web);
        }
    }
}
