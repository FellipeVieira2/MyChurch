using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using Mychurch.Common.Utils.Objects;

namespace MyChurch.Application.Asset.Queries.GetAllAssets
{
    public class GetAllAssetsQuery : JwtMemberDto, IRequest<PagedResultDto<AssetDto>>
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Value { get; set; }
        public AssetType? Type { get; set; }
        public string? IdentificationCode { get; set; }
        public string? Condition { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public string? Location { get; set; }
        public string? Responsible { get; set; }
        public DateTime? LastMaintenance { get; set; }
        public DateTime? NextMaintenance { get; set; }
        public DateTime? WarrantyUntil { get; set; }
        public string? Notes { get; set; }
         public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GetAllAssetsQueryHandler : IRequestHandler<GetAllAssetsQuery, PagedResultDto<AssetDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllAssetsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResultDto<AssetDto>> Handle(GetAllAssetsQuery request, CancellationToken cancellationToken)
        {
            // Busca o membro logado para obter o ChurchId
            var loggedMember = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (loggedMember == null)
                throw new UnauthorizedAccessException("Usuário não encontrado.");

            int churchId = loggedMember.ChurchId;

            var query = _unitOfWork.Assets.Query().Where(a => a.ChurchId == churchId);

            if (!string.IsNullOrEmpty(request.Name))
                query = query.Where(a => a.Name.Contains(request.Name));

            if (!string.IsNullOrEmpty(request.Description))
                query = query.Where(a => a.Description.Contains(request.Description));

            if (request.Value.HasValue)
                query = query.Where(a => a.Value == request.Value.Value);

            if (request.Type.HasValue)
                query = query.Where(a => a.Type == request.Type.Value);

            if (!string.IsNullOrEmpty(request.IdentificationCode))
                query = query.Where(a => a.IdentificationCode.Contains(request.IdentificationCode));

            if (!string.IsNullOrEmpty(request.Condition))
                query = query.Where(a => a.Condition.Contains(request.Condition));

            if (request.PurchaseDate.HasValue)
                query = query.Where(a => a.PurchaseDate == request.PurchaseDate.Value);

            if (!string.IsNullOrEmpty(request.Location))
                query = query.Where(a => a.Location.Contains(request.Location));

            if (!string.IsNullOrEmpty(request.Responsible))
                query = query.Where(a => a.Responsible.Contains(request.Responsible));

            if (request.LastMaintenance.HasValue)
                query = query.Where(a => a.LastMaintenance == request.LastMaintenance.Value);

            if (request.NextMaintenance.HasValue)
                query = query.Where(a => a.NextMaintenance == request.NextMaintenance.Value);

            if (request.WarrantyUntil.HasValue)
                query = query.Where(a => a.WarrantyUntil == request.WarrantyUntil.Value);

            if (!string.IsNullOrEmpty(request.Notes))
                query = query.Where(a => a.Notes.Contains(request.Notes));

            var total = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(a => a.Name)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResultDto<AssetDto>
            {
                TotalCount = total,
                PageNumber = request.Page,
                PageSize = request.PageSize,
                Items = items.Select(AssetDto.New).ToList()
            };
        }
    }
}
