using Tourest.Data.Entities;
using Tourest.ViewModels.Tour;

namespace Tourest.Data.Repositories
{
    public interface ITourRepository
    {
        Task<IEnumerable<Tour>> GetActiveToursAsync();
        Task<Tour?> GetByIdAsync(int id);
        
    }
}
