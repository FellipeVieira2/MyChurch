using MediatR;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MyChurch.Application.ImportedHymn.Commands
{
    public class UpdateImportedHymnCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Author { get; set; }
        public string? Lyrics { get; set; }
        public List<ImportedHymnStanzaDto> Stanzas { get; set; } = new();

        public class Handler : IRequestHandler<UpdateImportedHymnCommand, bool>
        {
            private readonly IUnitOfWork _unitOfWork;
            public Handler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }
            public async Task<bool> Handle(UpdateImportedHymnCommand request, CancellationToken cancellationToken)
            {
                var entity = await _unitOfWork.ImportedHymns.Query()
                    .Include(h => h.Stanzas)
                    .FirstOrDefaultAsync(h => h.Id == request.Id, cancellationToken);
                if (entity == null) return false;
                entity.Title = request.Title;
                entity.Author = request.Author;
                entity.Lyrics = request.Lyrics;
                entity.Stanzas = request.Stanzas.Select(s => new Domain.Entities.ImportedHymnStanza
                {
                    Id = s.Id,
                    Order = s.Order,
                    Text = s.Text,
                    ImportedHymnId = request.Id
                }).ToList();
                _unitOfWork.ImportedHymns.Update(entity);
                await _unitOfWork.CommitAsync();
                return true;
            }
        }
    }
}
