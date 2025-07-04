using MediatR;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MyChurch.Application.ImportedHymn.Queries
{
    public class GetAllImportedHymnsQuery : IRequest<List<ImportedHymnDto>>
    {
        public class Handler : IRequestHandler<GetAllImportedHymnsQuery, List<ImportedHymnDto>>
        {
            private readonly IUnitOfWork _unitOfWork;
            public Handler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }
            public async Task<List<ImportedHymnDto>> Handle(GetAllImportedHymnsQuery request, CancellationToken cancellationToken)
            {
                var hymns = await _unitOfWork.ImportedHymns.Query()
                    .Include(h => h.Stanzas)
                    .ToListAsync(cancellationToken);
                return hymns.Select(h => new ImportedHymnDto
                {
                    Id = h.Id,
                    Title = h.Title,
                    Author = h.Author,
                    Lyrics = h.Lyrics,
                    Stanzas = h.Stanzas?.OrderBy(s => s.Order).Select(s => new ImportedHymnStanzaDto
                    {
                        Id = s.Id,
                        Order = s.Order,
                        Text = s.Text
                    }).ToList() ?? new List<ImportedHymnStanzaDto>()
                }).ToList();
            }
        }
    }
}
