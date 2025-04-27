using Tourest.ViewModels.TourRating;
namespace Tourest.Services
{
    public interface IRatingService
    {
        Task<(bool Success, string ErrorMessage)> AddTourRatingAsync(CreateTourRatingViewModel model, int customerId);
        Task<int> GetTotalTourRatingCountAsync();
    }
}
