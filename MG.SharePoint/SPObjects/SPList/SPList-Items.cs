using Microsoft.SharePoint.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MG.SharePoint
{
    public partial class SPList : SPSecurable
    {
        public void GetItems(params string[] listItemProperties)
        {
            GetItems(100, listItemProperties);
        }

        public void GetItems(int rowLimit, params string[] listItemProperties)
        {
            var query = new CamlQuery
            {
                ViewXml = string.Format("<View><RowLimit>{0}</RowLimit></View>", Convert.ToString(rowLimit))
            };
            GetItems(query, listItemProperties);
        }

        public void GetItems(CamlQuery query, params string[] listItemProperties)
        {
            var expressions = GetPropertyExpressions<ListItem>(listItemProperties);
            ListItemCollection col = _list.GetItems(query);
            CTX.Lae(col, true, c => c.Include(expressions));
            CTX.Lae(_list, true, l => l.ItemCount);
            Items = (SPListItemCollection)col;
            ItemCount = _list.ItemCount;
        }
    }
}
