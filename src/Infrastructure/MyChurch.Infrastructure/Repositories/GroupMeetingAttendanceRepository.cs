using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class GroupMeetingAttendanceRepository : GenericRepository<GroupMeetingAttendance>, IGroupMeetingAttendanceRepository
    {
        public GroupMeetingAttendanceRepository(MyChurchDbContext context) : base(context) { }
    }
}
