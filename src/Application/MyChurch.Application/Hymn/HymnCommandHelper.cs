using MyChurch.Application.Dtos;
using MyChurch.Domain.Entities.Bible;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Hymn
{
    internal static class HymnCommandHelper
    {
        public static void Validate(HymnUpsertDto hymn)
        {
            if (string.IsNullOrWhiteSpace(hymn.Title))
            {
                ValidationException.ThrowException(nameof(hymn.Title), "O título do hino é obrigatório.");
            }

            if (hymn.Number <= 0)
            {
                ValidationException.ThrowException(nameof(hymn.Number), "O número do hino deve ser maior que zero.");
            }

            if (hymn.Verses == null || hymn.Verses.Count == 0)
            {
                ValidationException.ThrowException(nameof(hymn.Verses), "Informe ao menos uma estrofe.");
            }

            var duplicatedVerseNumber = hymn.Verses
                .GroupBy(v => v.Number)
                .FirstOrDefault(g => g.Key <= 0 || g.Count() > 1);

            if (duplicatedVerseNumber != null)
            {
                ValidationException.ThrowException(nameof(hymn.Verses), "As estrofes devem ter numeração única e maior que zero.");
            }

            if (hymn.Verses.Any(v => string.IsNullOrWhiteSpace(v.Text)))
            {
                ValidationException.ThrowException(nameof(hymn.Verses), "Todas as estrofes devem possuir texto.");
            }
        }

        public static string NormalizeText(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            return text.Replace("\r\n", "\n").Trim();
        }

        public static List<HymnVerse> BuildVerses(IEnumerable<HymnVerseInputDto> verses)
        {
            return verses
                .OrderBy(v => v.Number)
                .Select(v => new HymnVerse
                {
                    Number = v.Number,
                    Text = NormalizeText(v.Text)
                })
                .ToList();
        }
    }
}
