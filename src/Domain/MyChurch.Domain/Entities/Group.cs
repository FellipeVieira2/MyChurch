using System;
using System.Collections.Generic;
using MyChurch.Domain.Enum;

namespace MyChurch.Domain.Entities
{
    public class Group // Não herda IEntity, conforme instrução para o módulo Família, mas para o módulo de Grupos, herda IEntity
    {
        public int Id { get; private set; }
        public int ChurchId { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public GroupType Type { get; private set; }
        public int LeaderId { get; private set; }
        public Member Leader { get; private set; }
        public bool IsActive { get; private set; }
        public bool AcceptsNewMembers { get; private set; }
        public string? CoverImageUrl { get; private set; }
        public ICollection<GroupMember> Members { get; private set; } = new List<GroupMember>();
        public DateTime CreatedAt { get; private set; }

        private Group() { }

        public Group(int churchId, string name, string description, GroupType type, int leaderId, bool acceptsNewMembers, string? coverImageUrl)
        {
            ChurchId = churchId;
            Name = name;
            Description = description;
            Type = type;
            LeaderId = leaderId;
            AcceptsNewMembers = acceptsNewMembers;
            CoverImageUrl = coverImageUrl;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdateDetails(string name, string description, bool acceptsNewMembers, string? coverImageUrl)
        {
            Name = name;
            Description = description;
            AcceptsNewMembers = acceptsNewMembers;
            CoverImageUrl = coverImageUrl;
        }
        public void ChangeLeader(int newLeaderId)
        {
            LeaderId = newLeaderId;
        }
        public void Deactivate() => IsActive = false;
    }
}
