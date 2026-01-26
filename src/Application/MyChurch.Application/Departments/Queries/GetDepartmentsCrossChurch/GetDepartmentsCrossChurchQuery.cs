using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Departments.Queries.GetDepartmentsCrossChurch
{
    public class GetDepartmentsCrossChurchQuery : JwtMemberDto, IRequest<List<DepartmentDto>>
    {
        public int ParentChurchId { get; set; }
        public int BranchChurchId { get; set; }
    }

    public class GetDepartmentsCrossChurchQueryHandler : IRequestHandler<GetDepartmentsCrossChurchQuery, List<DepartmentDto>>
    {
        private readonly IUnitOfWork _uow;

        public GetDepartmentsCrossChurchQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<DepartmentDto>> Handle(GetDepartmentsCrossChurchQuery request, CancellationToken cancellationToken)
        {
            var actor = await _uow.Members.Query().AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (actor == null)
                ValidationException.ThrowException("Member", "Authenticated member does not exist.");

            var branch = await _uow.Churchs.Query().AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == request.BranchChurchId && c.ParentChurchId == request.ParentChurchId, cancellationToken);

            if (branch == null)
                ValidationException.ThrowException("Church", "Branch church not found or does not belong to this parent.");

            var departmentsQuery = _uow.Departments.Query()
                .AsNoTracking()
                .Where(d => d.ChurchId == request.BranchChurchId)
                .OrderBy(d => d.Name)
                .AsQueryable();

            // Admin da matriz pode ver todos os departamentos da filial
            if (actor.Role == UserRole.Admin)
            {
                if (actor.ChurchId != request.ParentChurchId)
                    ValidationException.ThrowException("Church", "You can only view departments for your own (parent) church.");

                return await departmentsQuery.Select(d => DepartmentDto.New(d)).ToListAsync(cancellationToken);
            }

            // GeneralLeader: só vê departamentos para os quais possui escopo ativo nessa filial
            var scopedDepartmentIds = await _uow.DepartmentGeneralLeaderScopes.Query().AsNoTracking()
                .Where(s => s.IsActive
                            && s.ParentChurchId == request.ParentChurchId
                            && s.BranchChurchId == request.BranchChurchId
                            && s.LeaderMemberId == actor.Id)
                .Select(s => s.DepartmentId)
                .Distinct()
                .ToListAsync(cancellationToken);

            if (scopedDepartmentIds.Count == 0)
                return new List<DepartmentDto>();

            return await departmentsQuery
                .Where(d => scopedDepartmentIds.Contains(d.Id))
                .Select(d => DepartmentDto.New(d))
                .ToListAsync(cancellationToken);
        }
    }
}
