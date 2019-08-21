using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Security;

namespace MG.SharePoint.PowerShell.Cmdlets.Permissions
{
    [Cmdlet(VerbsCommon.Add, "Permission", ConfirmImpact = ConfirmImpact.High, SupportsShouldProcess = true, DefaultParameterSetName = "ByRoleDefinitionName")]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(SPPermission))]
    public class AddPermission : PSCmdlet
    {
        #region FIELDS/CONSTANTS
        private RoleDefinitionCollection _roleCol;
        private SPBindingCollection _bindings;
        private List<SecurableObject> _secObjs;
        private bool _bi;
        private bool _force;
        private bool _passThru;
        private bool _recurse;

        #endregion

        #region PARAMETERS
        [Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        public SecurableObject InputObject { get; set; }

        [Parameter(Mandatory = true, Position = 0, ParameterSetName = "ByRoleDefinitionName")]
        [Parameter(Mandatory = true, Position = 0, ParameterSetName = "ByRoleDefinitionObject")]
        public PrincipalIdentity Principal { get; set; }

        [Parameter(Mandatory = true, Position = 0, ParameterSetName = "ByInputHashtable")]
        public IDictionary PermissionsTable { get; set; }

        [Parameter(Mandatory = true, Position = 1, ParameterSetName = "ByRoleDefinitionName")]
        public string RoleName { get; set; }

        [Parameter(Mandatory = true, Position = 1, ParameterSetName = "ByRoleDefinitionObject")]
        public RoleDefinition RoleDefinition { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter BreakInheritance
        {
            get => _bi;
            set => _bi = value;
        }

        [Parameter(Mandatory = false)]
        public SwitchParameter Recurse
        {
            get => _recurse;
            set => _recurse = value;
        }

        [Parameter(Mandatory = false)]
        public SwitchParameter PassThru
        {
            get => _passThru;
            set => _passThru = value;
        }

        [Parameter(Mandatory = false, DontShow = true)]
        public SwitchParameter Force
        {
            get => _force;
            set => _force = value;
        }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            _secObjs = new List<SecurableObject>();
            if (this.ParameterSetName == "ByInputHashtable")
                _bindings = new SPBindingCollection(this.PermissionsTable);

            else
                _bindings = new SPBindingCollection(2);
        }

        protected override void ProcessRecord()
        {
            if (!this.InputObject.CanSetPermissions())
                throw new ArgumentException("This SecurableObject cannot have custom permissions defined.");

            if (!this.MyInvocation.BoundParameters.ContainsKey("PermissionsTable"))
            {
                if (this.MyInvocation.BoundParameters.ContainsKey("RoleName") && this.RoleDefinition == null)
                {
                    if (_roleCol == null)
                        _roleCol = ((ClientContext)this.InputObject.Context).Web.RoleDefinitions;

                    try
                    {
                        this.RoleDefinition = _roleCol.GetByName(this.RoleName);
                        this.RoleDefinition.LoadDefinition();
                    }
                    catch (ServerException sex)
                    {
                        throw new ArgumentException(string.Format("No role definition named \"{0}\" was found in this site collection.", this.RoleName), sex);
                    }
                }

                Principal spPrin = null;
                if (this.Principal.IsLogonName)
                {
                    spPrin = ((ClientContext)this.InputObject.Context).Web.EnsureUser(this.Principal.LogonName);
                    spPrin.LoadProperty(x => x.Id, x => x.IsHiddenInUI, x => x.LoginName, x => x.PrincipalType, x => x.Title);
                }
                else
                    spPrin = this.Principal.Principal;

                _bindings.Add(spPrin, this.RoleDefinition);
            }
        }

        protected override void EndProcessing()
        {
            for (int i = 0; i < _secObjs.Count; i++)
            {
                SecurableObject secObj = _secObjs[i];
                if (_force || base.ShouldProcess("SecurableObject", "Add Permissions"))
                {
                    secObj.AddPermission(_bindings, _bi, _recurse);
                    if (_passThru)
                    {
                        base.WriteObject(secObj.GetPermissions("Title", "Id"), true);
                    }
                }
            }
        }

        #endregion

        #region METHODS


        #endregion
    }
}