using MediatR;
using MyChurch.Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using System.Threading;
using System.Threading.Tasks;

namespace MyChurch.Application.LivePresentation.Commands
{
    public class EndLivePresentationCommand : JwtMemberDto, IRequest<Unit>
    {
        public int PresentationId { get; set; }
    }

    public class EndLivePresentationCommandHandler : IRequestHandler<EndLivePresentationCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EndLivePresentationCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(EndLivePresentationCommand request, CancellationToken cancellationToken)
        {
            var member = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);
            if (member == null)
                throw new System.Exception("Usuário não encontrado para obter ChurchId.");

            var presentation = await _unitOfWork.Presentations.Query()
                .FirstOrDefaultAsync(p => p.Id == request.PresentationId && p.ChurchId == member.ChurchId, cancellationToken);
            if (presentation != null)
            {
                presentation.IsLive = false;
                _unitOfWork.Presentations.Update(presentation);
                await _unitOfWork.CommitAsync();
            }
            return Unit.Value;
        }
    }
}