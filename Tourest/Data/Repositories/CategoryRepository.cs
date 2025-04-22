using Microsoft.EntityFrameworkCore;
using Tourest.Data.Entities;

namespace Tourest.Data.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories
                                 .OrderBy(c => c.Name)
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(c => c.CategoryID == id);
        }
    }
}
