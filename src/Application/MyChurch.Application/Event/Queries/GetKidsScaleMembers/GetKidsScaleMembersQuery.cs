using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Event.Queries.GetKidsScaleMembers
{
    public class GetKidsScaleMembersQuery : JwtMemberDto, IRequest<List<KidsScaleMemberDto>>
    {
        public int EventId { get; set; }
    }

    public class GetKidsScaleMembersQueryHandler : IRequestHandler<GetKidsScaleMembersQuery, List<KidsScaleMemberDto>>
    {
        private readonly IUnitOfWork _uow;

        public GetKidsScaleMembersQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<KidsScaleMemberDto>> Handle(GetKidsScaleMembersQuery request, CancellationToken cancellationToken)
        {
            var loggedMember = await _uow.Members.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (loggedMember == null)
                ValidationException.ThrowException("Member", "Usuário autenticado não encontrado.");

            var ev = await _uow.Events.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == request.EventId && e.ChurchId == loggedMember.ChurchId, cancellationToken);

            if (ev == null)
                ValidationException.ThrowException("Event", "Evento não encontrado ou não pertence à sua igreja.");

            if (loggedMember.Role != UserRole.Admin && ev.DepartmentId.HasValue)
            {
                var hasDepartmentAccess = await _uow.DepartmentMembers.Query()
                    .AsNoTracking()
                    .AnyAsync(dm => dm.DepartmentId == ev.DepartmentId.Value && dm.MemberId == loggedMember.Id && dm.IsActive, cancellationToken);

                if (!hasDepartmentAccess)
                    ValidationException.ThrowException("Event", "Sem permissão para visualizar a escala kids deste evento.");
            }

            var items = await _uow.KidsScaleMembers.Query()
                .AsNoTracking()
                .Include(x => x.Member)
                .Where(x => x.EventId == request.EventId)
                .OrderBy(x => x.Order)
                .ThenBy(x => x.Member.Name)
                .ToListAsync(cancellationToken);

            return items.Select(KidsScaleMemberDto.New).ToList();
        }
    }
}
