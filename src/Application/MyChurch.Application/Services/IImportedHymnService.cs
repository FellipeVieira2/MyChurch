using System.Collections.Generic;
using System.Threading.Tasks;
using MyChurch.Application.Dtos;

namespace MyChurch.Application.Services
{
    public interface IImportedHymnService
    {
        Task<List<ImportedHymnDto>> GetAllAsync();
        Task<ImportedHymnDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(ImportedHymnDto dto);
        Task<bool> UpdateAsync(ImportedHymnDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
