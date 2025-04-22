using Tourest.Data.Entities;
using Tourest.TourGuide.ViewModels;

namespace Tourest.TourGuide.Services
{
    public interface ITourAssignmentService
    {
        Task<List<TourGuideAssignmentViewModel>> GetTourAssignmentsAsync(int tourGuideId);
    }
}
