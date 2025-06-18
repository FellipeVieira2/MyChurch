using MediatR;
using FluentValidation;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;
using MyChurch.Domain.Entities;

namespace MyChurch.Application.Onboarding.ActivateAccount
{
    // Novo comando: apenas validação de CPF + data de nascimento
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
                var member = _unitOfWork.Members.Query()
                    .FirstOrDefault(m => m.Documents.Any(d => d.Number == request.Identifier) && !m.IsActive);
                if (member == null)
                {
                    throw new MyChurch.Domain.Exceptions.ValidationException("Membro não encontrado ou já está ativo.");
                }
                if (member.BirthDate != request.BirthDate)
                {
                    throw new MyChurch.Domain.Exceptions.ValidationException("Data de nascimento inválida.");
                }
                // Retorna o hash/token para ativação de senha
                return member.PasswordHash;
            }
        }
    }

    // Remove o comando antigo de ativação de conta (agora o fluxo é separado)
}