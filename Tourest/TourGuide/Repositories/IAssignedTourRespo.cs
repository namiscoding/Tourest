using Tourest.Data.Entities;
using Tourest.TourGuide.ViewModels;

namespace Tourest.TourGuide.Repositories
{
    public interface IAssignedTourRespo
    {
        Task<List<TourGuideAssignmentViewModel>> GetTourAssigned(int tourGuideId);
        Task<List<Tourest.TourGuide.ViewModels.TourGuideRatingViewModel>> GetTourGuideRatingsAndComments(int? tourGuideId, int ?tourGroupId);
        Task<TourGuideAssignment> GetAssignmentByIdAsync(int assignmentId);
        Task UpdateAssignmentAsync(TourGuideAssignment assignment);
        Task UpdateTourGroupAsync(TourGroup tourGroup);
    }
}
