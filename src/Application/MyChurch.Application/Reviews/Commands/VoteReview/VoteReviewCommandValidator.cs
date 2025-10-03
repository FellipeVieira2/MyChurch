using FluentValidation;

namespace MyChurch.Application.Reviews.Commands.VoteReview
{
    public class VoteReviewCommandValidator : AbstractValidator<VoteReviewCommand>
    {
        public VoteReviewCommandValidator()
        {
            RuleFor(x => x.ReviewId)
                .GreaterThan(0)
                .WithMessage("ID da review inválido");

            RuleFor(x => x.UserId)
                .GreaterThan(0)
                .WithMessage("Usuário não autenticado");
        }
    }
}
