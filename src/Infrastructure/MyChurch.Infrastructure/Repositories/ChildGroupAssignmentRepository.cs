using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class ChildGroupAssignmentRepository : GenericRepository<ChildGroupAssignment>, IChildGroupAssignmentRepository
    {
        public ChildGroupAssignmentRepository(MyChurchDbContext context) : base(context) { }
    }
}
