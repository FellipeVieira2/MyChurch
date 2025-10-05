using MediatR;
using Microsoft.Extensions.Logging;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;
using MyChurch.Application.Dtos;
using System.Text.Json;

namespace MyChurch.Application.Church.Commands.UpdateChurchCharacteristics
{
    /// <summary>
    /// Comando para atualizar características avançadas da igreja (denominação, amenidades, idiomas)
    /// </summary>
    public class UpdateChurchCharacteristicsCommand : JwtMemberDto, IRequest<Unit>
    {
        /// <summary>Denominação da igreja (Ex: Batista, Assembleia de Deus, Adventista, Católica)</summary>
        public string? Denomination { get; set; }
        
        /// <summary>URL da foto de capa (base64 ou URL S3)</summary>
        public string? CoverPhoto { get; set; }
        
        /// <summary>Possui estacionamento</summary>
        public bool? HasParking { get; set; }
        
        /// <summary>Possui acessibilidade (rampa, elevador, etc)</summary>
        public bool? IsAccessible { get; set; }
        
        /// <summary>Possui transmissão ao vivo (YouTube, Facebook Live, etc)</summary>
        public bool? HasLiveStream { get; set; }
        
        /// <summary>Possui ministério infantil</summary>
        public bool? HasChildMinistry { get; set; }
        
        /// <summary>Idiomas falados nos cultos (Ex: ["Português", "Inglês", "Espanhol"])</summary>
        public List<string>? Languages { get; set; }
    }

    public class UpdateChurchCharacteristicsCommandHandler : IRequestHandler<UpdateChurchCharacteristicsCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateChurchCharacteristicsCommandHandler> _logger;

        public UpdateChurchCharacteristicsCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<UpdateChurchCharacteristicsCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdateChurchCharacteristicsCommand request, CancellationToken cancellationToken)
        {
            // ?? SEGURANÇA: Busca o membro logado
            var currentMember = _unitOfWork.Members.Query()
                .FirstOrDefault(m => m.Id == request.UserId);

            if (currentMember == null)
            {
                ValidationException.ThrowException("Member", "Usuário não encontrado");
            }

            // Apenas admin pode atualizar
            if (currentMember.Role != Domain.Enum.UserRole.Admin)
            {
                ValidationException.ThrowException("Permission", "Apenas administradores podem atualizar as características da igreja");
            }

            // Busca a igreja do membro
            var church = _unitOfWork.Churchs.Query()
                .FirstOrDefault(c => c.Id == currentMember.ChurchId);

            if (church == null)
            {
                ValidationException.ThrowException("Church", "Igreja não encontrada");
            }

            // Atualizar características (apenas se fornecidas)
            if (request.Denomination != null)
            {
                church.Denomination = request.Denomination;
            }

            if (request.CoverPhoto != null)
            {
                church.CoverPhoto = request.CoverPhoto;
            }

            if (request.HasParking.HasValue)
            {
                church.HasParking = request.HasParking.Value;
            }

            if (request.IsAccessible.HasValue)
            {
                church.IsAccessible = request.IsAccessible.Value;
            }

            if (request.HasLiveStream.HasValue)
            {
                church.HasLiveStream = request.HasLiveStream.Value;
            }

            if (request.HasChildMinistry.HasValue)
            {
                church.HasChildMinistry = request.HasChildMinistry.Value;
            }

            if (request.Languages != null)
            {
                // Serializar lista de idiomas em JSON
                church.Languages = JsonSerializer.Serialize(request.Languages);
            }

            church.Updated = DateTime.UtcNow;

            _unitOfWork.Churchs.Update(church);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Características atualizadas para igreja {ChurchId} por {MemberId}", 
                church.Id, currentMember.Id);

            return Unit.Value;
        }
    }
}
