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
        public string? SearchTerm { get; set; }
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
            var hymnsQuery = _unitOfWork.Hymns.Query()
                .AsNoTracking()
                .Include(h => h.HymnVerses)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.Trim().ToLower();

                hymnsQuery = hymnsQuery.Where(h =>
                    h.Title.ToLower().Contains(searchTerm) ||
                    h.Number.ToString().Contains(searchTerm) ||
                    h.Language.ToLower().Contains(searchTerm) ||
                    h.LyricsAuthor.ToLower().Contains(searchTerm) ||
                    h.MelodyAuthor.ToLower().Contains(searchTerm));
            }

            return await hymnsQuery
                .OrderBy(h => h.Number)
                .Select(h => new HymnSummaryDto
                {
                    Id = h.Id,
                    Number = h.Number,
                    Title = h.Title,
                    Language = h.Language,
                    LyricsAuthor = h.LyricsAuthor,
                    MelodyAuthor = h.MelodyAuthor,
                    HasChorus = !string.IsNullOrWhiteSpace(h.Chorus),
                    VerseCount = h.HymnVerses.Count
                })
                .ToListAsync(cancellationToken);
        }
    }
}
