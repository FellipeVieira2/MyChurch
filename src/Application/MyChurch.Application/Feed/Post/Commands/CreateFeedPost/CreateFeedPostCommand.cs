using System.Text.RegularExpressions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Feed.Post.Commands.CreateFeedPost
{
    public class CreateFeedPostCommand : JwtMemberDto, IRequest<int>
    {
        public string Content { get; set; }
    }
    public class CreateFeedPostCommandHandler : IRequestHandler<CreateFeedPostCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateFeedPostCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Handle(CreateFeedPostCommand request, CancellationToken cancellationToken)
        {
            // Busca o membro autenticado para obter o ChurchId
            var member = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (member == null)
                ValidationException.ThrowException("Member", "Authenticated member does not exist.");

            var sanitizedContent = SanitizeContent(request.Content);

            var post = new FeedPost
            {
                MemberId = member.Id,
                ChurchId = member.ChurchId,
                Content = request.Content,
                Created = DateTime.UtcNow
            };

            _unitOfWork.FeedPosts.Create(post);
            await _unitOfWork.CommitAsync();

            return post.Id;
        }
        private string SanitizeContent(string content)
        {
            // Remove qualquer ocorrência de <script>...</script> (case-insensitive)
            return Regex.Replace(content, "<script.*?>.*?</script>", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        }
    }
}
