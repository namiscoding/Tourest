using Tourest.Data.Entities;
using Tourest.ViewModels.Tour;

namespace Tourest.Data.Repositories
{
    public interface ITourRepository
    {
        
        Task<IEnumerable<Tour>> GetActiveToursAsync();
        Task<IEnumerable<Tour>> GetActiveToursAsync(
            IEnumerable<int>? categoryIds = null,
            IEnumerable<string>? destinations = null,
            IEnumerable<int>? ratings = null,
            string? sortBy = null, 
            int? minPrice = null, // THÊM
            int? maxPrice = null,
             // --- THÊM THAM SỐ ---
             string? searchDestination = null,
             string? searchCategoryName = null,
             DateTime? searchDate = null, // Nhận nhưng chưa dùng để lọc
             int? searchGuests = null);
        Task<Tour?> GetByIdAsync(int id);
        Task<IEnumerable<string>> GetDistinctActiveDestinationsAsync();
        //Task<(IEnumerable<Tour> Tours, int TotalCount)> GetToursPagedAsync(int pageIndex, int pageSize, string? searchTerm, string? statusFilter);
        //Task<Tour?> GetTourDetailsByIdAsync(int tourId); 
        //Task<Tour?> GetTourForEditByIdAsync(int tourId);
        //Task<Tour> AddTourAsync(Tour tour); 
        //Task UpdateTourAsync(Tour tour); 
        //Task DeleteTourAsync(int tourId); 
        //Task UpdateTourCategoriesAsync(int tourId, List<int> selectedCategoryIds); 
        //Task UpdateItineraryAsync(int tourId, List<ItineraryDay> currentItinerary); 
        //Task<bool> IsTourInUseAsync(int tourId); 
        Task<int> GetActiveTourCountAsync();
        Task<int> GetDistinctDestinationCountAsync();
        Task<IEnumerable<Tour>> GetFeaturedToursAsync(int count);
    }
}