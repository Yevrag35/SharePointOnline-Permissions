using Microsoft.SharePoint.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MG.SharePoint
{
    public class SPListCollection : SPCollection<SPList>
    {
        #region CONSTRUCTORS
        public SPListCollection()
            : base() { }

        public SPListCollection(int capacity)
            : base(capacity) { }

        #endregion

        public void Add(SPList item) => _list.Add(item);
        public void AddRange(IEnumerable<SPList> items) => _list.AddRange(items);

        public void LoadProperty(params string[] propertyNames)
        {
            if (propertyNames.Length > 0)
            {
                for (int i = 0; i < _list.Count; i++)
                {
                    _list[i].LoadProperty(propertyNames);
                }
            }
        }

        #region OPERATORS

        public static explicit operator SPListCollection(ListCollection listCol)
        {
            if (!listCol.AreItemsAvailable)
                CTX.Lae(listCol);

            var spListCol = new SPListCollection(listCol.Count);
            for (int i = 0; i < listCol.Count; i++)
            {
                var list = listCol[i];
                spListCol._list.Add((SPList)list);
            }
            return spListCol;
        }

        #endregion
    }
}
