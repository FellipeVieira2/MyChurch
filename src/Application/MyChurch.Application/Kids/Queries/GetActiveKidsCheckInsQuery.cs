using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Kids.Queries
{
    public class GetActiveKidsCheckInsQuery : JwtMemberDto, IRequest<List<KidsCheckInDto>>
    {
    }

    public class GetActiveKidsCheckInsQueryHandler : IRequestHandler<GetActiveKidsCheckInsQuery, List<KidsCheckInDto>>
    {
        private readonly IUnitOfWork _uow;

        public GetActiveKidsCheckInsQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<KidsCheckInDto>> Handle(GetActiveKidsCheckInsQuery request, CancellationToken cancellationToken)
        {
            var actor = await _uow.Members.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

            if (actor == null)
                ValidationException.ThrowException("Member", "Usuário autenticado não encontrado.");

            if (!IsKidsStaff(actor.Role))
                ValidationException.ThrowException("Auth", "Você não possui permissão para visualizar check-ins ativos do kids.");

            var checkIns = await _uow.KidsCheckIns.Query()
                .AsNoTracking()
                .Where(x => x.ChurchId == actor.ChurchId && x.CheckedOutAt == null)
                .Include(x => x.Child)
                .OrderByDescending(x => x.CheckedInAt)
                .ToListAsync(cancellationToken);

            return checkIns.Select(x => new KidsCheckInDto
            {
                Id = x.Id,
                ChildId = x.ChildId,
                ChildName = x.Child.FullName,
                EnvironmentName = x.EnvironmentName,
                CheckedInAt = x.CheckedInAt,
                PickupTokenExpiresAt = x.PickupTokenExpiresAt,
                PickupToken = x.PickupToken,
                QrCodeBase64 = string.Empty,
                Notes = x.Notes
            }).ToList();
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
