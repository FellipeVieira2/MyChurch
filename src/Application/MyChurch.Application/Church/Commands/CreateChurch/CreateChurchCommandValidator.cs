using FluentValidation;
using MyChurch.Domain.Entities;

namespace MyChurch.Application.Church.Commands.CreateChurchCommand
{
    public class CreateChurchCommandValidator : AbstractValidator<CreateChurchCommand>
    {
        public CreateChurchCommandValidator()
        {
            RuleFor(c => c.Name)
                .NotEmpty();

            RuleFor(c => c.Address)
                .NotEmpty();

            RuleFor(c => c.Phone)
                .NotEmpty();
        }
    }
}
