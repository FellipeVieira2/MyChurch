using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Departments.Queries.GetDepartmentMembers;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Departments.Queries.GetDepartmentMembersCrossChurch
{
    public class GetDepartmentMembersCrossChurchQuery : JwtMemberDto, IRequest<List<DepartmentMemberListItemDto>>
    {
        public int ParentChurchId { get; set; }
        public int BranchChurchId { get; set; }
        public int DepartmentId { get; set; }
        public bool OnlyLeaders { get; set; } = false;
    }

    public class GetDepartmentMembersCrossChurchQueryHandler : IRequestHandler<GetDepartmentMembersCrossChurchQuery, List<DepartmentMemberListItemDto>>
    {
        private readonly IUnitOfWork _uow;

        public GetDepartmentMembersCrossChurchQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<DepartmentMemberListItemDto>> Handle(GetDepartmentMembersCrossChurchQuery request, CancellationToken cancellationToken)
        {
            var leader = await _uow.Members.Query().AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (leader == null)
                ValidationException.ThrowException("Member", "Authenticated member does not exist.");

            if (leader.Role != UserRole.Admin)
            {
                var isGeneralLeaderInDept = await _uow.DepartmentMembers.Query().AsNoTracking()
                    .AnyAsync(dm => dm.MemberId == leader.Id && dm.DepartmentId == request.DepartmentId && dm.IsActive && dm.Role == DepartmentMemberRole.GeneralLeader, cancellationToken);

                if (!isGeneralLeaderInDept)
                    ValidationException.ThrowException("Department", "Sem permissão para visualizar o departamento em outras igrejas.");

                var hasScope = await _uow.DepartmentGeneralLeaderScopes.Query().AsNoTracking()
                    .AnyAsync(s =>
                        s.IsActive &&
                        s.ParentChurchId == request.ParentChurchId &&
                        s.BranchChurchId == request.BranchChurchId &&
                        s.DepartmentId == request.DepartmentId &&
                        s.LeaderMemberId == leader.Id,
                        cancellationToken);

                if (!hasScope)
                    ValidationException.ThrowException("Department", "Sem escopo para visualizar esta filial/departamento.");
            }

            var branch = await _uow.Churchs.Query().AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == request.BranchChurchId && c.ParentChurchId == request.ParentChurchId, cancellationToken);

            if (branch == null)
                ValidationException.ThrowException("Church", "Filial não pertence à matriz informada.");

            var dept = await _uow.Departments.Query().AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == request.DepartmentId && d.ChurchId == request.BranchChurchId, cancellationToken);

            if (dept == null)
                ValidationException.ThrowException("Department", "Departamento não encontrado para esta filial.");

            var listQuery = _uow.DepartmentMembers.Query()
                .AsNoTracking()
                .Include(dm => dm.Member)
                .Where(dm => dm.DepartmentId == request.DepartmentId);

            if (request.OnlyLeaders)
                listQuery = listQuery.Where(dm => dm.Role == DepartmentMemberRole.Manager || dm.Role == DepartmentMemberRole.Financial);

            var list = await listQuery
                .OrderBy(dm => dm.Member.Name)
                .Select(dm => new DepartmentMemberListItemDto
                {
                    DepartmentId = dm.DepartmentId,
                    MemberId = dm.MemberId,
                    MemberName = dm.Member.Name,
                    MemberEmail = dm.Member.Email,
                    Role = dm.Role,
                    IsActive = dm.IsActive,
                    JoinedAt = dm.JoinedAt
                })
                .ToListAsync(cancellationToken);

            return list;
        }
    }
}
