using System;
using System.Collections.Generic;

namespace MyChurch.Domain.Entities
{
    public class Family
    {
        public int Id { get; set; }
        public int ChurchId { get; set; }
        public string FamilyName { get; set; } // Ex: "Família Silva"
        public DateTime CreatedAt { get; set; }
        public virtual ICollection<Member> Members { get; set; } = new List<Member>();
        public virtual ICollection<Child> Children { get; set; } = new List<Child>();
    }
}
