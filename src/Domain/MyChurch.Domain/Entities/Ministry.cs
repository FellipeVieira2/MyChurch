namespace MyChurch.Domain.Entities
{
    /// <summary>
    /// Representa um ministério da igreja (louvor, jovens, crianças, etc.)
    /// </summary>
    public class Ministry
    {
        public int Id { get; set; }
        
        /// <summary>
        /// Nome do ministério (ex: Louvor, Jovens, Crianças, Evangelismo)
        /// </summary>
        public string Name { get; set; } = null!;
        
        /// <summary>
        /// Descrição e objetivos do ministério
        /// </summary>
        public string? Description { get; set; }
        
        /// <summary>
        /// ID do líder do ministério (Member)
        /// </summary>
        public int? LeaderId { get; set; }
        
        /// <summary>
        /// Líder do ministério
        /// </summary>
        public Member? Leader { get; set; }
        
        /// <summary>
        /// Igreja à qual o ministério pertence
        /// </summary>
        public int ChurchId { get; set; }
        public Church Church { get; set; } = null!;
        
        /// <summary>
        /// Foto/logo do ministério
        /// </summary>
        public string? Photo { get; set; }
        
        /// <summary>
        /// Cor associada ao ministério (para organização visual)
        /// </summary>
        public string? Color { get; set; }
        
        /// <summary>
        /// Dia da semana das reuniões (ex: "Segunda-feira", "Quarta-feira")
        /// </summary>
        public string? MeetingDay { get; set; }
        
        /// <summary>
        /// Horário das reuniões
        /// </summary>
        public TimeSpan? MeetingTime { get; set; }
        
        /// <summary>
        /// Local das reuniões
        /// </summary>
        public string? MeetingLocation { get; set; }
        
        /// <summary>
        /// Se o ministério está ativo
        /// </summary>
        public bool IsActive { get; set; } = true;
        
        /// <summary>
        /// Data de criação
        /// </summary>
        public DateTime Created { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Data da última atualização
        /// </summary>
        public DateTime? Updated { get; set; }
        
        /// <summary>
        /// Membros do ministério
        /// </summary>
        public ICollection<MinistryMember> MinistryMembers { get; set; } = new List<MinistryMember>();
        
        public void Update(
            string? name = null,
            string? description = null,
            int? leaderId = null,
            string? photo = null,
            string? color = null,
            string? meetingDay = null,
            TimeSpan? meetingTime = null,
            string? meetingLocation = null,
            bool? isActive = null)
        {
            if (name != null) Name = name;
            if (description != null) Description = description;
            if (leaderId.HasValue) LeaderId = leaderId;
            if (photo != null) Photo = photo;
            if (color != null) Color = color;
            if (meetingDay != null) MeetingDay = meetingDay;
            if (meetingTime.HasValue) MeetingTime = meetingTime;
            if (meetingLocation != null) MeetingLocation = meetingLocation;
            if (isActive.HasValue) IsActive = isActive.Value;
            
            Updated = DateTime.UtcNow;
        }
    }
}
