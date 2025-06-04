using System.Text.Json;
using System.Text.Json.Serialization;
using MediatR;
using Microsoft.AspNetCore.Http;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities.Bible;
using MyChurch.Infrastructure;

namespace MyChurch.Application.Bible.Commands.ImportBibleVersion
{
    public class ImportBibleVersionCommand : IRequest<int>
    {
        [JsonIgnore]
        public IFormFile File { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string Language { get; set; }
        public string? Description { get; set; }
        public string? Publisher { get; set; }
        public int? PublicationYear { get; set; }
    }
    public class ImportBibleVersionCommandHandler : IRequestHandler<ImportBibleVersionCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ImportBibleVersionCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Handle(ImportBibleVersionCommand request, CancellationToken cancellationToken)
        {
            if (request.File == null || request.File.Length == 0)
                throw new ArgumentException("Arquivo não enviado.");

            List<BibleBookJson>? bibleBooks;
            using (var stream = request.File.OpenReadStream())
            {
                bibleBooks = await JsonSerializer.DeserializeAsync<List<BibleBookJson>>(stream, cancellationToken: cancellationToken);
            }

            if (bibleBooks == null || !bibleBooks.Any())
                throw new ArgumentException("JSON inválido ou vazio.");

            var version = new Domain.Entities.Bible.Version
            {
                Name = request.Name,
                Abbreviation = request.Abbreviation,
                Language = request.Language,
                Description = request.Description,
                Publisher = request.Publisher,
                PublicationYear = request.PublicationYear,
                Books = new List<Book>()
            };

            int bookOrder = 1;
            int totalBooks = bibleBooks.Count;
            for (int i = 0; i < totalBooks; i++)
            {
                var bookJson = bibleBooks[i];
                var testament = i < 39 ? "Antigo Testamento" : "Novo Testamento";

                var book = new Book
                {
                    Name = GetBookNameByAbbrev(bookJson.abbrev),
                    Abbreviation = bookJson.abbrev,
                    Order = bookOrder++,
                    Testament = testament,
                    Chapters = new List<Chapter>()
                };

                int chapterNumber = 1;
                foreach (var chapterVerses in bookJson.chapters)
                {
                    var chapter = new Chapter
                    {
                        ChapterNumber = chapterNumber++,
                        Verses = chapterVerses.Select((text, idx) => new Verse
                        {
                            VerseNumber = idx + 1,
                            Text = text
                        }).ToList()
                    };
                    book.Chapters.Add(chapter);
                }
                version.Books.Add(book);
            }

            _unitOfWork.Versions.Create(version);
            await _unitOfWork.CommitAsync();

            return version.Id;
        }

        private class BibleBookJson
        {
            public string abbrev { get; set; }
            public List<List<string>> chapters { get; set; }
        }

        private string GetBookNameByAbbrev(string abbrev)
        {
            // Implemente o mapeamento real conforme sua necessidade
            return abbrev.ToUpper();
        }
    }
}