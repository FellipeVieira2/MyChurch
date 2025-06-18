using MediatR;
using MyChurch.Domain.Contracts;

namespace MyChurch.Application.Member.Commands.ApproveMemberRegistration
{
    public class ApproveMemberRegistrationCommand : IRequest
    {
        public string MemberId { get; set; }

        public class Handler : IRequestHandler<ApproveMemberRegistrationCommand>
        {
            private readonly IUnitOfWork _unitOfWork;

            public Handler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task Handle(ApproveMemberRegistrationCommand request, CancellationToken cancellationToken)
            {
                var member = _unitOfWork.Members.Query().FirstOrDefault(m => m.Id.ToString() == request.MemberId);
                if (member == null)
                {
                    throw new MyChurch.Domain.Exceptions.ValidationException("Membro não encontrado.");
                }
                member.ApproveRegistration();
                _unitOfWork.Members.Update(member);
                await _unitOfWork.CommitAsync();
            }
        }
    }
}