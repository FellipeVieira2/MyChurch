using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using Mychurch.Common.Utils.Objects;

namespace MyChurch.Application.Feed.Post.Queries.GetAllFeedPosts
{
    public class GetAllFeedPostsQuery : JwtMemberDto, IRequest<PagedResultDto<FeedPostDto>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GetAllFeedPostsQueryHandler : IRequestHandler<GetAllFeedPostsQuery, PagedResultDto<FeedPostDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllFeedPostsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResultDto<FeedPostDto>> Handle(GetAllFeedPostsQuery request, CancellationToken cancellationToken)
        {
            // Busca o membro logado para obter o ChurchId
            var loggedMember = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (loggedMember == null)
                throw new UnauthorizedAccessException("Usuário não encontrado.");

            int churchId = loggedMember.ChurchId;

            var query = _unitOfWork.FeedPosts.Query()
                .Include(p => p.Member)
                .Include(p => p.Likes)
                .Where(p => p.ChurchId == churchId);

            var total = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(p => p.Created)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResultDto<FeedPostDto>
            {
                TotalCount = total,
                PageNumber = request.Page,
                PageSize = request.PageSize,
                Items = items.Select(FeedPostDto.New).ToList()
            };
        }
    }
}
