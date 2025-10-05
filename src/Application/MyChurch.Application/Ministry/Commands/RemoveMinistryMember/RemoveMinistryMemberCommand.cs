using MediatR;

namespace MyChurch.Application.Ministries.Commands.RemoveMinistryMember
{
    public class RemoveMinistryMemberCommand : IRequest<bool>
    {
        public int MinistryId { get; set; }
        public int MemberId { get; set; }
    }
}
