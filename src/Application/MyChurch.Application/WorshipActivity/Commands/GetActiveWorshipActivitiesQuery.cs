using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Contracts;
using MyChurch.Application.Dtos;

namespace MyChurch.Application.WorshipActivity.Commands
{
    public class GetActiveWorshipActivitiesQuery : IRequest<List<WorshipActivityDto>>
    {
        public int WorshipServiceId { get; set; }
    }

    public class GetActiveWorshipActivitiesQueryHandler : IRequestHandler<GetActiveWorshipActivitiesQuery, List<WorshipActivityDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetActiveWorshipActivitiesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<WorshipActivityDto>> Handle(GetActiveWorshipActivitiesQuery request, CancellationToken cancellationToken)
        {
            var activities = await _unitOfWork.WorshipActivities.Query()
                .Where(a => a.WorshipServiceId == request.WorshipServiceId && a.IsCurrent)
                .ToListAsync(cancellationToken);

            return [.. activities.Select(WorshipActivityDto.New)];
        }
    }
}
