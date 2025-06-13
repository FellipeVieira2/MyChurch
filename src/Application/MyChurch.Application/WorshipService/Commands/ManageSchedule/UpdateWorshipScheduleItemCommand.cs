using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Exceptions;
using System.Threading;
using System.Threading.Tasks;

namespace MyChurch.Application.WorshipService.Commands.ManageSchedule
{
    public class UpdateWorshipScheduleItemCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public int WorshipServiceId { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
    }

    public class UpdateWorshipScheduleItemCommandHandler : IRequestHandler<UpdateWorshipScheduleItemCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        public UpdateWorshipScheduleItemCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
        public async Task<bool> Handle(UpdateWorshipScheduleItemCommand request, CancellationToken cancellationToken)
        {
            var item = await _unitOfWork.WorshipSchedules.Query()
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.WorshipServiceId == request.WorshipServiceId, cancellationToken);
            if (item == null)
                ValidationException.ThrowException("WorshipScheduleItem", "Item não encontrado.");
            item.Name = request.Name;
            item.Order = request.Order;
            _unitOfWork.WorshipSchedules.Update(item);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
