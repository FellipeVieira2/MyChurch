using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Feed.Post.Commands.DeleteFeedPost
{
    public class DeleteFeedPostCommand : JwtMemberDto, IRequest<bool>
    {
        public int PostId { get; set; }
    }

    public class DeleteFeedPostCommandHandler : IRequestHandler<DeleteFeedPostCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteFeedPostCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteFeedPostCommand request, CancellationToken cancellationToken)
        {
            // Busca o post pelo ID
            var post = await _unitOfWork.FeedPosts.Query()
                .Include(p => p.Member)
                .Include(x => x.Likes)
                .FirstOrDefaultAsync(p => p.Id == request.PostId, cancellationToken);

            if (post == null)
                ValidationException.ThrowException("FeedPost", "Post não encontrado.");

            // Verifica se o usuário logado é o autor do post
            if (post.MemberId != request.UserId)
                ValidationException.ThrowException("FeedPost", "Você só pode deletar seus próprios posts.");

            // Verifica se o prazo de 2 horas já passou
            if ((DateTime.UtcNow - post.Created).TotalHours > 2)
                ValidationException.ThrowException("FeedPost", "O post só pode ser deletado até 2 horas após a criação.");

            _unitOfWork.FeedPosts.Delete(post);
            await _unitOfWork.CommitAsync();

            return true;
        }
    }
}
