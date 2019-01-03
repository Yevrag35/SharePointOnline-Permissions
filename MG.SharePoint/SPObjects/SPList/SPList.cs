using Microsoft.SharePoint.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MG.SharePoint
{
    public partial class SPList : SPSecurable
    {
        private protected List _list;
        //private protected bool? _hup;

        public override string Name => _list.Title;
        public override object Id => _list.Id;
        public SPListItemCollection Items { get; internal set; }
        public int? ItemCount { get; internal set; }
        public DateTime Created => _list.Created;
        //public bool? HasUniquePermissions => _hup;

        public SPList(string listName)
            : this(FindListByName(listName))
        {
        }
        internal SPList(List list)
            : base(list)
        {
            CTX.Lae(list, true, 
                l => l.Title, 
                l => l.Id, 
                l => l.Created,
                l => l.ItemCount);
            _list = list;
        }

        public override object ShowOriginal() => _list;

        public override void Update() => _list.Update();

        public static explicit operator SPList(List realList) =>
            new SPList(realList);

        public static explicit operator SPList(string listName) =>
            new SPList(listName);

        private static List FindListByName(string listName)
        {
            if (listName.Contains("/") && !listName.StartsWith("/"))
            {
                listName = "/" + listName;
            }

            var allLists = CTX.SP1.Web.Lists;
            CTX.Lae(allLists, true, ls => ls.Include(
                    l => l.Title, l => l.RootFolder.ServerRelativeUrl
                )
            );
            return allLists.Single(
                l => l.Title.Equals(listName, StringComparison.InvariantCultureIgnoreCase) ||
                l.RootFolder.ServerRelativeUrl.Equals(listName, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
