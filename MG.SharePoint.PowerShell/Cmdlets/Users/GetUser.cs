using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;

namespace MG.SharePoint.PowerShell.Cmdlets.Users
{
    [Cmdlet(VerbsCommon.Get, "User", ConfirmImpact = ConfirmImpact.None)]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(User))]
    public class GetUser : BaseSPCmdlet
    {
        #region PRIVATE FIELDS/CONSTANTS
        private List<User> _users;

        #endregion

        #region PARAMETERS
        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public UserCollection UserCollection { get; set; }

        [Parameter(Mandatory = false, Position = 0)]
        public UserIdentity[] Identity { get; set; }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing() => base.BeginProcessing();

        protected override void ProcessRecord()
        {
            UserCollection uc = this.UserCollection;
            if (!this.MyInvocation.BoundParameters.ContainsKey("UserCollection"))
                uc = CTX.SP1.Web.SiteUsers;

            if (this.Identity != null && this.Identity.Length > 0)
            {
                for (int i = 0; i < this.Identity.Length; i++)
                {
                    var id = this.Identity[i];
                    User user = this.ResolveUser(id, uc);
                    if (user != null)
                    {
                        user.LoadUserProps();
                        base.WriteObject(user);
                    }
                }
            }
            else
            {
                uc.LoadUsers();
                base.WriteObject(uc, true);
            }
        }

        #endregion

        #region CMDLET METHODS
        private User ResolveUser(UserIdentity id, UserCollection uc)
        {
            return id.IsEmail
                ? uc.GetByEmail(id.AsEmail())
                : id.IsLoginName
                    ? uc.GetByLoginName(id.AsLogin())
                    : uc.GetById((int)id);
        }

        #endregion
    }
}