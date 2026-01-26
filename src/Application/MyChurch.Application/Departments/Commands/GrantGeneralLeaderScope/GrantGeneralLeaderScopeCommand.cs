using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Departments.Commands.GrantGeneralLeaderScope
{
    public class GrantGeneralLeaderScopeCommand : JwtMemberDto, IRequest<int>
    {
        public int ParentChurchId { get; set; }
        public int BranchChurchId { get; set; }
        public int DepartmentId { get; set; }
        public int LeaderMemberId { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class GrantGeneralLeaderScopeCommandHandler : IRequestHandler<GrantGeneralLeaderScopeCommand, int>
    {
        private readonly IUnitOfWork _uow;

        public GrantGeneralLeaderScopeCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<int> Handle(GrantGeneralLeaderScopeCommand request, CancellationToken cancellationToken)
        {
            var actor = await _uow.Members.Query().FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);
            if (actor == null)
                ValidationException.ThrowException("Member", "Authenticated member does not exist.");

            if (actor.Role != UserRole.Admin)
                ValidationException.ThrowException("Auth", "Only admins can manage department supervision scopes.");

            if (actor.ChurchId != request.ParentChurchId)
                ValidationException.ThrowException("Church", "You can only manage scopes for your own (parent) church.");

            var branch = await _uow.Churchs.Query().AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == request.BranchChurchId && c.ParentChurchId == request.ParentChurchId, cancellationToken);

            if (branch == null)
                ValidationException.ThrowException("Church", "Branch church not found or does not belong to this parent.");

            var dept = await _uow.Departments.Query().AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == request.DepartmentId && d.ChurchId == request.BranchChurchId, cancellationToken);

            if (dept == null)
                ValidationException.ThrowException("Department", "Department not found for this branch.");

            var leader = await _uow.Members.Query().AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == request.LeaderMemberId, cancellationToken);

            if (leader == null)
                ValidationException.ThrowException("Member", "Leader member not found.");

            // Deve ser GeneralLeader naquele departmento (na matriz ou em qualquer igreja, mas vinculado ao DepartmentId listado)
            var isGeneralLeader = await _uow.DepartmentMembers.Query().AsNoTracking()
                .AnyAsync(dm => dm.MemberId == leader.Id && dm.DepartmentId == request.DepartmentId && dm.IsActive && dm.Role == DepartmentMemberRole.GeneralLeader, cancellationToken);

            if (!isGeneralLeader)
                ValidationException.ThrowException("Department", "Member is not a GeneralLeader for this department.");

            var exists = await _uow.DepartmentGeneralLeaderScopes.Query()
                .FirstOrDefaultAsync(s => s.ParentChurchId == request.ParentChurchId && s.BranchChurchId == request.BranchChurchId && s.DepartmentId == request.DepartmentId && s.LeaderMemberId == request.LeaderMemberId, cancellationToken);

            if (exists != null)
            {
                exists.IsActive = request.IsActive;
                exists.Updated = DateTime.UtcNow;
                _uow.DepartmentGeneralLeaderScopes.Update(exists);
                await _uow.CommitAsync();
                return exists.Id;
            }

            var scope = new DepartmentGeneralLeaderScope
            {
                ParentChurchId = request.ParentChurchId,
                BranchChurchId = request.BranchChurchId,
                DepartmentId = request.DepartmentId,
                LeaderMemberId = request.LeaderMemberId,
                IsActive = request.IsActive,
                Created = DateTime.UtcNow
            };

            await _uow.DepartmentGeneralLeaderScopes.Create(scope);
            await _uow.CommitAsync();
            return scope.Id;
        }
    }
}
