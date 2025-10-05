using FluentValidation;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Services;
using static MyChurch.Application.Member.Commands.CreateMember.CreateMemberCommand;

namespace MyChurch.Application.Member.Commands.CreateMember
{
    public class CreateMemberCommandValidator : AbstractValidator<CreateMemberCommand>
    {
        public CreateMemberCommandValidator(IDocumentValidator documentValidator)
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("O nome é obrigatório.")
                .MaximumLength(100).WithMessage("O nome não pode ter mais de 100 caracteres.");

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("O email deve ser válido.")
                .When(x => !string.IsNullOrEmpty(x.Email));

            // Validação para lista de documentos
            RuleFor(x => x.Documents)
                .ForEach(docRule =>
                {
                    docRule.SetValidator(new MemberDocumentDtoValidator(documentValidator));
                }).When(x => x.Documents is not null && x.Documents.Any());

            RuleFor(x => x.Phone)
                .MaximumLength(20).WithMessage("O telefone não pode ter mais de 20 caracteres.");

            RuleFor(x => x.BirthDate)
                .LessThan(DateTime.Now).WithMessage("A data de nascimento deve ser no passado.");

            RuleFor(x => x.BaptizedDate)
                .LessThanOrEqualTo(DateTime.Now).WithMessage("A data de batismo deve ser no passado ou presente.");

            RuleFor(x => x.RoleMember)
                .IsInEnum().WithMessage("O papel do usuário deve ser válido.");

            RuleFor(x => x.MaritalStatus)
                .MaximumLength(50).WithMessage("O estado civil não pode ter mais de 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.MaritalStatus));

            RuleFor(x => x.MemberSince)
                .LessThanOrEqualTo(DateTime.Now).WithMessage("A data de entrada deve ser no passado ou presente.")
                .When(x => x.MemberSince.HasValue);

            RuleFor(x => x.Ministry)
                .MaximumLength(100).WithMessage("O ministério não pode ter mais de 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Ministry));

            RuleFor(x => x.Notes)
                .MaximumLength(1000).WithMessage("As observações não podem ter mais de 1000 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Notes));
        }
    }

    public class MemberDocumentDtoValidator : AbstractValidator<MemberDocumentDtoCreate>
    {
        public MemberDocumentDtoValidator(IDocumentValidator documentValidator)
        {
            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("O tipo de documento é inválido.");

            RuleFor(x => x.Number)
                .NotEmpty().WithMessage("O número do documento é obrigatório.")
                .MaximumLength(50).WithMessage("O número do documento não pode ter mais de 50 caracteres.");

            // Validação específica para CPF
            RuleFor(x => x.Number)
                .Must((doc, number) => documentValidator.IsValidCpf(number))
                .WithMessage("O CPF informado é inválido.")
                .When(x => x.Type == MemberDocumentType.CPF);

            // Validação específica para RG
            RuleFor(x => x.Number)
                .Must((doc, number) => documentValidator.IsValidRg(number))
                .WithMessage("O RG informado é inválido.")
                .When(x => x.Type == MemberDocumentType.RG);
        }
    }
}