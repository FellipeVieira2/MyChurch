using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Feed.Post.Queries.GetFeedPostById
{
    public class GetFeedPostByIdQuery : JwtMemberDto, IRequest<FeedPostDto>
    {
        public int PostId { get; set; }
    }

    public class GetFeedPostByIdQueryHandler : IRequestHandler<GetFeedPostByIdQuery, FeedPostDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetFeedPostByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<FeedPostDto> Handle(GetFeedPostByIdQuery request, CancellationToken cancellationToken)
        {
            // Busca o membro logado para obter o ChurchId
            var loggedMember = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (loggedMember == null)
                throw new UnauthorizedAccessException("Usuário não encontrado.");

            // Busca a igreja do membro logado
            var church = await _unitOfWork.Churchs.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == loggedMember.ChurchId, cancellationToken);

            var query = _unitOfWork.FeedPosts.Query()
                .Include(p => p.Member)
                .Include(p => p.Likes)
                .Include(p => p.Images)
                .Where(p => p.Id == request.PostId);

            if (church != null && church.ParentChurchId.HasValue)
            {
                var parentId = church.ParentChurchId.Value;
                query = query.Where(p =>
                    p.ChurchId == loggedMember.ChurchId ||
                    (p.ChurchId == parentId && p.VisibleToBranches));
            }
            else
            {
                query = query.Where(p => p.ChurchId == loggedMember.ChurchId);
            }

            var post = await query.FirstOrDefaultAsync(cancellationToken);

            if (post == null)
                ValidationException.ThrowException("FeedPost", "Post não encontrado ou não pertence à sua igreja.");

            return FeedPostDto.New(post);
        }
    }
}
