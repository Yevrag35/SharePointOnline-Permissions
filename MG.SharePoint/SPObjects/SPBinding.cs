using Microsoft.SharePoint.Client;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MG.SharePoint
{
    public class SPBinding : ICloneable, IComparable<SPBinding>, IEquatable<SPBinding>, ISPObject
    {
        public object Id => Principal.Id;
        public string Name { get; }
        public Principal Principal { get; }
        public RoleDefinition Definition { get; }

        public SPBinding(Principal prin, RoleDefinition def)
        {
            Principal res = !prin.IsPropertyReady(p => p.Id) || 
                !prin.IsPropertyReady(p => p.LoginName) ? 
                    LoadObject(prin) : prin;

            Principal = res;
            Definition = def;
            Name = res is User ? 
                ((User)res).Email : res.Title;
        }

        private Principal LoadObject(Principal obj)
        {
            if (obj is User)
                CTX.Lae((User)obj, true,
                    u => u.Email, u => u.LoginName, 
                    u => u.Title, u => u.Id);
            else
                CTX.Lae(obj, true,
                    p => p.LoginName, p => p.Title,
                    p => p.Id);

            return obj;
        }

        public int CompareTo(SPBinding other) =>
            ((int)Id).CompareTo(other.Id);

        public bool Equals(SPBinding other)
        {
            var speq = new BindingEquality();
            return speq.Equals(this, other);
        }

        public object Clone() =>
            this.MemberwiseClone();

        public object ShowOriginal() => 
            new KeyValuePair<Principal, RoleDefinition>(Principal, Definition);
    }
}
