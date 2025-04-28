using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using Tourest.Data;
using Tourest.Data.Repositories;
using Tourest.ViewModels.Tour;
using Tourest.ViewModels.TourManager;

namespace Tourest.Services
{
    public class TourManagerService : ITourManagerService
    {
        private readonly TourManagerRepository? tourManagerRepository;
        public TourManagerService(TourManagerRepository? tourManagerRepository)
        {
            this.tourManagerRepository = tourManagerRepository;
        }

        public List<TourGuideListViewModel> GetAllTourGuides()
        {
            return tourManagerRepository.GetAllTourGuidesWithUserInfo();
        }

        public Task<List<TourCustomerViewModel>> GetCustomersForTourAsync(int tourId)
        {
            return tourManagerRepository.GetCustomersByTourIdAsync(tourId);
        }

        public TourGuideDetailViewModel GetDetail(int id)
        {
            return tourManagerRepository.GetTourGuideDetailById(id);
        }

        public async Task<List<TourGuideFeedbackViewModel>> GetFeedbacksByTourGuideIdAsync(int tourGuideUserId)
        {
            // Gọi bất đồng bộ từ repository
            return await tourManagerRepository.GetFeedbacksByTourGuideIdAsync(tourGuideUserId);
        }
        public IEnumerable<TourListAllViewModel> GetAllTours()
        {
            return tourManagerRepository.GetAllTours();
        }

        public Task<TourListViewModel?> GetTourDetailsAsync(int id)
        {
            return tourManagerRepository.GetTourByIdAsync(id);
        }

        public Task CreateTourAsync(TourListViewModel tourViewModel)
        {
            return tourManagerRepository.AddTourAsync(tourViewModel);
        }

        public Task EditTourAsync(TourListViewModel tourViewModel)
        {
            return tourManagerRepository.UpdateTourAsync(tourViewModel);
        }

        public Task RemoveTourAsync(int id)
        {
            return tourManagerRepository.DeleteTourAsync(id);
        }

        public Task<TourListViewModel?> GetTourByIdAsync(int id)
        {
            return tourManagerRepository.GetTourByIDAsync(id);
        }

        public Task<List<TourGuideAssignmentViewModel>> GetTourGuideScheduleAsync(int tourGuideId)
        {
            return tourManagerRepository.GetTourGuideScheduleAsync(tourGuideId);
        }

        public Task<IEnumerable<UserViewModel>> GetUsersAsync()
        {
            return tourManagerRepository.GetUsers();
        }

        public Task<UserViewModel> GetUserByIdAsync(int id)
        {
            return tourManagerRepository.GetUserByIdAsync(id);
        }
    }
}
