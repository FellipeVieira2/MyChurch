using System.Threading.Tasks;
using MyChurch.Application.Services;
using MyChurch.Domain.Enum;

namespace MyChurch.Application.Services
{
    public class BibleService : IBibleService
    {
        public Task<string> GetVerseText(string book, int chapter, int startVerse, int endVerse)
        {
            return Task.FromResult($"{book} {chapter}:{startVerse}-{endVerse} (exemplo)");
        }
    }

    public class HymnService : IHymnService
    {

        Domain.Entities.Bible.HymnVerse IHymnService.GetHymnStanza(int hymnNumber, int stanzaNumber)
        {
            return new Domain.Entities.Bible.HymnVerse { Text = $"Hino {hymnNumber} Estrofe {stanzaNumber} (exemplo)" };
        }
    }

    public class HymnVerse
    {
        public string Text { get; set; }
    }
}
