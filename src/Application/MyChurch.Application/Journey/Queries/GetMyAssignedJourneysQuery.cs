using MediatR;
using Microsoft.EntityFrameworkCore;
using Mychurch.Common.Utils.Objects;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyChurch.Application.Journey.Queries
{
    public class GetMyAssignedJourneysQuery : JwtMemberDto, IRequest<PagedResultDto<JourneyDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GetMyAssignedJourneysQueryHandler : IRequestHandler<GetMyAssignedJourneysQuery, PagedResultDto<JourneyDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetMyAssignedJourneysQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResultDto<JourneyDto>> Handle(GetMyAssignedJourneysQuery request, CancellationToken cancellationToken)
        {
            var assignedJourneyIdsQuery = _unitOfWork.MemberJourneyAssignments.Query()
                .Where(a => a.MemberId == request.UserId)
                .Select(a => a.JourneyId);

            var totalCount = await assignedJourneyIdsQuery.CountAsync(cancellationToken);

            var assignedJourneyIds = await assignedJourneyIdsQuery.ToListAsync(cancellationToken);

            var journeysQuery = _unitOfWork.Journeys.Query()
                .Include(j => j.Stages)
                .Where(j => assignedJourneyIds.Contains(j.Id));

            var journeys = await journeysQuery
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var memberProgress = await _unitOfWork.MemberJourneyProgresses.Query()
                .Where(p => p.MemberId == request.UserId)
                .Select(p => p.JourneyStageId)
                .ToListAsync(cancellationToken);

            var journeyDtos = journeys.Select(j => JourneyDto.Create(j, memberProgress)).ToList();

            return new PagedResultDto<JourneyDto>(journeyDtos, request.PageNumber, request.PageSize, totalCount);
        }
    }
}
