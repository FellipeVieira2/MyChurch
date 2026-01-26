using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Departments.Queries.GetDepartmentMembers
{
    public class GetDepartmentMembersQuery : JwtMemberDto, IRequest<List<DepartmentMemberListItemDto>>
    {
        public int DepartmentId { get; set; }
    }

    public class DepartmentMemberListItemDto
    {
        public int DepartmentId { get; set; }
        public int MemberId { get; set; }
        public string? MemberName { get; set; }
        public string? MemberEmail { get; set; }
        public DepartmentMemberRole Role { get; set; }
        public bool IsActive { get; set; }
        public DateTime JoinedAt { get; set; }
    }

    public class GetDepartmentMembersQueryHandler : IRequestHandler<GetDepartmentMembersQuery, List<DepartmentMemberListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDepartmentMembersQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<DepartmentMemberListItemDto>> Handle(GetDepartmentMembersQuery request, CancellationToken cancellationToken)
        {
            var loggedMember = await _unitOfWork.Members.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (loggedMember == null)
                ValidationException.ThrowException("Member", "Usuário não encontrado.");

            var department = await _unitOfWork.Departments.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == request.DepartmentId && d.ChurchId == loggedMember.ChurchId, cancellationToken);

            if (department == null)
                ValidationException.ThrowException("Department", "Departamento não encontrado.");

            // Admin vê tudo; outros só com regra
            if (loggedMember.Role != UserRole.Admin)
            {
                var myDeptMembership = await _unitOfWork.DepartmentMembers.Query()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(dm => dm.DepartmentId == request.DepartmentId && dm.MemberId == loggedMember.Id && dm.IsActive, cancellationToken);

                if (myDeptMembership == null)
                    ValidationException.ThrowException("Department", "Sem permissão para visualizar este departamento.");

                // Se for GeneralLeader, pode ver membros do departamento desta igreja (filial) normalmente.
                // O acesso cross-filiais será via endpoints específicos da matriz (não aqui), para evitar leak entre filiais.
            }

            var list = await _unitOfWork.DepartmentMembers.Query()
                .AsNoTracking()
                .Include(dm => dm.Member)
                .Where(dm => dm.DepartmentId == request.DepartmentId)
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
