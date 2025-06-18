using System;

namespace MyChurch.Domain.Entities
{
    public class Campaign
    {
        public int Id { get; private set; } // Int, seguindo padrão do projeto
        public int ChurchId { get; private set; } // FK para a Igreja
        public string Name { get; private set; }
        public string Description { get; private set; }
        public decimal GoalAmount { get; private set; } // Meta financeira
        public decimal AmountRaised { get; private set; } // Valor já arrecadado
        public DateTime StartDate { get; private set; }
        public DateTime? EndDate { get; private set; } // Pode ser nula para campanhas contínuas
        public bool IsActive { get; private set; }
        public string? CoverImageUrl { get; private set; } // URL para uma imagem de capa
        public bool IsCompleted { get; private set; } // Indica se a campanha foi concluída (meta atingida ou encerrada)

        // EF Navigation
        public Church Church { get; private set; }

        // Construtor para EF
        protected Campaign() { }

        public Campaign(int churchId, string name, string description, decimal goalAmount, DateTime startDate, DateTime? endDate, string? coverImageUrl)
        {
            // Id será gerado pelo banco
            ChurchId = churchId;
            Name = name;
            Description = description;
            GoalAmount = goalAmount;
            StartDate = startDate;
            EndDate = endDate;
            CoverImageUrl = coverImageUrl;
            AmountRaised = 0;
            IsActive = true;
            IsCompleted = false;
        }

        public void AddContribution(decimal amount)
        {
            if (!IsActive || IsCompleted) return;
            AmountRaised += amount;
        }

        public void UpdateDetails(string name, string description, decimal goalAmount, DateTime startDate, DateTime? endDate, string? coverImageUrl)
        {
            Name = name;
            Description = description;
            GoalAmount = goalAmount;
            StartDate = startDate;
            EndDate = endDate;
            CoverImageUrl = coverImageUrl;
        }

        public void Complete()
        {
            IsCompleted = true;
            IsActive = false;
        }
    }
}
