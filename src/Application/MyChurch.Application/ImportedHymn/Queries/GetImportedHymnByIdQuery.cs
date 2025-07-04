using MediatR;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyChurch.Application.ImportedHymn.Queries
{
    public class GetImportedHymnByIdQuery : IRequest<ImportedHymnDto?>
    {
        public int Id { get; set; }

        public class Handler : IRequestHandler<GetImportedHymnByIdQuery, ImportedHymnDto?>
        {
            private readonly IUnitOfWork _unitOfWork;
            public Handler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }
            public async Task<ImportedHymnDto?> Handle(GetImportedHymnByIdQuery request, CancellationToken cancellationToken)
            {
                var hymn = await _unitOfWork.ImportedHymns.GetByIdWithStanzasAsync(request.Id);
                if (hymn == null) return null;
                return new ImportedHymnDto
                {
                    Id = hymn.Id,
                    Title = hymn.Title,
                    Author = hymn.Author,
                    Lyrics = hymn.Lyrics,
                    Stanzas = hymn.Stanzas?.OrderBy(s => s.Order).Select(s => new ImportedHymnStanzaDto
                    {
                        Id = s.Id,
                        Order = s.Order,
                        Text = s.Text
                    }).ToList() ?? new System.Collections.Generic.List<ImportedHymnStanzaDto>()
                };
            }
        }
    }
}
