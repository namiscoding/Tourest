using System;
using System.Collections.Generic;
using Tourest.Data.Entities;

namespace Tourest.TourGuide.ViewModels
{
    public class TourGuideRatingViewModel
    {
        // Thông tin chung về assignment
        public int AssignmentId { get; set; }
        public int TourGuideId { get; set; }
        public string TourGroupName { get; set; }
        public DateTime DepartureDate { get; set; }
        public string PickupPoint { get; set; }
        public int TotalAdults { get; set; }
        public int TotalChildren { get; set; }
        public int TotalPeople => TotalAdults + TotalChildren;
        public string TourName { get; set; }
        public string Status { get; set; }
        public DateTime AssignmentDate { get; set; }
        public string Destination { get; set; }
        public string Description { get; set; }
        public DateTime CompletedDate { get; set; }

        public string ImageUrl { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalRatings { get; set; }
        public RatingDistribution RatingDistribution { get; set; }

        
        public List<RatingDetail> Ratings { get; set; }
    }

    public class RatingDetail
    {
        public int RatingId { get; set; }
        public decimal RatingValue { get; set; }
        public string Comment { get; set; }
        public DateTime RatingDate { get; set; }

        // Thông tin khách hàng
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAvatar { get; set; }

        // Phản hồi của hướng dẫn viên
        public string GuideResponse { get; set; }
        public DateTime? ResponseDate { get; set; }
    }

    public class RatingDistribution
    {
        public int FiveStar { get; set; }
        public int FourStar { get; set; }
        public int ThreeStar { get; set; }
        public int TwoStar { get; set; }
        public int OneStar { get; set; }
    }
}