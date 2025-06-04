using MediatR;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;

namespace MyChurch.Application.Bible.Queries.GetVerseByChapterAndNumber
{
    public class GetVerseByChapterAndNumberQuery : JwtMemberDto, IRequest<BibleVerseDto?>
    {
        public int ChapterId { get; set; }
        public int VerseNumber { get; set; }
    }

    public class GetVerseByChapterAndNumberQueryHandler : IRequestHandler<GetVerseByChapterAndNumberQuery, BibleVerseDto?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetVerseByChapterAndNumberQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BibleVerseDto?> Handle(GetVerseByChapterAndNumberQuery request, CancellationToken cancellationToken)
        {
            var verse = _unitOfWork.Verses.Query()
                .FirstOrDefault(v => v.ChapterId == request.ChapterId && v.VerseNumber == request.VerseNumber);

            return verse != null ? BibleVerseDto.New(verse) : null;
        }
    }
}