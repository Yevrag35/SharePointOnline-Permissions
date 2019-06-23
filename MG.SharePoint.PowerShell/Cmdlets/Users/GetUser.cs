using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;

namespace MG.SharePoint.PowerShell.Cmdlets.Users
{
    [Cmdlet(VerbsCommon.Get, "User", ConfirmImpact = ConfirmImpact.None, DefaultParameterSetName = "ByUserCollectionAndIdentity")]
    [CmdletBinding(PositionalBinding = false)]
    [OutputType(typeof(User))]
    public class GetUser : BaseSPCmdlet
    {
        #region PRIVATE FIELDS/CONSTANTS

        #endregion

        #region PARAMETERS
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "ByUserObject")]
        public User InputObject{ get; set; }

        [Parameter(Mandatory = false, ParameterSetName = "ByUserCollectionAndIdentity")]
        public UserCollection UserCollection { get; set; }

        [Parameter(Mandatory = false, Position = 0, ParameterSetName = "ByUserCollectionAndIdentity")]
        public UserIdentity[] Identity { get; set; }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing() => base.BeginProcessing();

        protected override void ProcessRecord()
        {
            if (this.InputObject != null)
            {
                this.InputObject.LoadUserProps();
                base.WriteObject(this.InputObject);
            }
            else
            {
                UserCollection uc = this.UserCollection;
                if (!this.MyInvocation.BoundParameters.ContainsKey("UserCollection") &&
                    ParameterSetName == "ByUserCollectionAndIdentity")
                {
                    uc = CTX.SP1.Web.SiteUsers;
                }

                if (this.Identity != null && this.Identity.Length > 0)
                {
                    for (int i = 0; i < this.Identity.Length; i++)
                    {
                        UserIdentity id = this.Identity[i];
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
                    //uc.LoadProperty(c => c.Include(
                    //    x => x.AadObjectId, x => x.Alerts, x => x.Email, x => x.Groups.Include(
                    //        g => g.AllowMembersEditMembership, g => g.AllowRequestToJoinLeave, g => g.AutoAcceptRequestToJoinLeave,
                    //        g => g.CanCurrentUserEditMembership, g => g.CanCurrentUserManageGroup, g => g.CanCurrentUserViewMembership,
                    //        g => g.Description, g => g.Id, g => g.IsHiddenInUI, g => g.LoginName, g => g.OnlyAllowMembersViewMembership,
                    //        g => g.Owner, g => g.OwnerTitle, g => g.PrincipalType, g => g.RequestToJoinLeaveEmailSetting, g => g.Title,
                    //        g => g.Users.Include(
                    //            u => u.Title)
                    //        ),
                    //    x => x.IsEmailAuthenticationGuestUser, x => x.Id, x => x.IsHiddenInUI, x => x.IsShareByEmailGuestUser,
                    //    x => x.IsSiteAdmin, x => x.LoginName, x => x.PrincipalType, x => x.Title, x => x.UserId));
                    uc.LoadAllUsers();
                    base.WriteObject(uc, true);
                }
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