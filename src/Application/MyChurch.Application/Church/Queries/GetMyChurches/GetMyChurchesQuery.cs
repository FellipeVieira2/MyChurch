using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Church.Queries.GetMyChurches
{
    public class GetMyChurchesQuery : JwtMemberDto, IRequest<List<MyChurchListItemDto>>
    {
    }

    public class MyChurchListItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? ParentChurchId { get; set; }
        public bool IsBranch => ParentChurchId.HasValue;
    }

    public class GetMyChurchesQueryHandler : IRequestHandler<GetMyChurchesQuery, List<MyChurchListItemDto>>
    {
        private readonly IUnitOfWork _uow;

        public GetMyChurchesQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<MyChurchListItemDto>> Handle(GetMyChurchesQuery request, CancellationToken cancellationToken)
        {
            var member = await _uow.Members.Query().AsNoTracking().FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);
            if (member == null)
                ValidationException.ThrowException("Member", "Authenticated member does not exist.");

            // Admin matrícula: retorna matriz + filiais
            if (member.Role == UserRole.Admin)
            {
                var church = await _uow.Churchs.Query().AsNoTracking().FirstOrDefaultAsync(c => c.Id == member.ChurchId, cancellationToken);
                if (church == null)
                    ValidationException.ThrowException("Church", "Church not found.");

                var parentId = church.ParentChurchId ?? church.Id;

                var churches = await _uow.Churchs.Query()
                    .AsNoTracking()
                    .Where(c => c.Id == parentId || c.ParentChurchId == parentId)
                    .OrderBy(c => c.ParentChurchId.HasValue)
                    .ThenBy(c => c.Name)
                    .Select(c => new MyChurchListItemDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        ParentChurchId = c.ParentChurchId
                    })
                    .ToListAsync(cancellationToken);

                return churches;
            }

            // Outros papéis: apenas a própria igreja
            var own = await _uow.Churchs.Query()
                .AsNoTracking()
                .Where(c => c.Id == member.ChurchId)
                .Select(c => new MyChurchListItemDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    ParentChurchId = c.ParentChurchId
                })
                .ToListAsync(cancellationToken);

            return own;
        }
    }
}
