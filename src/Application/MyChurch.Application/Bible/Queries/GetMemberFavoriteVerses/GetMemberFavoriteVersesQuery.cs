using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyChurch.Application.Bible.Queries.GetMemberFavoriteVerses
{
    public class GetMemberFavoriteVersesQuery : JwtMemberDto, IRequest<List<MemberFavoriteVerseDto>>
    {
    }

    public class MemberFavoriteVerseDto
    {
        public int Id { get; set; }
        public int VersionId { get; set; }
        public string VersionName { get; set; }
        public string BookName { get; set; }
        public int ChapterNumber { get; set; }
        public int VerseNumber { get; set; }
        public string VerseText { get; set; }
        public DateTime DateFavorited { get; set; }
    }

    public class GetMemberFavoriteVersesQueryHandler : IRequestHandler<GetMemberFavoriteVersesQuery, List<MemberFavoriteVerseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetMemberFavoriteVersesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<MemberFavoriteVerseDto>> Handle(GetMemberFavoriteVersesQuery request, CancellationToken cancellationToken)
        {
            // Check if member exists
            var member = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (member == null)
                throw new UnauthorizedAccessException("Usuário não encontrado.");

            // Get all favorite verses for the member
            var favoriteVerses = await _unitOfWork.MemberFavoriteVerses.Query()
                .Where(fv => fv.MemberId == request.UserId)
                .OrderByDescending(fv => fv.DateFavorited)
                .ToListAsync(cancellationToken);

            var result = new List<MemberFavoriteVerseDto>();

            foreach (var fav in favoriteVerses)
            {
                // Get version name
                var version = await _unitOfWork.Versions.Query()
                    .FirstOrDefaultAsync(v => v.Id == fav.VersionId, cancellationToken);

                // Find the verse text
                var book = await _unitOfWork.Books.Query()
                    .Include(b => b.Chapters)
                    .ThenInclude(c => c.Verses)
                    .Where(b => b.VersionId == fav.VersionId && b.Name == fav.BookName)
                    .FirstOrDefaultAsync(cancellationToken);

                string verseText = "Texto não disponível";

                if (book != null)
                {
                    var chapter = book.Chapters.FirstOrDefault(c => c.ChapterNumber == fav.ChapterNumber);
                    if (chapter != null)
                    {
                        var verse = chapter.Verses.FirstOrDefault(v => v.VerseNumber == fav.VerseNumber);
                        if (verse != null)
                        {
                            verseText = verse.Text;
                        }
                    }
                }

                result.Add(new MemberFavoriteVerseDto
                {
                    Id = fav.Id,
                    VersionId = fav.VersionId,
                    VersionName = version?.Name ?? "Versão desconhecida",
                    BookName = fav.BookName,
                    ChapterNumber = fav.ChapterNumber,
                    VerseNumber = fav.VerseNumber,
                    VerseText = verseText,
                    DateFavorited = fav.DateFavorited
                });
            }

            return result;
        }
    }
}