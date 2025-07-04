using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace MyChurch.Application.Slide.Commands
{
    public class DeleteSlideCommand : IRequest<Unit>
    {
        public int Id { get; set; }
    }

    public class DeleteSlideCommandHandler : IRequestHandler<DeleteSlideCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteSlideCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeleteSlideCommand request, CancellationToken cancellationToken)
        {
            var slide = await _unitOfWork.Slides.Query().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
            if (slide != null)
            {
                _unitOfWork.Slides.Delete(slide);
                await _unitOfWork.CommitAsync();
            }
            return Unit.Value;
        }
    }
}