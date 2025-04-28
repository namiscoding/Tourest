using Microsoft.EntityFrameworkCore;
using Tourest.Data.Entities;

namespace Tourest.Data.Repositories
{
    public class TourGroupRepository : ITourGroupRepository
    {
        private readonly ApplicationDbContext _context;

        public TourGroupRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TourGroup?> FindByTourAndDateAsync(int tourId, DateTime departureDate)
        {
            // So sánh phần Date của DateTime
            return await _context.TourGroups
                .FirstOrDefaultAsync(tg => tg.TourID == tourId && tg.DepartureDate.Date == departureDate.Date);
        }

        public async Task AddAsync(TourGroup tourGroup)
        {
            await _context.TourGroups.AddAsync(tourGroup);
            
        }

        public async Task UpdateAsync(TourGroup tourGroup)
        {
            _context.TourGroups.Update(tourGroup);
            
            await Task.CompletedTask; 
        }
    }
}
