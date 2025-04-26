using NuGet.Protocol.Core.Types;
using Tourest.Data.Entities;
using Tourest.TourGuide.Repositories;
using Tourest.TourGuide.ViewModels;

namespace Tourest.TourGuide.Services
{
    public class TourAssignmentService : ITourAssignmentService
    {
        private readonly IAssignedTourRespo _assignedTourRespo;

        public TourAssignmentService(IAssignedTourRespo assignedTourRespo)
        {
            _assignedTourRespo = assignedTourRespo;
        }

        public async Task<(bool success, string message)> AcceptAssignmentAsync(int assignmentId)
        {
            try
            {
                var assignment = await _assignedTourRespo.GetAssignmentByIdAsync(assignmentId);
                if (assignment == null)
                {
                    return (false, "Assignment not found");
                }

              
                assignment.Status = "Confirmed";
                assignment.ConfirmationDate = DateTime.Now;

               
                assignment.TourGroup.Status = "Assigned";
                assignment.TourGroup.AssignedTourGuideID = assignment.TourGuideID;

                await _assignedTourRespo.UpdateAssignmentAsync(assignment);
                await _assignedTourRespo.UpdateTourGroupAsync(assignment.TourGroup);

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<List<TourGuideAssignmentViewModel>> GetTourAssignmentsAsync(int tourGuideId)
        {
           
            return await _assignedTourRespo.GetTourAssigned(tourGuideId);
        }

        public  async Task<List<Tourest.TourGuide.ViewModels.TourGuideRatingViewModel>> GetTourGuideRatingsAndComments(int tourGuideId, int tourGroupId)
        {
            return await _assignedTourRespo.GetTourGuideRatingsAndComments(tourGuideId, tourGroupId);
        }

        public async Task<(bool success, string message)> RejectAssignmentAsync(int assignmentId, string reason)
        {
            try
            {
                var assignment = await _assignedTourRespo.GetAssignmentByIdAsync(assignmentId);
                if (assignment == null)
                {
                    return (false, "Assignment not found");
                }

                
                assignment.Status = "Rejected";
                assignment.RejectionReason = reason;

                
                assignment.TourGroup.Status = "PendingAssignment";
                assignment.TourGroup.AssignedTourGuideID = null;

                await _assignedTourRespo.UpdateAssignmentAsync(assignment);
                await _assignedTourRespo.UpdateTourGroupAsync(assignment.TourGroup);

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
