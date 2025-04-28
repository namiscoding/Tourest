using Tourest.Data.Entities;

namespace Tourest.Data.Repositories
{
    public interface ITourGroupRepository
    {
        Task<TourGroup?> FindByTourAndDateAsync(int tourId, DateTime departureDate);
        Task AddAsync(TourGroup tourGroup);
        Task UpdateAsync(TourGroup tourGroup);
    }
}
