using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MG.SharePoint.PowerShell
{
    public class Identity
    {
        private readonly string _strVal;
        private Guid _guidVal;

        private const string CAST = "Cast";
        private const BindingFlags FLAGS = BindingFlags.NonPublic | BindingFlags.Instance;
        private static readonly MethodInfo castMethod = typeof(Identity).GetMethod(CAST, FLAGS);
        private static readonly Type STR_TYPE = typeof(string);
        private static readonly Type GUID_TYPE = typeof(Guid);

        public bool IsGuid { get; }
        public Type ImpliedType { get; }
        public object Value => !this.IsGuid
            ? _strVal
            : (object)_guidVal;

        private Identity(object incoming)
        {
            if (Guid.TryParse(Convert.ToString(incoming), out Guid guid))
            {
                _guidVal = guid;
                this.ImpliedType = GUID_TYPE;
                this.IsGuid = true;
            }
            else if (incoming is string str)
            {
                _strVal = str;
                this.ImpliedType = STR_TYPE;
                this.IsGuid = false;
            }
            else if (incoming is ValueType vt)
            {
                _strVal = Convert.ToString(vt);
                this.ImpliedType = vt.GetType();
                this.IsGuid = false;
            }
            else
                throw new ArgumentException("'incoming' is not of type \"System.String\" or \"System.Guid\" and cannot be converted to as such.");
        }

        public T GetValue<T>()
        {
            Type tt = typeof(T);
            if (tt.Equals(STR_TYPE) || (tt.IsValueType && !tt.Equals(GUID_TYPE)))
            {
                MethodInfo genMeth = castMethod.MakeGenericMethod(tt);
                try
                {
                    return (T)genMeth.Invoke(this, new object[1] { this.Value });
                }
                catch (TargetInvocationException tie)
                {
                    throw new InvalidCastException(tie.InnerException.Message);
                }
            }
            else if (tt.Equals(GUID_TYPE) && this.ImpliedType.Equals(GUID_TYPE))
            {
                object retVal = _guidVal;
                return (T)retVal;
            }
            else
                throw new InvalidCastException("Identity can only be converted to a 'System.String' or 'System.ValueType' value.");
        }

        private T Cast<T>(dynamic o) => (T)o;
        
        public static implicit operator Identity(Guid guid) => new Identity(guid);
        public static implicit operator Identity(string str) => new Identity(str);
        public static implicit operator Identity(ValueType vt) => new Identity(vt);

        public static explicit operator Guid(Identity identity)
        {
            if (identity.ImpliedType.Equals(typeof(Guid)))
                return identity._guidVal;

            else
                throw new InvalidCastException("Identity is not of type 'System.Guid'.");
        }
        public static explicit operator string(Identity identity)
        {
            if (!identity.IsGuid)
                return identity._strVal;

            else
                throw new InvalidCastException("Identity is not of type 'System.String'.");
        }
    }
}
