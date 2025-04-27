using Tourest.Data.Entities;

namespace Tourest.Data.Repositories
{
    public interface IRatingRepository
    {
        Task<Rating> AddAsync(Rating rating);
        Task<int> GetTotalTourRatingCountAsync();
    }
}
