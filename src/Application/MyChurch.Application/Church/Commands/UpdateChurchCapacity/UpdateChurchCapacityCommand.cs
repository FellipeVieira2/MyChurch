using MediatR;
using Microsoft.Extensions.Logging;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;
using MyChurch.Application.Dtos;
using System.Text.Json;

namespace MyChurch.Application.Church.Commands.UpdateChurchCapacity
{
    /// <summary>
    /// Comando para atualizar capacidade e infraestrutura da igreja
    /// </summary>
    public class UpdateChurchCapacityCommand : JwtMemberDto, IRequest<Unit>
    {
        /// <summary>Capacidade de assentos</summary>
        public int? SeatingCapacity { get; set; }
        
        /// <summary>Capacidade em pé (eventos especiais)</summary>
        public int? StandingCapacity { get; set; }
        
        /// <summary>Número de vagas de estacionamento</summary>
        public int? ParkingSpaces { get; set; }
        
        /// <summary>Possui WiFi</summary>
        public bool? HasWifi { get; set; }
        
        /// <summary>Senha do WiFi (opcional, para membros)</summary>
        public string? WifiPassword { get; set; }
        
        /// <summary>Possui cafeteria/lanchonete</summary>
        public bool? HasCafeteria { get; set; }
        
        /// <summary>Possui livraria</summary>
        public bool? HasBookstore { get; set; }
        
        /// <summary>Possui berçário</summary>
        public bool? HasNursery { get; set; }
        
        /// <summary>Sistema de som profissional</summary>
        public bool? HasSoundSystem { get; set; }
        
        /// <summary>Projetor/Telão</summary>
        public bool? HasProjector { get; set; }
        
        /// <summary>Ar condicionado</summary>
        public bool? HasAirConditioning { get; set; }
        
        /// <summary>Batistério</summary>
        public bool? HasBaptistery { get; set; }
        
        /// <summary>Instalações adicionais (Ex: ["Academia", "Quadra esportiva", "Auditório"])</summary>
        public List<string>? AdditionalFacilities { get; set; }
        
        /// <summary>Notas sobre equipamentos disponíveis</summary>
        public string? EquipmentNotes { get; set; }
    }

    public class UpdateChurchCapacityCommandHandler : IRequestHandler<UpdateChurchCapacityCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateChurchCapacityCommandHandler> _logger;

        public UpdateChurchCapacityCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<UpdateChurchCapacityCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdateChurchCapacityCommand request, CancellationToken cancellationToken)
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
                ValidationException.ThrowException("Permission", "Apenas administradores podem atualizar capacidade e infraestrutura");
            }

            // Busca a igreja do membro
            var church = _unitOfWork.Churchs.Query()
                .FirstOrDefault(c => c.Id == currentMember.ChurchId);

            if (church == null)
            {
                ValidationException.ThrowException("Church", "Igreja não encontrada");
            }

            // Validações de capacidade
            if (request.SeatingCapacity.HasValue && request.SeatingCapacity.Value < 0)
            {
                ValidationException.ThrowException("SeatingCapacity", "Capacidade de assentos deve ser positiva");
            }

            if (request.StandingCapacity.HasValue && request.StandingCapacity.Value < 0)
            {
                ValidationException.ThrowException("StandingCapacity", "Capacidade em pé deve ser positiva");
            }

            if (request.ParkingSpaces.HasValue && request.ParkingSpaces.Value < 0)
            {
                ValidationException.ThrowException("ParkingSpaces", "Número de vagas deve ser positivo");
            }

            // Serializar lista de instalações adicionais
            string? additionalFacilitiesJson = null;
            if (request.AdditionalFacilities != null)
            {
                additionalFacilitiesJson = JsonSerializer.Serialize(request.AdditionalFacilities);
            }

            // Atualizar capacidade e infraestrutura
            church.UpdateCapacityAndInfrastructure(
                seatingCapacity: request.SeatingCapacity,
                standingCapacity: request.StandingCapacity,
                parkingSpaces: request.ParkingSpaces,
                hasWifi: request.HasWifi,
                wifiPassword: request.WifiPassword,
                hasCafeteria: request.HasCafeteria,
                hasBookstore: request.HasBookstore,
                hasNursery: request.HasNursery,
                hasSoundSystem: request.HasSoundSystem,
                hasProjector: request.HasProjector,
                hasAirConditioning: request.HasAirConditioning,
                hasBaptistery: request.HasBaptistery,
                additionalFacilities: additionalFacilitiesJson,
                equipmentNotes: request.EquipmentNotes
            );

            _unitOfWork.Churchs.Update(church);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Capacidade e infraestrutura atualizadas para igreja {ChurchId} por {MemberId}", 
                church.Id, currentMember.Id);

            return Unit.Value;
        }
    }
}
