using Microsoft.SharePoint.Client;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;

namespace MG.SharePoint.PowerShell
{
    [Cmdlet(VerbsCommon.Add, "SPPermission", SupportsShouldProcess = true)]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(SPPermissionCollection))]
    public class AddSPPermission : GetSPPermission, IDynamicParameters
    {
        private protected Collection<Attribute> colAtt = new Collection<Attribute>()
        {
            new ParameterAttribute()
            {
                Mandatory = true,
                Position = 1
            },
            new AliasAttribute("Permission", "perm"),
            new AllowNullAttribute()
        };

        private protected string[] _roleNames;
        internal const string pName = "Role";

        private protected RuntimeDefinedParameterDictionary rtDict;

        [Parameter(Mandatory = false, Position = 0)]
        public string Principal { get; set; }

        [Parameter(Mandatory = false)]
        public Principal SPPrincipal { get; set; }

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

            if (string.IsNullOrEmpty(Principal) && SPPrincipal == null)
                throw new ArgumentNullException("Either Principal or SPPrincipal must be specified!");

            if (MyInvocation.BoundParameters.ContainsKey("Principal"))
                SPPrincipal = CTX.SP1.Web.EnsureUser(Principal);

            CTX.Lae(SPPrincipal, true);
        }

        protected override void ProcessRecord()
        {
            base.CheckParameters();

            // Get the chosen role
            RoleDefinition _role = CTX.AllRoles.Single(x => x.Name.Equals((string)rtDict[pName].Value, StringComparison.InvariantCultureIgnoreCase));

            if (SPObject.HasUniquePermissions.HasValue && (SPObject.HasUniquePermissions.Value || !SPObject.HasUniquePermissions.Value &&
                (_force || ShouldContinue(SPObject.Id.ToString(), "Break Inheritance"))))
                SPObject.AddPermission(SPPrincipal, _role, true);
            else
                throw new InvalidOperationException("I wouldn't do that if I were you...");

            SPObject.GetPermissions();
            WriteObject(SPObject.Permissions, false);
        }

        private protected RuntimeDefinedParameterDictionary DoDynamic()
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

        private protected string[] GetAllRoles()
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
    }
}
