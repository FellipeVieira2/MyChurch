using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Feed.Like.Commands.RemoveFeedLike
{
    public class RemoveFeedLikeCommand : JwtMemberDto, IRequest<Unit>
    {
        public int PostId { get; set; }
    }

    public class RemoveFeedLikeCommandHandler : IRequestHandler<RemoveFeedLikeCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RemoveFeedLikeCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(RemoveFeedLikeCommand request, CancellationToken cancellationToken)
        {
            // Busca o membro logado
            var member = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (member == null)
                throw new UnauthorizedAccessException("Usuário não encontrado.");

            // Busca o like do membro para o post
            var like = await _unitOfWork.FeedLikes.Query()
                .FirstOrDefaultAsync(l => l.FeedPostId == request.PostId && l.MemberId == member.Id, cancellationToken);

            if (like == null)
                ValidationException.ThrowException("FeedLike", "Você ainda não curtiu este post.");

            _unitOfWork.FeedLikes.Delete(like);
            await _unitOfWork.CommitAsync();

            return Unit.Value;
        }
    }
}
