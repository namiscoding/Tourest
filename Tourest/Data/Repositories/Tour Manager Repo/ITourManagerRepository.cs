﻿using Tourest.Data.Entities;
using Tourest.ViewModels.Tour;

namespace Tourest.Data.Repositories
{
    public interface ITourManagerRepository
    {
        List<TourGuideListViewModel> GetAllTourGuidesWithUserInfo();
        TourGuideDetailViewModel GetTourGuideDetailById(int id);
        Task<List<TourGuideFeedbackViewModel>> GetFeedbacksByTourGuideIdAsync(int tourGuideUserId);
        Task<List<TourCustomerViewModel>> GetCustomersByTourIdAsync(int tourId);
        IEnumerable<TourListAllViewModel> GetAllTours();

    }
}
