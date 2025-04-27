using Tourest.Data.Entities;
using Tourest.TourGuide.ViewModels;

namespace Tourest.TourGuide.Services
{
    public interface ITourAssignmentService
    {
        Task<List<TourGuideAssignmentViewModel>> GetTourAssignmentsAsync(int tourGuideId);
        Task<List<Tourest.TourGuide.ViewModels.TourGuideRatingViewModel>> GetTourGuideRatingsAndComments(int? tourGuideId, int ?tourGroupId);
        Task<(bool success, string message)> AcceptAssignmentAsync(int assignmentId);
        Task<(bool success, string message)> RejectAssignmentAsync(int assignmentId, string reason);
    }
}
