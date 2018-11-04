using Microsoft.SharePoint.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MG.SharePoint
{
    public partial class SPList : SPObject, ISPPermissions
    {
        private protected List _list;
        private protected bool? _hup;

        public override string Name => _list.Title;
        public override object Id => _list.Id;
        public SPListItemCollection Items { get; internal set; }
        public int? ItemCount { get; internal set; }
        public DateTime Created => _list.Created;
        public bool? HasUniquePermissions => _hup;

        public SPList(string listName)
        {
            var allLists = CTX.SP1.Web.Lists;
            CTX.Lae(allLists, true, ls => ls.Include(
                l => l.Title, l => l.Id, l => l.HasUniqueRoleAssignments, l => l.Created));
            _list = allLists.Single(l => string.Equals(
                l.Title, listName, StringComparison.InvariantCultureIgnoreCase));
            _hup = _list.IsPropertyAvailable("HasUniqueRoleAssignments") ?
                (bool?)_list.HasUniqueRoleAssignments : null;
        }
        internal SPList(List list)
        {
            if (!list.IsPropertyReady(l => l.Title))
            {
                CTX.Lae(list, true, 
                    l => l.Title, 
                    l => l.Id, 
                    l => l.HasUniqueRoleAssignments, 
                    l => l.Created,
                    l => l.ItemCount);
            }
            _list = list;
            _hup = _list.IsPropertyAvailable("HasUniqueRoleAssignments") ?
                (bool?)_list.HasUniqueRoleAssignments : null;
        }

        public override object ShowOriginal() => _list;

        public static explicit operator SPList(List realList) =>
            new SPList(realList);
    }
}
