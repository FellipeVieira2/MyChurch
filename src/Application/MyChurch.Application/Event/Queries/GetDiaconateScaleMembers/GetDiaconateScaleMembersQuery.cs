using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Event.Queries.GetDiaconateScaleMembers
{
    public class GetDiaconateScaleMembersQuery : JwtMemberDto, IRequest<List<DiaconateScaleMemberDto>>
    {
        public int EventId { get; set; }
    }

    public class GetDiaconateScaleMembersQueryHandler : IRequestHandler<GetDiaconateScaleMembersQuery, List<DiaconateScaleMemberDto>>
    {
        private readonly IUnitOfWork _uow;

        public GetDiaconateScaleMembersQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<DiaconateScaleMemberDto>> Handle(GetDiaconateScaleMembersQuery request, CancellationToken cancellationToken)
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
                    ValidationException.ThrowException("Event", "Sem permissão para visualizar a escala de diaconato deste evento.");
            }

            var items = await _uow.DiaconateScaleMembers.Query()
                .AsNoTracking()
                .Include(x => x.Member)
                .Where(x => x.EventId == request.EventId)
                .OrderBy(x => x.Order)
                .ThenBy(x => x.Member.Name)
                .ToListAsync(cancellationToken);

            return items.Select(DiaconateScaleMemberDto.New).ToList();
        }
    }
}
