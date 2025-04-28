using Tourest.ViewModels.Tour;
using Tourest.ViewModels.TourManager;
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
        Task<TourListViewModel?> GetTourByIdAsync(int id);
        Task<List<TourGuideAssignmentViewModel>> GetTourGuideScheduleAsync(int tourGuideId);
        Task<IEnumerable<UserViewModel>> GetUsersAsync();
        Task<UserViewModel> GetUserByIdAsync(int id);


    }
}
