using Tourest.Data.Entities;
using Tourest.ViewModels.Tour;

namespace Tourest.Data.Repositories
{
    public interface ITourRepository
    {
        
        Task<IEnumerable<Tour>> GetActiveToursAsync();
        Task<IEnumerable<Tour>> GetActiveToursAsync(
            IEnumerable<int>? categoryIds = null,
            IEnumerable<string>? destinations = null,
            IEnumerable<int>? ratings = null,
            string? sortBy = null, 
            int? minPrice = null, // THÊM
            int? maxPrice = null);
        Task<Tour?> GetByIdAsync(int id);
        Task<IEnumerable<string>> GetDistinctActiveDestinationsAsync();
    }
}
