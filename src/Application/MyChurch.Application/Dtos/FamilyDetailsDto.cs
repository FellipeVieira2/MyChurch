using System;
using System.Collections.Generic;

namespace MyChurch.Application.Dtos
{
    public class FamilyDetailsDto
    {
        public int FamilyId { get; set; }
        public string FamilyName { get; set; }
        public List<FamilyMemberDto> Members { get; set; }
        public List<FamilyChildDto> Children { get; set; }
    }
    public class FamilyMemberDto
    {
        public int MemberId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
    public class FamilyChildDto
    {
        public int ChildId { get; set; }
        public string FullName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Gender { get; set; }
    }
}
