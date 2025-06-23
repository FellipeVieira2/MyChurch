namespace MyChurch.Application.Dtos
{
    public class PreLaunchInterestDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string ChurchName { get; set; }
        public string ChurchRole { get; set; }
        public string Comments { get; set; }
        public DateTime RegisterDate { get; set; }
        public bool IsEmailConfirmed { get; set; }
    }
    
    public class CreatePreLaunchInterestDto
    {
        /// <summary>Nome completo</summary>
        /// <example>João Silva</example>
        public string Name { get; set; }
        
        /// <summary>Email para contato</summary>
        /// <example>joao.silva@email.com</example>
        public string Email { get; set; }
        
        /// <summary>Telefone</summary>
        /// <example>(11) 98765-4321</example>
        public string Phone { get; set; }
        
        /// <summary>Nome da igreja</summary>
        /// <example>Igreja Batista Central</example>
        public string ChurchName { get; set; }
        
        /// <summary>Cargo/função na igreja</summary>
        /// <example>Pastor</example>
        public string ChurchRole { get; set; }
        
        /// <summary>Comentários adicionais</summary>
        /// <example>Gostaria de saber mais sobre os recursos de gestão financeira</example>
        public string Comments { get; set; }
    }
}