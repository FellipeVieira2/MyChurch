using MyChurch.Domain.Entities;

namespace MyChurch.Domain.Contracts
{
    public interface IImportedHymnRepository : IGenericRepository<ImportedHymn>
    {
        // Métodos customizados se necessário
        Task<ImportedHymn?> GetByIdWithStanzasAsync(int id);
    }
}
