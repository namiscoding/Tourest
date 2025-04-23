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
            var assignedTours = _context.TourGuideAssignments
                .Where(m => m.TourGuideID == tourGuideId)
                .Include(a => a.TourGroup)
                    .ThenInclude(tg => tg.TourGuideRatings) 
                    .ThenInclude(tg => tg.Rating)
                .Include(a => a.TourGroup)
                    .ThenInclude(tg => tg.Bookings)       
                    .ThenInclude(b => b.Tour)            
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
                    AssignmentDate = a.AssignmentDate,
                    TourRating = a.TourGroup.TourGuideRatings.Select(r => r.Rating).ToList(),
                  
                })
                .ToListAsync();

            return assignedTours;
        }
        public async Task<TourGuideAssignmentViewModel> GetTourGuideRatingsAndComments(int tourGuideId, int tourGroupId)
        {
            
            var assignment = await _context.TourGuideAssignments
                .Where(tga => tga.TourGuideID == tourGuideId && tga.TourGroupID == tourGroupId)
                .Select(tga => new TourGuideAssignmentViewModel
                {
                    AssignmentId = tga.AssignmentID,
                    TourGuideId = tga.TourGuideID,
                    DepartureDate = tga.TourGroup.DepartureDate,
                    PickupPoint = tga.TourGroup.Bookings.Select(b => b.PickupPoint).FirstOrDefault(),
                    TotalAdults = tga.TourGroup.Bookings.Select(b => b.NumberOfAdults).FirstOrDefault(),
                    TotalChildren = tga.TourGroup.Bookings.Select(b => b.NumberOfChildren).FirstOrDefault(),
                    TourName = tga.TourGroup.Bookings.Select(b => b.Tour.Name).FirstOrDefault(),
                    Status = tga.Status,
                    AssignmentDate = tga.AssignmentDate
                })
                .FirstOrDefaultAsync();

            if (assignment == null)
                return null;

           
            assignment.TourRating = await _context.TourGuideRatings
                .Where(tgr => tgr.TourGuideID == tourGuideId && tgr.TourGroupID == tourGroupId)
                .Include(tgr => tgr.Rating)
                .Select(tgr => tgr.Rating)
                .ToListAsync();

            
            if (assignment.TourRating != null && assignment.TourRating.Any())
            {
                assignment.TourGuideRating = (double)assignment.TourRating.Average(r => r.RatingValue);
            }

            return assignment;
        }

      
    }
}
