using FluentValidation;

namespace MyChurch.Application.Asset.Commands.CreateAsset
{
    public class CreateAssetCommandValidator : AbstractValidator<CreateAssetCommand>
    {
        public CreateAssetCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("O nome do ativo é obrigatório.")
                .MaximumLength(100).WithMessage("O nome não pode ter mais de 100 caracteres.");

            RuleFor(x => x.Value)
                .GreaterThan(0).WithMessage("O valor do ativo deve ser maior que zero.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("A descrição é obrigatória.")
                .MaximumLength(500).WithMessage("A descrição não pode ter mais de 500 caracteres.");

            RuleFor(x => x.IdentificationCode)
                .NotEmpty().WithMessage("O código de identificação é obrigatório.")
                .MaximumLength(50).WithMessage("O código de identificação não pode ter mais de 50 caracteres.");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("O tipo do ativo deve ser válido.");

            RuleFor(x => x.Photo)
                .MaximumLength(100000).WithMessage("A foto em base64 não pode exceder 10.000 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Photo));
        }
    }
}
