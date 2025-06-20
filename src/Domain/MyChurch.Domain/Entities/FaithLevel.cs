namespace MyChurch.Domain.Entities
{
    public class FaithLevel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int PointsRequired { get; set; }
        public string IconUrl { get; set; }
    }
}
