using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mychurch.Common.Utils.Objects;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Member.Queries.GetAllMembers
{
    public class GetAllMembersQuery : JwtMemberDto, IRequest<PagedResultDto<MemberDto>>
    {
        /// <summary>Filtro por nome (contém)</summary>
        public string? Name { get; set; }

        /// <summary>Filtro por documento (contém)</summary>
        public string? Document { get; set; }

        /// <summary>Filtro por email (contém)</summary>
        public string? Email { get; set; }

        /// <summary>Filtro por data de nascimento exata</summary>
        public DateTime? BirthDate { get; set; }

        /// <summary>Filtro se é batizado</summary>
        public bool? IsBaptized { get; set; }

        /// <summary>Filtro por data de batismo exata</summary>
        public DateTime? BaptizedDate { get; set; }

        /// <summary>Filtro por papel (role)</summary>
        public UserRole? RoleMember { get; set; }

        /// <summary>Filtro por membro ativo</summary>
        public bool? IsActive { get; set; }

        /// <summary>Filtro por ministério (contém)</summary>
        public string? Ministry { get; set; }

        /// <summary>Filtro por estado civil (contém)</summary>
        public string? MaritalStatus { get; set; }

        /// <summary>Filtro por data de entrada como membro</summary>
        public DateTime? MemberSince { get; set; }

        /// <summary>Filtro por observações (contém)</summary>
        public string? Notes { get; set; }

        /// <summary>Número da página (começa em 1)</summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>Tamanho da página</summary>
        public int PageSize { get; set; } = 20;
    }

    public class GetAllMembersQueryHandler : IRequestHandler<GetAllMembersQuery, PagedResultDto<MemberDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllMembersQueryHandler> _logger;

        public GetAllMembersQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAllMembersQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<PagedResultDto<MemberDto>> Handle(GetAllMembersQuery request, CancellationToken cancellationToken)
        {
            // Busca o membro logado para obter o ChurchId
            var member = await _unitOfWork.Members.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (member == null)
            {
                _logger.LogWarning("Usuário não encontrado.");
                ValidationException.ThrowException("Member", "This Member does not exist.");
            }

            int churchId = member.ChurchId;

            // Inicia a query base filtrando por igreja
            var query = _unitOfWork.Members.Query()
                .AsNoTracking()
                .Where(m => m.ChurchId == churchId);

            // Filtros existentes
            if (!string.IsNullOrWhiteSpace(request.Name))
                query = query.Where(m => EF.Functions.Like(m.Name, $"%{request.Name}%"));

            if (!string.IsNullOrWhiteSpace(request.Document))
                query = query.Where(m => EF.Functions.Like(m.Document, $"%{request.Document}%"));

            if (!string.IsNullOrWhiteSpace(request.Email))
                query = query.Where(m => EF.Functions.Like(m.Email, $"%{request.Email}%"));

            if (request.BirthDate.HasValue)
                query = query.Where(m => m.BirthDate.Date == request.BirthDate.Value.Date);

            if (request.IsBaptized.HasValue)
                query = query.Where(m => m.IsBaptized == request.IsBaptized.Value);

            if (request.BaptizedDate.HasValue)
                query = query.Where(m => m.BaptizedDate.HasValue && m.BaptizedDate.Value.Date == request.BaptizedDate.Value.Date);

            if (request.RoleMember.HasValue)
                query = query.Where(m => m.Role == request.RoleMember.Value);

            // Novos filtros
            if (request.IsActive.HasValue)
                query = query.Where(m => m.IsActive == request.IsActive.Value);

            if (!string.IsNullOrWhiteSpace(request.Ministry))
                query = query.Where(m => EF.Functions.Like(m.Ministry, $"%{request.Ministry}%"));

            if (!string.IsNullOrWhiteSpace(request.MaritalStatus))
                query = query.Where(m => EF.Functions.Like(m.MaritalStatus, $"%{request.MaritalStatus}%"));

            if (request.MemberSince.HasValue)
                query = query.Where(m => m.MemberSince.HasValue && m.MemberSince.Value.Date == request.MemberSince.Value.Date);

            if (!string.IsNullOrWhiteSpace(request.Notes))
                query = query.Where(m => EF.Functions.Like(m.Notes, $"%{request.Notes}%"));

            // Conta o total de registros após os filtros
            var totalCount = await query.CountAsync(cancellationToken);

            // Aplica ordenação, paginação e projeta para DTO
            var items = query
                .OrderBy(m => m.Name)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(MemberDto.New)
                .ToList();

            // Retorna o resultado paginado
            return new PagedResultDto<MemberDto>(items, request.PageNumber, request.PageSize, totalCount);
        }
    }
}
