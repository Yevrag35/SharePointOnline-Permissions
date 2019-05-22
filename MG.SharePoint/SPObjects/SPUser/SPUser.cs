using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MG.SharePoint
{
    public class SPUser : SPObject
    {
        private User _user;
        private static readonly string[] IncludeThese = new string[3]
        {
            "AadObjectId", "Alerts", "UserId"
        };

        #region PROPERTIES
        public UserIdInfo AadObjectId { get; private set; }
        public AlertCollection Alerts { get; private set; }
        public string Email { get; private set; }
        public override object Id { get; internal set; }
        public bool IsEmailAuthenticationGuestUser { get; private set; }
        public bool IsHiddenInUI { get; private set; }
        public bool IsSharedByEmailGuestUser { get; private set; }
        public bool IsSiteAdmin { get; private set; }
        public string LoginName { get; private set; }
        public override string Name { get; internal set; }
        public PrincipalType PrincipalType { get; private set; }
        public UserIdInfo UserId { get; private set; }

        #endregion

        #region CONSTRUCTORS
        public SPUser(string userId)
            : this(CTX.SP1.Web.EnsureUser(userId)) { }

        internal SPUser(User user)
        {
            base.FormatObject(user, null, IncludeThese);
            this.Name = user.Title;

            _user = user;
        }

        #endregion

        #region METHODS
        public bool IsObjectPropertyInstantiated(string propertyName) => _user.IsObjectPropertyInstantiated(propertyName);
        public override void LoadProperty(params string[] propertyNames)
        {
            if (propertyNames.Length > 0)
            {
                base.Load(_user, propertyNames);
            }
        }
        public void RefreshLoad() => _user.RefreshLoad();
        public override ClientObject ShowOriginal() => _user;
        public void Update() => _user.Update();

        #endregion

        #region STATIC METHODS/OPERATORS
        public static explicit operator SPUser(User user) =>
            new SPUser(user);

        public static explicit operator User(SPUser spUser) =>
            spUser._user;

        public static SPUser GetUser(string userId) => new SPUser(userId);
        public static SPUser GetUserByEmail(string email)
        {
            SPUser retUser = null;
            User user = CTX.SP1.Web.SiteUsers.GetByEmail(email);
            if (user != null)
                retUser = new SPUser(user);
            
            return retUser;
        }
        public static SPUser GetUserById(int id)
        {
            SPUser retUser = null;
            User user = CTX.SP1.Web.SiteUsers.GetById(id);
            if (user != null)
                retUser = new SPUser(user);

            return retUser;
        }

        #endregion
    }
}
