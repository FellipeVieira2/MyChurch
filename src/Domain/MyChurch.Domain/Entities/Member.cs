// ... outros usings ...
using MyChurch.Domain.Enum;
using System.Collections.Generic;

namespace MyChurch.Domain.Entities
{
    public class Member
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Email { get; set; }
        // Remova ou descontinue o campo antigo:
        // public string? Document { get; set; }
        public string? Photo { get; set; }
        public string? PasswordHash { get; set; }
        public string? Password { get; set; }
        public string? Phone { get; set; }
        public DateTime BirthDate { get; set; }
        public bool IsBaptized { get; set; }
        public DateTime? BaptizedDate { get; set; }
        public bool IsTither { get; set; } = false;
        public int ChurchId { get; set; }
        public MaritalStatus? MaritalStatus { get; set; }
        public DateTime? MemberSince { get; set; }
        public string? Ministry { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Notes { get; set; }
        public int? AddressId { get; set; }
        public Address? Address { get; set; }

        // Naturalidade (cidade-estado)
        public string? BirthCity { get; set; }
        public string? BirthState { get; set; }

        // Novo: lista de documentos
        public ICollection<MemberDocument> Documents { get; set; } = new List<MemberDocument>();
        public ICollection<CreditCardInfo> CreditCardInfos { get; set; } = new List<CreditCardInfo>();
        public ICollection<PrayerRequest> PrayerRequests { get; set; } = new List<PrayerRequest>();
        public Church Church { get; set; }
        public UserRole Role { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public ICollection<Donation> Donations { get; set; }
        public ICollection<Event> Events { get; set; }
        public ICollection<CashFlowEntry> CashFlowEntries { get; set; }
    }
}