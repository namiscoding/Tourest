using Tourest.ViewModels.Tour;

namespace Tourest.Services
{
    public interface ITourManagerService
    {
        List<TourGuideListViewModel> GetAllTourGuides();
        TourGuideDetailViewModel GetDetail(int id);
        Task<List<TourGuideFeedbackViewModel>> GetFeedbacksByTourGuideIdAsync(int tourGuideUserId);
        Task<List<TourCustomerViewModel>> GetCustomersForTourAsync(int tourId);
        IEnumerable<TourListAllViewModel> GetAllTours();
        Task<TourListViewModel?> GetTourDetailsAsync(int id);
        Task CreateTourAsync(TourListViewModel tourViewModel);
        Task EditTourAsync(TourListViewModel tourViewModel);
        Task RemoveTourAsync(int id);


    }
}
