using Microsoft.AspNetCore.Authorization;
using MyChurch.Domain.Enum;

namespace MyChurch.Api.Web.Authorization
{
    /// <summary>
    /// Atributo para aplicar requisitos de permissão em controllers e actions
    /// </summary>
    public class HasPermissionAttribute : AuthorizeAttribute
    {
        public HasPermissionAttribute(Permission permission) 
            : base(permission.ToString())
        {
        }

        public HasPermissionAttribute(params Permission[] permissions)
            : base(string.Join(",", permissions.Select(p => p.ToString())))
        {
        }
    }

    /// <summary>
    /// Atributo que requer TODAS as permissões especificadas
    /// </summary>
    public class HasAllPermissionsAttribute : AuthorizeAttribute
    {
        public HasAllPermissionsAttribute(params Permission[] permissions)
            : base($"AllOf:{string.Join(",", permissions.Select(p => p.ToString()))}")
        {
        }
    }
}
