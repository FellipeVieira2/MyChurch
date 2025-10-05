using FluentValidation;
using MyChurch.Application.Church.Commands.UploadChurchPhoto;

namespace MyChurch.Application.Validators
{
    public class UploadChurchPhotoCommandValidator : AbstractValidator<UploadChurchPhotoCommand>
    {
        public UploadChurchPhotoCommandValidator()
        {
            RuleFor(x => x.ChurchId)
                .GreaterThan(0)
                .WithMessage("ID da igreja é obrigatório");

            RuleFor(x => x.PhotoBase64)
                .NotEmpty()
                .WithMessage("Foto é obrigatória")
                .Must(BeValidBase64)
                .WithMessage("Formato de foto inválido");

            RuleFor(x => x.Category)
                .IsInEnum()
                .WithMessage("Categoria inválida");

            RuleFor(x => x.Caption)
                .MaximumLength(500)
                .When(x => !string.IsNullOrEmpty(x.Caption))
                .WithMessage("Legenda deve ter no máximo 500 caracteres");
        }

        private bool BeValidBase64(string base64)
        {
            if (string.IsNullOrEmpty(base64))
                return false;

            try
            {
                var data = base64.Contains(',') ? base64.Split(',')[1] : base64;
                Convert.FromBase64String(data);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
