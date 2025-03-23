using MyChurch.Domain.Entities;

namespace MyChurch.Application.Dtos
{
    public class PlanDto
    {
        public int Id { get; set; }
        public string Name { get; set; } // Free, Basic, Premium  
        public decimal Price { get; set; }
        public int MaxMembers { get; set; } // Quantidade de membros permitidos  
        public int MaxEvents { get; set; } // Quantidade de eventos permitidos  
        public int MaxStorageGB { get; set; } // Espaço de armazenamento  

        public static PlanDto New(Plan plan)
        {
            return new PlanDto
            {
                Id = plan.Id,
                Name = plan.Name,
                Price = plan.Price,
                MaxMembers = plan.MaxMembers,
                MaxEvents = plan.MaxEvents,
                MaxStorageGB = plan.MaxStorageGB,
            };
        }
    }
}
