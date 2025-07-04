using MediatR;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;
using System.Text.Json.Serialization;

namespace MyChurch.Application.Member.Commands.ApproveMemberRegistration
{
    public class ApproveMemberRegistrationCommand : IRequest
    {
        [JsonIgnore]
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
                var member = _unitOfWork.Members.Query().FirstOrDefault(m => m.Id.ToString() == request.MemberId && m.PendingApproval);
                if (member == null)
                {
                    ValidationException.ThrowException("Approval","Membro não encontrado.");
                }
                member.ApproveRegistration();
                _unitOfWork.Members.Update(member);
                await _unitOfWork.CommitAsync();
            }
        }
    }

    public class DeclineMemberRegistrationCommand : IRequest
    {
        [JsonIgnore]
        public string MemberId { get; set; }
        public string? Reason { get; set; }

        public class Handler : IRequestHandler<DeclineMemberRegistrationCommand>
        {
            private readonly IUnitOfWork _unitOfWork;

            public Handler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task Handle(DeclineMemberRegistrationCommand request, CancellationToken cancellationToken)
            {
                var member = _unitOfWork.Members.Query().FirstOrDefault(m => m.Id.ToString() == request.MemberId && m.PendingApproval);
                if (member == null)
                {
                    ValidationException.ThrowException("Decline","Membro não encontrado.");
                }
                // Opcional: salvar motivo da recusa em um campo ou log
                member.PendingApproval = false;
                member.IsActive = false;
                _unitOfWork.Members.Update(member);
                await _unitOfWork.CommitAsync();
            }
        }
    }
}