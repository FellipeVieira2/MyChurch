using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace MyChurch.Application.Presentation.Commands
{
    public class DeletePresentationCommand : IRequest<Unit>
    {
        public int Id { get; set; }
    }

    public class DeletePresentationCommandHandler : IRequestHandler<DeletePresentationCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeletePresentationCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeletePresentationCommand request, CancellationToken cancellationToken)
        {
            var presentation = await _unitOfWork.Presentations.Query().FirstOrDefaultAsync(x => x.Id == request.Id);
            if (presentation != null)
            {
                _unitOfWork.Presentations.Delete(presentation);
                await _unitOfWork.CommitAsync();
            }
            return Unit.Value;
        }
    }
}