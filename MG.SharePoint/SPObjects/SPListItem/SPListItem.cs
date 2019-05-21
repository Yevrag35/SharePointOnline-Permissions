using Microsoft.SharePoint.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MG.SharePoint
{
    public partial class SPListItem : SPSecurable
    {
        private protected ListItem _li;

        #region CONSTRUCTORS
        internal SPListItem(ListItem listItem)
            : base(listItem)
        {
            base.FormatObject(listItem, null);
            this.Name = listItem.DisplayName;

            _li = listItem;
        }

        #endregion

        #region METHODS

        public override ClientObject ShowOriginal() => _li;

        public override void Update() => _li.Update();

        #endregion

        #region OPERATORS

        public static explicit operator SPListItem(ListItem li) => new SPListItem(li);

        #endregion
    }
}
