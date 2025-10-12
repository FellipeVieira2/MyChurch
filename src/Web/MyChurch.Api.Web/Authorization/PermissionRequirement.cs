using Microsoft.AspNetCore.Authorization;
using MyChurch.Domain.Enum;

namespace MyChurch.Api.Web.Authorization
{
    /// <summary>
    /// Requirement para verificar se o usuário possui uma permissão específica
    /// </summary>
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public Permission[] Permissions { get; }
        public bool RequireAll { get; }

        /// <summary>
        /// Cria um requirement de permissão
        /// </summary>
        /// <param name="permissions">Permissões necessárias</param>
        /// <param name="requireAll">Se true, o usuário deve ter TODAS as permissões. Se false, basta ter UMA.</param>
        public PermissionRequirement(Permission[] permissions, bool requireAll = false)
        {
            Permissions = permissions;
            RequireAll = requireAll;
        }

        public PermissionRequirement(Permission permission)
        {
            Permissions = new[] { permission };
            RequireAll = false;
        }
    }
}
