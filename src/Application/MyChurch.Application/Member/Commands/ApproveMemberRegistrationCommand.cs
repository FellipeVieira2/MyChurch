using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
                
                // ? Aprovar e ATIVAR a conta
                member.PendingApproval = false;
                member.IsActive = true; // ?? Conta ativa após aprovação!
                
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
            private readonly ILogger<Handler> _logger;

            public Handler(IUnitOfWork unitOfWork, ILogger<Handler> logger)
            {
                _unitOfWork = unitOfWork;
                _logger = logger;
            }

            public async Task Handle(DeclineMemberRegistrationCommand request, CancellationToken cancellationToken)
            {
                var member = await _unitOfWork.Members.Query()
                    .Include(m => m.Documents)
                    .Include(m => m.Address)
                    .FirstOrDefaultAsync(m => m.Id.ToString() == request.MemberId && m.PendingApproval, cancellationToken);
                
                if (member == null)
                {
                    ValidationException.ThrowException("Decline","Membro não encontrado ou já processado.");
                }
                
                _logger.LogWarning(
                    "Cadastro rejeitado e deletado - MemberId: {MemberId}, Nome: {Name}, Motivo: {Reason}",
                    member.Id, member.Name, request.Reason ?? "Não especificado");
                
                // ??? HARD DELETE: Remove completamente do banco
                // Remove documentos primeiro (dependência)
                if (member.Documents != null && member.Documents.Any())
                {
                    foreach (var doc in member.Documents.ToList())
                    {
                        _unitOfWork.MemberDocuments.Delete(doc);
                    }
                }
                
                // Remove o membro
                _unitOfWork.Members.Delete(member);
                await _unitOfWork.CommitAsync();
                
                _logger.LogInformation("Membro {MemberId} deletado permanentemente após rejeição", request.MemberId);
            }
        }
    }
}