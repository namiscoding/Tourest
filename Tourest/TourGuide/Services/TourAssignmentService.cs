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

       
        public async Task<List<TourGuideAssignmentViewModel>> GetTourAssignmentsAsync(int tourGuideId)
        {
           
            return await _assignedTourRespo.GetTourAssigned(tourGuideId);
        }
    }
}
