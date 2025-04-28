using Microsoft.EntityFrameworkCore;
using Tourest.Data.Entities;
using Tourest.ViewModels.Tour;
using Tourest.ViewModels.TourManager;
namespace Tourest.Data.Repositories
{
    public class TourManagerRepository : ITourManagerRepository
    {
        private readonly ApplicationDbContext _context;

        public TourManagerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<TourGuideListViewModel> GetAllTourGuidesWithUserInfo()
        {
            var result = (from tg in _context.TourGuides
                          join u in _context.Users on tg.TourGuideUserID equals u.UserID
                          select new TourGuideListViewModel
                          {
                              TourGuideUserID = tg.TourGuideUserID,
                              ExperienceLevel = tg.ExperienceLevel,
                              Fullname = u.FullName,
                              Email = u.Email,
                              PhoneNumber = u.PhoneNumber,
                              IsActive = u.IsActive,
                              ActiveStatus = u.IsActive ? "Active" : "Not Active"
                          }).ToList();

            return result;
        }
        public TourGuideDetailViewModel GetTourGuideDetailById(int id)
        {
            var result = (from tg in _context.TourGuides
                          join u in _context.Users on tg.TourGuideUserID equals u.UserID
                          where tg.TourGuideUserID == id
                          select new TourGuideDetailViewModel
                          {
                              Experience = tg.ExperienceLevel,
                              Languages = tg.LanguagesSpoken,
                              Specializations = tg.Specializations,
                              MaxGroupSizeCapacity = tg.MaxGroupSizeCapacity ?? 0,
                              Rating = tg.AverageRating ?? 0,
                              ProfilePictureUri = u.ProfilePictureUrl,
                              FullName = u.FullName,
                              Address = u.Address,
                              PhoneNumber = u.PhoneNumber,
                              Email = u.Email
                          }).FirstOrDefault();

            return result;
        }
        public async Task<List<TourGuideFeedbackViewModel>> GetFeedbacksByTourGuideIdAsync(int tourGuideUserId)
        {
            return await (from tg in _context.TourGuides
                          join tgr in _context.TourGuideRatings on tg.TourGuideUserID equals tgr.TourGuideID
                          join r in _context.Ratings on tgr.RatingID equals r.RatingID
                          join u in _context.Users on r.CustomerID equals u.UserID
                          where tg.TourGuideUserID == tourGuideUserId
                          select new TourGuideFeedbackViewModel
                          {
                              RatingValue = r.RatingValue,
                              Comment = r.Comment,
                              CustomerName = u.FullName,
                              RatingDate = r.RatingDate
                          }).ToListAsync();
        }

        public async Task<List<TourCustomerViewModel>> GetCustomersByTourIdAsync(int tourId)
        {
            var query = from t in _context.Tours
                        join b in _context.Bookings on t.TourID equals b.TourID
                        join u in _context.Users on b.CustomerID equals u.UserID
                        where b.TourID == tourId
                        select new TourCustomerViewModel
                        {
                            UserID = u.UserID,
                            FullName = u.FullName,
                            Email = u.Email,
                            PhoneNumber = u.PhoneNumber,
                            IsActive = u.IsActive,
                            ProfilePictureUrl = u.ProfilePictureUrl,
                            RegistrationDate = u.RegistrationDate,
                            Address = u.Address
                            // Thêm các thuộc tính khác nếu cần
                        };

            return await query.ToListAsync();
        }
        public IEnumerable<TourListAllViewModel> GetAllTours()
        {
            return _context.Tours
    .Select(t => new TourListAllViewModel
    {
        TourID = t.TourID,
        Name = t.Name,
    })
    .ToList();

        }

        public async Task<TourListViewModel?> GetTourByIdAsync(int id)
        {
            var tour = await _context.Tours
                .Where(t => t.TourID == id)
                .Select(t => new TourListViewModel
                {
                    TourId = t.TourID,
                    Name = t.Name,
                    Destination = t.Destination,
                    DurationDays = t.DurationDays,
                    DurationNights = t.DurationNights,
                    ChildPrice = t.ChildPrice,
                    AdultPrice = t.AdultPrice,
                    Status = t.Status,
                    AverageRating = t.AverageRating,
                    MinGroupSize = t.MinGroupSize,
                    MaxGroupSize = t.MaxGroupSize,
                    DeparturePoints = t.DeparturePoints,
                    IncludedServices = t.IncludedServices,
                    ExcludedServices = t.ExcludedServices,
                    ImageUrls = t.ImageUrls, 
                    Description = t.Description,
                    IsCancellable = t.IsCancellable,
                    CancellationPolicyDescription = t.CancellationPolicyDescription
                })
                .FirstOrDefaultAsync();

            if (tour != null && string.IsNullOrEmpty(tour.ImageUrls))
            {
                var defaultImagePath = Path.Combine("wwwroot/images/tours", $"{tour.TourId}.png");
                if (System.IO.File.Exists(defaultImagePath))
                {
                    tour.ImageUrls = $"/images/tours/{tour.TourId}.png";
                }
                else
                {
                    tour.ImageUrls = "/images/default.png"; 
                }
            }

            return tour;
        }

