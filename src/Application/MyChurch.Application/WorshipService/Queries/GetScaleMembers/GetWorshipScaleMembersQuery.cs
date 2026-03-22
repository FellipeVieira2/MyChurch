using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.WorshipService.Queries.GetScaleMembers
{
    public class GetWorshipScaleMembersQuery : JwtMemberDto, IRequest<List<WorshipScaleMemberDto>>
    {
        public int WorshipServiceId { get; set; }
    }

    public class GetWorshipScaleMembersQueryHandler : IRequestHandler<GetWorshipScaleMembersQuery, List<WorshipScaleMemberDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetWorshipScaleMembersQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<WorshipScaleMemberDto>> Handle(GetWorshipScaleMembersQuery request, CancellationToken cancellationToken)
        {
            var loggedMember = await _unitOfWork.Members.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (loggedMember == null)
                throw new UnauthorizedAccessException("Usuário não encontrado.");

            var worshipService = await _unitOfWork.WorshipServices.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(ws => ws.Id == request.WorshipServiceId && ws.ChurchId == loggedMember.ChurchId, cancellationToken);

            if (worshipService == null)
                ValidationException.ThrowException("WorshipService", "Culto não encontrado ou não pertence à sua igreja.");

            if (loggedMember.Role != UserRole.Admin && worshipService.DepartmentId.HasValue)
            {
                var hasAccess = await _unitOfWork.DepartmentMembers.Query()
                    .AsNoTracking()
                    .AnyAsync(dm => dm.DepartmentId == worshipService.DepartmentId.Value && dm.MemberId == loggedMember.Id && dm.IsActive, cancellationToken);

                if (!hasAccess)
                    ValidationException.ThrowException("WorshipScale", "Sem permissão para visualizar a escala deste culto.");
            }

            var items = await _unitOfWork.WorshipScaleMembers.Query()
                .AsNoTracking()
                .Include(x => x.Member)
                .Where(x => x.WorshipServiceId == request.WorshipServiceId)
                .OrderBy(x => x.Order)
                .ThenBy(x => x.Member.Name)
                .ToListAsync(cancellationToken);

            return items.Select(WorshipScaleMemberDto.New).ToList();
        }
    }
}
