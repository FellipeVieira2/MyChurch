using MediatR;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace MyChurch.Application.ImportedHymn.Commands
{
    public class CreateImportedHymnCommandHandler : IRequestHandler<CreateImportedHymnCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CreateImportedHymnCommandHandler(IUnitOfWork unitOfWork)
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
