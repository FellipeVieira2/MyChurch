using MediatR;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Church.Commands.UpdateChurchSchedule
{
    public class UpdateChurchScheduleCommand : JwtMemberDto, IRequest<ChurchScheduleDto>
    {
        public int Id { get; set; }
        public int ChurchId { get; set; }
        public DayOfWeek? DayOfWeek { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public string? ServiceType { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
    }

    public class UpdateChurchScheduleCommandHandler : IRequestHandler<UpdateChurchScheduleCommand, ChurchScheduleDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateChurchScheduleCommandHandler> _logger;

        public UpdateChurchScheduleCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<UpdateChurchScheduleCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ChurchScheduleDto> Handle(UpdateChurchScheduleCommand request, CancellationToken cancellationToken)
        {
            // Verifica permissão
            var member = _unitOfWork.Members.Query()
                .FirstOrDefault(m => m.Id == request.UserId && m.ChurchId == request.ChurchId);

            if (member == null || member.Role != Domain.Enum.UserRole.Admin)
            {
                ValidationException.ThrowException("UpdateSchedule", "Você não tem permissão para atualizar horários.");
            }

            var schedule = _unitOfWork.ChurchSchedules.Query()
                .FirstOrDefault(s => s.Id == request.Id && s.ChurchId == request.ChurchId);

            if (schedule == null)
            {
                ValidationException.ThrowException("UpdateSchedule", "Horário não encontrado.");
            }

            // Atualiza campos
            if (request.DayOfWeek.HasValue)
                schedule.DayOfWeek = request.DayOfWeek.Value;

            if (!string.IsNullOrEmpty(request.StartTime))
            {
                if (!TimeSpan.TryParse(request.StartTime, out var startTime))
                    ValidationException.ThrowException("StartTime", "Hora de início inválida");
                schedule.StartTime = startTime;
            }

            if (request.EndTime != null)
            {
                if (string.IsNullOrEmpty(request.EndTime))
                {
                    schedule.EndTime = null;
                }
                else if (TimeSpan.TryParse(request.EndTime, out var endTime))
                {
                    schedule.EndTime = endTime;
                }
                else
                {
                    ValidationException.ThrowException("EndTime", "Hora de término inválida");
                }
            }

            if (!string.IsNullOrEmpty(request.ServiceType))
                schedule.ServiceType = request.ServiceType;

            if (request.Description != null)
                schedule.Description = request.Description;

            if (request.IsActive.HasValue)
                schedule.IsActive = request.IsActive.Value;

            schedule.Updated = DateTime.UtcNow;

            _unitOfWork.ChurchSchedules.Update(schedule);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Horário atualizado: ID {Id}", request.Id);

            return ChurchScheduleDto.New(schedule);
        }
    }
}
