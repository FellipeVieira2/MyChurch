using MediatR;
using FluentValidation;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;
using MyChurch.Domain.Entities;

namespace MyChurch.Application.Onboarding.ActivateAccount
{
    // Comando de validação para ativação de conta
    public class ValidateMemberForActivationCommand : IRequest<string> // retorna o hash/token
    {
        public string Identifier { get; set; } // CPF
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

            public Handler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<string> Handle(ValidateMemberForActivationCommand request, CancellationToken cancellationToken)
            {
                // ?? SEGURANÇA: Verificar se conta ainda não foi ativada (PasswordHash vazio ou token temporário)
                var member = _unitOfWork.Members.Query()
                    .FirstOrDefault(m => (m.Documents.Any(d => d.Number == request.Identifier) 
                                       || m.Phone == request.Identifier 
                                       || m.Email.ToLower() == request.Identifier.ToLower()) 
                                       && string.IsNullOrEmpty(m.PasswordHash));
                
                if (member == null)
                {
                    MyChurch.Domain.Exceptions.ValidationException.ThrowException("Member","Membro não encontrado ou já está ativo.");
                }
                
                if (member.BirthDate != request.BirthDate)
                {
                    MyChurch.Domain.Exceptions.ValidationException.ThrowException("Member","Data de nascimento inválida.");
                }
                
                // Gerar novo token de ativação se necessário
                if (string.IsNullOrEmpty(member.PasswordHash))
                {
                    member.PasswordHash = Guid.NewGuid().ToString("N");
                    _unitOfWork.Members.Update(member);
                    await _unitOfWork.CommitAsync();
                }
                
                // Retorna o hash/token para ativação de senha
                return member.PasswordHash;
            }
        }
    }
}