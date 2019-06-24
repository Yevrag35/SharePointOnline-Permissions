using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MG.SharePoint
{
    public partial class SPWeb : SPSecurable
    {
        private Web _web;
        private static readonly string[] IncludeThese = new string[]
        {
            "Alerts", "AssociatedMemberGroup", "AssociatedOwnerGroup", "AssociatedVisitorGroup",
            "CurrentUser", "Navigation", "RegionalSettings", "ResourcePath", "TitleResource",
            "RoleDefinitions", "ServerRelativePath", "SupportedUILanguageIds", "ThemeInfo",
            "UserCustomActions"
        };

        protected override string NameProperty => "Title";
        protected override string IdProperty => "Id";

        public SPWeb() : this(CTX.DestinationSite)
        {
        }

        internal SPWeb(Web w)
            : base(w)
        {
            base.FormatObject(w, null, IncludeThese);
            this.Name = w.Title;
            _web = w;
        }

        public SPWeb(string relativeUrl)
            : this(GetWebByUrl(relativeUrl))
        {
        }

        public User EnsureUser(string userId) => _web.EnsureUser(userId);

        public override ClientObject ShowOriginal() => _web;

        public override void Update() => _web.Update();

        public static explicit operator SPWeb(Web w) =>
            new SPWeb(w);

        public static explicit operator SPWeb(string relativeUrl) =>
            new SPWeb(relativeUrl);

        private static Web GetWebByUrl(string relativeUrl)
        {
            if (relativeUrl.StartsWith("/") && relativeUrl != "/")
                relativeUrl = string.Join("/", relativeUrl.Split(
                    new string[1] { "/" }, StringSplitOptions.RemoveEmptyEntries).ToArray());

            CTX.Login(CTX.SpecifiedTenantName, relativeUrl, PromptBehavior.Auto);
            Web tempWeb = CTX.SP1.Web;
            return tempWeb;
        }
    }
}
