namespace MyChurch.Domain.Entities
{
    public class Plan
    {
        public int Id { get; set; }
        public string Name { get; set; } // Free, Basic, Premium  
        public decimal Price { get; set; }
        public int MaxMembers { get; set; } // Quantidade de membros permitidos  
        public int MaxEvents { get; set; } // Quantidade de eventos permitidos  
        public int MaxStorageGB { get; set; } // Espaço de armazenamento  
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
    }
}
