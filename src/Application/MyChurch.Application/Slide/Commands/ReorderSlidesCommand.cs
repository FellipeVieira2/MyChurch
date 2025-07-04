using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyChurch.Application.Slide.Commands
{
    public class ReorderSlidesCommand : IRequest<Unit>
    {
        public int PresentationId { get; set; }
        public List<int> OrderedSlideIds { get; set; }
    }

    public class ReorderSlidesCommandHandler : IRequestHandler<ReorderSlidesCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReorderSlidesCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(ReorderSlidesCommand request, CancellationToken cancellationToken)
        {
            var slides = await _unitOfWork.Slides.Query()
                .Where(s => s.PresentationId == request.PresentationId)
                .ToListAsync(cancellationToken);

            for (int i = 0; i < request.OrderedSlideIds.Count; i++)
            {
                var slide = slides.FirstOrDefault(s => s.Id == request.OrderedSlideIds[i]);
                if (slide != null)
                {
                    slide.OrderIndex = i;
                    _unitOfWork.Slides.Update(slide);
                }
            }
            await _unitOfWork.CommitAsync();
            return Unit.Value;
        }
    }
}