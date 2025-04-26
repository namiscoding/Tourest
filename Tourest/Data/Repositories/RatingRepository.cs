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
    }
}
