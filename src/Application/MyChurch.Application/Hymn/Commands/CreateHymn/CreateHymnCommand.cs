using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Hymn.Commands.CreateHymn
{
    public class CreateHymnCommand : HymnUpsertDto, IRequest<HymnDto>
    {
    }

    public class CreateHymnCommandHandler : IRequestHandler<CreateHymnCommand, HymnDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateHymnCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<HymnDto> Handle(CreateHymnCommand request, CancellationToken cancellationToken)
        {
            HymnCommandHelper.Validate(request);

            var alreadyExists = await _unitOfWork.Hymns.Query()
                .AsNoTracking()
                .AnyAsync(h => h.Number == request.Number, cancellationToken);

            if (alreadyExists)
            {
                ValidationException.ThrowException(nameof(request.Number), "Já existe um hino cadastrado com este número.");
            }

            var hymn = new Domain.Entities.Bible.Hymn
            {
                Title = HymnCommandHelper.NormalizeText(request.Title),
                Number = request.Number,
                Language = string.IsNullOrWhiteSpace(request.Language) ? "pt-BR" : HymnCommandHelper.NormalizeText(request.Language),
                Chorus = HymnCommandHelper.NormalizeText(request.Chorus),
                LyricsAuthor = HymnCommandHelper.NormalizeText(request.LyricsAuthor),
                MelodyAuthor = HymnCommandHelper.NormalizeText(request.MelodyAuthor),
                HymnVerses = HymnCommandHelper.BuildVerses(request.Verses)
            };

            await _unitOfWork.Hymns.Create(hymn);
            await _unitOfWork.CommitAsync();

            return new HymnDto
            {
                Id = hymn.Id,
                Title = hymn.Title,
                Number = hymn.Number,
                Language = hymn.Language,
                Chorus = hymn.Chorus,
                LyricsAuthor = hymn.LyricsAuthor,
                MelodyAuthor = hymn.MelodyAuthor,
                Verses = hymn.HymnVerses
                    .OrderBy(v => v.Number)
                    .Select(v => new HymnVerseDto
                    {
                        Id = v.Id,
                        Number = v.Number,
                        Text = v.Text
                    })
                    .ToList()
            };
        }
    }
}
