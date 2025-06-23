using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace MyChurch.Application.Member.Queries.GetMemberConfiguration
{
    public class GetMemberConfigurationQuery : JwtMemberDto, IRequest<MemberConfigurationDto>
    {
    }

    public class GetMemberConfigurationQueryHandler : IRequestHandler<GetMemberConfigurationQuery, MemberConfigurationDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetMemberConfigurationQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MemberConfigurationDto> Handle(GetMemberConfigurationQuery request, CancellationToken cancellationToken)
        {
            // Verifica se o usuário existe
            var member = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (member == null)
                throw new UnauthorizedAccessException("Usuário não encontrado.");

            // Busca as configurações do membro
            var config = await _unitOfWork.MemberConfigurations.Query()
                .FirstOrDefaultAsync(c => c.MemberId == request.UserId, cancellationToken);

            // Se não existir configuração, cria uma nova com valores padrão
            if (config == null)
            {
                config = new Domain.Entities.MemberConfiguration
                {
                    MemberId = request.UserId,
                    ThemePreference = "Light",
                    FontSize = "Medium",
                    EnableNotifications = true,
                    LastUpdated = DateTime.UtcNow
                };

                _unitOfWork.MemberConfigurations.Create(config);
                await _unitOfWork.CommitAsync();
            }

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