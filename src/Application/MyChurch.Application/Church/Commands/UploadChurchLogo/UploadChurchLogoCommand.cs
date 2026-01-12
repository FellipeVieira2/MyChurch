using System.Text.Json.Serialization;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;
using MyChurch.Infrastructure.Utils.S3;

namespace MyChurch.Application.Church.Commands.UploadChurchLogo
{
    public class UploadChurchLogoCommand : JwtMemberDto, IRequest<UploadChurchLogoResult>
    {
        [JsonIgnore]
        public int ChurchId { get; set; }

        public string? Base64Image { get; set; }
        public string? FileName { get; set; }
    }

    public class UploadChurchLogoResult
    {
        public string LogoUrl { get; set; } = string.Empty;
    }

    public class UploadChurchLogoCommandHandler : IRequestHandler<UploadChurchLogoCommand, UploadChurchLogoResult>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IS3Helper _s3Helper;
        private readonly ILogger<UploadChurchLogoCommandHandler> _logger;

        public UploadChurchLogoCommandHandler(IUnitOfWork unitOfWork, IS3Helper s3Helper, ILogger<UploadChurchLogoCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _s3Helper = s3Helper;
            _logger = logger;
        }

        public async Task<UploadChurchLogoResult> Handle(UploadChurchLogoCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Base64Image))
                ValidationException.ThrowException("Logo", "Base64Image is required.");

            // Admin só pode alterar a própria igreja
            var member = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (member == null || member.ChurchId != request.ChurchId)
                ValidationException.ThrowException("Logo", "You do not have permission to update this church.");

            var church = await _unitOfWork.Churchs.Query()
                .FirstOrDefaultAsync(c => c.Id == request.ChurchId, cancellationToken);

            if (church == null)
                ValidationException.ThrowException("Logo", "Church not found.");

            var base64 = request.Base64Image;
            if (base64.Contains(','))
                base64 = base64.Split(',')[1];

            var bytes = Convert.FromBase64String(base64);
            using var stream = new MemoryStream(bytes);

            // Mantém a extensão quando possível
            var ext = ".jpg";
            if (!string.IsNullOrWhiteSpace(request.FileName))
                ext = Path.GetExtension(request.FileName);

            if (string.IsNullOrWhiteSpace(ext))
                ext = ".jpg";

            var contentType = ext.ToLowerInvariant() switch
            {
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".webp" => "image/webp",
                _ => "image/jpeg"
            };

            var key = $"church/{request.ChurchId}/logo/{Guid.NewGuid()}{ext}";
            var logoUrl = await _s3Helper.UploadFileAsync(stream, key, contentType, cancellationToken);

            church.UpdateLogo(logoUrl);
            _unitOfWork.Churchs.Update(church);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Logo da igreja atualizado. ChurchId: {ChurchId}", church.Id);

            return new UploadChurchLogoResult { LogoUrl = logoUrl };
        }
    }
}
