using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.WorshipActivity.Commands
{
    public class StartWorshipCommand : IRequest<bool>
    {
        public int WorshipServiceId { get; set; }
    }

    public class StartWorshipCommandHandler : IRequestHandler<StartWorshipCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public StartWorshipCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(StartWorshipCommand request, CancellationToken cancellationToken)
        {
            var worshipService = await _unitOfWork.WorshipServices.Query()
                .FirstOrDefaultAsync(ws => ws.Id == request.WorshipServiceId, cancellationToken);

            if (worshipService == null)
                ValidationException.ThrowException("WorshipService", "Culto não encontrado.");

            if (worshipService.Status == WorshipServiceStatus.InProgress)
                ValidationException.ThrowException("WorshipService", "O culto já está em andamento.");

            worshipService.Status = WorshipServiceStatus.InProgress;
            worshipService.StartTime = DateTime.UtcNow;
            _unitOfWork.WorshipServices.Update(worshipService);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
