using Microsoft.EntityFrameworkCore;
using Tourest.Data.Entities;
namespace Tourest.Data.Repositories
{
    public class TourGuideRatingRepository : ITourGuideRatingRepository
    {
        private readonly ApplicationDbContext _context;
        public TourGuideRatingRepository(ApplicationDbContext context) { _context = context; }

        public async Task AddAsync(TourGuideRating tourGuideRating)
        {
            await _context.TourGuideRatings.AddAsync(tourGuideRating);
           
        }
    }
}
