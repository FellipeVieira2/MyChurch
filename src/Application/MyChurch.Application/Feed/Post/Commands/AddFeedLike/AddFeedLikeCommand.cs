using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Feed.Like.Commands.AddFeedLike
{
    public class AddFeedLikeCommand : JwtMemberDto, IRequest<Unit>
    {
        public int PostId { get; set; }
    }

    public class AddFeedLikeCommandHandler : IRequestHandler<AddFeedLikeCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddFeedLikeCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(AddFeedLikeCommand request, CancellationToken cancellationToken)
        {
            // Busca o membro logado
            var member = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (member == null)
                throw new UnauthorizedAccessException("Usuário não encontrado.");

            // Busca o post
            var post = await _unitOfWork.FeedPosts.Query()
                .FirstOrDefaultAsync(p => p.Id == request.PostId && p.ChurchId == member.ChurchId, cancellationToken);

            if (post == null)
                ValidationException.ThrowException("FeedPost", "Post não encontrado ou não pertence à sua igreja.");

            // Verifica se já existe like desse membro para esse post
            var alreadyLiked = await _unitOfWork.FeedLikes.Query()
                .AnyAsync(l => l.FeedPostId == request.PostId && l.MemberId == member.Id, cancellationToken);

            if (alreadyLiked)
                ValidationException.ThrowException("FeedLike", "Você já curtiu este post.");

            // Cria o like
            var like = new FeedLike
            {
                FeedPostId = request.PostId,
                MemberId = member.Id,
                Created = DateTime.UtcNow
            };

            _unitOfWork.FeedLikes.Create(like);
            await _unitOfWork.CommitAsync();

            return Unit.Value;
        }
    }
}
