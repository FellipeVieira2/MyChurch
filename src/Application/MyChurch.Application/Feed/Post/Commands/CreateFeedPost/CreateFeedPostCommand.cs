using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Exceptions;
using MyChurch.Infrastructure.Utils.S3;
using System.Text.RegularExpressions;

namespace MyChurch.Application.Feed.Post.Commands.CreateFeedPost
{
    public class CreateFeedPostCommand : JwtMemberDto, IRequest<int>
    {
        public string Content { get; set; }
        public List<string> Images { get; set; } = new List<string>();
        public bool VisibleToBranches { get; set; } = false;
    }
    public class CreateFeedPostCommandHandler : IRequestHandler<CreateFeedPostCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IS3Helper _s3Helper;


        public CreateFeedPostCommandHandler(IUnitOfWork unitOfWork, IS3Helper s3Helper)
        {
            _unitOfWork = unitOfWork;
            _s3Helper = s3Helper;
        }

        public async Task<int> Handle(CreateFeedPostCommand request, CancellationToken cancellationToken)
        {
            // Busca o membro autenticado para obter o ChurchId
            var member = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (member == null)
                ValidationException.ThrowException("Member", "Authenticated member does not exist.");

            var sanitizedContent = SanitizeContent(request.Content);

            List<FeedPostImage> images = new();
            foreach (var image in request.Images)
            {
                var sanitizedImage = image;
                if (image.Contains(','))
                    sanitizedImage = image.Split(',')[1];

                var photoBytes = Convert.FromBase64String(sanitizedImage);
                using var photoStream = new MemoryStream(photoBytes);

                var fileName = $"{Guid.NewGuid()}.jpg";
                // Upload e obtenção da URL do S3
                var fileUrl = await _s3Helper.UploadFileAsync(photoStream, fileName, "image/jpeg", cancellationToken);

                images.Add(new FeedPostImage
                {
                    FileName = fileUrl, // Salva a URL no FileName
                    Created = DateTime.UtcNow
                });
            }

            var post = new FeedPost
            {
                MemberId = member.Id,
                ChurchId = member.ChurchId,
                Content = request.Content,
                VisibleToBranches = request.VisibleToBranches,
                Created = DateTime.UtcNow,
                Images = images
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
