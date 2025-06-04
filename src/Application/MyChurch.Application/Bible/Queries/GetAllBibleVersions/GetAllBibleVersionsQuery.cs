using MediatR;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace MyChurch.Application.Bible.Queries.GetAllBibleVersions
{
    public class GetAllBibleVersionsQuery : JwtMemberDto, IRequest<List<BibleVersionDto>>
    {
    }

    public class GetAllBibleVersionsQueryHandler : IRequestHandler<GetAllBibleVersionsQuery, List<BibleVersionDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllBibleVersionsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<BibleVersionDto>> Handle(GetAllBibleVersionsQuery request, CancellationToken cancellationToken)
        {
            var versions = _unitOfWork.Versions.Query().Where(x => !string.IsNullOrEmpty(x.Language)).ToList();

            return [.. versions.Select(BibleVersionDto.New)];
        }
    }
}