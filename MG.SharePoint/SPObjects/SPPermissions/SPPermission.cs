using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;

namespace MG.SharePoint
{
    public class SPPermission : ICloneable, ISPObject
    {
        private protected readonly string _memTit;
        private protected readonly int _memLog;
        private protected readonly string[] _perms;
        private protected readonly RoleAssignment _roleAss;

        public object Id => _memLog;
        public string Name => _memTit;
        public string Permissions => string.Join(", ", _perms);
        public int PermissionCount => _perms.Length;

        #region Constructors
        internal SPPermission(RoleAssignment ass)
        {
            if (ass.IsPropertyReady(a => a.Member.Title))
            {
                CTX.Lae(ass, true, a => a.Member.Title,
                    a => a.Member.Id, a => a.RoleDefinitionBindings);
            }
            _memTit = ass.Member.Title;
            _memLog = ass.Member.Id;
            _perms = ParseBindings(ass.RoleDefinitionBindings);
            _roleAss = ass;
        }

        #endregion

        public static implicit operator SPPermission(RoleAssignment ass) =>
            new SPPermission(ass);

        public object Clone()
        {
            var perm = this.MemberwiseClone() as SPPermission;
            return perm;
        }
        public ClientContext GetContext() => (ClientContext)_roleAss.Context;
        bool ISPObject.IsObjectPropertyInstantiated(string propertyName) => _roleAss.IsObjectPropertyInstantiated(propertyName);
        private string[] ParseBindings(RoleDefinitionBindingCollection bindingCol)
        {
            var strPerms = new string[bindingCol.Count];
            for (int i = 0; i < bindingCol.Count; i++)
            {
                var bind = bindingCol[i];
                strPerms[i] = bind.Name;
            }
            return strPerms;
        }
        void ISPObject.RefreshLoad() => _roleAss.RefreshLoad();
        public ClientObject ShowOriginal() => _roleAss;
    }
}
