using Microsoft.SharePoint.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MG.SharePoint
{
    public partial class SPWeb : ISPPermissions
    {
        private protected Web _web;
        private protected bool? _hup;

        public string Name => _web.Title;
        public object Id => _web.Id;
        public string RelativeUrl => _web.ServerRelativeUrl;
        public DateTime Created => _web.Created;
        public bool? HasUniquePermissions => _hup;

        public SPWeb()
        {
            var tempWeb = CTX.SP1.Web;
            CTX.Lae(tempWeb, true, w => w.Id, w => w.Title,
                w => w.HasUniqueRoleAssignments, w => w.Created,
                w => w.ServerRelativeUrl);
            _web = tempWeb;
            _hup = _web.IsPropertyAvailable("HasUniqueRoleAssignments") ?
                (bool?)_web.HasUniqueRoleAssignments : null;
        }

        public object ShowOriginal() => _web;
    }
}
