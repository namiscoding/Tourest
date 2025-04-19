using Tourest.ViewModels.Tour;

namespace Tourest.Services
{
    public interface ITourService
    {
        Task<IEnumerable<TourListViewModel>> GetActiveToursForDisplayAsync();
    }
}
