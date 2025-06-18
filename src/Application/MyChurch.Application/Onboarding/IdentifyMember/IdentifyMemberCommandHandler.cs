using MediatR;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Application.Onboarding.IdentifyMember
{
    public class IdentifyMemberCommandHandler : IRequestHandler<IdentifyMemberCommand, IdentifyMemberResultDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public IdentifyMemberCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IdentifyMemberResultDto> Handle(IdentifyMemberCommand request, CancellationToken cancellationToken)
        {
            var member = _unitOfWork.Members.Query()
                .FirstOrDefault(m => m.Documents.Any(d => d.Number == request.Identifier) && m.ChurchId.ToString() == request.ChurchId);

            if (member == null)
            {
                return new IdentifyMemberResultDto { Status = "NotFound" };
            }
            if (member.IsActive)
            {
                return new IdentifyMemberResultDto { Status = "AlreadyActive" };
            }
            // Ofusca o nome: Exemplo "Fellipe V. S."
            string maskedName = MaskName(member.Name);
            return new IdentifyMemberResultDto { Status = "ActivationRequired", MaskedName = maskedName };
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