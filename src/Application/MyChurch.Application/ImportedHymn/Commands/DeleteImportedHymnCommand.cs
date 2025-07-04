using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Contracts;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace MyChurch.Application.ImportedHymn.Commands
{
    public class DeleteImportedHymnCommand : IRequest<bool>
    {
        [JsonIgnore]
        public int Id { get; set; }

        public class Handler : IRequestHandler<DeleteImportedHymnCommand, bool>
        {
            private readonly IUnitOfWork _unitOfWork;
            public Handler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }
            public async Task<bool> Handle(DeleteImportedHymnCommand request, CancellationToken cancellationToken)
            {
                var entity = await _unitOfWork.ImportedHymns.Query().FirstOrDefaultAsync(x => x.Id == request.Id);
                if (entity == null) return false;
                _unitOfWork.ImportedHymns.Delete(entity);
                await _unitOfWork.CommitAsync();
                return true;
            }
        }
    }
}
