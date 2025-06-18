using MediatR;
using Microsoft.EntityFrameworkCore;
using Mychurch.Common.Utils.Objects;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;

namespace MyChurch.Application.Campaign.Queries
{
    public class GetAllActiveCampaignsQuery : JwtMemberDto, IRequest<PagedResultDto<CampaignDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class CampaignDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal GoalAmount { get; set; }
        public decimal AmountRaised { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
        public string? CoverImageUrl { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class GetAllActiveCampaignsQueryHandler : IRequestHandler<GetAllActiveCampaignsQuery, PagedResultDto<CampaignDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAllActiveCampaignsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<PagedResultDto<CampaignDto>> Handle(GetAllActiveCampaignsQuery request, CancellationToken cancellationToken)
        {
            var member = await _unitOfWork.Members.Query().FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);
            if (member == null)
                return new PagedResultDto<CampaignDto>(new List<CampaignDto>(), request.PageNumber, request.PageSize, 0);
            int churchId = member.ChurchId;
            var query = _unitOfWork.Campaigns.Query()
                .Where(c => c.ChurchId == churchId && c.IsActive);

            int totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderByDescending(c => c.StartDate)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new CampaignDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    GoalAmount = c.GoalAmount,
                    AmountRaised = c.AmountRaised,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    IsActive = c.IsActive,
                    CoverImageUrl = c.CoverImageUrl,
                    IsCompleted = c.IsCompleted
                })
                .ToListAsync(cancellationToken);
            return new PagedResultDto<CampaignDto>(items, request.PageNumber, request.PageSize, totalCount);
        }
    }
}
