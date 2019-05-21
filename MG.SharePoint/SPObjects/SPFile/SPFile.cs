using Microsoft.SharePoint.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MG.SharePoint
{
    public partial class SPFile : SPSecurable
    {
        #region Private Properties/Fields
        private File _file;

        #endregion

        #region Constructors
        public SPFile(string serverRelativeUrl, bool isDifferentSite = false)
            : this(CTX.SP1.Web.GetFileByServerRelativeUrl(GetFormattedPath(serverRelativeUrl, isDifferentSite)))
        {
        }
        public SPFile(Guid fileId)
            : this(CTX.SP1.Web.GetFileById(fileId))
        {
        }
        internal SPFile(File file)
            : base(file.ListItemAllFields)
        {
            base.FormatObject(file, null);
            _file = file;
        }

        #endregion

        #region Methods
        public override ClientObject ShowOriginal() => _file;

        public override void Update() => _file.Update();

        private static string GetFormattedPath(string serverRelativeUrl, bool isDifferentSite)
        {
            if (!serverRelativeUrl.StartsWith(CTX.DestinationSite) && !isDifferentSite &&
                !serverRelativeUrl.StartsWith(CTX.SP1.Url))
            {
                serverRelativeUrl = CTX.DestinationSite + "/" + serverRelativeUrl;
            }
            else if (serverRelativeUrl.StartsWith(CTX.SP1.Url))
            {
                serverRelativeUrl = serverRelativeUrl.Replace(
                    CTX.SP1.Url.Replace(CTX.DestinationSite, string.Empty),
                    string.Empty
                );
            }
            return serverRelativeUrl;
        }

        #endregion

        #region Operators/Casts
        public static explicit operator SPFile(string relativeUrl)
        {
            if (relativeUrl.StartsWith(CTX.DestinationSite))
                relativeUrl = relativeUrl.Replace(CTX.DestinationSite + "/", string.Empty);
            return new SPFile(relativeUrl);
        }

        public static explicit operator SPFile(File file) =>
            new SPFile(file);

        #endregion
    }
}
