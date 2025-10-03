using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;
using MyChurch.Application.Dtos; // added

namespace MyChurch.Application.Visitors.Commands
{
    public class UpdateVisitorStatusCommand : JwtMemberDto, IRequest<bool>
    {
        public int VisitorId { get; set; }
        public VisitorStatus NewStatus { get; set; }
        public string? Note { get; set; }
    }

    public class UpdateVisitorStatusCommandHandler : IRequestHandler<UpdateVisitorStatusCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        public UpdateVisitorStatusCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(UpdateVisitorStatusCommand request, CancellationToken cancellationToken)
        {
            var visitor = await _unitOfWork.Visitors.Query().FirstOrDefaultAsync(v => v.Id == request.VisitorId, cancellationToken);
            if (visitor == null)
                ValidationException.ThrowException("Visitor", "Visitante não encontrado");

            var old = visitor.Status ?? VisitorStatus.New;
            if (old == request.NewStatus) return true;

            visitor.Status = request.NewStatus;
            _unitOfWork.Visitors.Update(visitor);
            _unitOfWork.VisitorStatusHistories.Create(new Domain.Entities.VisitorStatusHistory
            {
                VisitorId = visitor.Id,
                OldStatus = old,
                NewStatus = request.NewStatus,
                ChangedByMemberId = request.UserId,
                Note = request.Note,
                ChangedAt = DateTime.UtcNow
            });
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
