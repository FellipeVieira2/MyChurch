namespace MyChurch.Application.Onboarding.IdentifyMember
{
    public class IdentifyMemberResultDto
    {
        /// <summary>
        /// Status do membro:
        /// - NotFound: Membro não encontrado
        /// - PendingApproval: Cadastro aguardando aprovação do administrador
        /// - AlreadyActive: Conta aprovada e ativa - pode fazer login
        /// - Rejected: Cadastro foi rejeitado pelo administrador
        /// </summary>
        public string Status { get; set; }
        
        /// <summary>
        /// Nome mascarado do membro (ex: "João S.")
        /// </summary>
        public string? MaskedName { get; set; }
    }
}