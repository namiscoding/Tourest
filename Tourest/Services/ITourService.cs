using Tourest.Helpers;
using Tourest.ViewModels.Admin.AdminTour;
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
            int? maxPrice = null,
             string? searchDestination = null,
             string? searchCategoryName = null,
             DateTime? searchDate = null,
             int? searchGuests = null);
        Task<IEnumerable<string>> GetDestinationsForFilterAsync();
        Task<TourDetailsViewModel?> GetTourDetailsAsync(int id);
        Task<int> GetActiveTourCountAsync();
        Task<int> GetDistinctDestinationCountAsync();
        Task<List<TourListViewModel>> GetFeaturedToursForDisplayAsync(int count);
        Task<PaginatedList<AdminTourListViewModel>> GetToursForAdminAsync(int pageIndex, int pageSize, string? searchTerm, string? statusFilter);
        Task<AdminTourDetailsViewModel?> GetTourDetailsForAdminAsync(int tourId);
        Task<CreateEditTourViewModel?> GetTourForCreateAsync(); // Lấy Categories cho form Create
        Task<CreateEditTourViewModel?> GetTourForEditAsync(int tourId); // Lấy dữ liệu Tour + Categories cho form Edit
        Task<(bool Success, string ErrorMessage, int? CreatedTourId)> CreateTourAsync(CreateEditTourViewModel model, List<IFormFile>? imageFiles);
        Task<(bool Success, string ErrorMessage)> UpdateTourAsync(CreateEditTourViewModel model, List<IFormFile>? newImageFiles, List<string>? imagesToDeletePublicIds);
        Task<(bool Success, string ErrorMessage)> DeleteTourAsync(int tourId);
        Task<DeleteTourConfirmationViewModel?> GetTourForDeleteConfirmationAsync(int tourId); // Phương thức mới


    }
}
