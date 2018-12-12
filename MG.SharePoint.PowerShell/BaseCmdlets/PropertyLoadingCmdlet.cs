using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;

namespace MG.SharePoint.PowerShell
{
    public abstract class PropertyLoadingCmdlet : DynamicCmdlet
    {
        protected internal string[] Properties;
        internal const string pName = "Property";
        protected internal abstract string[] SkipThese { get; }
        protected internal abstract Type ThisType { get; }

        private Collection<Attribute> attCol = new Collection<Attribute>()
        {
            new ParameterAttribute()
            {
                Mandatory = false
            },
            new AliasAttribute("p", "prop", "props"),
            new AllowNullAttribute()
        };

        protected internal static string[] GetPropertyNames(Type spType, params string[] skipThese)
        {
            var allProps = spType.GetProperties();
            if (skipThese != null)
                allProps = allProps.Where(x => !skipThese.Contains(x.Name)).ToArray();

            var propNames = new string[allProps.Length];
            for (int i = 0; i < allProps.Length; i++)
            {
                propNames[i] = allProps[i].Name;
            }

            return propNames;
        }

        protected internal override RuntimeDefinedParameterDictionary DoDynamic() => DoDynamicProperties(ThisType, SkipThese);

        protected internal RuntimeDefinedParameterDictionary DoDynamicProperties(Type spType, params string[] skipThese)
        {
            if (rtDict == null)
            {
                Properties = GetPropertyNames(spType, skipThese);
                attCol.Add(new ValidateSetAttribute(Properties));
                var rtp = new RuntimeDefinedParameter(pName, typeof(string[]), attCol);
                rtDict = new RuntimeDefinedParameterDictionary()
                {
                    { pName, rtp }
                };
            }
            return rtDict;
        }

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
    }
}
