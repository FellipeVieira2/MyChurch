using MyChurch.Application.Dtos;
using MyChurch.Application.Presentation.Services;
using System.Drawing;

namespace MyChurch.Application.Hymn.Services
{
    public class HymnPresentationPreviewService
    {
        public HymnPresentationPreviewDto BuildPreview(HymnUpsertDto hymn)
        {
            HymnCommandHelper.Validate(hymn);

            var preview = new HymnPresentationPreviewDto
            {
                Title = HymnCommandHelper.NormalizeText(hymn.Title)
            };

            var order = 1;
            if (!string.IsNullOrWhiteSpace(hymn.Chorus))
            {
                order = AddSlides(preview.Slides, order, "Coro", null, true, HymnCommandHelper.NormalizeText(hymn.Chorus));
            }

            foreach (var verse in hymn.Verses.OrderBy(v => v.Number))
            {
                var verseText = HymnCommandHelper.NormalizeText(verse.Text);
                var displayText = verse.Number == 1
                    ? $"{preview.Title}\n\n\n{verseText}"
                    : verseText;

                order = AddSlides(preview.Slides, order, $"Estrofe {verse.Number}", verse.Number, false, displayText);
            }

            return preview;
        }

        private static int AddSlides(List<HymnPreviewSlideDto> slides, int orderStart, string section, int? verseNumber, bool isChorus, string displayText)
        {
            var pagedTexts = Paginate(displayText);

            for (var index = 0; index < pagedTexts.Count; index++)
            {
                var pageText = pagedTexts[index];
                slides.Add(new HymnPreviewSlideDto
                {
                    Order = orderStart + index,
                    Section = pagedTexts.Count == 1 ? section : $"{section} - parte {index + 1}",
                    VerseNumber = verseNumber,
                    IsChorus = isChorus,
                    DisplayText = pageText,
                    CharacterCount = pageText.Length,
                    LineCount = pageText.Split('\n').Length,
                    PreviewImageBase64 = $"data:image/png;base64,{Convert.ToBase64String(SlideImageGenerator.GenerateImage(pageText))}"
                });
            }

            return orderStart + pagedTexts.Count;
        }

        private static List<string> Paginate(string text)
        {
            var wrappedLines = WrapLines(text);
            var pages = new List<string>();

            using var bitmap = new Bitmap(1, 1);
            using var graphics = Graphics.FromImage(bitmap);
            using var font = new Font(SlideImageGenerator.FontFamilyName, SlideImageGenerator.FontSize, FontStyle.Bold);
            using var format = new StringFormat(StringFormatFlags.LineLimit)
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            var pageLines = new List<string>();
            foreach (var line in wrappedLines)
            {
                var candidateLines = pageLines.Concat(new[] { line }).ToList();
                var candidateText = string.Join("\n", candidateLines);
                var measuredHeight = graphics.MeasureString(candidateText, font, new SizeF(SlideImageGenerator.TextAreaWidth, SlideImageGenerator.TextAreaHeight), format).Height;

                if (pageLines.Count > 0 && measuredHeight > SlideImageGenerator.TextAreaHeight)
                {
                    pages.Add(string.Join("\n", pageLines));
                    pageLines = new List<string> { line };
                    continue;
                }

                pageLines = candidateLines;
            }

            if (pageLines.Count > 0)
            {
                pages.Add(string.Join("\n", pageLines));
            }

            return pages.Count == 0 ? new List<string> { text } : pages;
        }

        private static List<string> WrapLines(string text)
        {
            var lines = new List<string>();

            using var bitmap = new Bitmap(1, 1);
            using var graphics = Graphics.FromImage(bitmap);
            using var font = new Font(SlideImageGenerator.FontFamilyName, SlideImageGenerator.FontSize, FontStyle.Bold);

            foreach (var rawLine in text.Split('\n'))
            {
                if (string.IsNullOrWhiteSpace(rawLine))
                {
                    lines.Add(string.Empty);
                    continue;
                }

                var words = rawLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var currentLine = string.Empty;

                foreach (var word in words)
                {
                    var candidate = string.IsNullOrWhiteSpace(currentLine) ? word : $"{currentLine} {word}";
                    var candidateWidth = graphics.MeasureString(candidate, font).Width;

                    if (candidateWidth <= SlideImageGenerator.TextAreaWidth)
                    {
                        currentLine = candidate;
                        continue;
                    }

                    if (!string.IsNullOrWhiteSpace(currentLine))
                    {
                        lines.Add(currentLine);
                        currentLine = string.Empty;
                    }

                    currentLine = BreakLongWord(word, graphics, font, lines);
                }

                if (!string.IsNullOrWhiteSpace(currentLine))
                {
                    lines.Add(currentLine);
                }
            }

            return lines;
        }

        private static string BreakLongWord(string word, Graphics graphics, Font font, List<string> lines)
        {
            if (graphics.MeasureString(word, font).Width <= SlideImageGenerator.TextAreaWidth)
            {
                return word;
            }

            var current = string.Empty;
            foreach (var character in word)
            {
                var candidate = $"{current}{character}";
                if (graphics.MeasureString(candidate, font).Width <= SlideImageGenerator.TextAreaWidth)
                {
                    current = candidate;
                    continue;
                }

                if (!string.IsNullOrEmpty(current))
                {
                    lines.Add(current);
                }

                current = character.ToString();
            }

            return current;
        }
    }
}
