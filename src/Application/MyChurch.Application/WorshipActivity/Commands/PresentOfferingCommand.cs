using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Exceptions;
using System.Threading;
using System.Threading.Tasks;

namespace MyChurch.Application.WorshipActivity.Commands
{
    public class PresentOfferingCommand : IRequest<int>
    {
        public int WorshipServiceId { get; set; }
        public int? ActivityId { get; set; } // Se informado, finaliza; se não, cria nova
        public bool Finish { get; set; } = false;
    }

    public class PresentOfferingCommandHandler : IRequestHandler<PresentOfferingCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;

        public PresentOfferingCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Handle(PresentOfferingCommand request, CancellationToken cancellationToken)
        {
            if (request.Finish && request.ActivityId.HasValue)
            {
                // Finaliza a atividade de oferta existente
                var activity = await _unitOfWork.WorshipActivities.Query()
                    .FirstOrDefaultAsync(a => a.Id == request.ActivityId.Value && a.WorshipServiceId == request.WorshipServiceId, cancellationToken);

                if (activity == null)
                    ValidationException.ThrowException("WorshipActivity", "Atividade de oferta não encontrada para o culto.");

                activity.IsCurrent = false;
                await _unitOfWork.CommitAsync();
                return activity.Id;
            }
            else
            {
                // Cria nova atividade de oferta
                var activity = new Domain.Entities.WorshipActivity
                {
                    WorshipServiceId = request.WorshipServiceId,
                    Name = "Oferta",
                    Order = 2,
                    IsCurrent = true
                };
                _unitOfWork.WorshipActivities.Create(activity);
                await _unitOfWork.CommitAsync();
                return activity.Id;
            }
        }
    }
}