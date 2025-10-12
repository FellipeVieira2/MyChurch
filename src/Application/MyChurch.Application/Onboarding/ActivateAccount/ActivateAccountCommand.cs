using MediatR;
using FluentValidation;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Services;

namespace MyChurch.Application.Onboarding.ActivateAccount
{
    // Comando de validação para ativação de conta
    public class ValidateMemberForActivationCommand : IRequest<string> // retorna o hash/token
    {
        public string Identifier { get; set; } // CPF, Email ou Telefone
        public DateTime BirthDate { get; set; }

        public class Validator : AbstractValidator<ValidateMemberForActivationCommand>
        {
            public Validator()
            {
                RuleFor(x => x.Identifier).NotEmpty();
                RuleFor(x => x.BirthDate).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<ValidateMemberForActivationCommand, string>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IDocumentValidator _documentValidator;

            public Handler(IUnitOfWork unitOfWork, IDocumentValidator documentValidator)
            {
                _unitOfWork = unitOfWork;
                _documentValidator = documentValidator;
            }

            public async Task<string> Handle(ValidateMemberForActivationCommand request, CancellationToken cancellationToken)
            {
                // ?? Normalizar identifier (caso seja CPF)
                var normalizedIdentifier = _documentValidator.RemoveFormatting(request.Identifier);
                var originalIdentifier = request.Identifier;
                
                // ?? SEGURANÇA: Buscar membro que foi aprovado (!PendingApproval) mas ainda não tem senha
                var member = _unitOfWork.Members.Query()
                    .FirstOrDefault(m => (m.Documents.Any(d => d.Number == normalizedIdentifier) 
                                       || m.Phone == originalIdentifier 
                                       || m.Email.ToLower() == originalIdentifier.ToLower()) 
                                       && !m.PendingApproval);
                
                if (member == null)
                {
                    MyChurch.Domain.Exceptions.ValidationException.ThrowException("Member","Membro não encontrado ou ainda não foi aprovado pelo administrador.");
                }

                // Verificar se já tem senha ativa (formato BCrypt)
                bool hasPassword = !string.IsNullOrEmpty(member.PasswordHash) 
                                && member.PasswordHash.StartsWith("$2");
                
                if (hasPassword)
                {
                    MyChurch.Domain.Exceptions.ValidationException.ThrowException("Member","Esta conta já está ativa.");
                }
                
                if (member.BirthDate.Date != request.BirthDate.Date)
                {
                    MyChurch.Domain.Exceptions.ValidationException.ThrowException("Member","Data de nascimento inválida.");
                }
                
                // Gerar token de ativação temporário
                var activationToken = Guid.NewGuid().ToString("N");
                member.PasswordHash = activationToken;
                _unitOfWork.Members.Update(member);
                await _unitOfWork.CommitAsync();
                
                // Retorna o hash/token para ativação de senha
                return activationToken;
            }
        }
    }
}