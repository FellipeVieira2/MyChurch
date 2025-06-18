using System.Text.Json.Serialization;
using MediatR;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Infrastructure.Utils.S3;
using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Asset.Commands.UpdateAsset
{
    public class UpdateAssetCommand : JwtMemberDto, IRequest<AssetDto>
    {
        [JsonIgnore]
        public int AssetId { get; set; }
        /// <summary>Name</summary>
        public string? Name { get; set; }
        /// <summary>Value</summary>
        public decimal? Value { get; set; }
        /// <summary>Quantity</summary>
        public int? Quantity { get; set; }
        /// <summary>Description</summary>
        public string? Description { get; set; }
        /// <summary>Photo (Base64)</summary>
        public string? Photo { get; set; }
        /// <summary>Type</summary>
        public AssetType? Type { get; set; }
        /// <summary>Identification Code</summary>
        public string? IdentificationCode { get; set; }

        // Novos campos
        /// <summary>Condition</summary>
        public string? Condition { get; set; }
        /// <summary>Purchase Date</summary>
        public DateTime? PurchaseDate { get; set; }
        /// <summary>Location</summary>
        public string? Location { get; set; }
        /// <summary>Responsible</summary>
        public string? Responsible { get; set; }
        /// <summary>Last Maintenance</summary>
        public DateTime? LastMaintenance { get; set; }
        /// <summary>Next Maintenance</summary>
        public DateTime? NextMaintenance { get; set; }
        /// <summary>Warranty Until</summary>
        public DateTime? WarrantyUntil { get; set; }
        /// <summary>Notes</summary>
        public string? Notes { get; set; }
    }

    public class UpdateAssetCommandHandler : IRequestHandler<UpdateAssetCommand, AssetDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateAssetCommandHandler> _logger;
        private readonly IS3Helper _s3Helper;

        public UpdateAssetCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateAssetCommandHandler> logger, IS3Helper s3Helper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _s3Helper = s3Helper;
        }

        public async Task<AssetDto> Handle(UpdateAssetCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando atualização do ativo. AssetId: {AssetId}, UserId: {UserId}", request.AssetId, request.UserId);

            // Busca o membro logado para obter o ChurchId
            var loggedMember = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (loggedMember == null)
            {
                _logger.LogWarning("Membro não encontrado. UserId: {UserId}", request.UserId);
                ValidationException.ThrowException("Member", "This Member does not exist.");
            }

            int churchId = loggedMember.ChurchId;

            // Busca o ativo
            var asset = await _unitOfWork.Assets.Query()
                .FirstOrDefaultAsync(a => a.Id == request.AssetId && a.ChurchId == churchId, cancellationToken);

            if (asset == null)
            {
                _logger.LogWarning("Ativo não encontrado ou não pertence à igreja. AssetId: {AssetId}, ChurchId: {ChurchId}", request.AssetId, churchId);
                ValidationException.ThrowException("Asset", "Asset not found or does not belong to your church.");
            }

            // Validação de duplicidade de código de identificação, se for alterado
            if (!string.IsNullOrEmpty(request.IdentificationCode) && request.IdentificationCode != asset.IdentificationCode)
            {
                var exists = await _unitOfWork.Assets.Query()
                    .AnyAsync(a => a.ChurchId == churchId && a.IdentificationCode == request.IdentificationCode, cancellationToken);

                if (exists)
                {
                    _logger.LogWarning("Código de identificação duplicado para a igreja. IdentificationCode: {IdentificationCode}, ChurchId: {ChurchId}", request.IdentificationCode, churchId);
                    ValidationException.ThrowException("Asset", "An asset with this identification code already exists for this church.");
                }
            }

            // Atualização parcial dos campos
            if (!string.IsNullOrEmpty(request.Name))
                asset.Name = request.Name;

            if (request.Value.HasValue)
                asset.Value = request.Value.Value;

            if (request.Quantity.HasValue)
                asset.Quantity = request.Quantity.Value;

            if (!string.IsNullOrEmpty(request.Description))
                asset.Description = request.Description;

            if (request.Type.HasValue)
                asset.Type = request.Type.Value;

            if (!string.IsNullOrEmpty(request.IdentificationCode))
                asset.IdentificationCode = request.IdentificationCode;

            if (!string.IsNullOrEmpty(request.Condition))
                asset.Condition = request.Condition;

            if (request.PurchaseDate.HasValue)
                asset.PurchaseDate = request.PurchaseDate;

            if (!string.IsNullOrEmpty(request.Location))
                asset.Location = request.Location;

            if (!string.IsNullOrEmpty(request.Responsible))
                asset.Responsible = request.Responsible;

            if (request.LastMaintenance.HasValue)
                asset.LastMaintenance = request.LastMaintenance;

            if (request.NextMaintenance.HasValue)
                asset.NextMaintenance = request.NextMaintenance;

            if (request.WarrantyUntil.HasValue)
                asset.WarrantyUntil = request.WarrantyUntil;

            if (!string.IsNullOrEmpty(request.Notes))
                asset.Notes = request.Notes;

            if (!string.IsNullOrEmpty(request.Photo))
            {
                if (asset.Photo != request.Photo)
                {
                    _logger.LogInformation("Atualizando foto do ativo. AssetId: {AssetId}", asset.Id);
                    asset.Photo = await UploadPhotoAsync(request.Photo, cancellationToken);
                }
                
            }

            _unitOfWork.Assets.Update(asset);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Ativo atualizado com sucesso. AssetId: {AssetId}", asset.Id);

            return AssetDto.New(asset);
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