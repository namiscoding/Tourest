using Tourest.Data.Entities;

namespace Tourest.Data.Repositories
{
    public interface ITourRatingRepository
    {
        Task AddAsync(TourRating tourRating);
    }
}
