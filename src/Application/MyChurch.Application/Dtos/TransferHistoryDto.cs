namespace MyChurch.Application.Dtos
{
    public class TransferHistoryDto
    {
        public int Id { get; set; }
        public int ChurchId { get; set; }
        public int? BankingInfoId { get; set; }
        public decimal Amount { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? ScheduledFor { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? Status { get; set; }
        public string? Notes { get; set; }
        public string? FailureReason { get; set; }

        public static TransferHistoryDto New(Domain.Entities.TransferHistory entity)
        {
            return new TransferHistoryDto
            {
                Id = entity.Id,
                ChurchId = entity.ChurchId,
                BankingInfoId = entity.BankingInfoId,
                Amount = entity.Amount,
                RequestedAt = entity.RequestedAt,
                ScheduledFor = entity.ScheduledFor,
                CompletedAt = entity.CompletedAt,
                Status = entity.Status,
                Notes = entity.Notes,
                FailureReason = entity.FailureReason
            };
        }
    }
}
