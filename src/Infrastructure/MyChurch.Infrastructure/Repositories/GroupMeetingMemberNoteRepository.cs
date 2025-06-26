using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Repositories
{
    public class GroupMeetingMemberNoteRepository : GenericRepository<GroupMeetingMemberNote>, IGroupMeetingMemberNoteRepository
    {
        public GroupMeetingMemberNoteRepository(MyChurchDbContext context) : base(context) { }
    }
}
