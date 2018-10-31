using System;
using System.Collections;
using System.Collections.Generic;

namespace MG.SharePoint
{
    public interface IPermissionResolver
    {
        IEnumerable<SPBinding> ResolvePermissions(IDictionary permissionTable);
    }
}
