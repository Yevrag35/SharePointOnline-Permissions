using System;
using System.Collections;
using System.Collections.Generic;

namespace MG.SharePoint
{
    public interface ISPPermissionResolver
    {
        IEnumerable<SPBinding> ResolvePermissions(IDictionary permissionTable);
    }
}
