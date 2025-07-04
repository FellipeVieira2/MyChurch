using MyChurch.Application.Services;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.IO;
using MyChurch.Infrastructure.Utils.S3;
using Mychurch.Common.Utils.Objects;

namespace MyChurch.Application.Presentation.Services
{
    public class ContentGenerationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IS3Helper _s3Helper;

        public ContentGenerationService(IUnitOfWork unitOfWork, IS3Helper s3Helper)
        {
            _unitOfWork = unitOfWork;
            _s3Helper = s3Helper;
        }

        public async Task<Tuple<string, string>> GenerateSlideContent(SlideContentType type, string contentReferenceJson, int churchId)
        {
            dynamic contentRef = JsonConvert.DeserializeObject(contentReferenceJson);
            string displayText = null;
            string mediaUrl = null;

            switch (type)
            {
                case SlideContentType.BibleVerse:
                    int bookId = (int)contentRef.Book;
                    int chapter = (int)contentRef.Chapter;
                    int startVerse = (int)contentRef.StartVerse;
                    int endVerse = (int)contentRef.EndVerse;
                    var verses = await _unitOfWork.Verses.Query().Include(x => x.Chapter).ThenInclude(x => x.Book)
                        .Where(v => v.Id == startVerse )
                        .ToListAsync();
                    var verseText = string.Join(" ", verses.Select(v => v.Text));
                    displayText = $"{BibleBookNameProvider.GetBookName(verses.FirstOrDefault().Chapter.Book.Abbreviation) } {verses.FirstOrDefault().Chapter.ChapterNumber}:{verses.FirstOrDefault().VerseNumber}\n\n{verseText}";
                    break;
                case SlideContentType.HymnStanza:
                    int hymnNumber = (int)contentRef.HymnNumber;
                    int stanzaNumber = (int)contentRef.StanzaNumber;
                    string hymnVerse = null;
                    var hymn = await _unitOfWork.Hymns.Query()
                            .FirstOrDefaultAsync(h => h.Number == hymnNumber);
                    if (stanzaNumber == 0)
                    {
                        
                        hymnVerse = hymn.Chorus;
                    }
                    else
                    {
                        hymnVerse = _unitOfWork.HymnVerses.Query()
                            .FirstOrDefaultAsync(hv => hv.Number == stanzaNumber && hv.Hymn.Number == hymnNumber).Result?.Text;
                    }
                    // Troca <br> ou <br/> por \n para exibir corretamente na imagem
                    if (!string.IsNullOrEmpty(hymnVerse))
                    {
                        hymnVerse = hymnVerse.Replace("<br>", "\n").Replace("<br/>", "\n").Replace("<br />", "\n");
                    }
                    if (stanzaNumber == 1)
                    {

                        displayText = hymnVerse != null ? $"{hymn.Title} \n\n\n{hymnVerse}" : $"{hymn.Number}-{hymn.Title} -\n\nEstrofe {stanzaNumber}";
                    }else
                    {

                        displayText = hymnVerse != null ? $"{hymnVerse}" : $"{hymn.Number}-{hymn.Title} -\n\nEstrofe {stanzaNumber}";

                    }
                    break;
                case SlideContentType.ImportedHymnStanza:
                    int importedHymnId = (int)contentRef.ImportedHymnId;
                    int stanzaId = (int)contentRef.StanzaId;
                    var importedHymn = await _unitOfWork.ImportedHymns.GetByIdWithStanzasAsync(importedHymnId);
                    var stanza = importedHymn?.Stanzas.FirstOrDefault(s => s.Id == stanzaId);
                    if (importedHymn != null && stanza != null)
                    {
                        if (stanza.Order == 1)
                            displayText = $"{importedHymn.Title}\n\n{stanza.Text}";
                        else
                            displayText = stanza.Text;
                    }
                    else
                    {
                        displayText = "Estrofe não encontrada.";
                    }
                    break;
            }

            // Gera imagem e salva no S3
            if (!string.IsNullOrWhiteSpace(displayText))
            {
                byte[] imageBytes = SlideImageGenerator.GenerateImage(displayText);
                using var ms = new MemoryStream(imageBytes);
                string fileName = $"slides/{Guid.NewGuid()}.png";
                mediaUrl = await _s3Helper.UploadFileAsync(ms, fileName, "image/png");

            }

            return new Tuple<string, string>(displayText, mediaUrl);
        }
    }
}
