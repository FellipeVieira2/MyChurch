using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;

namespace MyChurch.Application.Hymn.Queries.GetHymnByNumber
{
    public class GetHymnByNumberQuery : IRequest<HymnDto>
    {
        public int Number { get; set; }
    }

    public class GetHymnByNumberQueryHandler : IRequestHandler<GetHymnByNumberQuery, HymnDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetHymnByNumberQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<HymnDto> Handle(GetHymnByNumberQuery request, CancellationToken cancellationToken)
        {
            var hymn = await _unitOfWork.Hymns.Query()
                .Include(h => h.HymnVerses)
                .FirstOrDefaultAsync(h => h.Number == request.Number, cancellationToken);
            if (hymn == null)
                return null;

            return new HymnDto
            {
                Id = hymn.Id,
                Title = hymn.Title,
                Number = hymn.Number,
                Language = hymn.Language,
                Chorus = hymn.Chorus,
                LyricsAuthor = hymn.LyricsAuthor,
                MelodyAuthor = hymn.MelodyAuthor,
                Verses = hymn.HymnVerses
                    .OrderBy(v => v.Number)
                    .Select(v => new HymnVerseDto { Id = v.Id, Number = v.Number, Text = v.Text })
                    .ToList()
            };
        }
    }
}