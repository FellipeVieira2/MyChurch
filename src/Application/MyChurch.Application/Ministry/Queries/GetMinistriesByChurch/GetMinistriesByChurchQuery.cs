using MediatR;
using MyChurch.Application.Dtos;

namespace MyChurch.Application.Ministries.Queries.GetMinistriesByChurch
{
    public class GetMinistriesByChurchQuery : IRequest<List<MinistryDto>>
    {
        public int ChurchId { get; set; }
        public bool? IsActive { get; set; }
    }
}
