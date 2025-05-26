using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;
using MyChurch.Infrastructure.Utils.S3;

namespace MyChurch.Application.Asset.Commands.CreateAsset
{
    public class CreateAssetCommand : JwtMemberDto, IRequest<int>
    {
        /// <summary>Name</summary>
        public string Name { get; set; } = null!;
        /// <summary>Value</summary>
        public decimal Value { get; set; }
        /// <summary>Description</summary>
        public string Description { get; set; } = null!;
        /// <summary>Photo (Base64)</summary>
        public string? Photo { get; set; }
        /// <summary>Type</summary>
        public AssetType Type { get; set; }
        /// <summary>Identification Code</summary>
        public string IdentificationCode { get; set; } = null!;
    }

    public class CreateAssetCommandHandler : IRequestHandler<CreateAssetCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateAssetCommandHandler> _logger;
        private readonly IS3Helper _s3Helper;

        public CreateAssetCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateAssetCommandHandler> logger, IS3Helper s3Helper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _s3Helper = s3Helper;
        }

        public async Task<int> Handle(CreateAssetCommand request, CancellationToken cancellationToken)
        {
            // Busca o membro logado para obter o ChurchId
            var loggedMember = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (loggedMember == null)
                ValidationException.ThrowException("Member", "This Member does not exist.");

            int churchId = loggedMember.ChurchId;

            // Validação de duplicidade de código de identificação para a igreja
            var exists = await _unitOfWork.Assets.Query()
                .AnyAsync(a => a.ChurchId == churchId && a.IdentificationCode == request.IdentificationCode, cancellationToken);

            if (exists)
                ValidationException.ThrowException("Asset", "An asset with this identification code already exists for this church.");

            var asset = new Domain.Entities.Asset
            {
                Name = request.Name,
                Value = request.Value,
                Description = request.Description,
                Type = request.Type,
                IdentificationCode = request.IdentificationCode,
                ChurchId = churchId,
                CreatedAt = DateTime.UtcNow
            };

            if (!string.IsNullOrEmpty(request.Photo))
            {
                asset.Photo = await UploadPhotoAsync(request.Photo, cancellationToken);
            }
            else
            {
                asset.Photo = string.Empty;
            }

            _unitOfWork.Assets.Create(asset);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Asset created with ID: {AssetId}", asset.Id);

            return asset.Id;
        }

        private async Task<string> UploadPhotoAsync(string photoBase64, CancellationToken cancellationToken)
        {
            if (photoBase64.Contains(','))
                photoBase64 = photoBase64.Split(',')[1];

            var photoBytes = Convert.FromBase64String(photoBase64);
            using var photoStream = new MemoryStream(photoBytes);
            var photoUrl = await _s3Helper.UploadFileAsync(photoStream, $"{Guid.NewGuid()}_asset.jpg", "image/jpeg", cancellationToken);

            return photoUrl;
        }
    }
}
