using Tourest.ViewModels.Tour;

namespace Tourest.Services
{
    public interface ITourService
    {
        Task<IEnumerable<TourListViewModel>> GetActiveToursForDisplayAsync();
        Task<IEnumerable<TourListViewModel>> GetActiveToursForDisplayAsync(
            IEnumerable<int>? categoryIds = null,
            IEnumerable<string>? destinations = null,
            IEnumerable<int>? ratings = null,
            string? sortBy = null,
            int? minPrice = null, // THÊM
            int? maxPrice = null);
        Task<IEnumerable<string>> GetDestinationsForFilterAsync();
        Task<TourDetailsViewModel?> GetTourDetailsAsync(int id);
    }
}
