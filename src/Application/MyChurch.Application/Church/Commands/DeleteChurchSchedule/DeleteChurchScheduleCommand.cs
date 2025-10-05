using MediatR;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Church.Commands.DeleteChurchSchedule
{
    public class DeleteChurchScheduleCommand : JwtMemberDto, IRequest<Unit>
    {
        public int Id { get; set; }
        public int ChurchId { get; set; }
    }

    public class DeleteChurchScheduleCommandHandler : IRequestHandler<DeleteChurchScheduleCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteChurchScheduleCommandHandler> _logger;

        public DeleteChurchScheduleCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<DeleteChurchScheduleCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteChurchScheduleCommand request, CancellationToken cancellationToken)
        {
            // Verifica permissão
            var member = _unitOfWork.Members.Query()
                .FirstOrDefault(m => m.Id == request.UserId && m.ChurchId == request.ChurchId);

            if (member == null || member.Role != Domain.Enum.UserRole.Admin)
            {
                ValidationException.ThrowException("DeleteSchedule", "Você não tem permissão para deletar horários.");
            }

            var schedule = _unitOfWork.ChurchSchedules.Query()
                .FirstOrDefault(s => s.Id == request.Id && s.ChurchId == request.ChurchId);

            if (schedule == null)
            {
                ValidationException.ThrowException("DeleteSchedule", "Horário não encontrado.");
            }

            _unitOfWork.ChurchSchedules.Delete(schedule);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Horário deletado: ID {Id}", request.Id);

            return Unit.Value;
        }
    }
}
