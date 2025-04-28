using Tourest.Data.Entities;
namespace Tourest.Data.Repositories
{
      public interface ITourGuideRatingRepository
        {
            Task AddAsync(TourGuideRating tourGuideRating);
            // Task<bool> ExistsAsync(int tourGroupId, int customerId); // Kiểm tra đã rate guide trong group này chưa
        }
    
}
