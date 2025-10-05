using MediatR;
using MyChurch.Application.Dtos;

namespace MyChurch.Application.Ministries.Queries.GetMinistryById
{
    public class GetMinistryByIdQuery : IRequest<MinistryDto?>
    {
        public int Id { get; set; }
    }
}
