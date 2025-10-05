namespace MyChurch.Domain.Entities
{
    /// <summary>
    /// Horários fixos de cultos e atividades da igreja
    /// </summary>
    public class ChurchSchedule
    {
        public int Id { get; set; }
        
        /// <summary>
        /// ID da igreja
        /// </summary>
        public int ChurchId { get; set; }
        
        /// <summary>
        /// Dia da semana (0 = Domingo, 6 = Sábado)
        /// </summary>
        public DayOfWeek DayOfWeek { get; set; }
        
        /// <summary>
        /// Hora de início (ex: 19:00)
        /// </summary>
        public TimeSpan StartTime { get; set; }
        
        /// <summary>
        /// Hora de término (opcional)
        /// </summary>
        public TimeSpan? EndTime { get; set; }
        
        /// <summary>
        /// Tipo de serviço (Culto, Escola Dominical, Vigília, etc)
        /// </summary>
        public string ServiceType { get; set; }
        
        /// <summary>
        /// Descrição adicional
        /// </summary>
        public string? Description { get; set; }
        
        /// <summary>
        /// Indica se o horário está ativo
        /// </summary>
        public bool IsActive { get; set; } = true;
        
        /// <summary>
        /// Data de criação
        /// </summary>
        public DateTime Created { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Data de atualização
        /// </summary>
        public DateTime? Updated { get; set; }
        
        // Navegação
        public Church Church { get; set; }
    }
}
