using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities.Bible;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyChurch.Application.Bible.Commands.AddFavoriteVerse
{
    public class AddFavoriteVerseCommand : JwtMemberDto, IRequest<int>
    {
        public int VersionId { get; set; }
        public string BookName { get; set; }
        public int ChapterNumber { get; set; }
        public int VerseNumber { get; set; }
    }

    public class AddFavoriteVerseCommandHandler : IRequestHandler<AddFavoriteVerseCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddFavoriteVerseCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Handle(AddFavoriteVerseCommand request, CancellationToken cancellationToken)
        {
            // Check if member exists
            var member = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (member == null)
                throw new UnauthorizedAccessException("Usuário não encontrado.");

            // Check if verse already exists as favorite for this member
            var existingFavorite = await _unitOfWork.MemberFavoriteVerses.Query()
                .FirstOrDefaultAsync(v => 
                    v.MemberId == request.UserId &&
                    v.VersionId == request.VersionId &&
                    v.BookName == request.BookName &&
                    v.ChapterNumber == request.ChapterNumber &&
                    v.VerseNumber == request.VerseNumber,
                    cancellationToken);

            if (existingFavorite != null)
            {
                // Verse is already a favorite, return its id
                return existingFavorite.Id;
            }

            // Add new favorite verse
            var favoriteVerse = new MemberFavoriteVerse
            {
                MemberId = request.UserId,
                VersionId = request.VersionId,
                BookName = request.BookName,
                ChapterNumber = request.ChapterNumber,
                VerseNumber = request.VerseNumber,
                DateFavorited = DateTime.UtcNow
            };

            _unitOfWork.MemberFavoriteVerses.Create(favoriteVerse);
            await _unitOfWork.CommitAsync();

            return favoriteVerse.Id;
        }
    }
}