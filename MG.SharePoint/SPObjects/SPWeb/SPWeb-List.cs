using Microsoft.SharePoint.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MG.SharePoint
{
    public partial class SPWeb : SPObject, ISPPermissions
    {
        public SPListCollection GetLists(params string[] loadProperties)
        {
            if (loadProperties == null)
            {
                Lists = (SPListCollection)_web.Lists;
            }
            else
            {
                var allLists = CTX.SP1.Web.Lists;
                var expressions = GetPropertyExpressions<List>(loadProperties).ToArray();
                CTX.Lae(allLists, true, al => al.Include(expressions));
                Lists = new SPListCollection();
                for (int i = 0; i < allLists.Count; i++)
                {
                    var splist = (SPList)allLists[i];
                    Lists.Add(splist);
                }
                Lists.IsReadOnly = true;
            }
            return Lists;
        }
    }
}
