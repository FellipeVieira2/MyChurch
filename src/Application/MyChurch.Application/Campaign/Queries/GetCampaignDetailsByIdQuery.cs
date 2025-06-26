using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Contracts;
using MyChurch.Application.Dtos;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace MyChurch.Application.Campaign.Queries
{
    public class GetCampaignDetailsByIdQuery : JwtMemberDto, IRequest<CampaignDetailsDto>
    {
        [JsonIgnore]
        public int Id { get; set; }
    }

    public class CampaignDetailsDto
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
        public decimal PercentComplete { get; set; } // Percentual da campanha
    }

    public class GetCampaignDetailsByIdQueryHandler : IRequestHandler<GetCampaignDetailsByIdQuery, CampaignDetailsDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetCampaignDetailsByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<CampaignDetailsDto> Handle(GetCampaignDetailsByIdQuery request, CancellationToken cancellationToken)
        {
            // Busca o membro logado
            var member = await _unitOfWork.Members.Query().FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);
            if (member == null)
                return null;
            int churchId = member.ChurchId;
            var campaign = await _unitOfWork.Campaigns.Query()
                .Where(c => c.Id == request.Id && c.ChurchId == churchId)
                .Select(c => new CampaignDetailsDto
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
                    IsCompleted = c.IsCompleted,
                    PercentComplete = c.GoalAmount > 0 ? (c.AmountRaised / c.GoalAmount) * 100 : 0
                })
                .FirstOrDefaultAsync(cancellationToken);
            return campaign;
        }
    }
}
