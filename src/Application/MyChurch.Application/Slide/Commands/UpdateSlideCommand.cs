using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace MyChurch.Application.Slide.Commands
{
    public class UpdateSlideCommand : IRequest<Unit>
    {
        public int Id { get; set; }
        public string ContentReferenceJson { get; set; }
    }

    public class UpdateSlideCommandHandler : IRequestHandler<UpdateSlideCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateSlideCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(UpdateSlideCommand request, CancellationToken cancellationToken)
        {
            var slide = await _unitOfWork.Slides.Query().FirstOrDefaultAsync(x => x.Id == request.Id);
            if (slide != null)
            {
                slide.ContentReferenceJson = request.ContentReferenceJson;
                _unitOfWork.Slides.Update(slide);
                await _unitOfWork.CommitAsync();
            }
            return Unit.Value;
        }
    }
}