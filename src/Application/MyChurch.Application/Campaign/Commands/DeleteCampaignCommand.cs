using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;

namespace MyChurch.Application.Campaign.Commands
{
    public class DeleteCampaignCommand : JwtMemberDto, IRequest<bool>
    {
        public int Id { get; set; }
    }

    public class DeleteCampaignCommandHandler : IRequestHandler<DeleteCampaignCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        public DeleteCampaignCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(DeleteCampaignCommand request, CancellationToken cancellationToken)
        {
            var campaign = await _unitOfWork.Campaigns.Query().FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
            if (campaign == null) return false;
            _unitOfWork.Campaigns.Delete(campaign);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
