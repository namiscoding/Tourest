using Tourest.Data.Entities;
using Tourest.ViewModels.Admin.AdminDashboard;

namespace Tourest.Data.Repositories
{
    public interface IRatingRepository
    {
        Task<Rating> AddAsync(Rating rating);
        Task<int> GetTotalTourRatingCountAsync();

    }
}
