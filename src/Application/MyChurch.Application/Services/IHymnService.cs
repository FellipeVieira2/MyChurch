using MyChurch.Domain.Entities;
using System.Threading.Tasks;

namespace MyChurch.Application.Services
{
    public interface IHymnService
    {
        Domain.Entities.Bible.HymnVerse GetHymnStanza(int hymnNumber, int stanzaNumber);
    }
}