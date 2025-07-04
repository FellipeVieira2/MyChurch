using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using System.Threading.Tasks;

namespace MyChurch.Infrastructure.Repositories
{
    public class ImportedHymnRepository : GenericRepository<ImportedHymn>, IImportedHymnRepository
    {
        public ImportedHymnRepository(MyChurchDbContext context) : base(context) { }

        public async Task<ImportedHymn?> GetByIdWithStanzasAsync(int id)
        {
            return await _context.Set<ImportedHymn>()
                .Include(h => h.Stanzas)
                .FirstOrDefaultAsync(h => h.Id == id);
        }
    }
}
