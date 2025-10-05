using FluentValidation;
using MyChurch.Application.Church.Commands.CreateChurchSchedule;

namespace MyChurch.Application.Validators
{
    public class CreateChurchScheduleCommandValidator : AbstractValidator<CreateChurchScheduleCommand>
    {
        public CreateChurchScheduleCommandValidator()
        {
            RuleFor(x => x.ChurchId)
                .GreaterThan(0)
                .WithMessage("ID da igreja é obrigatório");

            RuleFor(x => x.DayOfWeek)
                .IsInEnum()
                .WithMessage("Dia da semana inválido");

            RuleFor(x => x.StartTime)
                .NotEmpty()
                .WithMessage("Hora de início é obrigatória")
                .Matches(@"^([01]?[0-9]|2[0-3]):[0-5][0-9]$")
                .WithMessage("Hora de início deve estar no formato HH:mm (ex: 19:00)");

            RuleFor(x => x.EndTime)
                .Matches(@"^([01]?[0-9]|2[0-3]):[0-5][0-9]$")
                .When(x => !string.IsNullOrEmpty(x.EndTime))
                .WithMessage("Hora de término deve estar no formato HH:mm (ex: 21:00)");

            RuleFor(x => x.ServiceType)
                .NotEmpty()
                .WithMessage("Tipo de serviço é obrigatório")
                .MaximumLength(100)
                .WithMessage("Tipo de serviço deve ter no máximo 100 caracteres");

            RuleFor(x => x.Description)
                .MaximumLength(500)
                .When(x => !string.IsNullOrEmpty(x.Description))
                .WithMessage("Descrição deve ter no máximo 500 caracteres");
        }
    }
}
