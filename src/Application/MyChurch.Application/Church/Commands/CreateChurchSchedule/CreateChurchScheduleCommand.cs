using MediatR;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Church.Commands.CreateChurchSchedule
{
    /// <summary>
    /// Comando para criar um novo horário de culto/atividade
    /// </summary>
    public class CreateChurchScheduleCommand : JwtMemberDto, IRequest<ChurchScheduleDto>
    {
        /// <summary>ID da igreja</summary>
        public int ChurchId { get; set; }
        
        /// <summary>Dia da semana (0 = Domingo, 6 = Sábado)</summary>
        /// <example>0</example>
        public DayOfWeek DayOfWeek { get; set; }
        
        /// <summary>Hora de início (HH:mm)</summary>
        /// <example>19:00</example>
        public string StartTime { get; set; }
        
        /// <summary>Hora de término (HH:mm) - Opcional</summary>
        /// <example>21:00</example>
        public string? EndTime { get; set; }
        
        /// <summary>Tipo de serviço</summary>
        /// <example>Culto de Celebração</example>
        public string ServiceType { get; set; }
        
        /// <summary>Descrição adicional</summary>
        /// <example>Culto principal com louvor e pregação</example>
        public string? Description { get; set; }
    }

    public class CreateChurchScheduleCommandHandler : IRequestHandler<CreateChurchScheduleCommand, ChurchScheduleDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateChurchScheduleCommandHandler> _logger;

        public CreateChurchScheduleCommandHandler(
            IUnitOfWork unitOfWork, 
            ILogger<CreateChurchScheduleCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ChurchScheduleDto> Handle(CreateChurchScheduleCommand request, CancellationToken cancellationToken)
        {
            // Verifica se o usuário é admin da igreja
            var member = _unitOfWork.Members.Query()
                .FirstOrDefault(m => m.Id == request.UserId && m.ChurchId == request.ChurchId);

            if (member == null || member.Role != Domain.Enum.UserRole.Admin)
            {
                _logger.LogWarning("User {UserId} não tem permissão para criar horário na igreja {ChurchId}", 
                    request.UserId, request.ChurchId);
                ValidationException.ThrowException("CreateSchedule", "Você não tem permissão para criar horários nesta igreja.");
            }

            // Converte strings de hora para TimeSpan
            if (!TimeSpan.TryParse(request.StartTime, out var startTime))
            {
                ValidationException.ThrowException("StartTime", "Hora de início inválida. Use o formato HH:mm");
            }

            TimeSpan? endTime = null;
            if (!string.IsNullOrEmpty(request.EndTime))
            {
                if (!TimeSpan.TryParse(request.EndTime, out var parsedEndTime))
                {
                    ValidationException.ThrowException("EndTime", "Hora de término inválida. Use o formato HH:mm");
                }
                endTime = parsedEndTime;
            }

            // Cria o horário
            var schedule = new ChurchSchedule
            {
                ChurchId = request.ChurchId,
                DayOfWeek = request.DayOfWeek,
                StartTime = startTime,
                EndTime = endTime,
                ServiceType = request.ServiceType,
                Description = request.Description,
                IsActive = true,
                Created = DateTime.UtcNow
            };

            _unitOfWork.ChurchSchedules.Create(schedule);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Horário criado: {DayOfWeek} às {StartTime} - {ServiceType}", 
                request.DayOfWeek, request.StartTime, request.ServiceType);

            return ChurchScheduleDto.New(schedule);
        }
    }
}
