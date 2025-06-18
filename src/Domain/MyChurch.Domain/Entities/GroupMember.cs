using System;

namespace MyChurch.Domain.Entities
{
    public class GroupMember
    {
        public int Id { get; private set; }
        public int GroupId { get; private set; }
        public Group Group { get; private set; }
        public int MemberId { get; private set; }
        public Member Member { get; private set; }
        public string RoleInGroup { get; private set; } // Ex: "Líder", "Membro", "Líder em Treinamento"
        public DateTime DateJoined { get; private set; }

        private GroupMember() { }
        public GroupMember(int groupId, int memberId, string roleInGroup)
        {
            GroupId = groupId;
            MemberId = memberId;
            RoleInGroup = roleInGroup;
            DateJoined = DateTime.UtcNow;
        }
    }
}
