using MediatR;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;

namespace MyChurch.Application.WorshipService.Commands.PrayerRequest
{
    public class CreatePrayerRequestCommand : JwtMemberDto, IRequest<int>
    {
        public int WorshipServiceId { get; set; }
        public string Request { get; set; }
    }

    public class CreatePrayerRequestCommandHandler : IRequestHandler<CreatePrayerRequestCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreatePrayerRequestCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Handle(CreatePrayerRequestCommand request, CancellationToken cancellationToken)
        {
            var entity = new Domain.Entities.PrayerRequest
            {
                WorshipServiceId = request.WorshipServiceId,
                MemberId = request.UserId,
                Request = request.Request,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };
            await _unitOfWork.PrayerRequests.Create(entity);
            await _unitOfWork.CommitAsync();

            // Notificar admin do culto via SignalR
            

            return entity.Id;
        }
    }
}
