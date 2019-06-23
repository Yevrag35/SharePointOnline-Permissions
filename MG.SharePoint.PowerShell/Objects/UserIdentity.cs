using System;
using System.Collections.Generic;
using System.Linq;

namespace MG.SharePoint.PowerShell
{
    public class UserIdentity
    {
        #region FIELDS/CONSTANTS
        private string _login;
        private int _numId;
        private string _email;

        #endregion

        #region PROPERTIES
        //public bool IsGuidId { get; }
        public bool IsLoginName { get; }
        public bool IsNumericId { get; }
        public bool IsEmail { get; }

        #endregion

        #region CONSTRUCTORS
        private UserIdentity(string id)
        {
            if (int.TryParse(id, out int realId))
            {
                _numId = realId;
                this.IsNumericId = true;
            }
            else if (id.Contains("@"))
            {
                _email = id;
                this.IsEmail = true;
            }
            else
            {
                _login = id;
                this.IsLoginName = true;
            }
        }
        //private UserIdentity(Guid id)
        //{
        //    _id = id;
        //    this.IsGuidId = true;
        //}
        private UserIdentity(int numId)
        {
            _numId = numId;
            this.IsNumericId = true;
        }

        #endregion

        #region STATIC OPERATORS/CASTS
        public string AsEmail() => _email;
        public string AsLogin() => _login;

        public static implicit operator UserIdentity(string str) => new UserIdentity(str);
        //public static implicit operator UserIdentity(Guid id) => new UserIdentity(id);
        public static implicit operator UserIdentity(int numId) => new UserIdentity(numId);

        //public static explicit operator Guid(UserIdentity userId) => userId._id;
        public static explicit operator int(UserIdentity userId) => userId._numId;

        #endregion
    }
}