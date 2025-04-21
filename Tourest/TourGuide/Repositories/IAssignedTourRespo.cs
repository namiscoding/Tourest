using Tourest.Data.Entities;
using Tourest.TourGuide.ViewModels;

namespace Tourest.TourGuide.Repositories
{
    public interface IAssignedTourRespo
    {
        Task<List<TourGuideAssignmentViewModel>> GetTourAssigned(int tourGuideId);
    }
}
