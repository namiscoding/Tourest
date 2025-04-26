using Tourest.Data.Entities;

namespace Tourest.Data.Repositories
{
    public class TourRatingRepository : ITourRatingRepository
    {
        private readonly ApplicationDbContext _context;

        public TourRatingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TourRating tourRating)
        {
            await _context.TourRatings.AddAsync(tourRating);
            // KHÔNG gọi SaveChangesAsync() ở đây
        }
    }
}
