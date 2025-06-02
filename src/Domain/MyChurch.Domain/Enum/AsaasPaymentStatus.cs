namespace MyChurch.Domain.Enum
{
    public enum AsaasPaymentStatus
    {
        PENDING,            // Aguardando pagamento
        RECEIVED,          // Recebido
        CONFIRMED,         // Confirmado
        OVERDUE,          // Vencido
        REFUNDED,         // Reembolsado
        RECEIVED_IN_CASH, // Recebido em dinheiro
        REFUND_REQUESTED, // Reembolso solicitado
        CHARGEBACK_REQUESTED, // Chargeback solicitado
        CHARGEBACK_DISPUTE, // Em disputa de chargeback
        AWAITING_CHARGEBACK_REVERSAL, // Aguardando reversão de chargeback
        DUNNING_REQUESTED, // Em processo de recuperação
        DUNNING_RECEIVED, // Recuperado
        AWAITING_RISK_ANALYSIS // Em análise de risco
    }
}
