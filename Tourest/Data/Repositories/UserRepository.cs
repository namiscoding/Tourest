using Microsoft.EntityFrameworkCore;
using Tourest.Data.Entities;

namespace Tourest.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context) { _context = context; }
        public async Task<User?> GetGuideByIdAsync(int userId)
        {
            return await _context.Users
                .Where(u => u.UserID == userId && u.TourGuide != null) 
                .Include(u => u.TourGuide) 
                .Include(u => u.TourGuideRatingsReceived) 
                    .ThenInclude(tgr => tgr.Rating)
                        .ThenInclude(r => r.Customer) 
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }
    }
}
