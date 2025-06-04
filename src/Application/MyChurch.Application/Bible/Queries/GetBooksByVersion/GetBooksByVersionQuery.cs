using MediatR;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;

namespace MyChurch.Application.Bible.Queries.GetBooksByVersion
{
    public class GetBooksByVersionQuery : JwtMemberDto, IRequest<List<BibleBookDto>>
    {
        public int VersionId { get; set; }
    }

    public class GetBooksByVersionQueryHandler : IRequestHandler<GetBooksByVersionQuery, List<BibleBookDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetBooksByVersionQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<BibleBookDto>> Handle(GetBooksByVersionQuery request, CancellationToken cancellationToken)
        {
            var books = _unitOfWork.Books.Query()
                .Where(b => b.VersionId == request.VersionId)
                .OrderBy(b => b.Order)
                .ToList();

            // Explicitly specify the type argument for the Select method to resolve CS0411
            return books.Select(book => BibleBookDto.New(book)).ToList();
        }
    }
}