using MediatR;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyChurch.Application.ImportedHymn.Commands
{
    public class CreateImportedHymnCommand : IRequest<int>
    {
        public string Title { get; set; }
        public string? Author { get; set; }
        public string? Lyrics { get; set; }
        public List<ImportedHymnStanzaDto> Stanzas { get; set; } = new();

        public class Handler : IRequestHandler<CreateImportedHymnCommand, int>
        {
            private readonly IUnitOfWork _unitOfWork;
            public Handler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }
            public async Task<int> Handle(CreateImportedHymnCommand request, CancellationToken cancellationToken)
            {
                var entity = new Domain.Entities.ImportedHymn
                {
                    Title = request.Title,
                    Author = request.Author,
                    Lyrics = request.Lyrics,
                    Stanzas = request.Stanzas.Select(s => new ImportedHymnStanza
                    {
                        Order = s.Order,
                        Text = s.Text
                    }).ToList()
                };
                _unitOfWork.ImportedHymns.Create(entity);
                await _unitOfWork.CommitAsync();
                return entity.Id;
            }
        }
    }
}
