using MediatR;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;

namespace MyChurch.Application.Bible.Queries.GetChaptersByBook
{
    public class GetChaptersByBookQuery : JwtMemberDto, IRequest<List<BibleChapterDto>>
    {
        public int BookId { get; set; }
    }

    public class GetChaptersByBookQueryHandler : IRequestHandler<GetChaptersByBookQuery, List<BibleChapterDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetChaptersByBookQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<BibleChapterDto>> Handle(GetChaptersByBookQuery request, CancellationToken cancellationToken)
        {
            var chapters = _unitOfWork.Chapters.Query()
                .Where(c => c.BookId == request.BookId)
                .OrderBy(c => c.ChapterNumber)
                .ToList();

            return [.. chapters.Select(BibleChapterDto.New)];
        }
    }
}