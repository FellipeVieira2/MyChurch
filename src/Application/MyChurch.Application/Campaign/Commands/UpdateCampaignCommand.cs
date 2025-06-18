using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Contracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MyChurch.Application.Campaign.Commands
{
    public class UpdateCampaignCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal GoalAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? CoverImageUrl { get; set; }
    }

    public class UpdateCampaignCommandHandler : IRequestHandler<UpdateCampaignCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        public UpdateCampaignCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(UpdateCampaignCommand request, CancellationToken cancellationToken)
        {
            var campaign = await _unitOfWork.Campaigns.Query().FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
            if (campaign == null) return false;
            campaign.UpdateDetails(request.Name, request.Description, request.GoalAmount, request.StartDate, request.EndDate, request.CoverImageUrl);
            _unitOfWork.Campaigns.Update(campaign);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
