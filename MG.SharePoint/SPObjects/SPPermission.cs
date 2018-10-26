using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;

namespace MG.SharePoint
{
    public class SPPermission : ISPObject
    {
        private protected readonly string _memTit;
        private protected readonly int _memLog;
        private protected readonly string[] _perms;
        private protected readonly RoleAssignment _roleAss;

        public string Name => _memTit;
        public string Permissions => string.Join(", ", _perms);
        public object Id => _memLog;
        public int PermissionCount => _perms.Length;

        #region Constructors
        internal SPPermission(RoleAssignment ass)
        {
            _memTit = ass.Member.Title;
            _memLog = ass.Member.Id;
            _perms = ParseBindings(ass.RoleDefinitionBindings);
            _roleAss = ass;
        }

        #endregion

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

        public static implicit operator SPPermission(RoleAssignment ass) =>
            new SPPermission(ass);

        public object ShowOriginal() => _roleAss;
    }
}
