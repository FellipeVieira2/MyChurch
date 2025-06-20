using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;

namespace MyChurch.Application.Bible.Queries.GetChaptersByBookName
{
    public class GetChaptersByBookNameQuery : JwtMemberDto, IRequest<List<BibleChapterDto>>
    {
        public string BookName { get; set; }
        public int VersionId { get; set; }
    }

    public class GetChaptersByBookNameQueryHandler : IRequestHandler<GetChaptersByBookNameQuery, List<BibleChapterDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetChaptersByBookNameQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<BibleChapterDto>> Handle(GetChaptersByBookNameQuery request, CancellationToken cancellationToken)
        {
            var book = await _unitOfWork.Books.Query()
                .FirstOrDefaultAsync(b => 
                    b.VersionId == request.VersionId && 
                    b.Name.Equals(request.BookName, StringComparison.CurrentCultureIgnoreCase), 
                    cancellationToken);

            if (book == null)
                return new List<BibleChapterDto>();

            var chapters = _unitOfWork.Chapters.Query()
                .Where(c => c.BookId == book.Id)
                .OrderBy(c => c.ChapterNumber)
                .ToList();

            return [.. chapters.Select(BibleChapterDto.New)];
        }
    }
}