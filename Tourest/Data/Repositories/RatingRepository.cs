using Microsoft.EntityFrameworkCore;
using Tourest.Data.Entities;

namespace Tourest.Data.Repositories
{
    public class RatingRepository : IRatingRepository
    {
        private readonly ApplicationDbContext _context;

        public RatingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Rating> AddAsync(Rating rating)
        {
            await _context.Ratings.AddAsync(rating);
            
            return rating; 
        }

        public async Task<int> GetTotalTourRatingCountAsync()
        {
            // Đếm các bản ghi Rating có RatingType là "Tour"
            // Hoặc đếm trực tiếp từ bảng TourRatings nếu bạn chỉ quan tâm đến sự tồn tại của đánh giá tour
            // Cách 1: Đếm từ Rating (nếu có RatingType)
            return await _context.Ratings.CountAsync(r => r.RatingType == "Tour");
            // Cách 2: Đếm từ TourRating
            // return await _context.TourRatings.CountAsync();
        }
    }
}
