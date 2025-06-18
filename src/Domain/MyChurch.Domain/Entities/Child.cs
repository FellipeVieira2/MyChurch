using MyChurch.Domain.Enum;
using System;
using System.Collections.Generic;

namespace MyChurch.Domain.Entities
{
    public class Child
    {
        public int Id { get; set; }
        public int FamilyId { get; set; }
        public virtual Family Family { get; set; }
        public string FullName { get; set; }
        public DateTime BirthDate { get; set; }
        public Gender Gender { get; set; } // Enum
        public bool IsActive { get; set; } = true;
        public virtual ICollection<ChildGroupAssignment> GroupAssignments { get; set; } = new List<ChildGroupAssignment>();
    }
}
