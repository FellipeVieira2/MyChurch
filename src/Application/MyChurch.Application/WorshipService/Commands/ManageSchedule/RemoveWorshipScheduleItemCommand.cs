using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Exceptions;
using System.Threading;
using System.Threading.Tasks;

namespace MyChurch.Application.WorshipService.Commands.ManageSchedule
{
    public class RemoveWorshipScheduleItemCommand : JwtMemberDto, IRequest<bool>
    {
        public int Id { get; set; }
        public int WorshipServiceId { get; set; }
    }

    public class RemoveWorshipScheduleItemCommandHandler : IRequestHandler<RemoveWorshipScheduleItemCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        public RemoveWorshipScheduleItemCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
        public async Task<bool> Handle(RemoveWorshipScheduleItemCommand request, CancellationToken cancellationToken)
        {
            var item = await _unitOfWork.WorshipSchedules.Query()
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.WorshipServiceId == request.WorshipServiceId, cancellationToken);
            if (item == null)
                ValidationException.ThrowException("WorshipScheduleItem", "Item não encontrado.");
            _unitOfWork.WorshipSchedules.Delete(item);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
