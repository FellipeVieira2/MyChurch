using System;

namespace MyChurch.Domain.Entities
{
    public class PreLaunchInterest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string ChurchName { get; set; }
        public string ChurchRole { get; set; }
        public string Comments { get; set; }
        public DateTime RegisterDate { get; set; } = DateTime.UtcNow;
        public bool IsEmailConfirmed { get; set; } = false;
        public string ConfirmationToken { get; set; } = Guid.NewGuid().ToString("N");
    }
}