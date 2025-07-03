using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyChurch.Application.Hymn.Queries.GetAllHymnSummaries
{
    public class GetAllHymnSummariesQuery : IRequest<IEnumerable<HymnSummaryDto>>
    {
    }

    public class GetAllHymnSummariesQueryHandler : IRequestHandler<GetAllHymnSummariesQuery, IEnumerable<HymnSummaryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllHymnSummariesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<HymnSummaryDto>> Handle(GetAllHymnSummariesQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Hymns.Query()
                .AsNoTracking()
                .OrderBy(h => h.Number)
                .Select(h => new HymnSummaryDto
                {
                    Number = h.Number,
                    Title = h.Title
                })
                .ToListAsync(cancellationToken);
        }
    }
}
