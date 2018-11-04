﻿using Microsoft.SharePoint.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MG.SharePoint
{
    public partial class SPFile : SPObject, ISPPermissions
    {
        #region Private Properties/Fields
        private protected File _file;
        private protected bool? _hup;
        private protected string _srUrl;

        #endregion

        #region Public Properties/Fields
        public override string Name => _file.Name;
        public override object Id => _file.UniqueId;
        public string ServerRelativeUrl => _srUrl;
        public bool? HasUniquePermissions => _hup;

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
        {
            CTX.Lae(file, true, f => f.Name,
                f => f.UniqueId,
                f => f.ListItemAllFields.HasUniqueRoleAssignments,
                f => f.ServerRelativeUrl
            );
            _srUrl = file.ServerRelativeUrl;
            _hup = file.ListItemAllFields.IsPropertyAvailable("HasUniqueRoleAssignments") ?
                (bool?)file.ListItemAllFields.HasUniqueRoleAssignments : null;
            _file = file;
        }

        #endregion

        #region Methods
        public override object ShowOriginal() => _file;

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