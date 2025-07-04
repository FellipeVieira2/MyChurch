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

                activity.DonationTime = false;
                await _unitOfWork.CommitAsync();
                return activity.Id;
            }
            else
            {
                var activity = await _unitOfWork.WorshipActivities.Query()
                .Include(a => a.Bibles)
                .Include(a => a.Hymns)
                .FirstOrDefaultAsync(a => a.WorshipServiceId == request.WorshipServiceId && a.IsCurrent, cancellationToken);

                if (activity == null)
                {
                    activity = new Domain.Entities.WorshipActivity
                    {
                        WorshipServiceId = request.WorshipServiceId,
                        Name = "Louvor",
                        Order = 1,
                        IsCurrent = true,
                        Bibles = [],
                        DonationTime = true, // Marca como tempo de oferta
                        Hymns = []
                    };

                    _unitOfWork.WorshipActivities.Create(activity);
                    await _unitOfWork.CommitAsync();
                }
                else
                {
                    activity.DonationTime = true;
                    _unitOfWork.WorshipActivities.Update(activity);
                    await _unitOfWork.CommitAsync();
                }


                return activity.Id;
            }
        }
    }
}