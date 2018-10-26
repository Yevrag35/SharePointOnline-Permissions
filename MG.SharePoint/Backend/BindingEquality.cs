using System;
using System.Collections;
using System.Collections.Generic;

namespace MG.SharePoint
{
    public class BindingEquality : EqualityComparer<SPBinding>
    {
        public override bool Equals(SPBinding x, SPBinding y) =>
            x.Id.Equals(y.Id);

        public override int GetHashCode(SPBinding obj) =>
            throw new NotImplementedException();
    }
}
