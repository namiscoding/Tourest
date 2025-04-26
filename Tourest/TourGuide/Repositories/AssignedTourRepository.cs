using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Tourest.Data;
using Tourest.Data.Entities;
using Tourest.TourGuide.ViewModels;
using Tourest.ViewModels.TourGuide;

namespace Tourest.TourGuide.Repositories
{
    public class AssignedTourRepository : IAssignedTourRespo
    {
        ApplicationDbContext _context;
        public AssignedTourRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TourGuideAssignment> GetAssignmentByIdAsync(int assignmentId)
        {

            return await _context.TourGuideAssignments
                .Include(a => a.TourGroup)
                .FirstOrDefaultAsync(a => a.AssignmentID == assignmentId);
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
        public async Task<List<Tourest.TourGuide.ViewModels.TourGuideRatingViewModel>> GetTourGuideRatingsAndComments(int tourGuideId, int? tourGroupId = null)
        {
            // Lấy tất cả các assignment của tour guide
            var assignmentsQuery = _context.TourGuideAssignments
                .Where(tga => tga.TourGuideID == tourGuideId);

            // Nếu có tourGroupId, lọc theo tourGroupId
            if (tourGroupId.HasValue)
            {
                assignmentsQuery = assignmentsQuery.Where(tga => tga.TourGroupID == tourGroupId.Value);
            }

            var assignments = await assignmentsQuery
                .Select(tga => new
                {
                    Assignment = tga,
                    TourGroup = tga.TourGroup,
                    Tour = tga.TourGroup.Tour,
                    Bookings = tga.TourGroup.Bookings
                })
                .ToListAsync();

            if (!assignments.Any())
                return new List<Tourest.TourGuide.ViewModels.TourGuideRatingViewModel>();

            // Lấy tất cả các rating liên quan
            var tourGroupIds = assignments.Select(a => a.Assignment.TourGroupID).ToList();
            var allRatings = await _context.TourGuideRatings
                .Where(tgr => tgr.TourGuideID == tourGuideId && tourGroupIds.Contains(tgr.TourGroupID))
                .Include(tgr => tgr.Rating)
                .ThenInclude(r => r.Customer)
                .ToListAsync();

            var result = new List<Tourest.TourGuide.ViewModels.TourGuideRatingViewModel>();

            foreach (var assignment in assignments)
            {
               
                var ratings = allRatings
                    .Where(r => r.TourGroupID == assignment.Assignment.TourGroupID)
                    .ToList();

                
                var ratingDistribution = new RatingDistribution
                {
                    FiveStar = ratings.Count(r => r.Rating.RatingValue == 5),
                    FourStar = ratings.Count(r => r.Rating.RatingValue == 4),
                    ThreeStar = ratings.Count(r => r.Rating.RatingValue == 3),
                    TwoStar = ratings.Count(r => r.Rating.RatingValue == 2),
                    OneStar = ratings.Count(r => r.Rating.RatingValue == 1)
                };

                
                var viewModel = new Tourest.TourGuide.ViewModels.TourGuideRatingViewModel
                {
                    AssignmentId = assignment.Assignment.AssignmentID,
                    TourGuideId = assignment.Assignment.TourGuideID,
                    TourGroupName = assignment.TourGroup.Tour.Name,
                    DepartureDate = assignment.TourGroup.DepartureDate,
                    PickupPoint = assignment.TourGroup.Bookings.Select(b => b.PickupPoint).FirstOrDefault(),
                    TotalAdults = assignment.TourGroup.Bookings.Sum(b => b.NumberOfAdults),
                    TotalChildren = assignment.TourGroup.Bookings.Sum(b => b.NumberOfChildren),
                    TourName = assignment.Tour.Name,
                    Status = assignment.Assignment.Status,
                    AssignmentDate = assignment.Assignment.AssignmentDate,
                    Destination = assignment.Tour.Destination,
                    Description = assignment.Tour.Description,
                    CompletedDate = assignment.TourGroup.DepartureDate.AddDays(Math.Max(assignment.Tour.DurationDays, assignment.Tour.DurationNights)),
                    ImageUrl = assignment.Tour.ImageUrls,
                    AverageRating = ratings.Any() ? (decimal)ratings.Average(r => r.Rating.RatingValue) : 0,
                    TotalRatings = ratings.Count,
                    RatingDistribution = ratingDistribution,

                    Ratings = ratings.Select(r => new RatingDetail
                    {
                        RatingId = r.Rating.RatingID,
                        RatingValue = r.Rating.RatingValue,
                        Comment = r.Rating.Comment,
                        RatingDate = r.Rating.RatingDate,
                        CustomerId = r.Rating.CustomerID,
                        CustomerName = r.Rating.Customer.FullName,
                    }).ToList()
                };

                result.Add(viewModel);
            }

            return result;
        }

        public async Task UpdateAssignmentAsync(TourGuideAssignment assignment)
        {
            _context.Update(assignment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTourGroupAsync(TourGroup tourGroup)
        {
            _context.Update(tourGroup);
            await _context.SaveChangesAsync();
        }
    }
}
