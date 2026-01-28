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
        public string? Photo { get; set; }
        
        // 🔐 SEGURANÇA: Apenas PasswordHash deve ser usado - NUNCA armazenar senha em texto plano!
        public string? PasswordHash { get; set; }

        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiresAt { get; set; }
        
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

        public bool PendingApproval { get; set; }

        // Família
        public int? FamilyId { get; set; }
        public virtual Family? Family { get; set; }

        // Gamification and devotional progress
        public int TotalFaithPoints { get; set; }
        public int DevotionalStreak { get; set; }
        public DateTime? LastCompletedActivityDate { get; set; }
        public int? FaithLevelId { get; set; }
        public virtual FaithLevel? FaithLevel { get; set; }

        // Engagement Score
        public int EngagementScore { get; private set; }

        public void Update(
            string? name = null,
            string? email = null,
            string? phone = null,
            DateTime? birthDate = null,
            bool? isBaptized = null,
            DateTime? baptizedDate = null,
            bool? isTither = null,
            MaritalStatus? maritalStatus = null,
            DateTime? memberSince = null,
            string? ministry = null,
            bool? isActive = null,
            string? notes = null,
            string? photo = null,
            string? birthCity = null,
            string? birthState = null)
        {
            Name = name ?? Name;
            Email = email ?? Email;
            Phone = phone ?? Phone;
            if (birthDate.HasValue) BirthDate = birthDate.Value;
            if (isBaptized.HasValue) IsBaptized = isBaptized.Value;
            if (baptizedDate.HasValue) BaptizedDate = baptizedDate;
            if (isTither.HasValue) IsTither = isTither.Value;
            if (maritalStatus.HasValue) MaritalStatus = maritalStatus;
            if (memberSince.HasValue) MemberSince = memberSince;
            Ministry = ministry ?? Ministry;
            if (isActive.HasValue) IsActive = isActive.Value;
            Notes = notes ?? Notes;
            Photo = photo ?? Photo;
            BirthCity = birthCity ?? BirthCity;
            BirthState = birthState ?? BirthState;
        }

        public void ApproveRegistration()
        {
            if (!PendingApproval)
            {
                return;
            }
            
            // ✅ Aprovar e ativar a conta
            PendingApproval = false;
            IsActive = true; // 🎉 Conta ativa após aprovação do admin
        }

        public void ActivateAccount(string passwordHash)
        {
            if (IsActive) return;

            PasswordHash = passwordHash;
            IsActive = true;
            // Supondo que exista um campo ActivationToken, limpe-o aqui se necessário
            // ActivationToken = null;
        }

        public void MarkAsPendingApproval()
        {
            PendingApproval = true;
        }

        public void SetEngagementScore(int newScore)
        {
            EngagementScore = newScore >= 0 ? newScore : 0;
        }
    }
}