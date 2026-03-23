using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Kids.Queries
{
    public class GetMyChildrenQuery : JwtMemberDto, IRequest<List<KidsChildDto>>
    {
    }

    public class GetMyChildrenQueryHandler : IRequestHandler<GetMyChildrenQuery, List<KidsChildDto>>
    {
        private readonly IUnitOfWork _uow;

        public GetMyChildrenQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<KidsChildDto>> Handle(GetMyChildrenQuery request, CancellationToken cancellationToken)
        {
            var actor = await _uow.Members.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

            if (actor == null)
                ValidationException.ThrowException("Member", "Usuário autenticado não encontrado.");

            if (!actor.FamilyId.HasValue)
                return [];

            var children = await _uow.Children.Query()
                .AsNoTracking()
                .Where(x => x.FamilyId == actor.FamilyId.Value)
                .Include(x => x.PickupAuthorizations)
                .Include(x => x.KidsCheckIns)
                .OrderBy(x => x.FullName)
                .ToListAsync(cancellationToken);

            return children.Select(child =>
            {
                var activeCheckIn = child.KidsCheckIns
                    .Where(x => x.CheckedOutAt == null)
                    .OrderByDescending(x => x.CheckedInAt)
                    .FirstOrDefault();

                return new KidsChildDto
                {
                    Id = child.Id,
                    FullName = child.FullName,
                    BirthDate = child.BirthDate,
                    Gender = child.Gender.ToString(),
                    IsActive = child.IsActive,
                    AuthorizedPickups = child.PickupAuthorizations
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
                        .ToList(),
                    ActiveCheckIn = activeCheckIn == null
                        ? null
                        : new KidsCheckInSummaryDto
                        {
                            Id = activeCheckIn.Id,
                            EnvironmentName = activeCheckIn.EnvironmentName,
                            CheckedInAt = activeCheckIn.CheckedInAt,
                            PickupTokenExpiresAt = activeCheckIn.PickupTokenExpiresAt
                        }
                };
            }).ToList();
        }
    }
}
