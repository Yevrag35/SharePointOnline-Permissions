using System;

namespace MG.SharePoint
{
    public interface ISPHasPermissions
    {
        SPPermissionCollection Permissions { get; }

        SPPermissionCollection GetPermissions();
    }
}
