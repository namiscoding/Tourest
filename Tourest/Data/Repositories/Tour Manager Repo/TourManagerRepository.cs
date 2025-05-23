﻿using Microsoft.EntityFrameworkCore;
using Tourest.Data.Entities;
using Tourest.ViewModels.Tour;

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
                              ProfilePictureUri = u.ProfilePictureUrl
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
        // Bỏ comment các dòng nếu cần thêm dữ liệu khác từ Tour entity
    })
    .ToList();

        }
    }
}
