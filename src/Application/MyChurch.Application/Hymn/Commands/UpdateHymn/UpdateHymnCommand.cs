using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;
using System.Text.Json.Serialization;

namespace MyChurch.Application.Hymn.Commands.UpdateHymn
{
    public class UpdateHymnCommand : HymnUpsertDto, IRequest<bool>
    {
        [JsonIgnore]
        public int Id { get; set; }
    }

    public class UpdateHymnCommandHandler : IRequestHandler<UpdateHymnCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateHymnCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateHymnCommand request, CancellationToken cancellationToken)
        {
            HymnCommandHelper.Validate(request);

            var hymn = await _unitOfWork.Hymns.Query()
                .Include(h => h.HymnVerses)
                .FirstOrDefaultAsync(h => h.Id == request.Id, cancellationToken);

            if (hymn == null)
            {
                return false;
            }

            var duplicatedNumber = await _unitOfWork.Hymns.Query()
                .AsNoTracking()
                .AnyAsync(h => h.Id != request.Id && h.Number == request.Number, cancellationToken);

            if (duplicatedNumber)
            {
                ValidationException.ThrowException(nameof(request.Number), "Já existe um hino cadastrado com este número.");
            }

            hymn.Title = HymnCommandHelper.NormalizeText(request.Title);
            hymn.Number = request.Number;
            hymn.Language = string.IsNullOrWhiteSpace(request.Language) ? "pt-BR" : HymnCommandHelper.NormalizeText(request.Language);
            hymn.Chorus = HymnCommandHelper.NormalizeText(request.Chorus);
            hymn.LyricsAuthor = HymnCommandHelper.NormalizeText(request.LyricsAuthor);
            hymn.MelodyAuthor = HymnCommandHelper.NormalizeText(request.MelodyAuthor);

            hymn.HymnVerses.Clear();
            foreach (var verse in HymnCommandHelper.BuildVerses(request.Verses))
            {
                hymn.HymnVerses.Add(verse);
            }

            _unitOfWork.Hymns.Update(hymn);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
