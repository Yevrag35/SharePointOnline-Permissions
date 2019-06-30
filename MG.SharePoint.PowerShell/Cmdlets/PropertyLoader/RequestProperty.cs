using MG.Dynamic;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Reflection;

namespace MG.SharePoint.PowerShell
{
    [Cmdlet(VerbsLifecycle.Request, "Property", ConfirmImpact = ConfirmImpact.None)]
    [CmdletBinding(PositionalBinding = false)]
    [Alias("Load-Property")]
    public class RequestProperty : BaseSPCmdlet
    {
        #region PRIVATE FIELDS/CONSTANTS
        private const BindingFlags PUB_INST = BindingFlags.Instance | BindingFlags.Public;
        private const BindingFlags PRIV_INST = BindingFlags.Instance | BindingFlags.NonPublic;

        private static readonly string[] SkipThese = new string[6]
        {
            "Context", "Path", "ResourcePath", "Tag", "ServerObjectIsNull", "TypedObject"
        };

        #endregion

        #region PARAMETERS
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public ClientObject InputObject { get; set; }

        [Parameter(Mandatory = true, Position = 0)]
        [SupportsWildcards]
        public string[] Property { get; set; }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing() => base.BeginProcessing();

        protected override void ProcessRecord()
        {
            string[] matchingProps = this.MatchChosenToBacking(this.InputObject.GetType(), this.Property);

            Type inType = this.InputObject.GetType();
            MethodInfo mi = this.GetLoadPropertyMethod(inType);
            MethodInfo cm = this.GetCastMethod(inType);
            object castObj = cm.Invoke(this, new object[1] { this.InputObject });
            object outObj = null;
            try
            {
                outObj = mi.Invoke(this, new object[2] { castObj, matchingProps });
            }
            catch (TargetInvocationException tie)
            {
                base.WriteError(tie.InnerException, ErrorCategory.MetadataError, castObj);
            }
            base.WriteObject(outObj);
        }

        #endregion

        #region CMDLET METHODS
        private T Cast<T>(dynamic o) => (T)o;

        private T Load<T>(T obj, string[] props) where T : ClientObject
        {
            obj.LoadProperty(props);
            return obj;
        }

        private MethodInfo GetCastMethod(Type inType) => this.GetType().GetMethod("Cast", PRIV_INST).MakeGenericMethod(inType);
        private MethodInfo GetLoadPropertyMethod(Type inType) => this.GetType().GetMethod("Load", PRIV_INST).MakeGenericMethod(inType);

        private string[] MatchChosenToBacking(Type inputType, string[] chosenProps)
        {
            IEnumerable<PropertyInfo> props = inputType.GetProperties(PUB_INST).Where(x => !SkipThese.Contains(x.Name));
            var wcps = new WildcardPattern[chosenProps.Length];
            for (int s = 0; s < chosenProps.Length; s++)
            {
                wcps[s] = new WildcardPattern(chosenProps[s], WildcardOptions.IgnoreCase);
            }

            var list = new List<string>();
            foreach (PropertyInfo pi in props)
            {
                for (int n = 0; n < wcps.Length; n++)
                {
                    if (wcps[n].IsMatch(pi.Name))
                    {
                        list.Add(pi.Name);
                        break;
                    }
                }
            }
            return list.ToArray();
        }

        #endregion
    }
}