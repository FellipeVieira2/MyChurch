using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace MyChurch.Application.ChurchPromotion.Commands.TrackPromotionClick
{
    public class TrackChurchPromotionClickCommand : IRequest<bool>
    {
        public int PromotionId { get; set; }
    }

    public class TrackChurchPromotionClickCommandHandler : IRequestHandler<TrackChurchPromotionClickCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public TrackChurchPromotionClickCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(TrackChurchPromotionClickCommand request, CancellationToken cancellationToken)
        {
            var promotion = await _unitOfWork.ChurchPromotions.Query()
                .FirstOrDefaultAsync(p => p.Id == request.PromotionId, cancellationToken);

            if (promotion == null)
                return false;

            promotion.RegisterClick();
            _unitOfWork.ChurchPromotions.Update(promotion);
            await _unitOfWork.CommitAsync();

            return true;
        }
    }
}
