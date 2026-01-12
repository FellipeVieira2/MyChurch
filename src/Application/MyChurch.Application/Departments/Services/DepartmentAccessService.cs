using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;

namespace MyChurch.Application.Departments.Services
{
    public interface IDepartmentAccessService
    {
        Task<(bool hasAccess, bool canEdit)> CanAccessDepartmentFinancialAsync(int loggedMemberId, int departmentId, CancellationToken cancellationToken);
        Task<List<int>> GetAccessibleDepartmentIdsAsync(int loggedMemberId, CancellationToken cancellationToken);
    }

    public class DepartmentAccessService : IDepartmentAccessService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentAccessService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<(bool hasAccess, bool canEdit)> CanAccessDepartmentFinancialAsync(int loggedMemberId, int departmentId, CancellationToken cancellationToken)
        {
            var member = await _unitOfWork.Members.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == loggedMemberId, cancellationToken);

            if (member == null)
                return (false, false);

            if (member.Role == UserRole.Admin)
                return (true, true);

            var dm = await _unitOfWork.DepartmentMembers.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.DepartmentId == departmentId && x.MemberId == loggedMemberId && x.IsActive, cancellationToken);

            if (dm == null)
                return (false, false);

            var canEdit = dm.Role == DepartmentMemberRole.Financial || dm.Role == DepartmentMemberRole.Manager;
            return (true, canEdit);
        }

        public async Task<List<int>> GetAccessibleDepartmentIdsAsync(int loggedMemberId, CancellationToken cancellationToken)
        {
            var member = await _unitOfWork.Members.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == loggedMemberId, cancellationToken);

            if (member == null)
                return new List<int>();

            if (member.Role == UserRole.Admin)
            {
                return await _unitOfWork.Departments.Query()
                    .AsNoTracking()
                    .Where(d => d.ChurchId == member.ChurchId)
                    .Select(d => d.Id)
                    .ToListAsync(cancellationToken);
            }

            return await _unitOfWork.DepartmentMembers.Query()
                .AsNoTracking()
                .Where(dm => dm.MemberId == loggedMemberId && dm.IsActive)
                .Select(dm => dm.DepartmentId)
                .Distinct()
                .ToListAsync(cancellationToken);
        }
    }
}
