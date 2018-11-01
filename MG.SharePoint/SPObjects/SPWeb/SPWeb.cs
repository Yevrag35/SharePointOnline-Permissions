using Microsoft.SharePoint.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MG.SharePoint
{
    public partial class SPWeb : ISPObject
    {
        private protected Web _web;

        public string Name => _web.Title;
        public object Id => _web.Id;
        public string RelativeUrl => _web.ServerRelativeUrl;
        public DateTime Created => _web.Created;
        public bool HasUniquePermissions => _web.HasUniqueRoleAssignments;

        public SPWeb() : this("/")
        {
        }

        public SPWeb(string serverRelativeUrl)
        {
            if (string.IsNullOrEmpty(CTX.SP1.Url))
                throw new NotImplementedException();

            if (!MatchesContext(serverRelativeUrl))
                CTX.SP1 = NewContext(serverRelativeUrl);

            var tempWeb = CTX.SP1.Web;
            CTX.Lae(tempWeb, true, w => w.Id, w => w.Title,
                w => w.HasUniqueRoleAssignments, w => w.Created,
                w => w.ServerRelativeUrl);
            _web = tempWeb;
        }

        public object ShowOriginal() => _web;

        private protected bool MatchesContext(string incoming)
        {
            var currentUrl = new Uri(CTX.SP1.Url, UriKind.Absolute);
            var uri = currentUrl.PathAndQuery;
            if (string.IsNullOrEmpty(incoming))
                incoming = "/";
            else if (!incoming.StartsWith("/"))
                incoming = "/" + incoming;

            return string.Equals(incoming, uri, StringComparison.OrdinalIgnoreCase);
        }

        private protected CmdLetContext NewContext(string incoming)
        {
            if (incoming == "/")
                incoming = string.Empty;

            var wholeThing = new Uri(CTX.SP1.Url, UriKind.Absolute);
            var hostOnly = wholeThing.Scheme + "//" + wholeThing.Host;
            var newCtx = new CmdLetContext(hostOnly + incoming, (CmdLetContext)CTX.SP1);
            return newCtx;
        }
    }
}
