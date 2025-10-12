using MyChurch.Domain.Enum;

namespace MyChurch.Domain.Services
{
    /// <summary>
    /// Serviço para verificação e gestão de permissões
    /// </summary>
    public interface IPermissionService
    {
        /// <summary>
        /// Verifica se um membro possui uma permissão específica
        /// </summary>
        Task<bool> HasPermissionAsync(int memberId, Permission permission, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Verifica se um membro possui qualquer uma das permissões especificadas
        /// </summary>
        Task<bool> HasAnyPermissionAsync(int memberId, Permission[] permissions, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Verifica se um membro possui todas as permissões especificadas
        /// </summary>
        Task<bool> HasAllPermissionsAsync(int memberId, Permission[] permissions, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Obtém todas as permissões de um membro (role + customizadas)
        /// </summary>
        Task<List<Permission>> GetMemberPermissionsAsync(int memberId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Concede uma permissão customizada a um membro
        /// </summary>
        Task GrantPermissionAsync(int memberId, Permission permission, int grantedByMemberId, string? reason = null, DateTime? expiresAt = null, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Revoga uma permissão customizada de um membro
        /// </summary>
        Task RevokePermissionAsync(int memberId, Permission permission, int revokedByMemberId, string? reason = null, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Obtém as permissões padrão de uma role
        /// </summary>
        Task<List<Permission>> GetRolePermissionsAsync(UserRole role, int? churchId = null, CancellationToken cancellationToken = default);
    }
}
