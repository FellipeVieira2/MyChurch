using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Contracts;
using System.Text.Json.Serialization;

namespace MyChurch.Application.Hymn.Commands.DeleteHymn
{
    public class DeleteHymnCommand : IRequest<bool>
    {
        [JsonIgnore]
        public int Id { get; set; }
    }

    public class DeleteHymnCommandHandler : IRequestHandler<DeleteHymnCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteHymnCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteHymnCommand request, CancellationToken cancellationToken)
        {
            var hymn = await _unitOfWork.Hymns.Query()
                .FirstOrDefaultAsync(h => h.Id == request.Id, cancellationToken);

            if (hymn == null)
            {
                return false;
            }

            _unitOfWork.Hymns.Delete(hymn);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
