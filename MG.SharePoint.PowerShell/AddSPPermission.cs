using Microsoft.SharePoint.Client;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;

namespace MG.SharePoint.PowerShell
{
    [Cmdlet(VerbsCommon.Add, "SPPermission", SupportsShouldProcess = true,
        DefaultParameterSetName = "ByStringPrincipal")]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(SPPermission))]
    public class AddSPPermission : GetSPPermission, IDynamicParameters
    {
        private protected Collection<Attribute> colAtt = new Collection<Attribute>()
        {
            new ParameterAttribute()
            {
                Mandatory = true,
                Position = 1,
                ParameterSetName = "ByPrincipalObject"
            },
            new AliasAttribute("Permission", "perm"),
            new AllowNullAttribute()
        };

        private protected string[] _roleNames;
        internal const string pName = "Role";

        private protected RuntimeDefinedParameterDictionary rtDict;

        [Parameter(Mandatory = false, Position = 0, ParameterSetName = "ByPrincipalObject")]
        public string Principal { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "ByPrincipalObject")]
        public Principal SPPrincipal { get; set; }

        [Parameter(Mandatory = true, Position = 0, ParameterSetName = "ByPermissionHashtable")]
        public IDictionary ApplyPermissionSet { get; set; }

        private bool _force;
        [Parameter(Mandatory = false)]
        public SwitchParameter Force
        {
            get => _force;
            set => _force = value;
        }

        public object GetDynamicParameters() => DoDynamic();

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            DoDynamic();

            if (ParameterSetName == "ByPrincipalObject")
            {
                if (string.IsNullOrEmpty(Principal) && SPPrincipal == null)
                    throw new ArgumentNullException("Either Principal or SPPrincipal must be specified!");

                if (MyInvocation.BoundParameters.ContainsKey("Principal"))
                    SPPrincipal = CTX.SP1.Web.EnsureUser(Principal);

                CTX.Lae(SPPrincipal, true);
            }
        }

        protected override void ProcessRecord()
        {
            base.CheckParameters();

            if (SPObject.HasUniquePermissions.HasValue && (SPObject.HasUniquePermissions.Value || !SPObject.HasUniquePermissions.Value &&
                (_force || ShouldContinue(SPObject.Id.ToString(), "Break Inheritance"))))
            {
                switch (ParameterSetName)
                {
                    case "ByPermissionHashtable":
                        SPObject.AddPermission(ApplyPermissionSet, true);
                        break;
                    default:
                        // Get the chosen role
                        RoleDefinition _role = CTX.AllRoles.Single(x => x.Name.Equals((string)rtDict[pName].Value, StringComparison.InvariantCultureIgnoreCase));

                        SPObject.AddPermission(SPPrincipal, _role, true);
                        break;
                }
            }
                
            else
                throw new InvalidOperationException("I wouldn't do that if I were you...");

            SPObject.GetPermissions();
            WriteObject(SPObject.Permissions, true);
        }

        private RuntimeDefinedParameterDictionary DoDynamic()
        {
            if (rtDict == null)
            {
                GetAllRoles();
                colAtt.Add(new ValidateSetAttribute(_roleNames));
                var rtp = new RuntimeDefinedParameter(pName, typeof(string), colAtt);
                rtDict = new RuntimeDefinedParameterDictionary()
                {
                    { pName, rtp }
                };
            }
            return rtDict;
        }

        private string[] GetAllRoles()
        {
            if (_roleNames == null)
            {
                if (CTX.AllRoles == null)
                {
                    CTX.AllRoles = CTX.SP1.Web.RoleDefinitions;
                    CTX.Lae(CTX.AllRoles, true,
                        ar => ar.Include(
                            r => r.Name
                        )
                    );
                }
                _roleNames = new string[CTX.AllRoles.Count];
                for (int i = 0; i < CTX.AllRoles.Count; i++)
                {
                    var role = CTX.AllRoles[i];
                    _roleNames[i] = role.Name;
                }
            }
            return _roleNames;
        }

        //private void SetRecursivePermissions()
        //{
        //    SPObject
        //}
    }
}
