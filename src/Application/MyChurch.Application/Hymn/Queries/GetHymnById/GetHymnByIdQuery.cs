using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;

namespace MyChurch.Application.Hymn.Queries.GetHymnById
{
    public class GetHymnByIdQuery : IRequest<HymnDto?>
    {
        public int Id { get; set; }
    }

    public class GetHymnByIdQueryHandler : IRequestHandler<GetHymnByIdQuery, HymnDto?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetHymnByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<HymnDto?> Handle(GetHymnByIdQuery request, CancellationToken cancellationToken)
        {
            var hymn = await _unitOfWork.Hymns.Query()
                .AsNoTracking()
                .Include(h => h.HymnVerses)
                .FirstOrDefaultAsync(h => h.Id == request.Id, cancellationToken);

            if (hymn == null)
            {
                return null;
            }

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
                    .Select(v => new HymnVerseDto
                    {
                        Id = v.Id,
                        Number = v.Number,
                        Text = v.Text
                    })
                    .ToList()
            };
        }
    }
}
