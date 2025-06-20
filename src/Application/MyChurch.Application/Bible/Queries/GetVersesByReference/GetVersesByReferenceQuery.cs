using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;

namespace MyChurch.Application.Bible.Queries.GetVersesByReference
{
    public class GetVersesByReferenceQuery : JwtMemberDto, IRequest<List<BibleVerseDto>>
    {
        public int VersionId { get; set; }
        public string BookName { get; set; }
        public int ChapterNumber { get; set; }
    }

    public class GetVersesByReferenceQueryHandler : IRequestHandler<GetVersesByReferenceQuery, List<BibleVerseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetVersesByReferenceQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<BibleVerseDto>> Handle(GetVersesByReferenceQuery request, CancellationToken cancellationToken)
        {
            // Find the book by name and version ID
            var book = await _unitOfWork.Books.Query()
                .FirstOrDefaultAsync(b => 
                    b.VersionId == request.VersionId && 
                    b.Name.ToLower() == request.BookName.ToLower(), 
                    cancellationToken);

            if (book == null)
                return new List<BibleVerseDto>();

            // Find the chapter by book ID and chapter number
            var chapter = await _unitOfWork.Chapters.Query()
                .FirstOrDefaultAsync(c => 
                    c.BookId == book.Id && 
                    c.ChapterNumber == request.ChapterNumber, 
                    cancellationToken);

            if (chapter == null)
                return new List<BibleVerseDto>();

            // Get all verses for this chapter
            var verses = _unitOfWork.Verses.Query()
                .Where(v => v.ChapterId == chapter.Id)
                .OrderBy(v => v.VerseNumber)
                .ToList();

            return [.. verses.Select(BibleVerseDto.New)];
        }
    }
}