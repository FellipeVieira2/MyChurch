using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class GroupMeetingRepository : GenericRepository<GroupMeeting>, IGroupMeetingRepository
    {
        public GroupMeetingRepository(MyChurchDbContext context) : base(context) { }
    }
}
