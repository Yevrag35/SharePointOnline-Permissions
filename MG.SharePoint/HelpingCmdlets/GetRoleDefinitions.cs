using Microsoft.SharePoint.Client;
using System;
using System.Management.Automation;

namespace MG.SharePoint
{
    [Cmdlet(VerbsCommon.Get, "RoleDefinitions")]
    [OutputType(typeof(RoleDefinitionBindingCollection))]
    public class GetRoleDefinitions : PSCmdlet
    {
        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            var roleDefs = CTX.SP1.Web.RoleDefinitions;
            CTX.lae(roleDefs);
            WriteObject(roleDefs, false);
        }
    }
}
