using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace MG.SharePoint.PowerShell
{
    public abstract class DynamicCmdlet : BaseSPCmdlet, IDynamicParameters
    {
        protected internal RuntimeDefinedParameterDictionary rtDict;

        public object GetDynamicParameters() => base.CheckSession() ? DoDynamic() : null;

        protected internal abstract RuntimeDefinedParameterDictionary DoDynamic();

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            if (rtDict == null)
                rtDict = DoDynamic();
        }
    }
}
