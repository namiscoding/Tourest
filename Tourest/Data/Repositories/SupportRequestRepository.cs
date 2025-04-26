using Microsoft.EntityFrameworkCore;
using Tourest.Data.Entities;

namespace Tourest.Data.Repositories
{
    public class SupportRequestRepository : ISupportRequestRepository
    {
        private readonly ApplicationDbContext _context;

        public SupportRequestRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(SupportRequest request)
        {
            await _context.SupportRequests.AddAsync(request);
           
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<SupportRequest>> GetByCustomerIdAsync(int customerId)
        {
            return await _context.SupportRequests
                .Where(r => r.CustomerID == customerId)
                .OrderBy(r => r.SubmissionDate) 
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
