using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class MemberJourneyAssignmentRepository : GenericRepository<MemberJourneyAssignment>, IMemberJourneyAssignmentRepository
    {
        public MemberJourneyAssignmentRepository(MyChurchDbContext context) : base(context)
        {
        }
    }
}
