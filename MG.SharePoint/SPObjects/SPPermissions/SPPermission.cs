using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Utilities;
using System;
using System.Collections.Generic;

namespace MG.SharePoint
{
    public class SPPermission : ICloneable, ISPObject
    {
        private readonly string _memTit;
        private readonly int _memLog;
        private readonly string[] _perms;
        private readonly RoleAssignment _roleAss;
        private readonly PrincipalType _prinType;

        public object Id => _memLog;
        public string Name => _memTit;
        public object ObjectId { get; }
        public string ObjectName { get; }
        public string Permissions => string.Join(", ", _perms);
        public int PermissionCount => _perms.Length;
        public PrincipalType Type => _prinType;
        internal string LoginName { get; }

        #region Constructors
        internal SPPermission(SPSecurable securable, RoleAssignment ass)
        {
            if (!ass.IsPropertyReady(a => a.Member.Title))
            {
                CTX.Lae(ass, true, a => a.Member.Title, a => a.Member.PrincipalType, 
                    a => a.Member.LoginName, a => a.Member.Id, a => a.RoleDefinitionBindings);
            }
            _memTit = ass.Member.Title;
            _memLog = ass.Member.Id;
            _perms = ParseBindings(ass.RoleDefinitionBindings);
            _prinType = ass.Member.PrincipalType;
            this.LoginName = ass.Member.LoginName;
            this.ObjectId = securable.Id;
            this.ObjectName = securable.Name;
            _roleAss = ass;
        }

        #endregion
        public static SPPermission ResolvePermission(RoleAssignment ass, SPSecurable securable) =>
            new SPPermission(securable, ass);

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
