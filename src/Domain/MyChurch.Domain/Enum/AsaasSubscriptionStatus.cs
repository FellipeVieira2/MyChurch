namespace MyChurch.Domain.Enum
{
    public enum AsaasSubscriptionStatus
    {
        ACTIVE,         // Assinatura ativa
        EXPIRED,        // Assinatura expirada
        OVERDUE,        // Assinatura em atraso
        CANCELED,       // Assinatura cancelada
        PENDING,        // Aguardando pagamento do primeiro pagamento
    }
}