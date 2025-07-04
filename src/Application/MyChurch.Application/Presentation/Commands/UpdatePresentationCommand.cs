using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace MyChurch.Application.Presentation.Commands
{
    public class UpdatePresentationCommand : IRequest<Unit>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class UpdatePresentationCommandHandler : IRequestHandler<UpdatePresentationCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdatePresentationCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(UpdatePresentationCommand request, CancellationToken cancellationToken)
        {
            var presentation = await _unitOfWork.Presentations.Query().FirstOrDefaultAsync(x => x.Id == request.Id);
            if (presentation != null)
            {
                presentation.Name = request.Name;
                presentation.Description = request.Description;
                _unitOfWork.Presentations.Update(presentation);
                await _unitOfWork.CommitAsync();
            }
            return Unit.Value;
        }
    }
}