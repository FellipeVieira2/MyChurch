using MediatR;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Enum;

namespace MyChurch.Application.Permissions.Commands.GrantPermission
{
    public class GrantPermissionCommand : JwtMemberDto, IRequest
    {
        /// <summary>
        /// ID do membro que receberá a permissão
        /// </summary>
        public int TargetMemberId { get; set; }
        
        /// <summary>
        /// Permissão a ser concedida
        /// </summary>
        public Permission Permission { get; set; }
        
        /// <summary>
        /// Motivo da concessão (opcional)
        /// </summary>
        public string? Reason { get; set; }
        
        /// <summary>
        /// Data de expiração (null = permanente)
        /// </summary>
        public DateTime? ExpiresAt { get; set; }
    }
}
