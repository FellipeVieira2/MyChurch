using System.Threading.Tasks;

namespace MyChurch.Application.Services
{
    public interface IBibleService
    {
        Task<string> GetVerseText(string book, int chapter, int startVerse, int endVerse);
    }
}