using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;

namespace MyChurch.Application.Dtos
{
    public class CashFlowEntryDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string? Description { get; set; }
        public CashFlowType Type { get; set; }
        public int ChurchId { get; set; }
        public string? ChurchName { get; set; }
        public int? MemberId { get; set; }
        public string? MemberName { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public static CashFlowEntryDto New(CashFlowEntry entry)
        {
            return new CashFlowEntryDto
            {
                Id = entry.Id,
                Amount = entry.Amount,
                Date = entry.Date,
                Description = entry.Description,
                Type = entry.Type,
                ChurchId = entry.ChurchId,
                ChurchName = entry.Church?.Name,
                MemberId = entry.MemberId,
                MemberName = entry.Member?.Name,
                CategoryId = entry.CategoryId,
                CategoryName = entry.Category?.Name,
                Created = entry.Created,
                Updated = entry.Updated
            };
        }
    }
}