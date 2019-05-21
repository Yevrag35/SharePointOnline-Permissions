using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MG.SharePoint
{
    public class SPUserCollection : SPCollection<SPUser>
    {
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
        public SPUser GetByEmail(string email) => _list.Single(x => x.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase));

        #endregion

        #region STATIC METHODS/OPERATORS
        public static explicit operator SPUserCollection(UserCollection userCol) =>
            new SPUserCollection(userCol);

        #endregion
    }
}