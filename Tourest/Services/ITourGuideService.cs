using Tourest.ViewModels.TourGuide;

namespace Tourest.Services
{
    public interface ITourGuideService
    {
        Task<TourGuideDetailsViewModel?> GetTourGuideDetailsAsync(int userId);
    }
}
