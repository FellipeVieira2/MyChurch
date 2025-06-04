using MediatR;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;

namespace MyChurch.Application.Bible.Queries.GetVersesByChapter
{
    public class GetVersesByChapterQuery : JwtMemberDto, IRequest<List<BibleVerseDto>>
    {
        public int ChapterId { get; set; }
    }

    public class GetVersesByChapterQueryHandler : IRequestHandler<GetVersesByChapterQuery, List<BibleVerseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetVersesByChapterQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<BibleVerseDto>> Handle(GetVersesByChapterQuery request, CancellationToken cancellationToken)
        {
            var verses = _unitOfWork.Verses.Query()
                .Where(v => v.ChapterId == request.ChapterId)
                .OrderBy(v => v.VerseNumber)
                .ToList();

            return [.. verses.Select(BibleVerseDto.New)];
        }
    }
}