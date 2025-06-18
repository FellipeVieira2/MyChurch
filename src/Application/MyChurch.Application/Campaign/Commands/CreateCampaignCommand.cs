using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;

namespace MyChurch.Application.Campaign.Commands
{
    public class CreateCampaignCommand : JwtMemberDto, IRequest<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal GoalAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? CoverImageUrl { get; set; }
    }

    public class CreateCampaignCommandHandler : IRequestHandler<CreateCampaignCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CreateCampaignCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<int> Handle(CreateCampaignCommand request, CancellationToken cancellationToken)
        {
            var member = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            var campaign = new Domain.Entities.Campaign(member.ChurchId, request.Name, request.Description, request.GoalAmount, request.StartDate, request.EndDate, request.CoverImageUrl);
            _unitOfWork.Campaigns.Create(campaign);
            await _unitOfWork.CommitAsync();
            return campaign.Id;
        }
    }
}