        public async Task AddTourAsync(TourListViewModel tourViewModel)
        {
            var entity = new Tour
            {
                Name = tourViewModel.Name,
                Destination = tourViewModel.Destination,
                DurationDays = tourViewModel.DurationDays,
                ChildPrice = tourViewModel.ChildPrice,
                ImageUrls = tourViewModel.ImageUrls,
                AverageRating = tourViewModel.AverageRating,
                Description = tourViewModel.Description,
                DurationNights = tourViewModel.DurationNights,
                AdultPrice = tourViewModel.AdultPrice,
                Status = tourViewModel.Status,
                IncludedServices = tourViewModel.IncludedServices,
                ExcludedServices = tourViewModel.ExcludedServices,
                DeparturePoints = tourViewModel.DeparturePoints,
                MinGroupSize = tourViewModel.MinGroupSize,
                MaxGroupSize = tourViewModel.MaxGroupSize,
                CancellationPolicyDescription = tourViewModel.CancellationPolicyDescription,
                IsCancellable = tourViewModel.IsCancellable
            };
            _context.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTourAsync(TourListViewModel tourViewModel)
        {
            var entity = await _context.Tours.FindAsync(tourViewModel.TourId);
            if (entity != null)
            {
                entity.Name = tourViewModel.Name;
                entity.Destination = tourViewModel.Destination;
                entity.DurationDays = tourViewModel.DurationDays;
                entity.ChildPrice = tourViewModel.ChildPrice;
                entity.ImageUrls = tourViewModel.ImageUrls;
                entity.AverageRating = tourViewModel.AverageRating;
                entity.Description = tourViewModel.Description;
                entity.DurationNights = tourViewModel.DurationNights;
                entity.AdultPrice = tourViewModel.AdultPrice;
                entity.Status = tourViewModel.Status;
                entity.IncludedServices = tourViewModel.IncludedServices;
                entity.ExcludedServices = tourViewModel.ExcludedServices;
                entity.DeparturePoints = tourViewModel.DeparturePoints;
                entity.MinGroupSize = tourViewModel.MinGroupSize;
                entity.MaxGroupSize = tourViewModel.MaxGroupSize;
                entity.CancellationPolicyDescription = tourViewModel.CancellationPolicyDescription;
                entity.IsCancellable = tourViewModel.IsCancellable;
                await _context.SaveChangesAsync();
            }
        }
        public async Task DeleteTourAsync(int TourID)
        {
            var tour = await _context.Tours.FindAsync(TourID); 
            if (tour != null)
            {
                _context.Tours.Remove(tour);   
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Tour not found.");
            }
        }

        public async Task<TourListViewModel?> GetTourByIDAsync(int tourId)
        {
            var tour = await _context.Tours
                .Where(t => t.TourID == tourId)
                .Select(t => new TourListViewModel
                {
                    TourId = t.TourID,
                    Name = t.Name,
                    Destination = t.Destination,
                    DurationDays = t.DurationDays,
                    DurationNights = t.DurationNights,
                    ChildPrice = t.ChildPrice,
                    AdultPrice = t.AdultPrice,
                    Description = t.Description,
                    Status = t.Status,
                    MinGroupSize = t.MinGroupSize,
                    MaxGroupSize = t.MaxGroupSize,
                    DeparturePoints = t.DeparturePoints,
                    ExcludedServices = t.ExcludedServices,
                    IncludedServices = t.IncludedServices,
                    ImageUrls = t.ImageUrls,
                    IsCancellable = t.IsCancellable,
                    CancellationPolicyDescription = t.CancellationPolicyDescription
                })
                .FirstOrDefaultAsync();

            return tour;
        }

        public async Task<List<TourGuideAssignmentViewModel>> GetTourGuideScheduleAsync(int tourGuideId)
        {
            return await _context.TourGuideAssignments
                .Where(t => t.TourGuideID == tourGuideId)
                .Select(t => new TourGuideAssignmentViewModel
                {
                    AssignmentDate = t.AssignmentDate,
                    ConfirmationDate = t.ConfirmationDate,
                    Status = t.Status,
                    RejectionReason = t.RejectionReason
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<UserViewModel>> GetUsers()
        {
            return await _context.Users.Select(u => new UserViewModel
            {
                FullName = u.FullName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                ProfilePictureUrl = u.ProfilePictureUrl
            }).ToListAsync();

        }

        public async Task<UserViewModel> GetUserByIdAsync(int id)
        {
            return await _context.Users.Where(u => u.UserID == id).Select(u => new UserViewModel
            {
                FullName = u.FullName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                ProfilePictureUrl = u.ProfilePictureUrl
            }).FirstOrDefaultAsync();
        }

    }
}
