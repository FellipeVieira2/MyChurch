using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Common.Extensions;
using MyChurch.Application.Common.Models;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Member.Queries.GetAllMembers
{
    public class GetAllMembersQuery : JwtMemberDto, IRequest<PaginatedList<MemberDto>>
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
        
        /// <summary>Filtro por aprovação pendente</summary>
        public bool PendingApproval { get; set; } = false;

        /// <summary>Filtro por ministério</summary>
        public Ministry? Ministry { get; set; }

        /// <summary>Filtro por estado civil</summary>
        public MaritalStatus? MaritalStatus { get; set; }

        /// <summary>Filtro por data de entrada como membro</summary>
        public DateTime? MemberSince { get; set; }

        /// <summary>Filtro por observações (contém)</summary>
        public string? Notes { get; set; }

        /// <summary>Número da página (começa em 1)</summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>Tamanho da página (máximo 100)</summary>
        public int PageSize { get; set; } = 20;

        /// <summary>Campo para ordenação (Name, Email, BirthDate, Created)</summary>
        public string? SortBy { get; set; } = "Name";

        /// <summary>Direção da ordenação (asc ou desc)</summary>
        public string SortDirection { get; set; } = "asc";
    }

    public class GetAllMembersQueryHandler : IRequestHandler<GetAllMembersQuery, PaginatedList<MemberDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllMembersQueryHandler> _logger;

        public GetAllMembersQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAllMembersQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<PaginatedList<MemberDto>> Handle(GetAllMembersQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Getting members list - Page: {PageNumber}, PageSize: {PageSize}, SortBy: {SortBy}",
                request.PageNumber,
                request.PageSize,
                request.SortBy);

            // Busca o membro logado para obter o ChurchId
            var member = await _unitOfWork.Members.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (member == null)
            {
                _logger.LogWarning("User {UserId} not found", request.UserId);
                ValidationException.ThrowException("Member", "This Member does not exist.");
            }

            int churchId = member.ChurchId;

            // Inicia a query base filtrando por igreja
            var query = _unitOfWork.Members.Query()
                .AsNoTracking()
                .Include(x => x.Address)
                .Include(x => x.Documents)
                .Where(m => m.ChurchId == churchId);

            // Aplicar filtros
            query = ApplyFilters(query, request);

            // Aplica ordenação
            query = ApplySorting(query, request);

            // Conta o total
            var totalCount = await query.CountAsync(cancellationToken);

            // Aplica paginação manualmente e projeta
            var entities = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var items = entities.Select(MemberDto.New).ToList();

            // Cria resultado paginado
            var result = new PaginatedList<MemberDto>(
                items,
                totalCount,
                request.PageNumber,
                request.PageSize);

            _logger.LogInformation(
                "Retrieved {Count} members from {TotalCount} total (Page {PageNumber}/{TotalPages})",
                result.Items.Count,
                result.TotalCount,
                result.PageNumber,
                result.TotalPages);

            return result;
        }

        private static IQueryable<Domain.Entities.Member> ApplyFilters(
            IQueryable<Domain.Entities.Member> query,
            GetAllMembersQuery request)
        {
            if (!string.IsNullOrWhiteSpace(request.Name))
                query = query.Where(m => EF.Functions.Like(m.Name, $"%{request.Name}%"));

            if (!string.IsNullOrWhiteSpace(request.Document))
                query = query.Where(m => m.Documents.Any(x => x.Number == request.Document));

            if (!string.IsNullOrWhiteSpace(request.Email))
                query = query.Where(m => EF.Functions.Like(m.Email, $"%{request.Email}%"));

            if (request.BirthDate.HasValue)
                query = query.Where(m => m.BirthDate.Date == request.BirthDate.Value.Date);

            if (request.IsBaptized.HasValue)
                query = query.Where(m => m.IsBaptized == request.IsBaptized.Value);

            if (request.BaptizedDate.HasValue)
                query = query.Where(m => m.BaptizedDate.HasValue && 
                                        m.BaptizedDate.Value.Date == request.BaptizedDate.Value.Date);

            if (request.RoleMember.HasValue)
                query = query.Where(m => m.Role == request.RoleMember.Value);

            if (request.IsActive.HasValue)
                query = query.Where(m => m.IsActive == request.IsActive.Value);

            if (request.Ministry.HasValue)
                query = query.Where(m => m.Ministry == request.Ministry.Value.ToString());

            if (request.MaritalStatus.HasValue)
                query = query.Where(m => m.MaritalStatus == request.MaritalStatus.Value);

            if (request.MemberSince.HasValue)
                query = query.Where(m => m.MemberSince.HasValue && 
                                        m.MemberSince.Value.Date == request.MemberSince.Value.Date);

            if (!string.IsNullOrWhiteSpace(request.Notes))
                query = query.Where(m => EF.Functions.Like(m.Notes, $"%{request.Notes}%"));

            query = query.Where(m => m.PendingApproval == request.PendingApproval);

            return query;
        }

        private static IQueryable<Domain.Entities.Member> ApplySorting(
            IQueryable<Domain.Entities.Member> query,
            GetAllMembersQuery request)
        {
            var isDescending = request.SortDirection?.ToLower() == "desc";

            return request.SortBy?.ToLower() switch
            {
                "email" => isDescending ? query.OrderByDescending(m => m.Email) : query.OrderBy(m => m.Email),
                "birthdate" => isDescending ? query.OrderByDescending(m => m.BirthDate) : query.OrderBy(m => m.BirthDate),
                "created" => isDescending ? query.OrderByDescending(m => m.Created) : query.OrderBy(m => m.Created),
                _ => isDescending ? query.OrderByDescending(m => m.Name) : query.OrderBy(m => m.Name)
            };
        }
    }
}
