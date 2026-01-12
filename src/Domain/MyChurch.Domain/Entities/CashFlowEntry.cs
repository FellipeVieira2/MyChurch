using MyChurch.Domain.Enum;

namespace MyChurch.Domain.Entities
{
    public class CashFlowEntry
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string? Description { get; set; }
        public CashFlowType Type { get; set; }
        public int ChurchId { get; set; }
        public Church Church { get; set; } = null!;
        public int? MemberId { get; set; }
        public Member? Member { get; set; }
        public int CategoryId { get; set; }
        public CashFlowCategory Category { get; set; } = null!;
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime? Updated { get; set; }

        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }

        // 🔥 NOVOS CAMPOS - Integração e Auditoria
        /// <summary>
        /// Lançamento criado automaticamente pelo sistema (ex: doação confirmada)
        /// </summary>
        public bool IsAutomatic { get; set; }

        /// <summary>
        /// ID da doação vinculada (se houver)
        /// </summary>
        public int? DonationId { get; set; }
        public Donation? Donation { get; set; }

        /// <summary>
        /// ID do ativo vinculado (se for compra/venda de ativo)
        /// </summary>
        public int? AssetId { get; set; }
        public Asset? Asset { get; set; }

        /// <summary>
        /// URL do comprovante/recibo
        /// </summary>
        public string? ReceiptUrl { get; set; }

        /// <summary>
        /// Marca se foi conciliado com extrato bancário
        /// </summary>
        public bool IsReconciled { get; set; }

        /// <summary>
        /// Data da conciliação bancária
        /// </summary>
        public DateTime? ReconciledAt { get; set; }

        /// <summary>
        /// Notas/observações adicionais
        /// </summary>
        public string? Notes { get; set; }
    }
}
