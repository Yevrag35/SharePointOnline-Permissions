using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.SharePoint.Client;
using System;
using System.Linq;

namespace MG.SharePoint
{
    public partial class SPWeb : SPSecurable
    {
        private protected Web _web;
        //private protected bool? _hup;

        public override string Name => _web.Title;
        public override object Id => _web.Id;
        public SPListCollection Lists { get; internal set; }
        public string RelativeUrl => _web.ServerRelativeUrl;
        public DateTime Created => _web.Created;
        //public bool? HasUniquePermissions => _hup;

        public SPWeb() : this(CTX.DestinationSite)
        {
        }

        internal SPWeb(Web w)
            : base(w)
        {
            CTX.Lae(w, true, web => web.Id, 
                web => w.Title, web => web.Created, 
                web => web.ServerRelativeUrl);
            _web = w;
        }

        public SPWeb(string relativeUrl)
            : this(GetWebByUrl(relativeUrl))
        {
        }

        public override object ShowOriginal() => _web;

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
            var tempWeb = CTX.SP1.Web;
            return tempWeb;
        }
    }
}
