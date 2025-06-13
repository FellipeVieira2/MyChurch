namespace MyChurch.Domain.Entities
{
    public class WorshipScheduleItem
    {
        public int Id { get; set; }
        public int WorshipServiceId { get; set; }
        public string Name { get; set; } = string.Empty; // Ex: "Leitura Bíblica", "Louvor das crianças"
        public int Order { get; set; } // Ordem no cronograma

        // Navegação
        public WorshipService WorshipService { get; set; }
    }
}