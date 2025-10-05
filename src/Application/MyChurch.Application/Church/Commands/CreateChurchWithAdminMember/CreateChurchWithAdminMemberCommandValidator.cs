using FluentValidation;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Services;
using static MyChurch.Application.Church.Commands.CreateChurchWithAdminMember.CreateChurchWithAdminMemberCommand;

namespace MyChurch.Application.Church.Commands.CreateChurchWithAdminMember
{
    public class CreateChurchWithAdminMemberCommandValidator : AbstractValidator<CreateChurchWithAdminMemberCommand>
    {
        public CreateChurchWithAdminMemberCommandValidator(IDocumentValidator documentValidator)
        {
            // Validações da Igreja
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("O nome da igreja é obrigatório.")
                .MaximumLength(200).WithMessage("O nome da igreja não pode ter mais de 200 caracteres.");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("O telefone da igreja é obrigatório.")
                .MaximumLength(20).WithMessage("O telefone não pode ter mais de 20 caracteres.");

            RuleFor(x => x.Document)
                .NotEmpty().WithMessage("O CNPJ da igreja é obrigatório.");

            RuleFor(x => x.Address)
                .NotNull().WithMessage("O endereço da igreja é obrigatório.");

            // Validações do Admin
            RuleFor(x => x.AdminName)
                .NotEmpty().WithMessage("O nome do administrador é obrigatório.")
                .MaximumLength(100).WithMessage("O nome do administrador não pode ter mais de 100 caracteres.");

            RuleFor(x => x.AdminEmail)
                .EmailAddress().WithMessage("O email do administrador deve ser válido.")
                .When(x => !string.IsNullOrEmpty(x.AdminEmail));

            RuleFor(x => x.AdminPhone)
                .NotEmpty().WithMessage("O telefone do administrador é obrigatório.")
                .MaximumLength(20).WithMessage("O telefone não pode ter mais de 20 caracteres.");

            RuleFor(x => x.AdminBirthDate)
                .LessThan(DateTime.Now).WithMessage("A data de nascimento deve ser no passado.");

            RuleFor(x => x.AdminPassword)
                .NotEmpty().WithMessage("A senha é obrigatória.")
                .MinimumLength(6).WithMessage("A senha deve ter no mínimo 6 caracteres.");

            // Validação para lista de documentos do admin
            RuleFor(x => x.AdminDocuments)
                .NotEmpty().WithMessage("Pelo menos um documento do administrador é obrigatório.")
                .ForEach(docRule =>
                {
                    docRule.SetValidator(new AdminDocumentDtoValidator(documentValidator));
                });
        }
    }

    public class AdminDocumentDtoValidator : AbstractValidator<MemberDocumentDtoCreateChurch>
    {
        public AdminDocumentDtoValidator(IDocumentValidator documentValidator)
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
