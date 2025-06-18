namespace MyChurch.Domain.Entities
{
    public class ChildGroupAssignment
    {
        public int ChildId { get; set; }
        public virtual Child Child { get; set; }
        public int GroupId { get; set; }
        public virtual Group Group { get; set; }
    }
}
