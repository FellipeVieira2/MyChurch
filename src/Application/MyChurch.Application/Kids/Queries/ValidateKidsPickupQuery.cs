using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Kids.Queries
{
    public class ValidateKidsPickupQuery : JwtMemberDto, IRequest<KidsPickupValidationDto>
    {
        public string PickupToken { get; set; } = string.Empty;
    }

    public class ValidateKidsPickupQueryHandler : IRequestHandler<ValidateKidsPickupQuery, KidsPickupValidationDto>
    {
        private readonly IUnitOfWork _uow;

        public ValidateKidsPickupQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<KidsPickupValidationDto> Handle(ValidateKidsPickupQuery request, CancellationToken cancellationToken)
        {
            var actor = await _uow.Members.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

            if (actor == null)
                ValidationException.ThrowException("Member", "Usuário autenticado não encontrado.");

            if (!IsKidsStaff(actor.Role))
                ValidationException.ThrowException("Auth", "Você não possui permissão para validar retiradas do kids.");

            var token = request.PickupToken?.Trim();
            if (string.IsNullOrWhiteSpace(token))
                ValidationException.ThrowException("PickupToken", "O token de retirada é obrigatório.");

            var checkIn = await _uow.KidsCheckIns.Query()
                .AsNoTracking()
                .Where(x => x.PickupToken == token && x.CheckedOutAt == null && x.ChurchId == actor.ChurchId)
                .Include(x => x.Child)
                    .ThenInclude(x => x.PickupAuthorizations)
                .FirstOrDefaultAsync(cancellationToken);

            if (checkIn == null)
                ValidationException.ThrowException("CheckIn", "Check-in não encontrado para este QR code.");

            if (checkIn.PickupTokenExpiresAt < DateTime.UtcNow)
                ValidationException.ThrowException("CheckIn", "O QR code de retirada está expirado.");

            return new KidsPickupValidationDto
            {
                CheckInId = checkIn.Id,
                ChildId = checkIn.ChildId,
                ChildName = checkIn.Child.FullName,
                EnvironmentName = checkIn.EnvironmentName,
                CheckedInAt = checkIn.CheckedInAt,
                PickupTokenExpiresAt = checkIn.PickupTokenExpiresAt,
                AuthorizedPickups = checkIn.Child.PickupAuthorizations
                    .Where(x => x.IsActive)
                    .OrderBy(x => x.FullName)
                    .Select(x => new ChildPickupAuthorizationDto
                    {
                        Id = x.Id,
                        FullName = x.FullName,
                        Relationship = x.Relationship,
                        DocumentNumber = x.DocumentNumber,
                        PhoneNumber = x.PhoneNumber,
                        IsActive = x.IsActive
                    })
                    .ToList()
            };
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
