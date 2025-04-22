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

    }
}
