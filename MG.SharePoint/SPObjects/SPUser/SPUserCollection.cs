using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace MG.SharePoint
{
    public class SPUserCollection : SPCollection<SPUser>
    {
        private SPUserCollection(int capacity)
            : base(capacity) { }

        private SPUserCollection(UserCollection userCol)
            : base(userCol.Count)
        {
            foreach (User u in userCol)
            {
                var spu = (SPUser)u;
                _list.Add(spu);
            }
        }

        #region METHODS
        public void Add(SPUser user) => _list.Add(user);

        public SPUser GetByEmail(string email) => _list.Find(x => x.Email.Equals(email, StringComparison.CurrentCultureIgnoreCase));

        #endregion

        #region STATIC METHODS/OPERATORS

        public static explicit operator SPUserCollection(UserCollection userCol)
        {
            if (!userCol.AreItemsAvailable)
                CTX.Lae(userCol, true);

            return new SPUserCollection(userCol);
        }

        public SPUserCollection FindByWildcard(string wildcardEmail)
        {
            var wcp = new WildcardPattern(wildcardEmail);
            var newCol = new SPUserCollection(_list.Count);
            for (int i = 0; i < _list.Count; i++)
            {
                SPUser u = _list[i];
                if (wcp.IsMatch(u.Email))
                    newCol.Add(u);
            }
            return newCol;
        }

        #endregion
    }
}