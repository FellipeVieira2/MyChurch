using MediatR;
using Microsoft.EntityFrameworkCore;
using Mychurch.Common.Utils.Objects;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Departments.Queries.GetDepartments
{
    public class GetDepartmentsQuery : JwtMemberDto, IRequest<PagedResultDto<DepartmentDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public bool? IsActive { get; set; }
        public string? Name { get; set; }
    }

    public class GetDepartmentsQueryHandler : IRequestHandler<GetDepartmentsQuery, PagedResultDto<DepartmentDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDepartmentsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResultDto<DepartmentDto>> Handle(GetDepartmentsQuery request, CancellationToken cancellationToken)
        {
            var member = await _unitOfWork.Members.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (member == null)
                ValidationException.ThrowException("Member", "Usuário não encontrado.");

            var deptQuery = _unitOfWork.Departments.Query()
                .AsNoTracking()
                .Where(d => d.ChurchId == member.ChurchId);

            // Admin vê tudo; demais (Leader incluído) só os departamentos onde participam
            if (member.Role != UserRole.Admin)
            {
                var memberDepartmentIds = _unitOfWork.DepartmentMembers.Query()
                    .AsNoTracking()
                    .Where(dm => dm.MemberId == member.Id && dm.IsActive)
                    .Select(dm => dm.DepartmentId);

                deptQuery = deptQuery.Where(d => memberDepartmentIds.Contains(d.Id));
            }

            if (request.IsActive.HasValue)
                deptQuery = deptQuery.Where(d => d.IsActive == request.IsActive.Value);

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                var name = request.Name.Trim().ToLower();
                deptQuery = deptQuery.Where(d => d.Name.ToLower().Contains(name));
            }

            var total = await deptQuery.CountAsync(cancellationToken);

            var items = await deptQuery
                .OrderBy(d => d.Name)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResultDto<DepartmentDto>(items.Select(DepartmentDto.New).ToList(), request.PageNumber, request.PageSize, total);
        }
    }
}
