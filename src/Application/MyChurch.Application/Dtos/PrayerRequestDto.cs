using MyChurch.Domain.Entities;
using System;

namespace MyChurch.Application.Dtos
{
    public class PrayerRequestDto
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public string MemberName { get; set; }
        public string Request { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }

        public static PrayerRequestDto New(PrayerRequest entity)
        {
            return new PrayerRequestDto
            {
                Id = entity.Id,
                MemberId = entity.MemberId,
                MemberName = entity.Member?.Name,
                Request = entity.Request,
                CreatedAt = entity.CreatedAt,
                IsRead = entity.IsRead
            };
        }
    }
}
