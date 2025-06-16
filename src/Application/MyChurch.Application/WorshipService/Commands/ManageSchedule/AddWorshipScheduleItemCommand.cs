using MediatR;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Application.WorshipService.Commands.ManageSchedule
{
    public class AddWorshipScheduleItemCommand : IRequest<int>
    {
        public int WorshipServiceId { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
    }

    public class AddWorshipScheduleItemCommandHandler : IRequestHandler<AddWorshipScheduleItemCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        public AddWorshipScheduleItemCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Handle(AddWorshipScheduleItemCommand request, CancellationToken cancellationToken)
        {
            var item = new WorshipScheduleItem
            {
                WorshipServiceId = request.WorshipServiceId,
                Name = request.Name,
                Order = request.Order
            };
            _unitOfWork.WorshipSchedules.Create(item);
            await _unitOfWork.CommitAsync();
            return item.Id;
        }
    }
}
