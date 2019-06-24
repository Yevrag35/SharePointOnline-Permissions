using System;
using System.Collections.Generic;
using System.Linq;

namespace MG.SharePoint.PowerShell
{
    public class Identity
    {
        #region FIELDS/CONSTANTS
        private Guid _guid;
        private int _int;
        private string _str;
        private Uri _uri;

        #endregion

        #region PROPERTIES
        public bool IsGuid { get; }
        public bool IsNumeric { get; }
        public bool IsString { get; }
        public bool IsUrl { get; }
        public UriKind UriKind { get; }

        #endregion

        #region CONSTRUCTORS
        private Identity(string str)
        {
            if (int.TryParse(str, out int outInt))
            {
                this.IsNumeric = true;
                _int = outInt;
            }
            else if (Guid.TryParse(str, out Guid outGuid))
            {
                this.IsGuid = true;
                _guid = outGuid;
            }
            else if (str.StartsWith("/"))
            {
                if (Uri.TryCreate(str, UriKind.Absolute, out Uri outUri))
                {
                    _uri = outUri;
                    this.IsUrl = true;
                    this.UriKind = UriKind.Absolute;
                }
                else if (Uri.TryCreate(str, UriKind.Relative, out Uri relUri))
                {
                    _uri = relUri;
                    this.IsUrl = true;
                    this.UriKind = UriKind.Relative;
                }
                else
                {
                    _str = str;
                    this.IsString = true;
                }
            }
            else
            {
                _str = str;
                this.IsString = true;
            }
        }
        private Identity(Guid id)
        {
            _guid = id;
            this.IsGuid = true;
        }
        private Identity(int id)
        {
            _int = id;
            this.IsNumeric = true;
        }
        private Identity(Uri url)
        {
            _uri = url;
            this.IsUrl = true;
            this.UriKind = url.IsAbsoluteUri
                ? UriKind.Absolute
                : UriKind.Relative;
        }

        #endregion

        #region STATIC CASTS/OPERATORS
        public static implicit operator Identity(string str) => new Identity(str);
        public static implicit operator Identity(int num) => new Identity(num);
        public static implicit operator Identity(Guid guid) => new Identity(guid);
        public static implicit operator Identity(Uri url) => new Identity(url);

        public static explicit operator string(Identity id) => id._str;
        public static explicit operator int(Identity id) => id._int;
        public static explicit operator Guid(Identity id) => id._guid;
        public static explicit operator Uri(Identity id) => id._uri;

        #endregion
    }
}