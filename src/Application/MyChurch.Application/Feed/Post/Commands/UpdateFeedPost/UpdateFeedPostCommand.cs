using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;
using System;
using System.Text.Json.Serialization;

namespace MyChurch.Application.Feed.Post.Commands.UpdateFeedPost
{
    public class UpdateFeedPostCommand : JwtMemberDto, IRequest<FeedPostDto>
    {
        [JsonIgnore]
        public int PostId { get; set; }
        public string Content { get; set; }
    }

    public class UpdateFeedPostCommandHandler : IRequestHandler<UpdateFeedPostCommand, FeedPostDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateFeedPostCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<FeedPostDto> Handle(UpdateFeedPostCommand request, CancellationToken cancellationToken)
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
                ValidationException.ThrowException("FeedPost", "Você só pode atualizar seus próprios posts.");

            // Verifica se o prazo de 2 horas já passou
            if ((DateTime.UtcNow - post.Created).TotalHours > 2)
                ValidationException.ThrowException("FeedPost", "O post só pode ser editado até 2 horas após a criação.");

            // Atualiza o conteúdo e a data de atualização
            post.Content = request.Content;
            post.Updated = DateTime.UtcNow;

            _unitOfWork.FeedPosts.Update(post);
            await _unitOfWork.CommitAsync();

            return FeedPostDto.New(post);
        }
    }
}
