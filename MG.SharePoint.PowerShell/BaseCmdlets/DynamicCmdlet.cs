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

        protected internal abstract object DoDynamic();

        protected private void LoadWithDynamic(string paramName, SPObject spObj)
        {
            var addProps = rtDict[paramName].Value;
            string[] propNames = ((IEnumerable)addProps).Cast<string>().ToArray();
            spObj.LoadProperty(propNames);
        }

        protected private void LoadWithExplicit(string[] props, string[] references, SPObject spObj)
        {
            var psToLoad = new List<string>();

            var wco = WildcardOptions.IgnoreCase;
            for (int i = 0; i < props.Length; i++)
            {
                var p = props[i];
                var wcp = new WildcardPattern(p, wco);
                for (int t = 0; t < references.Length; t++)
                {
                    var name = references[t];
                    if (wcp.IsMatch(name))
                        psToLoad.Add(name);
                }
            }
            spObj.LoadProperty(psToLoad.ToArray());
        }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            if (rtDict == null)
            {

            }
        }
    }
}
