using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.PlatformAdmin.Queries.GetChurchLeads
{
    public class GetChurchLeadsQuery : JwtMemberDto, IRequest<List<ChurchLeadDto>>
    {
        public string? Search { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }

    public class ChurchLeadDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string Phone { get; set; } = string.Empty;
        public DateTime Created { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? AdminName { get; set; }
        public string? AdminEmail { get; set; }
        public string? AdminPhone { get; set; }
    }

    public class GetChurchLeadsQueryHandler : IRequestHandler<GetChurchLeadsQuery, List<ChurchLeadDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetChurchLeadsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ChurchLeadDto>> Handle(GetChurchLeadsQuery request, CancellationToken cancellationToken)
        {
            if (!Enum.TryParse<UserRole>(request.Role, true, out var role) || role != UserRole.PlatformAdmin)
                ValidationException.ThrowException("Auth", "Apenas PlatformAdmin pode acessar este recurso.");

            IQueryable<MyChurch.Domain.Entities.Church> query = _unitOfWork.Churchs.Query()
                .AsNoTracking()
                .Include(c => c.Address)
                .Include(c => c.Members);

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var s = request.Search.Trim();
                query = query.Where(c =>
                    c.Name.Contains(s) ||
                    (c.Email != null && c.Email.Contains(s)) ||
                    c.Phone.Contains(s) ||
                    (c.Address != null && (c.Address.City.Contains(s) || c.Address.State.Contains(s)))
                );
            }

            var churches = await query
                .OrderByDescending(c => c.Created)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return churches.Select(c =>
            {
                var admin = c.Members?.FirstOrDefault(m => m.Role == UserRole.Admin);
                return new ChurchLeadDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Email = c.Email,
                    Phone = c.Phone,
                    Created = c.Created,
                    City = c.Address?.City,
                    State = c.Address?.State,
                    AdminName = admin?.Name,
                    AdminEmail = admin?.Email,
                    AdminPhone = admin?.Phone
                };
            }).ToList();
        }
    }
}
