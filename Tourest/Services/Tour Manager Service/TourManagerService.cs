using Microsoft.EntityFrameworkCore;
using Tourest.Data;
using Tourest.Data.Repositories;
using Tourest.ViewModels.Tour;

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

        public TourGuideDetailViewModel GetDetail(int id)
        {
            return tourManagerRepository.GetTourGuideDetailById(id);
        }

        public async Task<List<TourGuideFeedbackViewModel>> GetFeedbacksByTourGuideIdAsync(int tourGuideUserId)
        {
            // Gọi bất đồng bộ từ repository
            return await tourManagerRepository.GetFeedbacksByTourGuideIdAsync(tourGuideUserId);
        }
    }
}
