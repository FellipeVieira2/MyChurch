using FluentValidation;
using MyChurch.Application.Event.Commands.CreateEvent;
using MyChurch.Domain.Enum;

namespace MyChurch.Application.Event.Commands.CreateEvent
{
    public class CreateEventCommandValidator : AbstractValidator<CreateEventCommand>
    {
        public CreateEventCommandValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("O título é obrigatório.")
                .MaximumLength(200).WithMessage("O título deve ter no máximo 200 caracteres.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("A descrição é obrigatória.");

            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("A data de início é obrigatória.");

            RuleFor(x => x.FinishDate)
                .NotEmpty().WithMessage("A data de término é obrigatória.")
                .GreaterThanOrEqualTo(x => x.Date).WithMessage("A data de término deve ser igual ou posterior à data de início.");

            RuleFor(x => x.Location)
                .NotEmpty().WithMessage("O local é obrigatório.")
                .MaximumLength(200).WithMessage("O local deve ter no máximo 200 caracteres.");
            // Validação de recorrência
            When(x => x.RecurrenceType.HasValue && x.RecurrenceType != EventRecurrenceType.None, () =>
            {
                RuleFor(x => x.Frequency)
                    .NotNull().WithMessage("A frequência é obrigatória para eventos recorrentes.")
                    .GreaterThan(0).WithMessage("A frequência deve ser maior que zero.");
            });
        }
    }
}
