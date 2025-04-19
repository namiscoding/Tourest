using Tourest.Data.Entities;

namespace Tourest.Data.Repositories
{
    public interface ITourRepository
    {
        Task<IEnumerable<Tour>> GetActiveToursAsync();
        Task<Tour?> GetByIdAsync(int id);
    }
}
