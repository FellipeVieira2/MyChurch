using System.Text.Json.Serialization;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Asset.Queries.GetAssetById
{
    public class GetAssetByIdQuery : JwtMemberDto, IRequest<AssetDto>
    {
        [JsonIgnore]
        public int Id { get; set; }
    }
    public class GetAssetByIdQueryHandler : IRequestHandler<GetAssetByIdQuery, AssetDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAssetByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AssetDto> Handle(GetAssetByIdQuery request, CancellationToken cancellationToken)
        {
            var loggedMember = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (loggedMember == null)
                ValidationException.ThrowException("Member", "This Member does not exist.");

            int churchId = loggedMember.ChurchId;

            var asset = await _unitOfWork.Assets.Query()
                .FirstOrDefaultAsync(a => a.Id == request.Id && a.ChurchId == churchId, cancellationToken);

            if (asset == null)
                ValidationException.ThrowException("Asset", "Asset not found or does not belong to your church.");

            return AssetDto.New(asset);
        }
    }
}