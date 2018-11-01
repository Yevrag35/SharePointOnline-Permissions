using Microsoft.SharePoint.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MG.SharePoint
{
    public partial class SPWeb : ISPObject, ISPHasPermissions
    {
        public SPPermissionCollection Permissions { get; internal set; }

        public SPPermissionCollection GetPermissions()
        {
            Permissions = _web.RoleAssignments;
            return Permissions;
        }
    }
}
