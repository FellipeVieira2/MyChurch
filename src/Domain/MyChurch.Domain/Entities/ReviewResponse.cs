using System;

namespace MyChurch.Domain.Entities
{
    /// <summary>
    /// Representa a resposta oficial de uma igreja a uma review
    /// </summary>
    public class ReviewResponse
    {
        public int Id { get; set; }
        
        /// <summary>
        /// ID da review que está sendo respondida
        /// </summary>
        public int ReviewId { get; set; }
        
        /// <summary>
        /// ID do membro (admin/líder) que respondeu pela igreja
        /// </summary>
        public int ResponderId { get; set; }
        
        /// <summary>
        /// Conteúdo da resposta
        /// </summary>
        public string Response { get; set; } = string.Empty;
        
        /// <summary>
        /// Data/hora da resposta
        /// </summary>
        public DateTime RespondedAt { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Editada posteriormente?
        /// </summary>
        public bool IsEdited { get; set; } = false;
        public DateTime? EditedAt { get; set; }
        
        // Relacionamentos
        public Review Review { get; set; } = null!;
        public Member Responder { get; set; } = null!;
        
        /// <summary>
        /// Edita a resposta
        /// </summary>
        public void Edit(string newResponse)
        {
            Response = newResponse;
            IsEdited = true;
            EditedAt = DateTime.UtcNow;
        }
    }
}
