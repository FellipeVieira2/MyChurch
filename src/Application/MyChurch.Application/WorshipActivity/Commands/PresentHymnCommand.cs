using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Application.Hymn.Queries.GetHymnByNumber;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Exceptions;
using System.Text.Json.Serialization;

namespace MyChurch.Application.WorshipActivity.Commands
{
    public class PresentHymnCommand : IRequest<object>
    {
        [JsonIgnore]
        public int WorshipServiceId { get; set; }
        [JsonIgnore]

        public int HymnNumber { get; set; }
        [JsonIgnore]

        public int VerseNumber { get; set; }
    }

    public class PresentHymnCommandHandler : IRequestHandler<PresentHymnCommand, object>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISender _sender;

        public PresentHymnCommandHandler(IUnitOfWork unitOfWork, ISender sender)
        {
            _unitOfWork = unitOfWork;
            _sender = sender;
        }

        public async Task<object> Handle(PresentHymnCommand request, CancellationToken cancellationToken)
        {
            var hymnQuery = new GetHymnByNumberQuery { Number = request.HymnNumber };
            var hymnDto = await _sender.Send(hymnQuery, cancellationToken);

            if (hymnDto == null)
            {
                ValidationException.ThrowException("Hymn", "Hino não encontrado.");
            }

            var worshipService = await _unitOfWork.WorshipServices.Query()
                .FirstOrDefaultAsync(ws => ws.Id == request.WorshipServiceId, cancellationToken);

            if (worshipService == null)
            {
                ValidationException.ThrowException("WorshipService", "Culto não encontrado.");
            }

            var currentActivities = await _unitOfWork.WorshipActivities.Query()
                .Where(wa => wa.WorshipServiceId == request.WorshipServiceId && wa.IsCurrent)
                .ToListAsync(cancellationToken);

            foreach (var act in currentActivities)
            {
                act.IsCurrent = false;
                _unitOfWork.WorshipActivities.Update(act);
            }

            var newActivity = new Domain.Entities.WorshipActivity
            {
                WorshipServiceId = request.WorshipServiceId,
                IsCurrent = true,
                Hymns = new List<WorshipActivityHymn> { new WorshipActivityHymn { HymnId = hymnDto.Id } }
            };

            _unitOfWork.WorshipActivities.Create(newActivity);
            await _unitOfWork.CommitAsync();

            return new { hymnDto, VerseFocus = request.VerseNumber };
        }
    }
}