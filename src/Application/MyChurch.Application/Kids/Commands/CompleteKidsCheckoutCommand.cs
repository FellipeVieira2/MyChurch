using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Kids.Commands
{
    public class CompleteKidsCheckoutCommand : JwtMemberDto, IRequest<bool>
    {
        public int CheckInId { get; set; }
        public int AuthorizedPickupId { get; set; }
        public string PickupToken { get; set; } = string.Empty;
    }

    public class CompleteKidsCheckoutCommandHandler : IRequestHandler<CompleteKidsCheckoutCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public CompleteKidsCheckoutCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(CompleteKidsCheckoutCommand request, CancellationToken cancellationToken)
        {
            var actor = await _uow.Members.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

            if (actor == null)
                ValidationException.ThrowException("Member", "Usuário autenticado não encontrado.");

            if (!IsKidsStaff(actor.Role))
                ValidationException.ThrowException("Auth", "Você não possui permissão para finalizar retiradas do kids.");

            var token = request.PickupToken?.Trim();
            if (string.IsNullOrWhiteSpace(token))
                ValidationException.ThrowException("PickupToken", "O token de retirada é obrigatório.");

            var checkIn = await _uow.KidsCheckIns.Query()
                .Include(x => x.Child)
                    .ThenInclude(x => x.PickupAuthorizations)
                .FirstOrDefaultAsync(x => x.Id == request.CheckInId && x.ChurchId == actor.ChurchId && x.CheckedOutAt == null, cancellationToken);

            if (checkIn == null)
                ValidationException.ThrowException("CheckIn", "Check-in ativo não encontrado.");

            if (!string.Equals(checkIn.PickupToken, token, StringComparison.Ordinal))
                ValidationException.ThrowException("PickupToken", "O QR code informado não corresponde ao check-in.");

            if (checkIn.PickupTokenExpiresAt < DateTime.UtcNow)
                ValidationException.ThrowException("CheckIn", "O QR code de retirada está expirado.");

            var authorizedPickup = checkIn.Child.PickupAuthorizations
                .FirstOrDefault(x => x.Id == request.AuthorizedPickupId && x.IsActive);

            if (authorizedPickup == null)
                ValidationException.ThrowException("AuthorizedPickup", "A pessoa selecionada não está autorizada para esta retirada.");

            checkIn.CheckedOutAt = DateTime.UtcNow;
            checkIn.CheckedOutByMemberId = actor.Id;
            checkIn.AuthorizedPickupId = authorizedPickup.Id;

            _uow.KidsCheckIns.Update(checkIn);
            await _uow.CommitAsync();
            return true;
        }

        private static bool IsKidsStaff(UserRole role)
        {
            return role is UserRole.Admin
                or UserRole.Administration
                or UserRole.Pastor
                or UserRole.Minister
                or UserRole.Leader
                or UserRole.Worker
                or UserRole.Deacon
                or UserRole.Diaconate
                or UserRole.Kids
                or UserRole.Elder;
        }
    }
}
