using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mychurch.Common.Utils.Objects;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Member.Queries.GetAllCreditCardsMember
{
    public class GetAllCreditCardsMemberQuery : JwtMemberDto, IRequest<PagedResultDto<CreditCardInfoDto>>
    {
        /// <summary>Número da página (começa em 1)</summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>Tamanho da página</summary>
        public int PageSize { get; set; } = 20;
    }

    public class CreditCardInfoDto
    {
        public int Id { get; set; }
        public string Last4Digits { get; set; }
        public string CardBrand { get; set; }
        public string CardHash { get; set; }
        public string Created { get; set; }

        public static CreditCardInfoDto New(Domain.Entities.CreditCardInfo card)
        {
            return new CreditCardInfoDto
            {
                Id = card.Id,
                Last4Digits = card.Last4Digits,
                CardBrand = card.CardBrand,
                CardHash = card.CardHash,
                Created = card.Created.ToString("yyyy-MM-dd HH:mm:ss")
            };
        }
    }

    public class GetAllCreditCardsMemberHandler : IRequestHandler<GetAllCreditCardsMemberQuery, PagedResultDto<CreditCardInfoDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllCreditCardsMemberHandler> _logger;

        public GetAllCreditCardsMemberHandler(IUnitOfWork unitOfWork, ILogger<GetAllCreditCardsMemberHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<PagedResultDto<CreditCardInfoDto>> Handle(GetAllCreditCardsMemberQuery request, CancellationToken cancellationToken)
        {
            var member = await _unitOfWork.Members.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (member == null)
            {
                _logger.LogWarning("Usuário não encontrado.");
                ValidationException.ThrowException("Member", "Este membro não existe.");
            }

            var query = _unitOfWork.CreditCardInfos.Query()
                .AsNoTracking()
                .Where(c => c.MemberId == request.UserId);

            var totalCount = await query.CountAsync(cancellationToken);

            var cards = await query
                .OrderByDescending(c => c.Created)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var items = cards
                .Select(CreditCardInfoDto.New)
                .ToList();

            var pagedResult = new PagedResultDto<CreditCardInfoDto>(items, request.PageNumber, request.PageSize, totalCount);

            return pagedResult;
        }
    }
}