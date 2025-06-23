using MediatR;
using Microsoft.Extensions.Logging;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.PreLaunch.Commands.ConfirmPreLaunchInterest
{
    public class ConfirmPreLaunchInterestCommand : IRequest<bool>
    {
        public string Token { get; set; }
    }

    public class ConfirmPreLaunchInterestCommandHandler : IRequestHandler<ConfirmPreLaunchInterestCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ConfirmPreLaunchInterestCommandHandler> _logger;

        public ConfirmPreLaunchInterestCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<ConfirmPreLaunchInterestCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(ConfirmPreLaunchInterestCommand request, CancellationToken cancellationToken)
        {
            // Validate token
            if (string.IsNullOrWhiteSpace(request.Token))
            {
                ValidationException.ThrowException("Token", "Token de confirmação inválido.");
            }

            // Find interest by token
            var interest = await _unitOfWork.PreLaunchInterests.GetByConfirmationTokenAsync(request.Token, cancellationToken);
            if (interest == null)
            {
                _logger.LogWarning("Invalid confirmation token: {Token}", request.Token);
                ValidationException.ThrowException("Token","O token de confirmação é inválido ou expirou.");
            }

            // Check if already confirmed
            if (interest.IsEmailConfirmed)
            {
                _logger.LogInformation("Email already confirmed: {Email}", interest.Email);
                return true; // Already confirmed, return success
            }

            // Confirm email
            interest.IsEmailConfirmed = true;
            _unitOfWork.PreLaunchInterests.Update(interest);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Email confirmed successfully: {Email}", interest.Email);
            return true;
        }
    }
}