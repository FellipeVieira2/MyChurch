using MediatR;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Services;

namespace MyChurch.Application.Onboarding.IdentifyMember
{
    public class IdentifyMemberCommand : IRequest<IdentifyMemberResultDto>
    {
        public string Identifier { get; set; } // CPF, Email ou Telefone
        public string ChurchId { get; set; }

        public class Handler : IRequestHandler<IdentifyMemberCommand, IdentifyMemberResultDto>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IDocumentValidator _documentValidator;

            public Handler(IUnitOfWork unitOfWork, IDocumentValidator documentValidator)
            {
                _unitOfWork = unitOfWork;
                _documentValidator = documentValidator;
            }

            public async Task<IdentifyMemberResultDto> Handle(IdentifyMemberCommand request, CancellationToken cancellationToken)
            {
                // ?? Normalizar identifier (caso seja CPF)
                var normalizedIdentifier = _documentValidator.RemoveFormatting(request.Identifier);
                var originalIdentifier = request.Identifier;
                
                var member = _unitOfWork.Members.Query()
                    .FirstOrDefault(m => (m.Documents.Any(d => d.Number == normalizedIdentifier) 
                                       || m.Phone == originalIdentifier 
                                       || m.Email.ToLower() == originalIdentifier.ToLower()) 
                                       && m.ChurchId.ToString() == request.ChurchId);

                if (member == null)
                {
                    return new IdentifyMemberResultDto { Status = "NotFound" };
                }
                
                // ? Se ainda está pendente de aprovação do admin
                if (member.PendingApproval)
                {
                    return new IdentifyMemberResultDto { Status = "PendingApproval" };
                }
                
                // ? Se foi aprovado e está ativo, pode fazer login
                if (!member.PendingApproval && member.IsActive)
                {
                    return new IdentifyMemberResultDto { Status = "AlreadyActive" };
                }
                
                // ? Cadastro foi rejeitado
                string maskedName = MaskName(member.Name);
                return new IdentifyMemberResultDto { Status = "Rejected", MaskedName = maskedName };
            }

            private string MaskName(string name)
            {
                if (string.IsNullOrWhiteSpace(name)) return string.Empty;
                var parts = name.Split(' ');
                if (parts.Length == 1) return parts[0][0] + ".";
                var masked = parts[0];
                for (int i = 1; i < parts.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(parts[i]))
                        masked += " " + parts[i][0] + ".";
                }
                return masked;
            }
        }
    }
}