using FluentValidation;

namespace MyChurch.Application.Asset.Commands.UpdateAsset
{
    public class UpdateAssetCommandValidator : AbstractValidator<UpdateAssetCommand>
    {
        public UpdateAssetCommandValidator()
        {
            RuleFor(x => x.AssetId)
                .GreaterThan(0).WithMessage("O ID do ativo é obrigatório.");

            RuleFor(x => x.Name)
                .MaximumLength(100).WithMessage("O nome não pode ter mais de 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Name));

            RuleFor(x => x.Value)
                .GreaterThan(0).WithMessage("O valor do ativo deve ser maior que zero.")
                .When(x => x.Value.HasValue);

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("A descrição não pode ter mais de 500 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.IdentificationCode)
                .MaximumLength(50).WithMessage("O código de identificação não pode ter mais de 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.IdentificationCode));

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("O tipo do ativo deve ser válido.")
                .When(x => x.Type.HasValue);

            RuleFor(x => x.Photo)
                .MaximumLength(10000).WithMessage("A foto em base64 não pode exceder 10.000 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Photo));
        }
    }
}