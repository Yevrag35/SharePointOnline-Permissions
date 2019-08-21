using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MG.SharePoint
{
    public static class RoleDefinitionExtensions
    {
        public static void LoadAllDefinitions(this RoleDefinitionCollection roleCol)
        {
            roleCol.Context.Load(roleCol, rc => rc.Include(
                r => r.Description, r => r.Hidden, r => r.Id, r => r.Name,
                r => r.Order, r => r.RoleTypeKind));
            roleCol.Context.ExecuteQuery();
        }

        public static void LoadDefinitionNames(this RoleDefinitionCollection roleCol)
        {
            roleCol.Context.Load(roleCol, rc => rc.Include(r => r.Name));
            roleCol.Context.ExecuteQuery();
        }

        public static void LoadDefinition(this RoleDefinition roleDef)
        {
            roleDef.Context.Load(roleDef, rd => rd.Description, rd => rd.Hidden,
                rd => rd.Id, rd => rd.Name, rd => rd.Order, rd => rd.RoleTypeKind);
            roleDef.Context.ExecuteQuery();
        }
    }
}
