using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.SharePoint.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MG.SharePoint
{
    public partial class SPWeb : SPObject, ISPPermissions
    {
        private protected Web _web;
        private protected bool? _hup;

        public override string Name => _web.Title;
        public override object Id => _web.Id;
        public SPListCollection Lists { get; internal set; }
        public string RelativeUrl => _web.ServerRelativeUrl;
        public DateTime Created => _web.Created;
        public bool? HasUniquePermissions => _hup;

        public SPWeb() : this("/")
        {
        }

        public SPWeb(string relativeUrl)
        {
            if (relativeUrl.StartsWith("/") && relativeUrl != "/")
                relativeUrl = string.Join("/", relativeUrl.Split(
                    new string[1] { "/" }, StringSplitOptions.RemoveEmptyEntries).ToArray());

            CTX.Login(CTX.SpecifiedTenantName, relativeUrl, PromptBehavior.Auto);

            var tempWeb = CTX.SP1.Web;
            CTX.Lae(tempWeb, true, w => w.Id, w => w.Title,
                w => w.HasUniqueRoleAssignments, w => w.Created,
                w => w.ServerRelativeUrl);
            _web = tempWeb;
            _hup = _web.IsPropertyAvailable("HasUniqueRoleAssignments") ?
                (bool?)_web.HasUniqueRoleAssignments : null;
        }

        public override object ShowOriginal() => _web;
    }
}
