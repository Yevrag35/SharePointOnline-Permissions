using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MG.SharePoint
{
    public class SPGroup : SPObject
    {
        private Group _group;

        #region PROPERTIES
        public bool AllowMembersEditMembership { get; private set; }
        public bool AllowRequestToJoinLeave { get; private set; }
        public bool AutoAcceptRequestToJoinLeave { get; private set; }
        public bool CanCurrentUserEditMembership { get; private set; }
        public bool CanCurrentUserManageGroup { get; private set; }
        public bool CanCurrentUserViewMembership { get; private set; }
        public string Description { get; private set; }
        public override object Id { get; internal set; }
        public bool IsHiddenInUI { get; private set; }
        public string LoginName { get; private set; }
        public override string Name { get; internal set; }
        public bool OnlyAllowMembersViewMembership { get; private set; }
        public Principal Owner { get; private set; }
        public string OwnerTitle { get; private set; }
        public PrincipalType PrincipalType { get; private set; }
        public string RequestToJoinLeaveEmailSetting { get; private set; }
        public SPUserCollection Users { get; private set; }

        #endregion

        #region CONSTRUCTORS
        private SPGroup(Group group)
        {
            base.FormatObject(group, null);
            this.Name = group.Title;
            _group = group;
        }
        public SPGroup(string groupName)
            : this(CTX.SP1.Web.SiteGroups.GetByName(groupName)) { }

        #endregion

        #region METHODS
        public override void LoadProperty(params string[] propertyNames)
        {
            if (propertyNames.Length > 0)
            {
                base.Load(_group, propertyNames);
            }

        }
        public override ClientObject ShowOriginal() => _group;

        #endregion

        #region STATIC METHODS/OPERATORS
        public static explicit operator SPGroup(Group group) => new SPGroup(group);

        #endregion
    }
}
