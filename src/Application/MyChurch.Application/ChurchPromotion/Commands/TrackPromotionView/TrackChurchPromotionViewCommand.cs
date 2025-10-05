using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;
using System.Threading;
using System.Threading.Tasks;

namespace MyChurch.Application.ChurchPromotion.Commands.TrackPromotionView
{
    public class TrackChurchPromotionViewCommand : IRequest<bool>
    {
        public int PromotionId { get; set; }
    }

    public class TrackChurchPromotionViewCommandHandler : IRequestHandler<TrackChurchPromotionViewCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public TrackChurchPromotionViewCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(TrackChurchPromotionViewCommand request, CancellationToken cancellationToken)
        {
            var promotion = await _unitOfWork.ChurchPromotions.Query()
                .FirstOrDefaultAsync(p => p.Id == request.PromotionId, cancellationToken);

            if (promotion == null)
                return false;

            promotion.RegisterView();
            _unitOfWork.ChurchPromotions.Update(promotion);
            await _unitOfWork.CommitAsync();

            return true;
        }
    }
}
