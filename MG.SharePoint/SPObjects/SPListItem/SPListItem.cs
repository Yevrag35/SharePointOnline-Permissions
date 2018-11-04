using Microsoft.SharePoint.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MG.SharePoint
{
    public partial class SPListItem : SPObject, ISPPermissions
    {
        private protected ListItem _li;

        private protected bool? _hup;

        public override string Name => _li.DisplayName;
        public override object Id => _li.Id;
        public bool? HasUniquePermissions => _hup;
        public SPPermissionCollection Permissions { get; internal set; }

        #region Constructors
        internal SPListItem(ListItem listItem)
        {
            if (!listItem.IsPropertyReady(li => li.DisplayName))
            {
                CTX.Lae(listItem, true, li => li.DisplayName, li => li.Id,
                    li => li.HasUniqueRoleAssignments);
            }
            _hup = listItem.IsPropertyAvailable("HasUniqueRoleAssignments") ?
                (bool?)listItem.HasUniqueRoleAssignments : null;
            _li = listItem;
        }

        #endregion

        public override object ShowOriginal() => _li;

        public static implicit operator SPListItem(ListItem li) =>
            new SPListItem(li);
    }
}
