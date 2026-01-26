using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Departments.Queries.GetMyDepartmentSupervisionScopes
{
    public class GetMyDepartmentSupervisionScopesQuery : JwtMemberDto, IRequest<List<DepartmentSupervisionScopeDto>>
    {
    }

    public class DepartmentSupervisionScopeDto
    {
        public int ParentChurchId { get; set; }
        public string ParentChurchName { get; set; } = string.Empty;
        public int BranchChurchId { get; set; }
        public string BranchChurchName { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
    }

    public class GetMyDepartmentSupervisionScopesQueryHandler : IRequestHandler<GetMyDepartmentSupervisionScopesQuery, List<DepartmentSupervisionScopeDto>>
    {
        private readonly IUnitOfWork _uow;

        public GetMyDepartmentSupervisionScopesQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<DepartmentSupervisionScopeDto>> Handle(GetMyDepartmentSupervisionScopesQuery request, CancellationToken cancellationToken)
        {
            var member = await _uow.Members.Query().AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (member == null)
                ValidationException.ThrowException("Member", "Authenticated member does not exist.");

            // Apenas GeneralLeader faz sentido consumir isso; admin não precisa, mas pode para debug
            if (member.Role != UserRole.Admin)
            {
                var isGeneralLeaderAnywhere = await _uow.DepartmentMembers.Query().AsNoTracking()
                    .AnyAsync(dm => dm.MemberId == member.Id && dm.IsActive && dm.Role == DepartmentMemberRole.GeneralLeader, cancellationToken);

                if (!isGeneralLeaderAnywhere)
                    return new List<DepartmentSupervisionScopeDto>();
            }

            // Lista scopes ativos do membro e traz nomes de matriz/filial/departamento
            var scopes = await _uow.DepartmentGeneralLeaderScopes.Query()
                .AsNoTracking()
                .Where(s => s.IsActive && s.LeaderMemberId == member.Id)
                .Join(_uow.Churchs.Query().AsNoTracking(), s => s.ParentChurchId, c => c.Id,
                    (s, parent) => new { s, parent })
                .Join(_uow.Churchs.Query().AsNoTracking(), x => x.s.BranchChurchId, c => c.Id,
                    (x, branch) => new { x.s, x.parent, branch })
                .Join(_uow.Departments.Query().AsNoTracking(), x => x.s.DepartmentId, d => d.Id,
                    (x, dept) => new DepartmentSupervisionScopeDto
                    {
                        ParentChurchId = x.parent.Id,
                        ParentChurchName = x.parent.Name,
                        BranchChurchId = x.branch.Id,
                        BranchChurchName = x.branch.Name,
                        DepartmentId = dept.Id,
                        DepartmentName = dept.Name
                    })
                .OrderBy(x => x.ParentChurchName)
                .ThenBy(x => x.BranchChurchName)
                .ThenBy(x => x.DepartmentName)
                .ToListAsync(cancellationToken);

            return scopes;
        }
    }
}
