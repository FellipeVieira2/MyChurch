using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MyChurch.Application.Bible.Commands.RemoveFavoriteVerse
{
    public class RemoveFavoriteVerseCommand : JwtMemberDto, IRequest<Unit>
    {
        public int FavoriteVerseId { get; set; }
    }

    public class RemoveFavoriteVerseCommandHandler : IRequestHandler<RemoveFavoriteVerseCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RemoveFavoriteVerseCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(RemoveFavoriteVerseCommand request, CancellationToken cancellationToken)
        {
            // Check if member exists
            var member = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (member == null)
                throw new UnauthorizedAccessException("Usuário não encontrado.");

            // Find the favorite verse
            var favoriteVerse = await _unitOfWork.MemberFavoriteVerses.Query()
                .FirstOrDefaultAsync(fv => fv.Id == request.FavoriteVerseId && fv.MemberId == request.UserId, cancellationToken);

            if (favoriteVerse == null)
               ValidationException.ThrowException("verse","Versículo favorito não encontrado ou não pertence ao usuário.");

            // Remove the favorite verse
            _unitOfWork.MemberFavoriteVerses.Delete(favoriteVerse);
            await _unitOfWork.CommitAsync();

            return Unit.Value;
        }
    }
}