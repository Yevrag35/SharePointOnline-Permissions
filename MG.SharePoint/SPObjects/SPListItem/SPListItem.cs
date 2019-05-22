using Microsoft.SharePoint.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MG.SharePoint
{
    public partial class SPListItem : SPSecurable
    {
        private protected ListItem _li;
        private static readonly string[] SkipThese = new string[1]
        {
            "IconOverlay"
        };
        private static readonly string[] IncludeThese = new string[3]
        {
            "ComplianceInfo", "ContentType", "Versions"
        };

        protected override string NameProperty => "DisplayName";
        protected override string IdProperty => "Id";

        #region CONSTRUCTORS
        internal SPListItem(ListItem listItem)
            : base(listItem)
        {
            base.FormatObject(listItem, SkipThese, IncludeThese);
            this.Name = listItem.DisplayName;

            _li = listItem;
        }

        #endregion

        #region METHODS

        public override ClientObject ShowOriginal() => _li;

        public override void Update() => _li.Update();

        #endregion

        #region OPERATORS

        //public static IEnumerable<SPListItem> GetItems(List list)
        //{
        //    var items = new List<SPListItem>();
        //    var query = new CamlQuery
        //    {
        //        ViewXml = string.Format("<View><RowLimit>{0}</RowLimit></View>", Convert.ToString(100))
        //    };
        //    var listItemCol = list.GetItems(query);
        //    CTX.SP1.Load(listItemCol);
        //    CTX.SP1.ExecuteQuery();

        //    CTX.SP1.Load(listItemCol, col => col.Include(
        //        x => x.Id
        //    ));
        //    CTX.SP1.ExecuteQuery();

        //    foreach (ListItem li in listItemCol)
        //    {
        //        items.Add((SPListItem)li);
        //    }
        //    return items;
        //}

        public static explicit operator SPListItem(ListItem li) => new SPListItem(li);

        #endregion
    }
}
