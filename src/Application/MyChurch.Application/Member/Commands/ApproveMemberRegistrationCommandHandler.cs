using MediatR;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;
using MyChurch.Domain.Entities;

namespace MyChurch.Application.Member.Commands.ApproveMemberRegistration
{
    public class ApproveMemberRegistrationCommandHandler : IRequestHandler<ApproveMemberRegistrationCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ApproveMemberRegistrationCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(ApproveMemberRegistrationCommand request, CancellationToken cancellationToken)
        {
            var member = _unitOfWork.Members.Query().FirstOrDefault(m => m.Id.ToString() == request.MemberId);
            if (member == null)
            {
                throw new ValidationException("Membro não encontrado.");
            }
            member.ApproveRegistration();
            _unitOfWork.Members.Update(member);
            await _unitOfWork.CommitAsync();
        }
    }
}