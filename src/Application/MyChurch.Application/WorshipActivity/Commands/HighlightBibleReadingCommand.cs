using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MyChurch.Application.WorshipActivity.Commands
{
    public class HighlightBibleReadingCommand : IRequest<int> // Retorna o Id da atividade
    {
        public int WorshipServiceId { get; set; }
        public int VersionId { get; set; }
        public int BookId { get; set; }
        public int ChapterId { get; set; }
        public int? VerseId { get; set; }
        public bool? Finish { get; set; } // Se true, finaliza a atividade
        public int? ActivityId { get; set; } // Para finalizar por id
    }

    public class HighlightBibleReadingCommandHandler : IRequestHandler<HighlightBibleReadingCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;

        public HighlightBibleReadingCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Handle(HighlightBibleReadingCommand request, CancellationToken cancellationToken)
        {
            Domain.Entities.WorshipActivity activity = null;
            if (request.Finish == true && request.ActivityId.HasValue)
            {
                // Finaliza a atividade pelo id
                activity = await _unitOfWork.WorshipActivities.Query()
                    .Include(a => a.Bibles)
                    .FirstOrDefaultAsync(a => a.Id == request.ActivityId.Value && a.WorshipServiceId == request.WorshipServiceId, cancellationToken);
                if (activity != null)
                {
                    activity.IsCurrent = false;
                    _unitOfWork.WorshipActivities.Update(activity);
                    await _unitOfWork.CommitAsync();
                    return activity.Id;
                }
                return 0;
            }

            // Busca a atividade de leitura bíblica atual
            activity = await _unitOfWork.WorshipActivities.Query()
                .Include(a => a.Bibles)
                .FirstOrDefaultAsync(a => a.WorshipServiceId == request.WorshipServiceId && a.Name == "Leitura Bíblica" && a.IsCurrent, cancellationToken);

            if (activity == null)
            {
                activity = new Domain.Entities.WorshipActivity
                {
                    WorshipServiceId = request.WorshipServiceId,
                    Name = "Leitura Bíblica",
                    Order = 1,
                    IsCurrent = true,
                    Bibles = new List<WorshipActivityBible>()
                };
                _unitOfWork.WorshipActivities.Create(activity);
                await _unitOfWork.CommitAsync();
            }
            else
            {
                activity.Bibles.Clear();
            }
            activity.Bibles.Add(new WorshipActivityBible
            {
                BibleVersionId = request.VersionId,
                BookId = request.BookId,
                ChapterId = request.ChapterId,
                VerseStart = request.VerseId ?? 1,
                VerseEnd = request.VerseId
            });
            activity.IsCurrent = true;
            _unitOfWork.WorshipActivities.Update(activity);
            await _unitOfWork.CommitAsync();
            return activity.Id;
        }
    }
}