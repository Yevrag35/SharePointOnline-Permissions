using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MG.SharePoint
{
    public class SPGroupCollection : SPCollection<SPGroup>
    {
        private SPGroupCollection(GroupCollection groupCol)
            : base(groupCol.Count)
        {
            foreach (Group g in groupCol)
            {
                var grp = (SPGroup)g;
                _list.Add(grp);
            }
        }

        public bool ContainsGroupByLoginName(string loginName)
        {
            bool result = false;
            for (int i = 0; i < _list.Count; i++)
            {
                if (_list[i].LoginName.Equals(loginName))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }
        public bool ContainsGroupByName(string name)
        {
            bool result = false;
            for (int i = 0; i < _list.Count; i++)
            {
                if (_list[i].Name.Equals(name))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        public static explicit operator SPGroupCollection(GroupCollection groupCol)
        {
            if (!groupCol.AreItemsAvailable)
                CTX.Lae(groupCol, true);

            return new SPGroupCollection(groupCol);
        }
    }
}