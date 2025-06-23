using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace MyChurch.Application.Member.Commands.UpdateMemberConfiguration
{
    public class UpdateMemberConfigurationCommand : JwtMemberDto, IRequest<MemberConfigurationDto>
    {
        public int? PreferredBibleVersionId { get; set; }
        public string ThemePreference { get; set; }
        public string FontSize { get; set; }
        public bool? EnableNotifications { get; set; }
    }

    public class UpdateMemberConfigurationCommandHandler : IRequestHandler<UpdateMemberConfigurationCommand, MemberConfigurationDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateMemberConfigurationCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MemberConfigurationDto> Handle(UpdateMemberConfigurationCommand request, CancellationToken cancellationToken)
        {
            // Verifica se o usuário existe
            var member = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (member == null)
                throw new UnauthorizedAccessException("Usuário não encontrado.");

            // Busca as configurações do membro
            var config = await _unitOfWork.MemberConfigurations.Query()
                .FirstOrDefaultAsync(c => c.MemberId == request.UserId, cancellationToken);

            // Se não existir configuração, cria uma nova
            if (config == null)
            {
                config = new Domain.Entities.MemberConfiguration
                {
                    MemberId = request.UserId,
                    PreferredBibleVersionId = request.PreferredBibleVersionId,
                    ThemePreference = request.ThemePreference ?? "Light",
                    FontSize = request.FontSize ?? "Medium",
                    EnableNotifications = request.EnableNotifications ?? true,
                    LastUpdated = DateTime.UtcNow
                };

                _unitOfWork.MemberConfigurations.Create(config);
            }
            else
            {
                // Atualiza apenas os campos que foram enviados
                if (request.PreferredBibleVersionId != null)
                    config.PreferredBibleVersionId = request.PreferredBibleVersionId;
                
                if (!string.IsNullOrEmpty(request.ThemePreference))
                    config.ThemePreference = request.ThemePreference;
                
                if (!string.IsNullOrEmpty(request.FontSize))
                    config.FontSize = request.FontSize;
                
                if (request.EnableNotifications.HasValue)
                    config.EnableNotifications = request.EnableNotifications.Value;
                
                config.LastUpdated = DateTime.UtcNow;
            }

            await _unitOfWork.CommitAsync();

            // Busca o nome da versão da Bíblia se estiver definida
            string versionName = null;
            if (config.PreferredBibleVersionId.HasValue)
            {
                var version = await _unitOfWork.Versions.Query()
                    .FirstOrDefaultAsync(v => v.Id == config.PreferredBibleVersionId.Value, cancellationToken);
                versionName = version?.Name;
            }

            return MemberConfigurationDto.FromEntity(config, versionName);
        }
    }
}