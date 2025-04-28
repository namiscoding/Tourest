using Tourest.Data.Entities;
using Tourest.ViewModels.Tour;
using Tourest.ViewModels.TourManager;

namespace Tourest.Data.Repositories
{
    public interface ITourManagerRepository
    {
        List<TourGuideListViewModel> GetAllTourGuidesWithUserInfo();
        TourGuideDetailViewModel GetTourGuideDetailById(int id);
        Task<List<TourGuideFeedbackViewModel>> GetFeedbacksByTourGuideIdAsync(int tourGuideUserId);
        Task<List<TourCustomerViewModel>> GetCustomersByTourIdAsync(int tourId);
        IEnumerable<TourListAllViewModel> GetAllTours();
        Task<TourListViewModel?> GetTourByIdAsync(int id);
        Task AddTourAsync(TourListViewModel tourViewModel);
        Task UpdateTourAsync(TourListViewModel tourViewModel);
        Task DeleteTourAsync(int id);
        Task<TourListViewModel?> GetTourByIDAsync(int tourId);
        Task<List<TourGuideAssignmentViewModel>> GetTourGuideScheduleAsync(int tourGuideId);
        Task<IEnumerable<UserViewModel>> GetUsers();
        Task<UserViewModel> GetUserByIdAsync(int id);
    }
}
