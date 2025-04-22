using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Tourest.Data;
using Tourest.Data.Entities;
using Tourest.TourGuide.ViewModels;

namespace Tourest.TourGuide.Repositories
{
    public class AssignedTourRepository : IAssignedTourRespo
    {
        ApplicationDbContext _context;
        public AssignedTourRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public Task<List<TourGuideAssignmentViewModel>> GetTourAssigned(int tourGuideId)
        {

            var assignedTours =  _context.TourGuideAssignments
               .Where(m => m.TourGuideID == tourGuideId)
               .Include(a => a.TourGroup)
                   .ThenInclude(g => g.Bookings)
                   .ThenInclude(z => z.Tour)
               .Select(a => new TourGuideAssignmentViewModel
               {
                   AssignmentId = a.AssignmentID,
                   TourGuideId = a.TourGuideID,
                   DepartureDate = a.TourGroup.DepartureDate,
                   PickupPoint = a.TourGroup.Bookings.Select(b => b.PickupPoint).FirstOrDefault(),
                   TotalAdults = a.TourGroup.Bookings.Select(b => b.NumberOfAdults).FirstOrDefault(),
                   TotalChildren = a.TourGroup.Bookings.Select(b => b.NumberOfChildren).FirstOrDefault(),
                   TourName = a.TourGroup.Bookings.Select(b => b.Tour.Name).FirstOrDefault(),
                   Status = a.Status,
                   AssignmentDate = a.AssignmentDate
                   
               })
               .ToListAsync();

            return assignedTours;
        }
    }
}
